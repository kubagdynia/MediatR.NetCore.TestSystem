using System.Collections.Generic;

namespace Kernel
{
    public class BaseDomainResponse
    {
        public IList<string>? Errors { get; private set; }

        public void AddError(string errorMessage)
        {
            if (Errors == null)
            {
                Errors = new List<string>();
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Errors.Add(errorMessage);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

    }
}
