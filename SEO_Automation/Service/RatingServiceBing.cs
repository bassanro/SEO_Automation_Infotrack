using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SEO_Automation.Service
{

    /// <summary>
    /// Finds the ranking of text to match the URL on Bing website specific implementation 
    /// Uses Selenium tool to launch an instance of chrome and calculate the ranking.
    /// </summary>
    public class RatingServiceBing : IRatingService
    {
        private readonly string url;
        private ChromeDriver _chromeDriver;
        private bool _intialized = false;
        private const string HttpsBing = "https://www.bing.com";

        public RatingServiceBing(string url)
        {
            this.url = url;
        }

        public void Init(string searchWebsite)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArguments("--disable-gpu");
                options.AddArguments("--headless");

                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _chromeDriver = new ChromeDriver(path, options);
                _chromeDriver.Navigate().GoToUrl(searchWebsite);
                _intialized = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new System.Exception("Problem initializing Bing Rating service");
            }

        }

        public Task<int> GetRanking(string word)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!_intialized)
                {
                    Init(HttpsBing);
                }

                int ranking = 1;
                try
                {
                    // search by "q" name.
                    _chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(word);
                    _chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Enter);

                    var listOfLinks = new List<string>();
                    bool searchResult = false;
                    do
                    {
                        searchResult = GetLink(ref ranking, ref listOfLinks);
                        _chromeDriver.FindElement(By.ClassName("sw_next")).Click();
                    } while (!searchResult);

                    //Clean text area to search for next element. 
                    _chromeDriver.FindElement(By.ClassName("sb_form_q")).Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine("OOPS. BROKE MYSELF", e.Message);
                }

                if (_chromeDriver != null)
                {
                    _chromeDriver.Quit();
                }
                return ranking;
            });
        }

        bool GetLink(ref int ranking, ref List<string> listOfLinks)
        {
            try
            {
                List<IWebElement> searchSites = new List<IWebElement>();
                for (int index = 1; index <= 6; index++)
                {
                    searchSites.Add(_chromeDriver.FindElement(By.XPath("//*[@id=\"b_results\"]/li[" + index + "]/h2/a")));
                }

                foreach (var site in searchSites)
                {
                    try
                    {
                        if (ranking >= 100)
                        {
                            ranking = 0;
                            Console.WriteLine("Top 100 elements selected.");
                            return true;
                        }
                        var link = site.GetAttribute("href").ToString();
                        if (listOfLinks.Exists(x => string.Equals(x, link)))
                        {
                            continue;
                        }
                        listOfLinks.Add(link);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Continue with exception  ${e.Message}");
                        ranking++;
                    }
                }

                int distIndex = listOfLinks.FindIndex(link => link.Contains(url));

                ranking = distIndex;
                if (distIndex != -1)
                {
                    //Found the ranking. Since FindIndex starts from 0 we need to increment by 1.
                    ranking++;
                    Console.WriteLine("Found the URL at rank.", ranking);
                    return true;
                }
                else
                {
                    ranking = listOfLinks.Count;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Some Exception Caught ${e.Message}");
            }
            return false;
        }
    }
}
