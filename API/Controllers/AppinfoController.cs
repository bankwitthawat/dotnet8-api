

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppInfoController : ControllerBase
    {
        private IHostingEnvironment _hostingEnv;
        public AppInfoController(IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
        }
        
        [HttpGet(Name = "GetAppInfo")]
        public AppInfo Get()
        {
            return new AppInfo()
            {
                AssemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown",
                ControlVersion = "API 0.0.1",
                Environment = _hostingEnv.EnvironmentName,
            };
        }
    }

    public class AppInfo
    {
        public string Name { get; set; } = "Widely Smart Sale - Core API";
        public string AssemblyVersion { get; set; } = string.Empty;
        public string ControlVersion { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }
}