using NUnit.Framework;
using System;
using System.Linq;

namespace backoff_retry.tests.UnitTests
{
    [TestFixture]
    public class Failure
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Should_Fail_Initital_Attempt_And_Each_Explictely_Specified_Backoff_Retry(int maxAttempts)
        {
            Action failingFunction = () => throw new Exception("function failed");

            var backoffRetry = new BackoffRetry(failingFunction);

            bool result = backoffRetry.AttemptExponential(maxAttempts, TimeSpan.FromMilliseconds(100));

            Assert.That(result, Is.False);
            Assert.That(backoffRetry.RetryAttempts, Is.EqualTo(maxAttempts));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(maxAttempts + 1));
        }
    }
}
