using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Invoices.Repositories;
using Kernel.Exceptions;
using MediatR;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Invoices.Events;
using Invoices.Tests.Fakes;
using Kernel.Messages;

namespace Invoices.Tests
{
    [TestFixture]
    public class InvoiceOperationsTests
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task All_created_invoices_should_be_added_to_the_repository(int count)
        {
            ServiceProvider serviceProvider = TestHelper.PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();


                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "J/01/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                GetInvoicesQueryResponse queryResponse = await messageManager.Send(new GetInvoicesQuery());

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
            ServiceProvider serviceProvider = TestHelper.PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "JK/02/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                // Assert
                foreach (var createdInvoice in createInvoiceResponses)
                {
                    GetInvoiceQueryResponse queryResponse = await messageManager.Send(new GetInvoiceQuery(createdInvoice.Id));
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
            ServiceProvider serviceProvider = TestHelper.PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                // Arrange

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                List<CreateInvoiceCommandResponse> createInvoiceResponses = new List<CreateInvoiceCommandResponse>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "JK/02/2019", creationDate: DateTime.Now)));
                    createInvoiceResponses.Add(createInvoiceResponse);
                }

                // Assert
                foreach (var createdInvoice in createInvoiceResponses)
                {
                    RemoveInvoiceCommandResponse removeResponse = await messageManager.Send(new RemoveInvoiceCommand(createdInvoice.Id));
                    removeResponse.Removed.Should().BeTrue();
                }

                // Repo should be empty
                GetInvoicesQueryResponse queryResponse = await messageManager.Send(new GetInvoicesQuery());
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
            ServiceProvider serviceProvider = TestHelper.PrepareServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();                

                DomainException ex = Assert.ThrowsAsync<DomainException>(async () =>
                {
                    _ = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: invalidInvoiceNumber,
                            creationDate: DateTime.Now)));
                });

                ex.DomainErrors.Should().HaveCount(1);
                ex.DomainErrors.First().ErrorCode.Should().BeEquivalentTo("LengthValidator");
                ex.DomainErrors.First().PropertyName.Should().BeEquivalentTo("Invoice.Number");

                GetInvoicesQueryResponse result = await messageManager.Send(new GetInvoicesQuery());

                result.Invoices.Should().HaveCount(0);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Invoice_created_event_handler_should_be_called_the_same_number_of_times_as_added_invoices(int count)
        {
            // Arrange
            ServiceCollection services = TestHelper.PrepareServiceCollection();

            services.AddSingleton<Counter>();

            // Replace the registered event class
            ServiceDescriptor serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(INotificationHandler<InvoiceCreatedEvent>));
            services.Remove(serviceDescriptor);
            services.AddScoped<INotificationHandler<InvoiceCreatedEvent>, FakeInvoiceCreatedEventHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                // empty repository
                var invoiceRepository = scopedServices.GetRequiredService<IInvoiceRepository>();

                Counter counter = scopedServices.GetRequiredService<Counter>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "J/01/2019", creationDate: DateTime.Now)));
                }

                // Assert
                counter.Get().Should().Be(count);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Get_invoice_handler_should_be_called_the_same_number_of_times_as_get_invoice_query(int count)
        {
            // Arrange
            ServiceCollection services = TestHelper.PrepareServiceCollection();

            services.AddSingleton<Counter>();

            // Replace the registered event class
            ServiceDescriptor serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRequestHandler<GetInvoiceQuery, GetInvoiceQueryResponse>));
            services.Remove(serviceDescriptor);
            services.AddScoped<IRequestHandler<GetInvoiceQuery, GetInvoiceQueryResponse>, FakeGetInvoiceHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                Counter counter = scopedServices.GetRequiredService<Counter>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    GetInvoiceQueryResponse queryResponse = await messageManager.Send(new GetInvoiceQuery(Guid.NewGuid()));
                }

                // Assert
                counter.Get().Should().Be(count);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Get_invoices_handler_should_be_called_the_same_number_of_times_as_get_invoices_query(int count)
        {
            // Arrange
            ServiceCollection services = TestHelper.PrepareServiceCollection();

            services.AddSingleton<Counter>();

            // Replace the registered event class
            ServiceDescriptor serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRequestHandler<GetInvoicesQuery, GetInvoicesQueryResponse>));
            services.Remove(serviceDescriptor);
            services.AddScoped<IRequestHandler<GetInvoicesQuery, GetInvoicesQueryResponse>, FakeGetInvoicesHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                Counter counter = scopedServices.GetRequiredService<Counter>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    GetInvoicesQueryResponse queryResponse = await messageManager.Send(new GetInvoicesQuery());
                }

                // Assert
                counter.Get().Should().Be(count);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Create_invoice_handler_should_be_called_the_same_number_of_times_as_create_invoice_command(int count)
        {
            // Arrange
            ServiceCollection services = TestHelper.PrepareServiceCollection();

            services.AddSingleton<Counter>();

            // Replace the registered event class
            ServiceDescriptor serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>));
            services.Remove(serviceDescriptor);
            services.AddScoped<IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>, FakeCreateInvoiceHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                Counter counter = scopedServices.GetRequiredService<Counter>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    CreateInvoiceCommandResponse createInvoiceResponse = await messageManager.Send(
                        new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: "J/01/2019", creationDate: DateTime.Now)));
                }

                // Assert
                counter.Get().Should().Be(count);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Remove_invoice_handler_should_be_called_the_same_number_of_times_as_remove_invoice_command1(int count)
        {
            // Arrange
            ServiceCollection services = TestHelper.PrepareServiceCollection();

            services.AddSingleton<Counter>();

            // Replace the registered event class
            ServiceDescriptor serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRequestHandler<RemoveInvoiceCommand, RemoveInvoiceCommandResponse>));
            services.Remove(serviceDescriptor);
            services.AddScoped<IRequestHandler<RemoveInvoiceCommand, RemoveInvoiceCommandResponse>, FakeRemoveInvoiceHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                IMessageManager messageManager = scopedServices.GetRequiredService<IMessageManager>();

                Counter counter = scopedServices.GetRequiredService<Counter>();

                // Act
                for (int i = 0; i < count; i++)
                {
                    RemoveInvoiceCommandResponse removeResponse = await messageManager.Send(new RemoveInvoiceCommand(Guid.NewGuid()));
                }

                // Assert
                counter.Get().Should().Be(count);
            }
        }
    }
}