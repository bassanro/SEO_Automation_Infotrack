namespace SEO_Automation.Service.Factory
{
    /// <summary>
    /// Specific Implementation of creating a concrete class.
    /// </summary>
    public class RatingServiceGoogleFactory : RatingServiceFactory
    {
        public override IRatingService Create(string url) => new RatingServiceGoogle(url);
    }
}
