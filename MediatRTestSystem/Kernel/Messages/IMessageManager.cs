using Hangfire;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Kernel.Messages
{
    public interface IMessageManager
    {
        void Publish(INotification notification);

        void Send(IRequest request);

        void Send(IRequest request, string parentJobId, JobContinuationOptions continuationOption);

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);

        void Schedule(IRequest request, DateTimeOffset scheduleAt);

        void Schedule(IRequest request, TimeSpan delay);
    }
}