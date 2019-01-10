using System;
using System.Collections.Generic;
using System.Threading;

namespace backoff_retry
{
    public class BackoffRetry
    {
        private Action _functionToAttempt;

        private List<Exception> _exceptions;

        public int Attempts { get; private set; } = 0;

        public IEnumerable<Exception> Exceptions => _exceptions;

        public BackoffRetry(Action functionToAttempt)
        {
            this._exceptions = new List<Exception>();

            this._functionToAttempt = functionToAttempt;
        }

        /// <summary>
        /// Attempt operation with exponentially increasing backoff times. 
        /// Initial iteration happens immediately; backoffs only begin to occur if initial attempt is unsuccessful.
        /// </summary>
        /// <param name="maxAttempts">The amount of times to try the operation, this includes the initial immediate attempt. This means a minimum of 2 attempts.</param>
        /// <param name="initialBackoff">The initial amount of time to backoff. This increases exponentially with each attempt.</param>
        /// <returns>If the operation succeeded during one of the attempts.</returns>
        public bool AttemptExponential(int maxAttempts, TimeSpan initialBackoff)
        {
            if (maxAttempts < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Must request a minimum of 2 attempts.");
            }

            var backoffs = GenerateExponentialBackoffs(maxAttempts, initialBackoff).GetEnumerator();

            do
            {
                Attempts++;

                try
                {
                    this._functionToAttempt();

                    return true;
                }
                catch (Exception ex)
                {
                    this._exceptions.Add(ex);

                    Thread.Sleep(backoffs.Current);
                }
            } while (backoffs.MoveNext());

            return false;
        }

        private IList<TimeSpan> GenerateExponentialBackoffs(int maxAttempts, TimeSpan initialBackoff)
        {
            var backoffs = new List<TimeSpan>();

            for (int i = 0; i < maxAttempts - 1; i++)
            {
                var multiplier = (int)Math.Pow(2, i);

                backoffs.Add(TimeSpan.FromTicks(initialBackoff.Ticks * multiplier));
            }

            return backoffs;
        }
    }
}
