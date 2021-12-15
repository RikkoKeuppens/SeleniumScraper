using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose between youtube (y), indeed (i) and extra (x). Pres 'stop' to stop.");
            string siteChoice = Console.ReadLine();
            while (siteChoice != "stop")
            {
                ChromeOptions chOptions = new ChromeOptions();
                //chOptions.AddArgument("--headless");
                chOptions.AddArgument("--disable-notifications");
                chOptions.AddArgument("--silent");
                chOptions.AddArgument("--log-level=3");
                var chromeDriverService = ChromeDriverService.CreateDefaultService("./");
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriverService.SuppressInitialDiagnosticInformation = true;
                IWebDriver driver = new ChromeDriver(chromeDriverService, chOptions);

                if (siteChoice == "y" || siteChoice == "youtube")
                {
                    Console.WriteLine("Choose a video title u want to scrape:");
                    string title = Console.ReadLine();

                    //go to youtube and maximize
                    driver.Navigate().GoToUrl("https://www.youtube.com");
                    driver.Manage().Window.Maximize();
                    var confirm = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[5]/div[2]/ytd-button-renderer[2]/a"));
                    confirm.Click();

                    //search input
                    var element = driver.FindElement(By.XPath("/html/body/ytd-app/div/div/ytd-masthead/div[3]/div[2]/ytd-searchbox/form/div[1]/div[1]/input"));
                    element.Click();
                    element.SendKeys(title);
                    element.Submit();

                    //most recent
                    var recent = driver.FindElement(By.XPath("/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[1]/div[2]/ytd-search-sub-menu-renderer/div[1]/div/ytd-toggle-button-renderer/a/tp-yt-paper-button"));
                    recent.Click();
                    var uploadDate = driver.FindElement(By.XPath("/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[1]/div[2]/ytd-search-sub-menu-renderer/div[1]/iron-collapse/div/ytd-search-filter-group-renderer[5]/ytd-search-filter-renderer[2]/a/div/yt-formatted-string"));
                    uploadDate.Click();
                    System.Threading.Thread.Sleep(1000); //wait 1 second, otherwise it will scrape the most popular videos instead of the most recent

                    Console.WriteLine();
                    for (int i = 1; i < 6; i++)
                    {
                        string videoNumber = i.ToString();

                        string link = "/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + videoNumber + "]/div[1]/div/div[1]/div/h3/a";
                        var videoLink = driver.FindElement(By.XPath(link)).GetDomProperty("href");

                        string Title = "(//*[@id=\"video-title\"]/yt-formatted-string)[" + videoNumber + "]";
                        var videoTitle = driver.FindElement(By.XPath(Title)).Text;

                        string Author = "/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + videoNumber + "]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a";
                        var videoAuthor = driver.FindElement(By.XPath(Author)).Text;

                        string Views = "(//*[@id=\"metadata-line\"]//span[1])[" + videoNumber + "]";
                        string videoViews = driver.FindElement(By.XPath(Views)).Text;

                        Console.WriteLine("----------------------------------------------------------------------------------");
                        Console.WriteLine("Link of the video: " + videoLink);
                        Console.WriteLine("Title of the video: " + videoTitle);
                        Console.WriteLine("Author: " + videoAuthor);
                        Console.WriteLine("Amount of views: " + videoViews);
                    }
                    Console.WriteLine();

                }
                else if (siteChoice == "i" || siteChoice == "indeed")
                {
                    Console.WriteLine("Choose a search term u want to scrape on indeed:");
                    string term = Console.ReadLine();

                    driver.Navigate().GoToUrl("https://be.indeed.com/");
                    driver.Manage().Window.Maximize();

                    var element = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/span/div[3]/div[1]/div/div/form/div/div[1]/div/div[1]/div/div[2]/input"));
                    element.Click();
                    element.SendKeys(term);
                    element.Submit();
                }
                else if (siteChoice == "x")
                {
                    Console.WriteLine("Give your search term");
                    string term = Console.ReadLine();

                    driver.Navigate().GoToUrl("https://www.hln.be/");
                    driver.Manage().Window.Maximize();

                    var confirm = driver.FindElement(By.XPath("/html/body/div/div[2]/div[3]/button[0]"));
                    confirm.Click();

                } else
                {
                    Console.WriteLine("That was a wrong option, try again");
                }
                Console.WriteLine("Choose between youtube (y), indeed (i) and extra (x). Pres 'stop' to stop.");
                siteChoice = Console.ReadLine();
            }
        }
    }
}
