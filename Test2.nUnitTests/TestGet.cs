using Moq;
using Request1;

namespace YourUnitTestsNamespace
{
    [TestFixture]
    public class Program1Tests
    {
        [Test]
        public async Task TestProcessUserInformationAsync_SuccessfulRequest()
        {

            var base_url = "https://sef.podkolzin.consulting/swagger/index.html"; 
            var offset = 0;

            var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

            mockHttpClientWrapper.Setup(m => m.GetAsync($"{base_url}?offset={offset}"))
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var program = new Request(mockHttpClientWrapper.Object);

            async Task CallAsyncMethod()
            {
                await program.ProcessUserInformationAsync();
            }

            await CallAsyncMethod();
        }
    }
}
