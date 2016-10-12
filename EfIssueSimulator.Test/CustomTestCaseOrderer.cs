using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EfIssueSimulatorTests
{
    public class CustomTestCaseOrderer : ITestCaseOrderer
    {
        public const string TypeName = AssembyName + "." + nameof(CustomTestCaseOrderer);

        public const string AssembyName = nameof(EfIssueSimulatorTests);

        public static readonly ConcurrentDictionary<string, ConcurrentQueue<string>>
            QueuedTests = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
            => testCases.OrderBy(GetOrder);

        private static int GetOrder<TTestCase>(TTestCase testCase)
            where TTestCase : ITestCase
        {
            var testMethod = testCase.TestMethod;
            QueuedTests.GetOrAdd(testMethod.TestClass.Class.Name, key => new ConcurrentQueue<string>())
                .Enqueue(testMethod.Method.Name);

            return testMethod.Method.ToRuntimeMethod().GetCustomAttribute<OrderAttribute>()?.Order ?? 0;
        }
    }
}