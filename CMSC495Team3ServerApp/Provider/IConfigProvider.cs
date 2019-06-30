namespace CMSC495Team3ServerApp.Provider
{
    public interface IConfigProvider
    {
        string DatabaseConnectionString { get; }

        string ExposedHttpUrl { get; }
    }
}