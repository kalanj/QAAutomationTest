// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Firefox;
using System.Linq;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace SeleniumCSharpTutorial
{
    [TestFixture]
    public class TestClass 
    {
        IWebDriver driver = null;

    
        [Test, Description("1. Test")]
        public void TestMethod1()
        {
            try
            {
                driver = new FirefoxDriver();
                driver.Manage().Window.Maximize();
                driver.Url = "https://www.google.com/";
                IWebElement element = driver.FindElement(By.Name("q"));
                element.SendKeys("demoqa.com");
                element.Submit();
                
                Thread.Sleep(1000);

                IWebElement firstLink = driver.FindElement(By.XPath("(//*[@class='LC20lb DKV0Md'])[1]"));
                firstLink.Click();
                
                Thread.Sleep(1000);
                
                IWebElement interactions = driver.FindElement(By.XPath("//*[text()='Interactions']"));
                interactions.Click();
                Thread.Sleep(1000);

                driver.Navigate().GoToUrl("https://demoqa.com/droppable");

                var ele1 = driver.FindElement(By.XPath(".//*[@id='draggable']"));
                var ele2 = driver.FindElement(By.XPath("(.//*[@id='droppable'])[1]"));

                Actions builder1 = new Actions(driver);
                IAction dragAndDrop1 = builder1.ClickAndHold(ele1).MoveToElement(ele2).Release(ele1).Build();
                dragAndDrop1.Perform();
                Thread.Sleep(1000);
               

                //Take screenshot and save it to Destop as TestSuccess1.jpeg
                ITakesScreenshot ts = driver as ITakesScreenshot;
                Screenshot screenshot = ts.GetScreenshot();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                screenshot.SaveAsFile(path + "\\TestSuccess1.jpeg", ScreenshotImageFormat.Jpeg);

                driver.Quit();
            }

            catch (Exception e)
            {
                //Take screenshot and save it to Destop as TestFail1.jpeg
                ITakesScreenshot ts = driver as ITakesScreenshot;
                Screenshot screenshot = ts.GetScreenshot();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                screenshot.SaveAsFile(path + "\\TestFail1.jpeg", ScreenshotImageFormat.Jpeg);
                
                //Write StackTrace
                Console.WriteLine(e.StackTrace);
                throw;
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }
        
        [Test, Description("2. Test")]
        public void TestMethod2()
        {
            try
            {
                driver = new FirefoxDriver();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://www.google.com/");
                driver.FindElement(By.Name("q")).SendKeys("cheese" + Keys.Enter);
                wait.Until(webDriver => webDriver.FindElement(By.CssSelector("h3")).Displayed);

                //Get search results-stats and return only results
                IWebElement numberOfResults = driver.FindElement(By.XPath("//*[@id='result-stats']"));
                Regex rgx = new Regex("[^0-9]");
                var stringToNumber = rgx.Replace(numberOfResults.Text, "");
                //Remove seconds how many it took for selected search, last three digits
                stringToNumber = stringToNumber.Remove(stringToNumber.Length - 3); 
                int result = int.Parse(stringToNumber);

                //Compare is result equal to expected 777 with a message they are not and there is to much cheese
                Assert.AreEqual(777, result, "There is too much cheese on the internet");

                driver.Quit();
            }

            catch (Exception e)
            {
                //Take screenshot and save it to Destop as TestFail1.jpeg
                ITakesScreenshot ts = driver as ITakesScreenshot;
                Screenshot screenshot = ts.GetScreenshot();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                screenshot.SaveAsFile(path + "\\TestFail2.jpeg" , ScreenshotImageFormat.Jpeg);
                
                //Write StackTrace
                Console.WriteLine(e.StackTrace);
                throw;
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }

        [Test, Description("3. Test")]
        public void TestMethod3()
        {
            try
            {
                driver = new FirefoxDriver();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Manage().Window.Maximize();
                driver.Url = "https://orangehrm-demo-7x.orangehrmlive.com/";
                IWebElement loginBtn = driver.FindElement(By.Name("Submit"));
                loginBtn.Submit();
                wait.Until(webDriver => webDriver.FindElement(By.CssSelector(" div.menu-visible:nth-child(2)")).Displayed);

                IWebElement recruitmentMenu = driver.FindElement(By.Id("menu_recruitment_viewRecruitmentModule"));
                recruitmentMenu.Click();

                IWebElement candidates = driver.FindElement(By.Id("menu_recruitment_viewCandidates"));
                candidates.Click();

                IWebElement iFrame = driver.FindElement(By.Id("noncoreIframe"));

                driver.SwitchTo().Frame(iFrame);

                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                ResetCandidatesFilter(wait);
                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                int firstResult = GetNumberOfCandidatsFromText();
                Console.WriteLine(firstResult);

                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                IWebElement greenBtn = driver.FindElement(By.XPath("//i[@class='large mdi-content-add material-icons']"));
                greenBtn.Click();


                //Select resume file from Desktop folder
                IWebElement selectResume = driver.FindElement(By.XPath("//*[@id='addCandidate_resume']"));
                try
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\NebojsaK-CV.doc";
                    if (!File.Exists(path))
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            sw.WriteLine("Short Resume");
                            sw.WriteLine("Name: Nebojsa Kalanj");
                            sw.WriteLine("Email: kalanj.nebojsa@gmail.com");
                            sw.WriteLine("");
                            sw.WriteLine("Best regards,");
                            sw.WriteLine("Nebojsa K.");
                        }
                    //Path.ChangeExtension(path, ".doc");
                    selectResume.SendKeys(path);
                }
                catch
                {
                    Console.WriteLine("Resume not created at line 187");
                }


                IWebElement firstName = driver.FindElement(By.XPath("//input[@id='addCandidate_firstName']"));
           
                String name = String.Format("QA Automation - <{0}>", System.DateTime.Today.Date.ToShortDateString());
                firstName.SendKeys(name);

                IWebElement lastName = driver.FindElement(By.XPath("//input[@id='addCandidate_lastName']"));
                lastName.SendKeys("Kalanj");

                IWebElement email = driver.FindElement(By.XPath("//input[@id='addCandidate_email']"));
                email.SendKeys("kalanj.nebojsa@gmail.com");



                IWebElement saveBtn = driver.FindElement(By.XPath("//a[@id='saveCandidateButton']"));
                saveBtn.Click();

                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                ResetCandidatesFilter(wait);
                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                int secondResult = GetNumberOfCandidatsFromText();
                Assert.Greater(secondResult, firstResult, "Number of candidates was {0} now it is {1}", firstResult, secondResult);

                IWebElement checkbox = driver.FindElement(By.XPath("//html[1]/body[1]/div[1]/div[2]/div[1]/div[7]/div[1]/div[2]/table[1]/tbody[1]/tr[1]/td[1]/label[1]"));
                checkbox.Click();
                
                IWebElement threeDots = driver.FindElement(By.XPath("//a[@id='ohrmList_Menu']"));
                threeDots.Click();

                wait.Until(webDriver => webDriver.FindElement(By.Id("ohrmList_dropDownMenu")).Displayed);

                IWebElement deleteCandidate = driver.FindElement(By.XPath("//a[@id='deleteItemBtn']"));
                deleteCandidate.Click();

                wait.Until(webDriver => webDriver.FindElement(By.XPath("//div[@id='modal-delete-candidate']")).Displayed);

                IWebElement yesDelete = driver.FindElement(By.XPath("//a[@id='candidate-delete-button']"));
                yesDelete.Click();

                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);
                ResetCandidatesFilter(wait);
                wait.Until(webDriver => webDriver.FindElement(By.Id("content")).Displayed);

                int thirdResult = GetNumberOfCandidatsFromText();
                Assert.Less(thirdResult, secondResult, "Number of candidates was {0} now it is {1}", secondResult, thirdResult);

                IWebElement openSideMenu = driver.FindElement(By.XPath("//a[@id='user-dropdown']"));
                openSideMenu.Click();

                IWebElement logoutLink = driver.FindElement(By.XPath("//a[@id='logoutLink']"));
                logoutLink.Click();
 
                driver.Quit();
            }

            catch (Exception e)
            {
                ITakesScreenshot ts = driver as ITakesScreenshot;
                Screenshot screenshot = ts.GetScreenshot();

                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                screenshot.SaveAsFile(path + "\\TestFail2.jpeg", ScreenshotImageFormat.Jpeg);

                Console.WriteLine(e.StackTrace);
                throw;
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }

        private void ResetCandidatesFilter(WebDriverWait wait)
        {
            IWebElement searchBtn = driver.FindElement(By.XPath("//a[@id='candidateSearchBtn']"));
            searchBtn.Click();

            wait.Until(webDriver => webDriver.FindElement(By.XPath("//div[@id='candidateSearchDropDown']")).Displayed);

            IWebElement resetBtn = driver.FindElement(By.Id("resetCandidatesFormBtn "));
            resetBtn.Click();

            
        }

        private int GetNumberOfCandidatsFromText()
        {
            //Get search results-stats and return only results
            IWebElement numberOfResults = driver.FindElement(By.XPath("//div[@id='fromToOf']"));
            Regex rgx = new Regex("[^0-9]");
            var stringToNumber = rgx.Replace(numberOfResults.Text, "");
            //Remove seconds how many it took for selected search, last three digits
            stringToNumber = stringToNumber.Substring(stringToNumber.Length - 3);
            int result = int.Parse(stringToNumber);
            return result;
        }
    }
}
