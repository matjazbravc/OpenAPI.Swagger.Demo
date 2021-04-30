using CompanyWebApi.Configurations;
using CompanyWebApi.Contracts.Converters;
using CompanyWebApi.Contracts.Dto;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Core.Auth;
using CompanyWebApi.Extensions;
using CompanyWebApi.Middleware;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Services.Authorization;
using CompanyWebApi.Services.Controllers;
using CompanyWebApi.Services.Repositories;
using CompanyWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using CompanyWebApi.Services.Helpers;
using Serilog.Events;

namespace CompanyWebApi
{
    public class Startup
    {
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

            RegisterConfigurations(services);
            RegisterServices(services);

            // Register services required by authentication services
            ConfigureAuthentication(services, Configuration);

            services.AddCorsPolicy("EnableCORS");

            // Adds service API versioning
            services.AddAndConfigureApiVersioning();

            // Adds services for controllers
            services.AddControllers(options =>
            {
                // Adds a convention to let Swagger understand the different API versions
                options.Conventions.Add(new GroupingByNamespaceConvention());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true; // To disable the automatic 400 behavior, set the SuppressModelStateInvalidFilter property to true
                options.SuppressMapClientErrors = true;
                options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Adds Swagger support
            services.AddSwaggerMiddleware();

            // Add Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableDetailedErrors();
                options.UseSqlite(Configuration.GetConnectionString("SqLiteConnectionString"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            // Configure Database context
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var context = serviceScope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context?.Database.EnsureCreated();
                SeedData.Initialize(context);
            }

            // Register Swagger and SwaggerUI middleware
            app.UseSwaggerMiddleware(config);

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            // For elevated security, it is recommended to remove this middleware and set your server to only listen on https. 
            // A slightly less secure option would be to redirect http to 400, 505, etc.
            app.UseHttpsRedirection();

            // Adds middleware for streamlined request logging
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "{Host} {Protocol} {RequestMethod} {RequestPath} {EndpointName} responded {StatusCode} in {Elapsed} ms";
                options.EnrichDiagnosticContext = RequestLogHelper.EnrichDiagnosticContext;
            });

            // Adds global error handling middleware
            app.UseApiExceptionHandling();

            // Adds enpoint routing middleware
            app.UseRouting();

            // Adds a CORS middleware
            app.UseCors("EnableCORS");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Adds enpoints for controller actions without specifyinf any routes
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Register a configuration instances which TOptions will bind against
        /// </summary>
        /// <param name="services"></param>
        protected void RegisterConfigurations(IServiceCollection services)
        {
            services.Configure<AuthSettings>(Configuration.GetSection(nameof(AuthSettings)));
            services.Configure<SwaggerConfig>(Configuration.GetSection(nameof(SwaggerConfig)));
        }

        /// <summary>
        /// Register services required by authentication services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        protected void ConfigureAuthentication(IServiceCollection services, IConfiguration config)
        {
            var authSettings = config.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            var key = Encoding.UTF8.GetBytes(authSettings.SecretKey);
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

        /// <summary>
        /// Register services and middlewares
        /// </summary>
        /// <param name="services"></param>
        protected virtual void RegisterServices(IServiceCollection services)
        {
            // Register middlewares
            services.AddTransient<ApiExceptionHandlingMiddleware>();

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
