using Kernel.BaseApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Invoices;
using Kernel.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System;

namespace Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddInvoices(registerValidators: true);

            services.AddSwagger<Startup>(includeXmlComments: true, name: "v1", title: "My API", version: "v1");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);            

            app.UseCustomSwagger("/swagger/v1/swagger.json", "My API V1");
        }
    }
}
