using System;
using System.Collections.Generic;
using System.Threading;

namespace backoff_retry
{
    public class BackoffRetry
    {
        private Action _functionToAttempt;

        private List<Exception> _exceptions;

        public int RetryAttempts { get; private set; }

        public IEnumerable<Exception> Exceptions => _exceptions;

        public BackoffRetry(Action functionToAttempt)
        {
            this._exceptions = new List<Exception>();

            this._functionToAttempt = functionToAttempt;
        }

        /// <summary>
        /// Attempt operation with exponentially increasing backoff times. 
        /// Intital iteration happens immediately; backoffs only begin to occur if intial attempt is unsuccessful.
        /// </summary>
        /// <param name="maxRetryAttempts"></param>
        /// <param name="initialBackoff"></param>
        /// <returns>If the </returns>
        public bool AttemptExponential(int maxRetryAttempts, TimeSpan initialBackoff)
        {
            var backoffs = GenerateExponentialBackoffs(maxRetryAttempts, initialBackoff);

            foreach (var backoff in backoffs)
            {
                try
                {
                    Thread.Sleep(backoff);

                    this._functionToAttempt();

                    return true;
                }
                catch (Exception ex)
                {
                    this._exceptions.Add(ex);

                    bool firstAttempt = backoffs.IndexOf(backoff) == 0;

                    if (!firstAttempt)
                    {
                        RetryAttempts++;
                    }
                }
            }

            return false;
        }

        private IList<TimeSpan> GenerateExponentialBackoffs(int maxAttempts, TimeSpan backoffSeed)
        {
            var backoffs = new List<TimeSpan>()
                {
                    TimeSpan.FromTicks(0)
                };

            for (int i = 0; i < maxAttempts; i++)
            {
                var multiplier = (int)Math.Pow(2, i);

                backoffs.Add(TimeSpan.FromTicks(backoffSeed.Ticks * multiplier));
            }

            return backoffs;
        }
    }
}
