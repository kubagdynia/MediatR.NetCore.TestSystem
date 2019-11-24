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

        /// <summary>
        /// Emit only once and almost immediately after creation. 
        /// </summary>
        /// <param name="notification">Notification to be emitted.</param>
        public void EmitEvent(INotification notification)
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

        /// <summary>
        /// Emit only once, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="notification">Notification to be emitted.</param>
        /// <param name="scheduleAt">The moment of time at which the job will be enqueued.</param>
        /// <returns>Unique identifier of a created job.</returns>
        public string EmitScheduledEvent(INotification notification, DateTimeOffset scheduleAt)
        {
            CheckIfOperationIsSupported();

            var mediatorSerializedObject = SerializeObject(notification);
            
            return BackgroundJob.Schedule(() => _messageExecutor.ExecuteEvent(mediatorSerializedObject), scheduleAt);
        }

        /// <summary>
        /// Emit only once, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="notification">Notification to be emitted.</param>
        /// <param name="delay">After what delay the job will be enqueued.</param>
        /// <returns>Unique identifier of a created job.</returns>
        public string EmitScheduledEvent(INotification notification, TimeSpan delay)
        {
            CheckIfOperationIsSupported();

            var mediatorSerializedObject = SerializeObject(notification);
            var newTime = DateTime.Now + delay;

            return BackgroundJob.Schedule(() => _messageExecutor.ExecuteEvent(mediatorSerializedObject), newTime);
        }

        /// <summary>
        /// Emit many times on the specified CRON schedule.
        /// </summary>
        /// <param name="notification">Notification to be emitted.</param>
        /// <param name="recurringJobId">Recurring job id</param>
        /// <param name="cronExpression">http://en.wikipedia.org/wiki/Cron#CRON_expression</param>
        public void EmitScheduledRecurringEvent(INotification notification, string recurringJobId, string cronExpression)
        {
            CheckIfOperationIsSupported();
            
            var mediatorSerializedObject = SerializeObject(notification);

            RecurringJob.AddOrUpdate(recurringJobId, () => _messageExecutor.ExecuteEvent(mediatorSerializedObject),
                cronExpression, TimeZoneInfo.Local);
        }

        /// <summary>
        /// Execute the command only once almost immediately after creation. 
        /// </summary>
        /// <param name="request">Request to be send.</param>
        public void SendCommand(IRequest request)
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

        /// <summary>
        /// Execute the command only once but when its parent job has been finished. 
        /// </summary>
        /// <param name="request">Request to be send.</param>
        /// <param name="parentJobId">Parent job id.</param>
        /// <param name="continuationOption">Continuation option.</param>
        /// <returns>Unique identifier of a created job.</returns>
        public string SendCommand(IRequest request, string parentJobId, JobContinuationOptions continuationOption)
        {
            CheckIfOperationIsSupported();

            var mediatorSerializedObject = SerializeObject(request);

            return BackgroundJob.ContinueJobWith(parentJobId,
                () => _messageExecutor.ExecuteCommand(mediatorSerializedObject), continuationOption);
        }

        /// <summary>
        /// Execute the command only once and return the result.
        /// </summary>
        /// <param name="request">Request to be send.</param>
        /// <typeparam name="TResponse">Object that will be returned</typeparam>
        /// <returns>Response from the command handler</returns>
        public Task<TResponse> SendCommand<TResponse>(IRequest<TResponse> request)
        {
            var response = _mediator.Send(request);
            return response;
        }

        /// <summary>
        /// Execute the command only once, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="request">Request to be send.</param>
        /// <param name="scheduleAt">The moment of time at which the command will be executed.</param>
        /// <returns>Unique identifier of a created job.</returns>
        public string SendScheduledCommand(IRequest request, DateTimeOffset scheduleAt)
        {
            CheckIfOperationIsSupported();

            var mediatorSerializedObject = SerializeObject(request);

            return BackgroundJob.Schedule(() => _messageExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
        }

        /// <summary>
        /// Execute the command only once, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="request">Request to be send.</param>
        /// <param name="delay">After what delay the command will be executed.</param>
        /// <returns>Unique identifier of a created job.</returns>
        public string SendScheduledCommand(IRequest request, TimeSpan delay)
        {
            CheckIfOperationIsSupported();
            
            var mediatorSerializedObject = SerializeObject(request);
            var newTime = DateTime.Now + delay;

            return BackgroundJob.Schedule(() => _messageExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
        }

        /// <summary>
        /// Execute the command many times on the specified CRON schedule.
        /// </summary>
        /// <param name="request">Request to be send.</param>
        /// <param name="recurringJobId">Recurring job id.</param>
        /// <param name="cronExpression">http://en.wikipedia.org/wiki/Cron#CRON_expression</param>
        public void SendScheduledRecurringCommand(IRequest request, string recurringJobId, string cronExpression)
        {
            CheckIfOperationIsSupported();
            
            var mediatorSerializedObject = SerializeObject(request);
            
            RecurringJob.AddOrUpdate(recurringJobId, () => _messageExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression, TimeZoneInfo.Local);
        }

        private MediatorSerializedObject SerializeObject(object mediatorObject)
        {
            string assemblyQualifiedName = mediatorObject.GetType().AssemblyQualifiedName;

            string data = JsonSerializer.Serialize(mediatorObject, BaseJsonOptions.GetJsonSerializerOptions);

            return new MediatorSerializedObject(assemblyQualifiedName, data);
        }
        
        private void CheckIfOperationIsSupported()
        {
            if (!_hangfireConfig.Enabled)
            {
                throw new NotSupportedException("Operation is not supported because Hangfire is off");
            }
        }
    }
}
