using NUnit.Framework;
using System;
using System.Linq;

namespace backoff_retry.tests.UnitTests
{
    [TestFixture]
    public class Failure
    {
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(6)]
        public void Should_Fail_Initital_Attempt_And_Each_Explictely_Specified_Backoff_Retry(int maxAttempts)
        {
            Action failingFunction = () => throw new Exception("function failed");

            var backoffRetry = new BackoffRetry(failingFunction);

            bool result = backoffRetry.AttemptExponential(maxAttempts, TimeSpan.FromMilliseconds(10));

            Assert.That(result, Is.False);
            Assert.That(backoffRetry.Attempts, Is.EqualTo(maxAttempts));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(maxAttempts));
        }

        [Test]
        public void Should_Throw_ArgumentOutOfRangeException_When_Requesting_Only_One_Attempt()
        {
            var backoffRetry = new BackoffRetry(() => { });

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => backoffRetry.AttemptExponential(1, TimeSpan.FromMilliseconds(10)));
        }
    }
}
