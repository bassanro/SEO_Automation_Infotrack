using System.Threading.Tasks;

namespace SEO_Automation.Service
{


    /// <summary>
    /// Interface for calculating the rating for different providers.
    /// </summary>
    public interface IRatingService
    {
        Task<int> GetRanking(string word);
    }
}
