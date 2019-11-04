using FluentValidation;
using Kernel.Behaviors;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IO;
using Swashbuckle.AspNetCore.Filters;

namespace Kernel.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKernel(this IServiceCollection services)
            => AddKernel(services, new Assembly[] { }, false);

        public static IServiceCollection AddKernel(this IServiceCollection services, Assembly assembly, bool registerValidators = false)
            => AddKernel(services, new Assembly[] { assembly }, registerValidators);

        public static IServiceCollection AddKernel(this IServiceCollection services, Assembly[] assemblies, bool registerValidators = false)
        {
            if (registerValidators)
            {
                if (assemblies == null)
                {
                    services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, ServiceLifetime.Transient);
                }
                else
                {
                    services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Transient);
                }
            }

            if (assemblies == null || assemblies.Length == 0)
            {
                services.AddMediatR(new[] { Assembly.GetExecutingAssembly() });
            }
            else
            {
                services.AddMediatR(assemblies);
            }            

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            return services;
        }

        public static void AddSwagger<T>(this IServiceCollection services, Assembly assembly, bool includeXmlComments = false, string name = "v1", string title = "My API", string version = "v1")
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
        }
    }
}
