using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace TodoListWebApp.Tests.UnitTests {

    [TestClass]
    public class TodoListPageTests {

        private IWebDriver driver;
        private string appURL;

        public TodoListPageTests () { }

        [TestMethod]
        [TestCategory ("Chrome")]
        public void TestAddTodoItem () {
            driver.Navigate ().GoToUrl (appURL + "/Todo");
            driver.FindElement (By.Name ("item")).SendKeys ("Todo Item 1");
            driver.FindElement (By.Id ("submit")).Click ();
            ReadOnlyCollection<IWebElement> elements = driver.FindElements (By.XPath ("//td"));
            Assert.IsTrue (elements != null);
            Assert.IsTrue (elements.Count >= 1);

            bool found = false;
            foreach (IWebElement element in elements) {
                if (element.Text == "Todo Item 1") {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue (found, "Verified add item");
        }

        [TestInitialize ()]
        public void SetupTest () {
            // appURL = "https://mcw-todo-app.azurewebsites.net";
            appURL = "http://localhost:8484";
            string driverLocation;

            string browser = "Chrome";
            switch (browser) {
                case "Firefox":
                    FirefoxOptions opts = new FirefoxOptions ();
                    opts.AddArguments ("--headless");
                    driverLocation = Environment.GetEnvironmentVariable ("GeckoWebDriver");
                    driver = driverLocation == null ? new FirefoxDriver (driverLocation, opts) : new FirefoxDriver (opts);
                    break;
                case "IE":
                    driverLocation = Environment.GetEnvironmentVariable ("IEWebDriver");
                    driver = driverLocation == null ? new InternetExplorerDriver (driverLocation) : new InternetExplorerDriver ();
                    break;
                default:
                    ChromeOptions options = new ChromeOptions ();
                    options.AddArguments ("headless");
                    driverLocation = Environment.GetEnvironmentVariable ("ChromeWebDriver");
                    driver = driverLocation == null ? new ChromeDriver (options) : new ChromeDriver (driverLocation, options);
                    break;
            }

        }

        [TestCleanup ()]
        public void CleanupTests () {
            if (driver != null) {
                driver.Quit ();
            }
        }
    }
}