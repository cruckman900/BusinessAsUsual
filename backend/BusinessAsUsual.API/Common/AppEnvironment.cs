using BusinessAsUsual.Application.Common;

namespace BusinessAsUsual.API.Common
{
    public class AppEnvironment : IAppEnvironment
    {
        private readonly IWebHostEnvironment _env;

        public AppEnvironment(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ContentRootPath => _env.ContentRootPath;
        public string EnvironmentName => _env.EnvironmentName;
    }
}
