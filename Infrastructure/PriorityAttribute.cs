namespace RestfulApiDev.Tests.Infrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class PriorityAttribute : Attribute
{
    public int Order { get; }
    public PriorityAttribute(int order) => Order = order;
}