using System.Net.Http;
using Isidore.MagicMirror.Users.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Isidore.MagicMirror.API.Tests.Users
{
    public class UsersControllerTests
    {
        private HttpClient _client;
        private TestServer _testServer;

        public UsersControllerTests()
        {
            _testServer = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>());
            _client = _testServer.CreateClient();
        }
    }
}
