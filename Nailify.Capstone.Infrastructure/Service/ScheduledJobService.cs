using Hangfire;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class ScheduledJobService : IScheduledJobService
    {
        public string Enqueue(Expression<Action> methodCall)
            => BackgroundJob.Enqueue(methodCall);
        public string Enqueue<T>(Expression<Action<T>> methodCall)
            => BackgroundJob.Enqueue<T>(methodCall);
        public string Schedule(Expression<Action> methodCall, TimeSpan delay)
            => BackgroundJob.Schedule(methodCall, delay);
        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
            => BackgroundJob.Schedule<T>(methodCall, delay);
        public bool Delete(string jobId)
            => BackgroundJob.Delete(jobId);
    }
}
