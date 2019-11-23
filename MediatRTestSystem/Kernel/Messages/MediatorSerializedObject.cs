using System.Linq;

namespace Kernel.Messages
{
    public class MediatorSerializedObject
    {
        public string? AssemblyQualifiedName { get; private set; }

        public string? Data { get; private set; }

        public MediatorSerializedObject(string assemblyQualifiedName, string data)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
            Data = data;
        }

        /// <summary>
        /// Override for Hangfire dashboard display
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string? fullTypeName = GetFullTypeName();
            var commandName = fullTypeName is null ? "No command name" : fullTypeName.Split('.').Last();
            return commandName;
        }

        private string? GetFullTypeName()
        {
            if (AssemblyQualifiedName is null)
            {
                return null;
            }

            string? fullTypeName = null;

            var parts = AssemblyQualifiedName.Split(',');
            if (parts.Length > 0)
            {
                fullTypeName = parts[0];
            }

            return fullTypeName;
        }
    }
}
