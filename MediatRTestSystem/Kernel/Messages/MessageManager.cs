using Hangfire;
using Kernel.Configurations;
using Kernel.Serializers;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kernel.Messages
{
    public class MessageManager : IMessageManager
    {
        private readonly IMessageExecutor _messageExecutor;
        private readonly IMediator _mediator;
        private readonly HangfireConfiguration _hangfireConfig;

        public MessageManager(IMessageExecutor messageExecutor, IMediator mediator, IOptions<HangfireConfiguration> hangfireConfig)
        {
            _messageExecutor = messageExecutor;
            _mediator = mediator;
            _hangfireConfig = hangfireConfig.Value;
        }

        public void Publish(INotification notification)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(notification);

                BackgroundJob.Enqueue(() => _messageExecutor.ExecuteEvent(mediatorSerializedObject));
            }
            else
            {
                _mediator.Publish(notification);
            }
        }

        public void PublishSchedule(INotification notification, DateTimeOffset scheduleAt)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(notification);

                BackgroundJob.Schedule(() => _messageExecutor.ExecuteEvent(mediatorSerializedObject), scheduleAt);
            }
        }

        public void PublishSchedule(INotification notification, TimeSpan delay)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(notification);
                var newTime = DateTime.Now + delay;

                BackgroundJob.Schedule(() => _messageExecutor.ExecuteEvent(mediatorSerializedObject), newTime);
            }
        }

        public void PublishScheduleRecurring(INotification notification, string name, string cronExpression)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(notification);

                RecurringJob.AddOrUpdate(name, () => _messageExecutor.ExecuteEvent(mediatorSerializedObject), cronExpression, TimeZoneInfo.Local);
            }
        }

        public void Send(IRequest request)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(request);

                BackgroundJob.Enqueue(() => _messageExecutor.ExecuteCommand(mediatorSerializedObject));
            }
            else
            {
                _mediator.Send(request);
            }
        }

        public void Send(IRequest request, string parentJobId, JobContinuationOptions continuationOption)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(request);

                BackgroundJob.ContinueJobWith(parentJobId, () => _messageExecutor.ExecuteCommand(mediatorSerializedObject), continuationOption);
            }
            else
            {
                _mediator.Send(request);
            }
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var response = _mediator.Send(request);
            return response;
        }

        public void Schedule(IRequest request, DateTimeOffset scheduleAt)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(request);

                BackgroundJob.Schedule(() => _messageExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
            }
        }

        public void Schedule(IRequest request, TimeSpan delay)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(request);
                var newTime = DateTime.Now + delay;

                BackgroundJob.Schedule(() => _messageExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
            }
        }

        public void ScheduleRecurring(IRequest request, string name, string cronExpression)
        {
            if (_hangfireConfig.Enabled)
            {
                var mediatorSerializedObject = SerializeObject(request);

                RecurringJob.AddOrUpdate(name, () => _messageExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression, TimeZoneInfo.Local);
            }
        }

        private MediatorSerializedObject SerializeObject(object mediatorObject)
        {
            string assemblyQualifiedName = mediatorObject.GetType().AssemblyQualifiedName;

            string data = JsonSerializer.Serialize(mediatorObject, BaseJsonOptions.GetJsonSerializerOptions);

            return new MediatorSerializedObject(assemblyQualifiedName, data);
        }
    }
}
