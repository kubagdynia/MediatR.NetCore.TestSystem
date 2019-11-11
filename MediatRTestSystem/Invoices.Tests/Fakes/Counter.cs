using System.Threading;

namespace Invoices.Tests.Fakes
{
    public class Counter
    {
        private int _count = 0;

        public int Decrement() => Interlocked.Decrement(ref _count);

        public int Add(int value) => Interlocked.Add(ref _count, value);

        public int Get() => _count;

        public int Increment() => Interlocked.Increment(ref _count);

        public int Subtract(int value) => Interlocked.Add(ref _count, -value);
    }
}
