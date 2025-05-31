using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServerPerfectCV.WebApi.IntegrationTests.TestBase;
using Xunit;

namespace ServerPerfectCV.WebApi.IntegrationTests.Controllers
{
    public class OrdersControllerTests : IntegrationTestBase
    {
        public OrdersControllerTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task List_ReturnsAllOrders_WhenPaginationRequestIsValid()
        {
            // Arrange
            var user = DbInitializer.AddUser(DbContext);
            var items = DbInitializer.AddItems(DbContext, 10);
            
            // Create multiple orders
            var orders = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                orders.Add(DbInitializer.AddOrder(DbContext, user.Id, items));
            }
            
            var request = new PaginationRequest
            {
                Limit = 10,
                Offset = 0
            };
            
            // Act
            var response = await PostAsJsonAsync("/api/orders/list", request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var paginationData = await DeserializeResponse<PaginationData<OrderResponse>>(response);
            paginationData.Should().NotBeNull();
            paginationData.Total.Should().Be(5);
            paginationData.Items.Should().HaveCount(5);
        }
        
        [Fact]
        public async Task List_ReturnsPaginatedOrders_WhenPageSizeIsLimited()
        {
            // Arrange
            var user = DbInitializer.AddUser(DbContext);
            var items = DbInitializer.AddItems(DbContext, 10);
            
            // Create multiple orders
            var orders = new List<Order>();
            for (int i = 0; i < 10; i++)
            {
                orders.Add(DbInitializer.AddOrder(DbContext, user.Id, items));
            }
            
            var request = new PaginationRequest
            {
                Limit = 5,
                Offset = 0
            };
            
            // Act
            var response = await PostAsJsonAsync("/api/orders/list", request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var paginationData = await DeserializeResponse<PaginationData<OrderResponse>>(response);
            paginationData.Should().NotBeNull();
            paginationData.Total.Should().Be(10);
            paginationData.Items.Should().HaveCount(5);
        }
        
        [Fact]
        public async Task GetById_ReturnsOrder_WhenOrderExists()
        {
            // Arrange
            var user = DbInitializer.AddUser(DbContext);
            var items = DbInitializer.AddItems(DbContext, 5);
            var order = DbInitializer.AddOrder(DbContext, user.Id, items);
            
            // Act
            var response = await GetAsync($"/api/orders/{order.Id}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var orderResponse = await DeserializeResponse<OrderResponse>(response);
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
            var nonExistentOrderId = Guid.NewGuid();
            
            // Act
            var response = await GetAsync($"/api/orders/{nonExistentOrderId}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Create_CreatesNewOrder_WhenRequestIsValid()
        {
            // Arrange
            var user = DbInitializer.AddUser(DbContext);
            var items = DbInitializer.AddItems(DbContext, 3);
            
            AuthenticateAsync(user.Id.ToString());
            
            var request = new OrderCreateRequest
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
            var response = await PostAsJsonAsync("/api/orders", request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var location = response.Headers.Location;
            location.Should().NotBeNull();
            
            // Get the order id from the location header path
            var locationPath = location!.ToString();
            var orderId = Guid.Parse(locationPath.Substring(locationPath.LastIndexOf('/') + 1));
            
            // Verify order was created in the database
            var createdOrder = await DbContext.Orders.FindAsync(orderId);
            createdOrder.Should().NotBeNull();
            createdOrder!.UserId.Should().Be(user.Id);
        }
    }
}
