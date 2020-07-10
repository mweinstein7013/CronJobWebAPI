using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Cronos;
using CronWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace CronWebAPI.Models
{
    public class CronObject
    {
        public string JobName { get; set; }
        public string CronSchedule { get; set; }
        public DateTimeOffset NextRun { get { return CronTrigger.GetNextFireTimeUtc().Value; } }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastRun { get; set; }
        public bool Succeeded { get; set; }
        public string Queue { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public IJobDetail CronJob { get; set; }

        [JsonIgnore]
        public ITrigger CronTrigger { get;  set; }
    }
}
