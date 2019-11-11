using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Invoices.Repositories;
using Kernel.Behaviors;
using Kernel.Exceptions;
using MediatR;
using MediatR.Pipeline;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Invoices.Tests
{
    [TestFixture]
    public class InvoiceOperationsTests
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task If_you_created_X_invoices_All_these_invoices_should_be_added_to_the_repository(int count)
        {
            ServiceProvider serviceProvider = PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMediator mediator = scopedServices.GetRequiredService<IMediator>();

                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await mediator.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "J/01/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                GetInvoicesQueryResponse queryResponse = await mediator.Send(new GetInvoicesQuery());

                // Assert
                queryResponse.Invoices.Should().HaveCount(count);

                // checking if the invoices ids match
                for (int i = 0; i < queryResponse.Invoices.Count(); i++)
                {
                    Guid repoInvoiceId = queryResponse.Invoices.ElementAt(i).Id;
                    Guid createdInvoiceId = createInvoiceResponses[i].Id;

                    repoInvoiceId.ToString().Should().BeEquivalentTo(createdInvoiceId.ToString());
                }
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task All_created_invoices_should_be_able_to_get_by_passing_their_id(int count)
        {
            ServiceProvider serviceProvider = PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMediator mediator = scopedServices.GetRequiredService<IMediator>();

                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await mediator.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "JK/02/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                // Assert
                foreach (var createdInvoice in createInvoiceResponses)
                {
                    GetInvoiceQueryResponse queryResponse = await mediator.Send(new GetInvoiceQuery(createdInvoice.Id));
                    queryResponse.Invoice.Should().NotBeNull();
                    queryResponse.Invoice.Id.ToString().Should().BeEquivalentTo(createdInvoice.Id.ToString());
                }
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Id_should_be_possible_to_delete_all_created_invoices(int count)
        {
            ServiceProvider serviceProvider = PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMediator mediator = scopedServices.GetRequiredService<IMediator>();

                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await mediator.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "JK/02/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                // Assert
                foreach (var createdInvoice in createInvoiceResponses)
                {
                    RemoveInvoiceCommandResponse removeResponse = await mediator.Send(new RemoveInvoiceCommand(createdInvoice.Id));
                    removeResponse.Removed.Should().BeTrue();
                }

                // Repo should be empty
                GetInvoicesQueryResponse queryResponse = await mediator.Send(new GetInvoicesQuery());
                queryResponse.Invoices.Should().HaveCount(0);
            }
        }

        [TestCase("11")]
        [TestCase("XX")]
        [TestCase("000")]
        [TestCase("")]
        [TestCase("0")]
        public async Task Providing_invalid_invoice_number_when_creating_the_invoice_should_thrown_DomainException(string invalidInvoiceNumber)
        {
            ServiceProvider serviceProvider = PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                var mediator = scopedServices.GetRequiredService<IMediator>();

                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();                

                DomainException ex = Assert.ThrowsAsync<DomainException>(async () =>
                {
                    _ = await mediator.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: invalidInvoiceNumber,
                            creationDate: DateTime.Now)));
                });

                ex.DomainErrors.Should().HaveCount(1);
                ex.DomainErrors.First().ErrorCode.Should().BeEquivalentTo("LengthValidator");
                ex.DomainErrors.First().PropertyName.Should().BeEquivalentTo("Invoice.Number");

                GetInvoicesQueryResponse result = await mediator.Send(new GetInvoicesQuery());

                result.Invoices.Should().HaveCount(0);
            }
        }

        private ServiceProvider PrepareServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IInvoiceRepository, InvoiceMemoryRepository>();

            services.AddValidatorsFromAssemblies(new[] { typeof(Invoice).Assembly });

            services.AddMediatR(typeof(Invoice));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}