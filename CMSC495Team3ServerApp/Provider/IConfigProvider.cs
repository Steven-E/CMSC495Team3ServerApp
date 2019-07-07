namespace CMSC495Team3ServerApp.Provider
{
    public interface IConfigProvider
    {
        string DatabaseConnectionString { get; }

        string ExposedHttpUrl { get; }

        //string PublicAddressBase { get; }

        string UntappdApiUrlBase { get; }

        string UntappdAppClientId { get; }

        string UntappdAppClientSecret { get; }

        //string UntappdClientCallBackRoute { get; }

        //string UntappdOAuthAppAuthenticationRoute { get; }
    }
}