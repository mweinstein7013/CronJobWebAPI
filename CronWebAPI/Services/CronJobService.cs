using Azure.Storage.Queues;
using CronWebAPI.Models;
using CronWebAPI.Repository;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CronWebAPI.Services
{
    public class CronJobService : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var queueName = dataMap.GetString(nameof(CronMessage.QueueName));
            var queueConnection = dataMap.GetString(nameof(CronMessage.QueueConnection));
            var message = dataMap.GetString(nameof(CronMessage.Message));

            QueueClient client = new QueueClient(queueConnection, queueName);
            if (client.Exists())
            {
                var utfBytes = System.Text.Encoding.UTF8.GetBytes(message);
                await client.SendMessageAsync(System.Convert.ToBase64String(utfBytes));
                Console.WriteLine("The message has been sent!");
            }
        }
    }
}
