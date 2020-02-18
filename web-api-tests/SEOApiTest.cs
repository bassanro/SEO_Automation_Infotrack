using Xunit;
using System.Net;
using System.Linq;
using SEO_Automation;
using System.Net.Http;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace web_api_tests
{
    public class SEOApiTest
    {
        private readonly HttpClient _client;

        public SEOApiTest(ITestOutputHelper testOutputHelper)
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _client = server.CreateClient();
        }

        private bool isCachedHeader(HttpResponseMessage response)
        {
            HttpHeaders headers = response.Headers;
            IEnumerable<string> values;
            string isCached = "false";
            if (headers.TryGetValues("CACHE", out values))
            {
                isCached = values.First();
            }
            return bool.Parse(isCached);
        }

        /// <summary>
        ///  This test performs basic validation check. If any one of the paramater(searchOption) of API is mising it should
        ///  return BadRequest.
        /// </summary>
        /// <param name="method"></param>
        [Theory]
        [InlineData("GET")]
        public async Task SEOValidateTestAsync(string method)
        {
            // Arrange
            var validateRequest = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements&url=infotrack");

            // Act
            var validateResponse = await _client.SendAsync(validateRequest);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, validateResponse.StatusCode);
        }

        /// <summary>
        /// This test confirms that the sanity checks are working. Basic functionality is working. 
        /// We also confirm that cache data is correctly fetched and validated.
        /// </summary>
        /// <param name="method"></param>
        [Theory]
        [InlineData("GET")]
        public async Task SEOGetCacheTestAsync(string method)
        {


            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements&url=infotrack&searchOption=1");
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(isCachedHeader(response));

            // Cached request
            var cacheRequest = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements&url=infotrack&searchOption=1");

            // Act
            var cacheResponse = await _client.SendAsync(cacheRequest);

            // Assert
            cacheResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, cacheResponse.StatusCode);
            Assert.True(isCachedHeader(cacheResponse));
        }


        /// <summary>
        /// This verifies that multiple keywords in seach string works as expected. 
        /// Also if the ranking exists in cache for a keyword, then relevant header is also returned to identiy the same.
        /// </summary>
        /// <param name="method"></param>
        [Theory]
        [InlineData("GET")]
        public async Task SEOGetMultipleKeywordTestAsync(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements, settlement&url=infotrack&searchOption=1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(isCachedHeader(response));

            // Cached request
            var cacheRequest = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements,settlement&url=infotrack&searchOption=1");

            // Act
            var cacheResponse = await _client.SendAsync(cacheRequest);

            // Assert
            cacheResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, cacheResponse.StatusCode);
            Assert.True(isCachedHeader(cacheResponse));
        }


        /// <summary>
        /// Santiy check to see that the functionality is working fine for Bing provider as well.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("GET")]
        public async Task SEOGetMultipleKeywordBingTestAsync(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/searchRating?searchString=e-settlements, settlement&url=infotrack&searchOption=2");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(isCachedHeader(response));
        }
    }
}
