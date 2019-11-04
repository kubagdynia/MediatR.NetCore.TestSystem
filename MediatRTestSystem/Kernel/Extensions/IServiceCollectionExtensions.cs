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

        public static void AddSwagger(this IServiceCollection services, string name = "v1", string title = "My API", string version = "v1")
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: name, new OpenApiInfo { Title = title, Version = version });
            });
        }
    }
}
