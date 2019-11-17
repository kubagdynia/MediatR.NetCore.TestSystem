using Microsoft.Extensions.Configuration;
using FluentValidation;
using Kernel.Behaviors;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.IO;
using Swashbuckle.AspNetCore.Filters;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Kernel.Configurations;

namespace Kernel.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKernel(this IServiceCollection services)
            => AddKernel(services, Array.Empty<Assembly>());

        public static IServiceCollection AddKernel(this IServiceCollection services, Assembly assembly, bool registerValidators = false)
            => AddKernel(services, new[] { assembly }, registerValidators);

        public static IServiceCollection AddKernel(this IServiceCollection services, Assembly[] assemblies, bool registerValidators = false)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetExecutingAssembly() };
            }

            if (registerValidators)
            {
                services.AddValidatorsFromAssemblies(assemblies);
            }

            services.AddMediatR(assemblies);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            return services;
        }

        public static IServiceCollection AddSwagger<T>(this IServiceCollection services, bool includeXmlComments = false,
            string name = "v1", string title = "My API", string version = "v1")
            => AddSwagger<T>(services, typeof(T).Assembly, includeXmlComments, name, title, version);

        public static IServiceCollection AddSwagger<T>(this IServiceCollection services, Assembly assembly, bool includeXmlComments = false,
            string name = "v1", string title = "My API", string version = "v1")
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: name, new OpenApiInfo { Title = title, Version = version });

                if (includeXmlComments)
                {
                    c.ExampleFilters();

                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }

            });

            if (includeXmlComments)
            {
                services.AddSwaggerExamplesFromAssemblyOf<T>();
            }

            return services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
        {
            HangfireConfiguration? hangfireConfiguration = config.GetSection("Hangfire").Get<HangfireConfiguration>();

            if (hangfireConfiguration is null)
            {
                throw new ArgumentNullException(nameof(hangfireConfiguration));
            }

            services.AddHangfire(x => x.UseSqlServerStorage(hangfireConfiguration.ConnectionString));

            services.AddHangfireServer(options =>
            {
                if (hangfireConfiguration.MaxDefaultWorkerCount > 0)
                {
                    options.WorkerCount = Math.Min(Environment.ProcessorCount * 5, hangfireConfiguration.MaxDefaultWorkerCount);
                }
                
                if (hangfireConfiguration.Queues != null && hangfireConfiguration.Queues.Length > 0)
                {
                    options.Queues = hangfireConfiguration.Queues;
                }
                else
                {
                    options.Queues = new[] { "default" };
                }
            });

            return services;
        }
    }
}
