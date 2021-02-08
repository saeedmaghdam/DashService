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
                    FullName = job.JobInstance.GetType().FullName,
                    Namespace = job.JobInstance.GetType().Namespace,
                    Name = job.JobInstance.Name,
                    Description = job.JobInstance.Description,
                    JobStatus = job.JobStartingTask.Status.ToString(),
                    Version = job.JobInstance.Version,
                    ViewId = job.PluggedinAssembly.UniqueId
                })))
            );
        }
    }
}
