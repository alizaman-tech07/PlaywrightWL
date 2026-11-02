using System.Collections.Concurrent;
using System.Threading;
using AventStack.ExtentReports;

namespace PlaywrightTest2.Utilities
{
    public static class ExtentTestManager
    {
        private static readonly ConcurrentDictionary<int, ExtentTest> _testMap = new();

        public static ExtentTest GetTest()
        {
            return _testMap[Thread.CurrentThread.ManagedThreadId];
        }

        // Null-safe accessor so Utilities called before BaseTest.Setup wires a test
        // (e.g. during migration) do not throw KeyNotFoundException.
        public static ExtentTest? TryGetTest()
        {
            return _testMap.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var test) ? test : null;
        }

        public static void CreateTest(string testName)
        {
            var test = ExtentManager.GetInstance().CreateTest(testName);
            _testMap[Thread.CurrentThread.ManagedThreadId] = test;
        }
    }
}
