namespace CompanyWebApi.Core.Auth
{
    public interface IJwtFactory
    {
        string EncodeToken(string userName);
    }
}
