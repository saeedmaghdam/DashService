using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashService.Framework;

namespace DashService.WebApi.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobContainer _jobContainer;

        public JobController(IJobContainer jobContainer)
        {
            _jobContainer = jobContainer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jobs = _jobContainer.JobInstances;

            return await Task.FromResult(
                Ok(new ApiResult<IEnumerable<dynamic>>(jobs.Select(job => new {
                    FullName = job.JobAssembly.Instance.GetType().FullName,
                    Namespace = job.JobAssembly.Instance.GetType().Namespace,
                    Name = job.JobAssembly.Instance.Name,
                    Description = job.JobAssembly.Instance.Description,
                    JobStatus = job.JobStartingTask.Status.ToString(),
                    Version = job.JobAssembly.Instance.Version,
                    ViewId = job.JobAssembly.UniqueId
                })))
            );
        }
    }
}
