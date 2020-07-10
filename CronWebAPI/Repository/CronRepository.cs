using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronWebAPI.Models;
using CronWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;

namespace CronWebAPI.Repository
{
    public class CronRepository
    {
        public IDictionary<string, CronObject> CronJobs = new Dictionary<string, CronObject>();
        public readonly ISchedulerFactory SchedularFactory;
        public readonly IScheduler Schedular;
        
        public CronRepository()
        {
            SchedularFactory = new StdSchedulerFactory();
            Schedular = SchedularFactory.GetScheduler().Result;
            Schedular.Start();
        }

        public async Task<IList<CronObject>> GetAllCronJobs()
        {
            return CronJobs.Values.ToList();
        }

        public async Task<bool> UpsertCronJob(CronMessage message)
        {
            var cronJob = await GetCronJob(message.JobName);
            if(cronJob == null)
            {
                return await CreateAndStartCronJob(message);
            }
            else
            {
                var isDeleted = await DeleteCronJob(message.JobName);
                return await CreateAndStartCronJob(message);
            }
            return true;
        }

        private async Task<bool> CreateAndStartCronJob(CronMessage message)
        {
            var cronObject = await CreateCronJob(message);

            var isScheduled = await ScheduleCronJob(cronObject);
            if (isScheduled)
            {
                CronJobs.Add(cronObject.JobName, cronObject);
                return true;
            }
            return false;
        }

        public async Task<CronObject> GetCronJob(string jobName)
        {
            if (CronJobs.ContainsKey(jobName)) return CronJobs[jobName];
            return null;
        }

        private async Task<bool> ScheduleCronJob(CronObject message)
        {
            await Schedular.ScheduleJob(message.CronJob, message.CronTrigger);

            return true;
        }

        private async Task<CronObject> CreateCronJob(CronMessage message)
        {
            var job = JobBuilder.Create<CronJobService>()
                .WithIdentity(message.JobName)
                .UsingJobData(nameof(CronMessage.QueueConnection), message.QueueConnection)
                .UsingJobData(nameof(CronMessage.QueueName), message.QueueName)
                .UsingJobData(nameof(CronMessage.Message), message.Message)
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(job)
                .WithCronSchedule(message.Cron)
                .WithIdentity($"{message.JobName}Trigger")
                .StartNow()
                .Build();

            return new CronObject()
            {
                Succeeded = false,
                JobName = message.JobName,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                CronSchedule = message.Cron,
                LastRun = DateTime.MinValue,
                Queue = message.QueueName,
                CronJob = job,
                CronTrigger = trigger,
            };
        }

        public async Task<bool> DeleteCronJob(string jobName)
        {
            var job = await GetCronJob(jobName);
            if (job == null) return false;
            return await DeleteCronJob(job);
        }

        private async Task<bool> DeleteCronJob(CronObject job)
        {
            if (!CronJobs.ContainsKey(job.JobName)) return false;
            await Schedular.PauseJob(job.CronJob.Key);
            await Schedular.DeleteJob(job.CronJob.Key);
            CronJobs.Remove(job.JobName);
            return true;
        }
    }
}
