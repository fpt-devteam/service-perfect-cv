using FluentAssertions;
using ServerPerfectCV.WebApi.IntegrationTests.TestBase;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ServerPerfectCV.WebApi.IntegrationTests.Controllers
{
    public class OrdersControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        [Fact]
        public async Task List_ReturnsAllOrders_WhenPaginationRequestIsValid()
        {
            // Arrange
            User user = DbInitializer.AddUser(DbContext);
            List<Item> items = DbInitializer.AddItems(DbContext, 10);

            // Create multiple orders
            List<Order> orders = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                orders.Add(DbInitializer.AddOrder(DbContext, user.Id, items));
            }

            PaginationRequest request = new PaginationRequest
            {
                Limit = 10,
                Offset = 0
            };

            // Act
            HttpResponseMessage response = await PostAsJsonAsync("/api/orders/list", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            PaginationData<OrderResponse>? paginationData = await DeserializeResponse<PaginationData<OrderResponse>>(response);
            paginationData.Should().NotBeNull();
            paginationData!.Total.Should().Be(5);
            paginationData.Items.Should().HaveCount(5);
        }

        [Fact]
        public async Task List_ReturnsPaginatedOrders_WhenPageSizeIsLimited()
        {
            // Arrange
            User user = DbInitializer.AddUser(DbContext);
            List<Item> items = DbInitializer.AddItems(DbContext, 10);

            // Create multiple orders
            List<Order> orders = new List<Order>();
            for (int i = 0; i < 10; i++)
            {
                orders.Add(DbInitializer.AddOrder(DbContext, user.Id, items));
            }

            PaginationRequest request = new PaginationRequest
            {
                Limit = 5,
                Offset = 0
            };

            // Act
            HttpResponseMessage response = await PostAsJsonAsync("/api/orders/list", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            PaginationData<OrderResponse> paginationData = await DeserializeResponse<PaginationData<OrderResponse>>(response);
            paginationData.Should().NotBeNull();
            paginationData.Total.Should().Be(10);
            paginationData.Items.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetById_ReturnsOrder_WhenOrderExists()
        {
            // Arrange
            User user = DbInitializer.AddUser(DbContext);
            List<Item> items = DbInitializer.AddItems(DbContext, 5);
            Order order = DbInitializer.AddOrder(DbContext, user.Id, items);

            // Act
            HttpResponseMessage response = await GetAsync($"/api/orders/{order.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            OrderResponse orderResponse = await DeserializeResponse<OrderResponse>(response);
            orderResponse.Should().NotBeNull();
            orderResponse.OrderId.Should().Be(order.Id);
            orderResponse.UserId.Should().Be(user.Id);
            orderResponse.Status.Should().Be(order.Status.ToString());
            orderResponse.Items.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            Guid nonExistentOrderId = Guid.NewGuid();

            // Act
            HttpResponseMessage response = await GetAsync($"/api/orders/{nonExistentOrderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_CreatesNewOrder_WhenRequestIsValid()
        {
            // Arrange
            User user = DbInitializer.AddUser(DbContext);
            List<Item> items = DbInitializer.AddItems(DbContext, 3);

            AuthenticateAsync(user.Id.ToString());

            OrderCreateRequest request = new OrderCreateRequest
            {
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest
                    {
                        ItemId = items[0].Id,
                        Quantity = 2
                    },
                    new OrderItemRequest
                    {
                        ItemId = items[1].Id,
                        Quantity = 1
                    }
                }
            };

            // Act
            HttpResponseMessage response = await PostAsJsonAsync("/api/orders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            Uri location = response.Headers.Location;
            location.Should().NotBeNull();

            // Get the order id from the location header path
            string locationPath = location!.ToString();
            Guid orderId = Guid.Parse(locationPath.Substring(locationPath.LastIndexOf('/') + 1));

            // Verify order was created in the database
            Order createdOrder = await DbContext.Orders.FindAsync(orderId);
            createdOrder.Should().NotBeNull();
            createdOrder!.UserId.Should().Be(user.Id);
        }
    }
}