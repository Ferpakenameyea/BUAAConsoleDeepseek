using BUAADeepseekWebAPI;
using BUAADeepseekWebAPI.Credentials;
using Microsoft.Extensions.Configuration;

namespace BUAADeepseek_Test
{
    public class Tests
    {
        private string Cookie = "{Cookie}";

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Tests>()
                .Build();

            Cookie = configuration["UserSecrets:Cookie"];

            if (string.IsNullOrEmpty(Cookie) || Cookie == "{Cookie}")
            {
                throw new DependencyNotProvidedException(nameof(Cookie));
            }
        }

        [Test]
        public async Task GetUserInfoTest_ShouldSuccess()
        {
            var deepSeek = new BUAADeepseek(new MemoryCredentialProvider(Cookie));

            bool chatReceived = false;

            await deepSeek.ChatAsync("Hello!", response =>
            {
                if (!string.IsNullOrEmpty(response.Data?.Answer))
                {
                    chatReceived = true;
                }
            });

            Assert.That(chatReceived, Is.True);
        }

        
    }
}