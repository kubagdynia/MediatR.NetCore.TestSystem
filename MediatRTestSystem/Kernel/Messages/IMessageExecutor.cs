using System.Threading.Tasks;

namespace Kernel.Messages
{
    public interface IMessageExecutor
    {
        Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject);
        Task ExecuteEvent(MediatorSerializedObject mediatorSerializedObject);
    }
}