using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashService.WebApi.Controllers
{
    public class JobController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jobs = Context.JobContainer.PluginableJobs;

            return await Task.FromResult(
                Ok(new ApiResult<IEnumerable<dynamic>>(jobs.Select(job => new {
                    FullName = job.PluggedinAssembly.JobInstance.GetType().FullName,
                    Namespace = job.PluggedinAssembly.JobInstance.GetType().Namespace,
                    Name = job.PluggedinAssembly.JobInstance.Name,
                    Description = job.PluggedinAssembly.JobInstance.Description,
                    JobStatus = job.JobStartingTask.Status.ToString(),
                    Version = job.PluggedinAssembly.JobInstance.Version,
                    ViewId = job.PluggedinAssembly.UniqueId
                })))
            );
        }
    }
}
