using System.Collections.Generic;

namespace SEO_Automation.Model
{

    /// <summary>
    /// Defines the model class JSONResult used for serializtion while returning the response to front-end.
    /// </summary>
    public class JSONResult
    {
        public string SearchString;
        public string Url;
        public List<int> Ranking;

        public JSONResult(string searchString, string url, List<int> ranking)
        {
            SearchString = searchString;
            Url = url;
            Ranking = ranking;
        }
    }
}
