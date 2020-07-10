using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CronWebAPI.Models;
using CronWebAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CronWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CronController : ControllerBase
    {
        private readonly CronRepository _cronRepository;

        public CronController(CronRepository cronRepository)
        {
            _cronRepository = cronRepository;
        }

        [HttpGet]
        [Route("/jobs")]
        public async Task<IList<CronObject>> GetAllCronJobs()
        {
            return await _cronRepository.GetAllCronJobs();
        }

        [HttpPost]
        [Route("/jobs")]
        public async Task<ActionResult> UpsertCronJob([FromBody] CronMessage message)
        {
            var upserted = await _cronRepository.UpsertCronJob(message);
            if (!upserted) throw new Exception("Could not upsert job.");
            return Ok();
        }

        [HttpGet]
        [Route("/jobs/{jobName}")]
        public async Task<ActionResult<CronObject>> GetCronJob(string jobName)
        {
            var cronJob = await _cronRepository.GetCronJob(jobName);
            if (cronJob != null) return cronJob;
            else return NotFound();
        }

        [HttpDelete]
        [Route("/jobs/{jobName}")]
        public async Task<ActionResult> StopCronJob(string jobName)
        {
            var isStopped = await _cronRepository.DeleteCronJob(jobName);
            if (!isStopped) throw new Exception("Could not stop job.");
            return Ok();
        }
    }
}
