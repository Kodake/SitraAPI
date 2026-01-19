using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SITRA.API.Middlewares;
using System.Net;

namespace SITRA.Tests
{
    public class IpBlockingMiddlewareTests
    {
        private readonly MemoryCache _cache;
        private readonly Mock<ILogger<IpBlockingMiddleware>> _loggerMock;

        public IpBlockingMiddlewareTests()
        {
            var options = Options.Create(new MemoryCacheOptions());
            _cache = new MemoryCache(options);
            _loggerMock = new Mock<ILogger<IpBlockingMiddleware>>();
        }
        
        private IConfiguration GetConfig(int maxAttempts = 5, int duration = 1)
        {
             var inMemorySettings = new Dictionary<string, string> {
                {"IpBlocking:MaxFailedAttempts", maxAttempts.ToString()},
                {"IpBlocking:BlockDurationMinutes", duration.ToString()}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        [Fact]
        public async Task InvokeAsync_RealContext_ShouldBlockIp_After10FailedAttempts()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("127.0.0.1");
            
            RequestDelegate next = (ctx) =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            var middleware = new IpBlockingMiddleware(next, _cache, _loggerMock.Object, GetConfig());

            // Act: 10 Failed Requests
            for (int i = 0; i < 5; i++)
            {
                var context = new DefaultHttpContext();
                context.Connection.RemoteIpAddress = ipAddress;
                // Init response status to 200 default
                context.Response.StatusCode = 200; 

                await middleware.InvokeAsync(context);
            }

            // Assert: 11th Request should be 403 Forbidden without calling next
            bool nextCalled = false;
            RequestDelegate nextCheck = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var checkMiddleware = new IpBlockingMiddleware(nextCheck, _cache, _loggerMock.Object, GetConfig());
            var checkContext = new DefaultHttpContext();
            checkContext.Connection.RemoteIpAddress = ipAddress;

            await checkMiddleware.InvokeAsync(checkContext);

            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status403Forbidden, checkContext.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldNotBlock_IfUnderThreshold()
        {
             // Arrange
            var ipAddress = IPAddress.Parse("127.0.0.1");
            
            RequestDelegate next = (ctx) =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            var middleware = new IpBlockingMiddleware(next, _cache, _loggerMock.Object, GetConfig());

            // Act: 9 Failed Requests
            for (int i = 0; i < 9; i++)
            {
                var context = new DefaultHttpContext();
                context.Connection.RemoteIpAddress = ipAddress;
                await middleware.InvokeAsync(context);
            }

            // Assert: 10th Request should still pass to next
            bool nextCalled = false;
            RequestDelegate nextCheck = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var checkMiddleware = new IpBlockingMiddleware(nextCheck, _cache, _loggerMock.Object, GetConfig());
            var checkContext = new DefaultHttpContext();
            checkContext.Connection.RemoteIpAddress = ipAddress;

            await checkMiddleware.InvokeAsync(checkContext);

            Assert.True(nextCalled);
            Assert.Equal(200, checkContext.Response.StatusCode);
        }

         [Fact]
        public async Task InvokeAsync_ShouldNotCount_SuccessfulRequests()
        {
             // Arrange
            var ipAddress = IPAddress.Parse("10.0.0.1");
            
            RequestDelegate next = (ctx) =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            };

            var middleware = new IpBlockingMiddleware(next, _cache, _loggerMock.Object, GetConfig());

            // Act: 20 Success Requests
            for (int i = 0; i < 20; i++)
            {
                var context = new DefaultHttpContext();
                context.Connection.RemoteIpAddress = ipAddress;
                await middleware.InvokeAsync(context);
            }

            // Assert: 21st Request should still pass
            bool nextCalled = false;
            RequestDelegate nextCheck = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var checkMiddleware = new IpBlockingMiddleware(nextCheck, _cache, _loggerMock.Object, GetConfig());
            var checkContext = new DefaultHttpContext();
            checkContext.Connection.RemoteIpAddress = ipAddress;

            await checkMiddleware.InvokeAsync(checkContext);

            Assert.True(nextCalled);
            Assert.NotEqual(StatusCodes.Status403Forbidden, checkContext.Response.StatusCode);
        }
    }
}
