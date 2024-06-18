using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Persistence.Context;
using Persistence.IRepository;
using System;

namespace Api.Tests
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        public IDepthChartCommandRepository DepthChartCommandRepository;
        public IDepthChartQueryRepository DepthChartQueryRepository;
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                AddRepositories(services);
            });

            return base.CreateHost(builder);
        }

        private void AddRepositories(IServiceCollection services)
        {
            if (DepthChartCommandRepository != null)
                services.AddSingleton(DepthChartCommandRepository);
            if (DepthChartQueryRepository != null)
                services.AddSingleton(DepthChartQueryRepository);
        }
    }
}
