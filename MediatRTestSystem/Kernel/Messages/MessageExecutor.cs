using Kernel.Serializers;
using MediatR;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kernel.Messages
{
    public class MessageExecutor : IMessageExecutor
    {
        private readonly IMediator _mediator;

        public MessageExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
        {
            var request = GetMessage<IRequest>(mediatorSerializedObject);
            return _mediator.Send(request);
        }

        public Task ExecuteEvent(MediatorSerializedObject mediatorSerializedObject)
        {
            var notification = GetMessage<INotification>(mediatorSerializedObject);
            return _mediator.Publish(notification);
        }

        private T GetMessage<T>(MediatorSerializedObject mediatorSerializedObject) where T : class
        {
            if (mediatorSerializedObject is null)
            {
                throw new ArgumentNullException(nameof(mediatorSerializedObject));
            }

            if (mediatorSerializedObject.AssemblyQualifiedName is null)
            {
                throw new ArgumentNullException(nameof(mediatorSerializedObject.AssemblyQualifiedName));
            }

            var type = Type.GetType(mediatorSerializedObject.AssemblyQualifiedName);

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!(JsonSerializer.Deserialize(mediatorSerializedObject.Data, type, BaseJsonOptions.GetJsonSerializerOptions) is T notification))
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return notification;
        }
    }
}
