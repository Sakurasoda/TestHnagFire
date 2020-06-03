using System;
using System.Linq;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Management;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // add before session
            services.AddDistributedMemoryCache();

            services.AddHangfire(configuration =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .UseColouredConsoleLogProvider()
                             .UseDashboardMetric(DashboardMetrics.ServerCount)
                             .UseDashboardMetric(DashboardMetrics.RecurringJobCount)
                             .UseDashboardMetric(DashboardMetrics.RetriesCount)

                             //.UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.EnqueuedCountOrNull)
                             //.UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.FailedCountOrNull)
                             .UseDashboardMetric(DashboardMetrics
                                                         .EnqueuedAndQueueCount)
                             .UseDashboardMetric(DashboardMetrics
                                                         .ScheduledCount)
                             .UseDashboardMetric(DashboardMetrics
                                                         .ProcessingCount)
                             .UseDashboardMetric(DashboardMetrics
                                                         .SucceededCount)
                             .UseDashboardMetric(DashboardMetrics.FailedCount)
                             .UseDashboardMetric(DashboardMetrics.DeletedCount)
                             .UseDashboardMetric(DashboardMetrics
                                                         .AwaitingCount)
                             .UseConsole() //from Hangfire.Console
                             .UseMemoryStorage()
                             .UseManagementPages(p => p.AddJobs(() => GetModuleTypes())); //from Hangfire.Dashboard.Management;
            });

            services.AddSingleton(s => new RecurringJobManager());
        }

        private Type[] GetModuleTypes()
        {
            var assemblies = new[] {typeof(Job).Assembly};

            var moduleTypes = assemblies.SelectMany(r =>
            {
                try
                {
                    return r.GetTypes();
                }
                catch (Exception e)
                {
                    return new Type[] { };
                }
            }).ToArray();

            return moduleTypes;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();

            app.UseHangfireDashboard(pathMatch: "/hangfire",
                                     options: new DashboardOptions
                                     {
                                         Authorization = new[] { new MyAuthorizationFilter() }
                                     });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}