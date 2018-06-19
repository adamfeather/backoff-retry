using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace backoff_retry
{
    public class BackoffRetry
    {
        public BackoffRetry()
        {
        }

        public bool Attempt()
        {
            var list_of_integers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            IEnumerable<Foo> result =
                list_of_integers
                    .Where(GreaterThanFive)
                    .Select(NewFooObject);

            return true;
        }

        private Foo NewFooObject(int i)
        {
            return new Foo(i);
        }

        private static bool GreaterThanFive(int i)
        {
            return i > 5;
        }
    }
}
