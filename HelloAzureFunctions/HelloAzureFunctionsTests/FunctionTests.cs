using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
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

        [TestCase("", "Did not serialise the request correctly. No zen for you today")]
        [TestCase("Bob Smith", "Thankyou for contributing, Bob Smith")]
        public async Task AzureFunction_TestZenMessage(string pusherName, string expected)
        {
            // Arrange
            var request = CreateHttpRequest(pusherName);

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
            Assert.That(response.Value, Is.EqualTo(expected));
            Assert.IsTrue("My first Azure functions end point has been called!".Equals(logMessage));

        }

        private HttpRequest CreateHttpRequest(string pusherName = "")
        {
            var context = new DefaultHttpContext();
            var request = context.Request;

            if(!string.IsNullOrEmpty(pusherName))
            {
                var root = new Root();
                root.pusher = new Pusher();
                root.pusher.name = pusherName;

                request.Body = SerializeToStream(root);
            }


            return request;
        }

        private MemoryStream SerializeToStream(object o)
        {
            var jsonString = JsonConvert.SerializeObject(o);

            byte[] byteArray = Encoding.ASCII.GetBytes(jsonString);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

    }
}