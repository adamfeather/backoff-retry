using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace backoff_retry.tests.UnitTests
{
    [TestFixture]
    public class Success
    {
        [Test]
        public void Should_Suceed_On_First_Attempt()
        {
            Action successfulFunction = () => { Thread.Sleep(10); };

            var backoffRetry = new BackoffRetry(successfulFunction);

            bool result = backoffRetry.AttemptExponential(10, TimeSpan.FromMilliseconds(10));

            Assert.That(result, Is.True);
            Assert.That(backoffRetry.Attempts, Is.EqualTo(1));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void Should_Succeed_After_Specified_Attempt(int succeedOnAttempt)
        {
            int attempt = 1;

            Action successfulFunction = () =>
            {
                if (attempt++ < succeedOnAttempt)
                {
                    throw new Exception("Not time to succeed yet");
                }
            };

            var backoffRetry = new BackoffRetry(successfulFunction);

            bool result = backoffRetry.AttemptExponential(10, TimeSpan.FromMilliseconds(10));

            Assert.That(result, Is.True);
            Assert.That(backoffRetry.Attempts, Is.EqualTo(succeedOnAttempt));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(succeedOnAttempt - 1));
        }
    }
}
