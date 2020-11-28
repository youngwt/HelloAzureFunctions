using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using NUnit.Framework;

namespace HelloAzureFunctionsTests
{
    public class FunctionTests
    {
        Mock<ILogger> _logger;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
        }

        [Test]
        public void NUnitIsConfigured()
        {
            Assert.Pass();
        }

        [Test]
        public async Task AzureFunctionReturnsOk()
        {
            // Arrange
            var request = CreateHttpRequest();

            // the following log mock verification code was from an answer from SO
            // https://stackoverflow.com/questions/52707702/how-do-you-mock-ilogger-loginformation

            string logMessage = null ;
            _logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception?)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    logMessage = (string?)invokeMethod?.Invoke(formatter, new[] { state, exception });

                }));

            // Act
            var response = (OkObjectResult) await HelloAzureFunctions.HelloAzureFunctions.Run(request, _logger.Object);

            // Assert
            Assert.That(response.Value, Is.EqualTo("Reports Ok"));
            Assert.IsTrue("My first Azure functions end point has been called!".Equals(logMessage));

        }

        private HttpRequest CreateHttpRequest()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            return request;
        }

    }
}