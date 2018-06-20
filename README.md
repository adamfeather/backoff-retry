# backoff-retry
Backoff and retry logic for .NET

Usage:

The constructor requires an ```Action``` to be passed as the function to attempt with exponetially backed-off retries. An anonymous method which closes over any variables you require should be used.

e.g.

```c#
int i = 10;

var backoff = new BackoffRetry(() => { i++; throw new Exception("Kaboom!"); });

var success = backoff.AttemptExponential(10, TimeSpan.FromMilliseconds(10));

Console.WriteLine(i);

Console.WriteLine($"Success: { success } | Attempts: {backoff.Attempts }");
```
