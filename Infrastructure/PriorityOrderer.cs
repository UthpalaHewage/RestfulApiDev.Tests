using Xunit.Abstractions;
using Xunit.Sdk;

namespace RestfulApiDev.Tests.Infrastructure;

public sealed class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases
            .Select(tc =>
            {
                var priority = tc.TestMethod.Method
                    .GetCustomAttributes(typeof(PriorityAttribute).AssemblyQualifiedName!)
                    .FirstOrDefault();

                var order = priority is null
                    ? int.MaxValue
                    : priority.GetNamedArgument<int>(nameof(PriorityAttribute.Order));

                return new { TestCase = tc, Order = order };
            })
            .OrderBy(x => x.Order)
            .ThenBy(x => x.TestCase.DisplayName)
            .Select(x => x.TestCase);
    }
}