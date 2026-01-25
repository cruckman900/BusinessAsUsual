namespace BusinessAsUsual.Application.Common
{
    public interface IAppEnvironment
    {
        string ContentRootPath { get; }
        string EnvironmentName { get; }
    }
}
