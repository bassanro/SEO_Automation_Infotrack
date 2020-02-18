using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEO_Automation.Service.Factory
{

    /// <summary>
    /// Specific Implementation of creating a concrete class.
    /// </summary>
    public class RatingServiceBingFactory : RatingServiceFactory
    {
        public override IRatingService Create(string url) => new RatingServiceBing(url);
    }
}
