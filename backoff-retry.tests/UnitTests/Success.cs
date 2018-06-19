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
            Action successfulFunction = () => { Thread.Sleep(100); };

            var backoffRetry = new BackoffRetry(successfulFunction);

            bool result = backoffRetry.AttemptExponential(10, TimeSpan.FromMilliseconds(10));

            Assert.That(result, Is.True);
            Assert.That(backoffRetry.RetryAttempts, Is.EqualTo(0));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void Should_Fail_First_Attempt_And_Then_Succeed_After_Specified_Retry(int succeedOnRetry)
        {
            bool intialAttempt = true;
            int retry = 0;

            Action successfulFunction = () =>
            {
                if (intialAttempt)
                {
                    intialAttempt = false;
                    throw new Exception("Fail on first attempt");
                }

                if (retry++ < succeedOnRetry)
                {
                    throw new Exception("Not time to succeed yet");
                }
            };

            var backoffRetry = new BackoffRetry(successfulFunction);

            bool result = backoffRetry.AttemptExponential(10, TimeSpan.FromMilliseconds(10));

            Assert.That(result, Is.True);
            Assert.That(backoffRetry.RetryAttempts, Is.EqualTo(succeedOnRetry));
            Assert.That(backoffRetry.Exceptions.Count(), Is.EqualTo(succeedOnRetry + 1));
        }
    }
}
