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
            Console.WriteLine("Welkom op mijn webscraper");
            Console.WriteLine("Kies een website die je wil scrapen...");
            Console.WriteLine("Opties: youtube(y), indeed(i), newyorktimes(n) of druk 'ctrl + c' om te stoppen");
            string siteChoice = Console.ReadLine();

            ChromeOptions chOptions = new ChromeOptions();
            //chOptions.AddArgument("--headless");
            //chOptions.AddArgument("--disable-notifications");
            //chOptions.AddArgument("--silent");
            chOptions.AddArgument("--log-level=3");
            var chromeDriverService = ChromeDriverService.CreateDefaultService("./");
            chromeDriverService.HideCommandPromptWindow = true;
            //chromeDriverService.SuppressInitialDiagnosticInformation = true;

            while (siteChoice != "")
            {
                if (siteChoice == "y" || siteChoice == "youtube")
                {
                    Console.WriteLine("Welke video wil je zoeken?");
                    string title = Console.ReadLine();

                    //go to youtube and maximize
                    IWebDriver driver = new ChromeDriver(chromeDriverService, chOptions);
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
                    System.Threading.Thread.Sleep(500); //wait half a second, otherwise it will scrape the most popular videos instead of the most recent

                    Console.WriteLine();

                    int counter = 1;
                    int stopCounter = 0;

                    while (counter < 6 && stopCounter < 1)
                    {
                        try
                        {
                            string videoNumber = counter.ToString();

                            string link = "/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + videoNumber + "]/div[1]/div/div[1]/div/h3/a";
                            var videoLink = driver.FindElement(By.XPath(link)).GetDomProperty("href");

                            string Title = "(//*[@id=\"video-title\"]/yt-formatted-string)[" + videoNumber + "]";
                            var videoTitle = driver.FindElement(By.XPath(Title)).Text;

                            string Author = "/html/body/ytd-app/div/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + videoNumber + "]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a";
                            var videoAuthor = driver.FindElement(By.XPath(Author)).Text;

                            string Views = "(//*[@id=\"metadata-line\"]//span[1])[" + videoNumber + "]";
                            string videoViews = driver.FindElement(By.XPath(Views)).Text;

                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Console.WriteLine("Titel van de video: " + videoTitle);
                            Console.WriteLine("Auteur: " + videoAuthor);
                            Console.WriteLine("Aantal weergaven: " + videoViews);
                            Console.WriteLine("Link van de video: " + videoLink);

                            string filepath = "c:/DevOpsScraperOutput/" + title.ToString() + ".csv";
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
                            {
                                file.WriteLine(videoTitle + "," + videoAuthor + "," + videoViews + "," + videoLink);
                            }
                                counter++;
                        }
                        catch
                        {
                            stopCounter++;
                        }
                        if (counter == 1)
                        {
                            Console.WriteLine("Er zijn geen resultaten voor " + title);
                            Console.WriteLine("Probeer het opnieuw:");
                        }
                    }
                    if (counter > 1)
                    {
                        Console.WriteLine("----------------------------------------------------------------------------------");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Opties: youtube(y), indeed(i), newyorktimes(n) of druk 'ctrl + c' om te stoppen");
                    siteChoice = Console.ReadLine();

                }
                else if (siteChoice == "i" || siteChoice == "indeed")
                {
                    Console.WriteLine("Kies de job die je wil opzoeken: ");
                    string term = Console.ReadLine();

                    IWebDriver driver = new ChromeDriver(chromeDriverService, chOptions);
                    driver.Navigate().GoToUrl("https://be.indeed.com/");
                    driver.Manage().Window.Maximize();

                    var element = driver.FindElement(By.XPath("//*[@id=\"text-input-what\"]"));
                    element.Click();
                    element.SendKeys(term);
                    element.Submit();

                    int counter = 1;
                    int stopCounter = 0;

                    while (stopCounter < 1)
                    {
                        try
                        {
                            var button = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td/div/div[2]/div/div[1]/button/div[1]"));
                            button.Click();

                            var lastDays = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td/div/div[2]/div/div[1]/ul/li[2]/a"));
                            lastDays.Click();

                            System.Threading.Thread.Sleep(500); //Wait for popup
                            var closeButton = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/button"));
                            closeButton.Click();

                            System.Threading.Thread.Sleep(500); // wait for newest adds

                            while (counter < 17)
                            {
                                Console.WriteLine("------------------------------------------------------------");
                                string addNumber = counter.ToString();

                                string Title = "//*[@id=\"mosaic-provider-jobcards\"]/a[" + addNumber + "]/div[1]/div/div[1]/div/table[1]/tbody/tr/td/div[1]/h2/span";
                                var addTitle = driver.FindElement(By.XPath(Title)).Text.Trim();

                                string CompanyName = "//*[@id=\"mosaic-provider-jobcards\"]/a[" + addNumber + "]/div[1]/div/div[1]/div/table[1]/tbody/tr/td/div[2]/pre/span";
                                var addCompanyName = driver.FindElement(By.XPath(CompanyName)).Text.Trim();

                                string Location = "//*[@id=\"mosaic-provider-jobcards\"]/a[" + addNumber + "]/div[1]/div/div[1]/div/table[1]/tbody/tr/td/div[2]/pre/div";
                                var addLocation = driver.FindElement(By.XPath(Location)).GetAttribute("innerHTML");

                                string Link = "//*[@id=\"mosaic-provider-jobcards\"]/a[" + addNumber + "]";
                                var addLink = driver.FindElement(By.XPath(Link)).GetAttribute("href").Trim();

                                Console.WriteLine("Title of the add: " + addTitle);
                                Console.WriteLine("Company name: " + addCompanyName);
                                Console.WriteLine("Company location: " + addLocation);
                                Console.WriteLine("link for the add: " + addLink);

                                string filepath = "c:/DevOpsScraperOutput/" + term.ToString() + ".csv";
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
                                {
                                    file.WriteLine(addTitle + "," + addCompanyName + "," + addLocation + "," + addLink);
                                }

                                counter++;
                            }
                        }
                        catch
                        {
                            stopCounter++;
                        }
                    }
                    if (counter == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Er zijn geen resultaten voor " + term);
                        Console.WriteLine("Probeer het opnieuw:");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Opties: youtube(y), indeed(i), newyorktimes(n) of druk 'ctrl + c' om te stoppen");
                    siteChoice = Console.ReadLine();
                }
                else if (siteChoice == "n" || siteChoice == "newyorktimes")
                {
                    Console.WriteLine("Kies een atrikel dat je wil opzoek, gebruik Engels: ");
                    string term = Console.ReadLine();

                    IWebDriver driver = new ChromeDriver(chromeDriverService, chOptions);
                    driver.Navigate().GoToUrl("https://www.nytimes.com/");
                    driver.Manage().Window.Maximize();

                    var search = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/header/section[1]/div[1]/div[2]/button"));
                    search.Click();
                    var searchTerm = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/header/section[1]/div[1]/div[2]/div/form/div/input"));
                    searchTerm.SendKeys(term);
                    var go = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/header/section[1]/div[1]/div[2]/div/form/button"));
                    go.Click();

                    var date = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[1]/div[2]/div/div/div[1]/div/div/button"));
                    date.Click();
                    var yesterday = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div[1]/div[1]/div[2]/div/div/div[1]/div/div/div/ul/li[2]/button"));
                    yesterday.Click();

                    int counter = 1;
                    int stopCount = 0;

                    System.Threading.Thread.Sleep(500); // wait for newest adds
                    Console.WriteLine();
                    while (counter < 10 && stopCount != 2)
                    {
                        try
                        {
                            string addNumber = counter.ToString();

                            string Title = "//*[@id=\"site-content\"]/div[1]/div[2]/div[1]/ol/li[" + addNumber + "]/div/div/div/a/h4";
                            var articleTitle = driver.FindElement(By.XPath(Title)).Text;

                            string Date = "/html/body/div[1]/div[2]/main/div[1]/div[2]/div[1]/ol/li[" + addNumber + "]/div/span";
                            var articleDate = driver.FindElement(By.XPath(Date)).Text;

                            string Author = "//*[@id=\"site-content\"]/div[1]/div[2]/div[1]/ol/li[" + addNumber + "]/div/div/div/a/p[2]";
                            var articleAuthor = driver.FindElement(By.XPath(Author)).Text;

                            string Link = "//*[@id=\"site-content\"]/div[1]/div[2]/div[1]/ol/li[" + addNumber + "]/div/div/div/a";
                            var articleLink = driver.FindElement(By.XPath(Link)).GetAttribute("href");

                            Console.WriteLine("------------------------------------------------------------");
                            Console.WriteLine("Title of the article: " + articleTitle);
                            Console.WriteLine("Date of the article: " + articleDate);
                            Console.WriteLine(articleAuthor);
                            Console.WriteLine("Link to the article: " + articleLink);
                            stopCount = 0;
                        }
                        catch
                        {
                            counter++;
                            stopCount++;
                        }
                        try
                        {   string addNumber = counter.ToString();

                            string Title = "//*[@id=\"site-content\"]/div[1]/div[2]/div[2]/ol/li[" + addNumber + "]/div/div/div/a/h4";
                            var articleTitle = driver.FindElement(By.XPath(Title)).Text;

                            string Date = "//*[@id=\"site-content\"]/div[1]/div[2]/div[2]/ol/li[" + addNumber + "]/div/span";
                            var articleDate = driver.FindElement(By.XPath(Date)).Text;

                            string Author = "//*[@id=\"site-content\"]/div[1]/div[2]/div[2]/ol/li[" + addNumber + "]/div/div/div/a/p[2]";
                            var articleAuthor = driver.FindElement(By.XPath(Author)).Text;

                            string Link = "//*[@id=\"site-content\"]/div[1]/div[2]/div[2]/ol/li[" + addNumber + "]/div/div/div/a";
                            var articleLink = driver.FindElement(By.XPath(Link)).GetAttribute("href");


                            Console.WriteLine("------------------------------------------------------------");
                            Console.WriteLine("Title of the article: " + articleTitle);
                            Console.WriteLine("Date of the article: " + articleDate);
                            Console.WriteLine(articleAuthor);
                            Console.WriteLine("Link to the article: " + articleLink);

                            string filepath = "c:/DevOpsScraperOutput/" + term.ToString() + ".csv";
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
                            {
                                file.WriteLine(articleTitle + "," + articleDate + "," + articleAuthor + "," + articleLink);
                            }

                            stopCount = 0;
                        }
                        catch
                        {
                            counter++;
                            stopCount++;
                        }
                    }
                    if (counter == 3)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Er zijn geen resultaten voor " + term);
                        Console.WriteLine("Probeer het opnieuw:");
                        Console.WriteLine();
                    }
                    if (counter > 3)
                    {
                        Console.WriteLine("----------------------------------------------------------------------------------");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Opties: youtube(y), indeed(i), newyorktimes(n) of druk 'ctrl + c' om te stoppen");
                    siteChoice = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("\"" + siteChoice + "\" is een verkeerde keuze, probeer opnieuw");
                    Console.WriteLine("Opties: youtube(y), indeed(i), newyorktimes(n) of druk 'ctrl + c' om te stoppen");
                    siteChoice = Console.ReadLine();
                }
            }
        }
    }
}
