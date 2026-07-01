using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IScheduledJobService
    {
        // Enqueue: Chạy ngay lập tức (Fire-and-forget)
        string Enqueue(Expression<Action> methodCall);
        string Enqueue<T>(Expression<Action<T>> methodCall);
        // Schedule: Chạy sau một khoảng thời gian trì hoãn (Delayed Job)
        string Schedule(Expression<Action> methodCall, TimeSpan delay);
        string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
        // Delete: Hủy một Job đang chờ chạy
        bool Delete(string jobId);
    }
}
