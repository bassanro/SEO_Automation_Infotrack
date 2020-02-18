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
    /// Finds the ranking of text to match the URL on Google website specific implementation 
    /// Uses Selenium tool to launch an instance of chrome and calculate the ranking.
    /// </summary>
    public class RatingServiceGoogle : IRatingService
    {
        private readonly string url;
        private ChromeDriver _chromeDriver;
        private bool _intialized = false;
        private const string HttpsGoogle = "https://www.google.com";

        public RatingServiceGoogle(string url)
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
                throw new System.Exception("Problem initializing Google Rating service");
            }

        }

        public Task<int> GetRanking(string word)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!_intialized)
                {
                    Init(HttpsGoogle);
                }

                int ranking = 1;
                try
                {
                    // search by "q" name.
                    _chromeDriver.FindElement(By.Name("q")).SendKeys(word);
                    _chromeDriver.FindElement(By.Name("q")).SendKeys(Keys.Enter);

                    var listOfLinks = new List<string>();
                    bool searchResult = false;
                    do
                    {
                        searchResult = GetLink(ref ranking, ref listOfLinks);
                        _chromeDriver.FindElementByXPath("//*[@id=\"pnnext\"]/span[2]").Click();
                    } while (!searchResult);

                    //Clean text area to search for next element. 
                    _chromeDriver.FindElementByXPath("//*[@id=\"tsf\"]/div[2]/div[1]/div[2]/div/div[2]/input").Clear();
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
                var searchSites = _chromeDriver.FindElements(By.ClassName("TbwUpd"));
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
                        var link = site.FindElement(By.ClassName("iUh30")).GetAttribute("innerText");
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
