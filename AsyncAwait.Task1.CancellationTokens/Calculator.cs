using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal static class Calculator
{
    // todo: change this method to support cancellation token
    public static long Calculate(int n /*, CancellationToken token*/)
    {
        long sum = 0;

        for (var i = 0; i < n; i++)
        {
            // i + 1 is to allow 2147483647 (Max(Int32)) 
            sum = sum + (i + 1);
            Thread.Sleep(10);
        }

        return sum;
    }


    public static Task<long> CalculateAsync(int n,  CancellationToken token) {

        return Task.Run(async () =>
        {
            long sum = 0;
            for (var i = 0; i < n; i++)
            {
                // i + 1 is to allow 2147483647 (Max(Int32)) 
                sum = sum + (i + 1);
                token.ThrowIfCancellationRequested();
                await Task.Delay(1000, token);
            }
            return sum;
        }, token);
    }
}
