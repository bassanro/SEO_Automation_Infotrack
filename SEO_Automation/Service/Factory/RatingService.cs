using System;
using System.Collections.Generic;
using System.Reflection;

namespace SEO_Automation.Service.Factory
{

    /// <summary>
    /// Factory class with thread safe singleton instance to register and define search Provider types.
    /// </summary>
    public class RatingService
    {
        private static readonly Dictionary<SearchType, RatingServiceFactory> _factories = new Dictionary<SearchType, RatingServiceFactory>();
        private static readonly Lazy<RatingService> lazy = new Lazy<RatingService>(() => new RatingService());


        static RatingService()
        {
            var factoryNamespace = Assembly.GetExecutingAssembly().GetName().Name + ".Service.Factory.RatingService";

            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                var type = Type.GetType(factoryNamespace + Enum.GetName(typeof(SearchType), searchType) + "Factory");
                var factory = (RatingServiceFactory)Activator.CreateInstance(type);

                _factories.Add(searchType, factory);
            }
        }

        public static RatingService Instance => lazy.Value;
        public static IRatingService ExecuteCreation(SearchType searchType, string url) => _factories[searchType].Create(url);
    }
}
