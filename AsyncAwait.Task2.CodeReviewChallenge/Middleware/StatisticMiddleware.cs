using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;
        await _statisticService.RegisterVisitAsync(path);
        long count = await _statisticService.GetVisitsCountAsync(path);
        Console.WriteLine("Path {0}",path);
        Console.WriteLine("Count {0}", count);
        context.Response.Headers.Add(
                CustomHttpHeaders.TotalPageVisits,
                count.ToString());
        /// Убрали использование Task.Run так так это не CPU bound task
        /// sleep также убрали так как он не нужен 

        //var staticRegTask = Task.Run(
        //    () => _statisticService.RegisterVisitAsync(path)
        //        .ConfigureAwait(false)
        //        .GetAwaiter().OnCompleted(UpdateHeaders));
        //Console.WriteLine(staticRegTask.Status); // just for debugging purposes

        //void UpdateHeaders()
        //{
        //    context.Response.Headers.Add(
        //        CustomHttpHeaders.TotalPageVisits,
        //        _statisticService.GetVisitsCountAsync(path).GetAwaiter().GetResult().ToString());
        //}

        //Thread.Sleep(3000); // without this the statistic counter does not work
        await _next(context);
    }
}
