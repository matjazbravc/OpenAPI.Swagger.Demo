using CompanyWebApi.Configurations;
using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Core.Auth;
using CompanyWebApi.Extensions;
using CompanyWebApi.Middleware;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Services.Authorization;
using CompanyWebApi.Services.Repositories;
using CompanyWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi
{
    // Create web APIs with ASP.NET Core
    // https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1
    public class Startup
    {
        private const string API_NAME = "Company Web API";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services required for using options
            services.AddOptions();

            // Add the whole configuration object here
            services.AddSingleton(Configuration);

            // Configure DI for application services
            RegisterServices(services);

            // Configure JWT authentication
            ConfigureAuthentication(services);

            services.AddCorsPolicy("EnableCORS");

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter =
                        true; // To disable the automatic 400 behavior, set the SuppressModelStateInvalidFilter property to true
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddApiVersioningExtension();
            services.AddSwaggerExtension(API_NAME);

            // Add Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqLiteConnectionString")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure Database context
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var context = serviceScope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //context.Database.EnsureDeleted();
                context?.Database.EnsureCreated();
                SeedData.Initialize(context);
            }

            app.UseExceptionHandler("/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            // For elevated security, it is recommended to remove this middleware and set your server to only listen on https. 
            // A slightly less secure option would be to redirect http to 400, 505, etc.
            app.UseHttpsRedirection();

            app.UseCors("EnableCORS");

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseErrorHandlingMiddleware();

            // Request/Response logging middleware
            app.UseApiLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerExtension();
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            // Configure AuthSettings            
            var authSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSettings);

            var key = Encoding.UTF8.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]);
            var signingKey = new SymmetricSecurityKey(key);
            var jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true, // Audience will be validated during token validation
                ValidAudience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true, 
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.ClaimsIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
                opt.TokenValidationParameters = tokenValidationParameters;
            });
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {
            // Register middlewares
            services.AddTransient<ApiLogging>();
            services.AddTransient<ErrorHandlerMiddleware>();

            // Services
            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddScoped<IUserService, UserService>();

            //*********************************************************************************
            // Registering multiple implementations of the same interface IRepository<TEntity>
            //*********************************************************************************
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Entity to Dto Converters
            services.AddTransient<IConverter<Company, CompanyDto>, CompanyToDtoConverter>();
            services.AddTransient<IConverter<IList<Company>, IList<CompanyDto>>, CompanyToDtoConverter>();
            services.AddTransient<IConverter<Department, DepartmentDto>, DepartmentToDtoConverter>();
            services.AddTransient<IConverter<IList<Department>, IList<DepartmentDto>>, DepartmentToDtoConverter>();
            services.AddTransient<IConverter<Employee, EmployeeDto>, EmployeeToDtoConverter>();
            services.AddTransient<IConverter<IList<Employee>, IList<EmployeeDto>>, EmployeeToDtoConverter>();
        }
    }
}
