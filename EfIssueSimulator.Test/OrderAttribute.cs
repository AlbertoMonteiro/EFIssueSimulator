using System;

namespace EfIssueSimulatorTests
{
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int i)
        {
            Order = i;
        }
    }
}