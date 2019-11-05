using FluentValidation;
using Kernel.Behaviors;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.IO;
using Swashbuckle.AspNetCore.Filters;

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

        public static void AddSwagger<T>(this IServiceCollection services, bool includeXmlComments = false,
            string name = "v1", string title = "My API", string version = "v1")
            => AddSwagger<T>(services, typeof(T).Assembly, includeXmlComments, name, title, version);

        public static void AddSwagger<T>(this IServiceCollection services, Assembly assembly, bool includeXmlComments = false,
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
        }
    }
}
