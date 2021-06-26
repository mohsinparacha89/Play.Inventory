using Microsoft.Extensions.DependencyInjection;
using Play.Inventory.Clients;
using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Play.Inventory.Services
{
    public static class ServiceExtensions
    {
        
        public static IServiceCollection AddPlayHttpClient(this IServiceCollection services)
        {
            Random jitterer = new();

            services.AddHttpClient<CatalogClient>(c =>
            {
                c.BaseAddress = new Uri("https://localhost:5001");
            })
                .AddTransientHttpErrorPolicy(builder =>builder.Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000))
                , onRetry: (outcome, timespan, retryAttempt) =>
               {
                    //Log something here...
                }))
                .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                    3,TimeSpan.FromSeconds(10),
                    onBreak : (outcome,TimeSpan) => 
                    { 
                     //log something here...
                    },
                    onReset : ()=>
                    {
                        //Do something...
                    }
                    ))
                  .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
            return services;
        }
    }
}
