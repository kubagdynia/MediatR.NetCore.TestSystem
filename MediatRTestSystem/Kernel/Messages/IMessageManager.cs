using Hangfire;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Kernel.Messages
{
    public interface IMessageManager
    {
        void EmitEvent(INotification notification);

        string EmitScheduledEvent(INotification notification, DateTimeOffset scheduleAt);

        string EmitScheduledEvent(INotification notification, TimeSpan delay);

        void EmitScheduledRecurringEvent(INotification notification, string recurringJobId, string cronExpression);

        void SendCommand(IRequest request);

        string SendCommand(IRequest request, string parentJobId, JobContinuationOptions continuationOption);

        Task<TResponse> SendCommand<TResponse>(IRequest<TResponse> request);

        string SendScheduledCommand(IRequest request, DateTimeOffset scheduleAt);

        string SendScheduledCommand(IRequest request, TimeSpan delay);

        void SendScheduledRecurringCommand(IRequest request, string recurringJobId, string cronExpression);
    }
}