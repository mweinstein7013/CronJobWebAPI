using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronWebAPI.Models
{
    public class CronMessage
    {
        public string Cron { get; set; }
        public string QueueName { get; set; }
        public string QueueConnection { get; set; }
        public string JobName { get; set; }
        public string Message { get; set; }
    }
}
