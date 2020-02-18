using System;
using System.Net;
using Newtonsoft.Json;
using SEO_Automation.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SEO_Automation.Service.Factory;
using Microsoft.Extensions.Caching.Memory;

namespace SEO_Automation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchRatingController : Controller
    {

        private readonly ILogger<SearchRatingController> _logger;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _options;

        public SearchRatingController(ILogger<SearchRatingController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
            _options = new MemoryCacheEntryOptions

            {
                SlidingExpiration = TimeSpan.FromHours(1), // cache will expire in 1 hr.
            };
        }



        //https://localhost:44327/api/searchRating?searchString=e-settlements&url=www.sympli.com&searchOption=1
        /// <summary>
        /// This API returns the ranking of text against a url in different types of Search Enginer(E.g. Google, Bing)
        /// </summary>
        /// <param name="uri"></param>
        /// 
        /// URI object: 
        ///            1) searchString: The text to search
        ///            2) url : the url to find the ranking with the given search text. 
        ///            3) searchOption : The search provider to use viz. a) Google - 1 b) Bing - 2 (Defined in SearchType enum)
        /// 
        /// <returns>
        /// The serialized JSON response with the original searchString, url and calculated raking. 
        /// </returns>
        /// <error>
        /// Throws RestException: NoContent, BadRequest on validation or other generic error.
        ///                       This is caught by middleware error handling pipeline.
        /// </error>

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery]URI uri)
        {
            var ranking = await LookupRankingByKeyword(uri.searchString, uri.url, uri.searchOption);
            string jsonResult = JsonConvert.SerializeObject(new JSONResult(uri.searchString, uri.url, ranking), Formatting.Indented);

            if (string.IsNullOrEmpty(jsonResult))
            {
                throw new RestException(HttpStatusCode.NoContent, new { activity = "Could not find activity" });
            }

            return new OkObjectResult(jsonResult);
        }



        /// <summary>
        /// This either searches the ranking in in-memory cache or calcualate the new one from searchProvider.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="url"></param>
        /// <param name="searchOption"></param>
        /// <returns>
        /// Async - The List of ranking for various keyword seperated by comma.
        /// </returns>
        private async Task<List<int>> LookupRankingByKeyword(string searchString, string url, int searchOption)
        {
            var rankingList = new List<int>();
            string[] words = searchString.Split(",");

            var ratingService = RatingService.ExecuteCreation((SearchType)searchOption, url);

            foreach (var word in words)
            {
                if (!_cache.TryGetValue($"{word}-{url}-{searchOption}", out int ranking))
                {
                    Console.WriteLine("Cache miss....loading from service into cache");
                    var result = await ratingService.GetRanking(word);
                    rankingList.Add(result);
                    ranking = result;

                    _cache.Set($"{word}-{url}-{searchOption}", ranking, _options);
                }
                else
                {
                    Console.WriteLine("Cache hit");
                    Response.Headers.Add("CACHE", "true");
                    rankingList.Add(ranking);
                }
            }
            return rankingList;
        }
    }
}