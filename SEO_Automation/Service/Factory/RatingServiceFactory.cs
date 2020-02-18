using System.Threading.Tasks;

namespace SEO_Automation.Service.Factory
{

    /// <summary>
    /// Abstract class to defined the creating a concrete class.
    /// </summary>
    public abstract class RatingServiceFactory
    {
        public abstract IRatingService Create(string url);
    }
}
