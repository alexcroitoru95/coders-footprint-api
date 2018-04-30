using Coder_s_Footprint.Models;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using OpenQA.Selenium.PhantomJS;
using System.Linq;
using OpenQA.Selenium.Support.UI;

namespace Coder_s_Footprint.Controllers
{
    [Authorize]
    [RoutePrefix("api/Platforms")]
    public class PlatformsController : ApiController
    {
        private static List<int> ListOfPoints;
        private static List<string> ListOfPlatformsTested;
        private static Random random = new Random();

        static PlatformsController()
        {
            ListOfPoints = new List<int>();
            ListOfPlatformsTested = new List<string>();
        }

        [AcceptVerbs("POST")]
        [Route("BugCrowd/")]
        public PlatformApiCollection BugCrowdChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = true;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 10);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://tracker.bugcrowd.com/user/sign_up");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var user_email = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#tracker_user_email")));
                user_email.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#tracker_user_password")));
                user_password.SendKeys("FakePassw0rD1@");

                var user_password_confirm = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#tracker_user_password_confirmation")));
                user_password_confirm.SendKeys("FakePassw0rD1@");

                var submit_button = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#new_tracker_user > div > fieldset:nth-child(2) > input.bc-btn.bc-btn--block")));
                submit_button.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#new_tracker_user > div > fieldset:nth-child(2) > div.field-error-messages > div")));

                if (error.Text.Contains("has already been taken"))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch(Exception ex)
            {
                if(ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }
            
            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("BugCrowd", value, exists, TestedAt, GetPoints(exists, 5), timedOut)
            };

            CalculateTotalPoints(exists, value, "BugCrowd", GetPoints(exists, 5));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("Microsoft/")]
        public PlatformApiCollection MicrosoftChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = true;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 10);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://signup.live.com/signup?wa=wsignin1.0&wtrealm=urn%3afederation%3aMicrosoftOnline&wctx=estsredirect%3d2%26estsrequest%3drQIIAbPSyigpKSi20tdPTE7OL80r0SvPzEvJLy9OrCotStVLzs_Vyy9Kz0wBsaLYgYJJmTk5SYyGRUJcAi8NHT40Gba57pHO3dXv_8dsFiNHTmYZWNMqRj1C5uoHlyYVJxdlFpRk5ucVX2BkfMHI2MXEYmhgbLyJiSXYMcDzBFPzSblbTIL-RemeKeHFbqkpqUWJINWPmHhDi1OL_PNyKkPys1PzJjHz5eSnZ-bFFxelxafl5JcDBYA2FCQml8SXZCZnp5bsYlYxMUpJNLdMMdI1SzQ21DUxNbLUTTQzN9Q1NE82MzQwMzI3T7Y8wLKB8wKLwC0W1tTE4kyjHyyMi1iBHk1Q_1ki7PPJo6VN--j0NV_fnWLVz9COCMwrNc_N1i71jnAKMi2pzMit9M_Rdklxc_JM9Un3Dw_RLijy9_Uo8bQ1tTLcxUlieAAA0&id=&cbcxt=azubill&lw=1&fl=easi2&bk=1468239878&uaid=f04031e931824586bc1b6dba8f4ffc36&cru=https%3a%2f%2flogin.live.com%2flogin.srf%3fwa%3dwsignin1.0%26wtrealm%3durn%253afederation%253aMicrosoftOnline%26wctx%3destsredirect%253d2%2526estsrequest%253drQIIAbPSyigpKSi20tdPTE7OL80r0SvPzEvJLy9OrCotStVLzs_Vyy9Kz0wBsaLYgYJJmTk5SYyGRUJcAi8NHT40Gba57pHO3dXv_8dsFiNHTmYZWNMqRj1C5uoHlyYVJxdlFpRk5ucVX2BkfMHI2MXEYmhgbLyJiSXYMcDzBFPzSblbTIL-RemeKeHFbqkpqUWJINWPmHhDi1OL_PNyKkPys1PzJjHz5eSnZ-bFFxelxafl5JcDBYA2FCQml8SXZCZnp5bsYlYxMUpJNLdMMdI1SzQ21DUxNbLUTTQzN9Q1NE82MzQwMzI3T7Y8wLKB8wKLwC0W1tTE4kyjHyyMi1iBHk1Q_1ki7PPJo6VN--j0NV_fnWLVz9COCMwrNc_N1i71jnAKMi2pzMit9M_Rdklxc_JM9Un3Dw_RLijy9_Uo8bQ1tTLcxUlieAAA0%26id%3d%26cbcxt%3dazubill%26lw%3d1%26fl%3deasi2%26uaid%3df04031e931824586bc1b6dba8f4ffc36%26lc%3d1033&mkt=EN-US&lc=1033&sl=1&lic=1");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var user_email = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("MemberName")));
                user_email.SendKeys(value);

                var submit_button = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#iSignupAction")));
                submit_button.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#MemberNameError")));

                if ((error.Text.Contains("is already a Microsoft account.")) || (error.Text.Contains("Try another name or")))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("Microsoft", value, exists, TestedAt, GetPoints(exists, 6), timedOut)
            };

            CalculateTotalPoints(exists, value, "Microsoft", GetPoints(exists, 6));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("StackOverflow/")]
        public PlatformApiCollection StackOverflowChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = true;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 5);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://openid.stackexchange.com/account/register");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var user_email = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("email")));
                user_email.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("password")));
                user_password.SendKeys("FakePassw0rD1@");

                var user_password_confirm = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("password2")));
                user_password_confirm.SendKeys("FakePassw0rD1@");

                var submit_button = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#mainbar > div.registration-form > form > table > tbody > tr:nth-child(6) > td > input")));
                submit_button.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#mainbar > div.error > p")));

                if ((error.Text.Contains("Email already in use.")))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("StackOverflow", value, exists, TestedAt, GetPoints(exists, 2), timedOut)
            };

            CalculateTotalPoints(exists, value, "Stack Overflow", GetPoints(exists, 2));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("Apple/")]
        public PlatformApiCollection AppleDeveloperChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = false;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 10);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://appleid.apple.com/account#!&page=create");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var user_email = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[type=email]")));
                user_email.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("password")));
                user_password.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("span.form-message")));

                if ((error.Text.Contains("This email address is not available. Choose a different address.")))
                {
                    driver.Quit();
                    exists = true;
                }
                else
                {
                    driver.Quit();
                    exists = false;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("Apple", value, exists, TestedAt, 5, timedOut)
            };

            CalculateTotalPoints(exists, value, "Apple", GetPoints(exists, 3));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("XDADev/")]
        public PlatformApiCollection XDAChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = true;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 10);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://forum.xda-developers.com/register.php");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var username = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#regusername")));
                username.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#regpassword")));
                user_password.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#reg_verif_div")));

                if (error.Text.Contains("That username is already in use"))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("XDA Developer", value, exists, TestedAt, GetPoints(exists, 3), timedOut)
            };

            CalculateTotalPoints(exists, value, "XDA Developer", GetPoints(exists, 3));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("Codecademy/")]
        public PlatformApiCollection Codecademy([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = false;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 5);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://www.codecademy.com/");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var username = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("user[username]")));
                username.SendKeys(RandomString(15));

                var user_email = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("user[email]")));
                user_email.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("user[password]")));
                user_password.SendKeys(RandomString(15));

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("ul[class^='fieldErrors']")));

                if (error.Text.Contains("Email is already taken"))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("Codecademy", value, exists, TestedAt, GetPoints(exists, 4), timedOut)
            };

            CalculateTotalPoints(exists, value, "Codecademy", GetPoints(exists, 4));

            return PAC;
        }

        [AcceptVerbs("POST")]
        [Route("Google/")]
        public PlatformApiCollection GoogleAccountChecker([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            bool exists = false, timedOut = false, driverOld = false;

            var webDriverWaitUntilTimeout = new TimeSpan(0, 0, 10);

            DateTime TestedAt = DateTime.Now;

            if (value == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            var driver = new PhantomJSDriver(GetPhantomJSDriverService(driverOld));

            driver.Navigate().GoToUrl("https://accounts.google.com/signup/v2/webcreateaccount?service=ahsid&continue=https%3A%2F%2Fdevelopers.google.com%2F%3Frefresh%3D1&flowName=GlifWebSignIn&flowEntry=SignUp&hl=en");

            WebDriverWait wait = new WebDriverWait(driver, webDriverWaitUntilTimeout);

            try
            {
                var user_first_name = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#firstName")));
                user_first_name.SendKeys(RandomString(15));

                var user_last_name = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#lastName")));
                user_last_name.SendKeys(RandomString(15));

                var username = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#username")));
                username.SendKeys(value);

                var user_password = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("Passwd")));
                user_password.SendKeys("FakePassw0rD1@");

                var user_password_confirm = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("ConfirmPasswd")));
                user_password_confirm.SendKeys("FakePassw0rD1@");

                var submit_button = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#accountDetailsNext > content > span")));
                submit_button.Click();

                var error = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#view_container > form > div.mbekbe.bxPAYd > div > div.RCum0c > div:nth-child(2) > div.fQxwff > div > div.LXRPh > div.dEOOab.RxsGPe")));

                if(error.Text.Contains("That username is taken. Try another."))
                {
                    driver.Quit();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is WebDriverException)
                {
                    timedOut = true;
                }

                driver.Quit();
                exists = false;
            }

            PlatformApiCollection PAC = new PlatformApiCollection
            {
                Platforms = GetPlatformModel("Google", value, exists, TestedAt, GetPoints(exists, 7), timedOut)
            };

            CalculateTotalPoints(exists, value, "Google", GetPoints(exists, 7));

            return PAC;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static void TakeScreenshot(IWebDriver driver, String screenshotName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile("E:\\" + screenshotName + ".jpg", ScreenshotImageFormat.Jpeg);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetPoints(bool exists, int id)
        {
            int points = 0;

            if(exists == false)
            {
                return points;
            }
            else
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.PlatformPoints WHERE Id=@id;")
                    {
                        Connection = connection
                    };
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (SqlDataReader sqlReader = command.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            points = sqlReader.GetInt32(2);
                        }
                    }
                };

                return points;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static void CalculateTotalPoints(bool hasAccount, string email, string testedPlatformName, int points)
        {
            ModelLogDatabase modelLogDB = new ModelLogDatabase();
            StringBuilder concatenateTestedPlatforms = new StringBuilder();
            DateTime testedAt = DateTime.Now;

            if (hasAccount == false)
            {
               points = 0;
            }

            ListOfPoints.Add(points);
            ListOfPlatformsTested.Add(testedPlatformName);

            if (testedPlatformName.Contains("Google"))
            {
                foreach(string testedPlatform in ListOfPlatformsTested)
                {
                    if (testedPlatform.Contains("Google"))
                    {
                       concatenateTestedPlatforms.Append(testedPlatform);
                    }
                    else
                    {
                        concatenateTestedPlatforms.Append(testedPlatform).Append("+");
                    }
                }

                foreach(int currentScore in ListOfPoints)
                {
                    modelLogDB.TotalPoints += currentScore;
                }

                modelLogDB.TestedAt = testedAt.ToString("dd/MM/yyyy HH:mm");
                modelLogDB.Email = email;

                AddToCodersFootprintLogDB(modelLogDB.TestedAt, modelLogDB.Email, concatenateTestedPlatforms.ToString(), modelLogDB.TotalPoints);

                ListOfPoints.Clear();
                ListOfPlatformsTested.Clear();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static void AddToCodersFootprintLogDB(string testedAt, string email, string platformsTested, int totalPoints)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("INSERT INTO dbo.Log([Tested At], [E-mail], [Platforms Tested], [Total Points]) VALUES(@testedAt,@email,@platformsTested,@totalPoints);")
                {
                    Connection = connection
                };

                command.Parameters.AddWithValue("@testedAt", testedAt);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@platformsTested", platformsTested);
                command.Parameters.AddWithValue("@totalPoints", totalPoints);

                connection.Open();
                command.ExecuteNonQuery();
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static List<PlatformApi> GetPlatformModel(string platformName, string email, bool exists, DateTime testedAt, int points, bool timedOut)
        {
            List<PlatformApi> platformModel = new List<PlatformApi>
            {
                new PlatformApi()
                {
                    Platform_Name = platformName,
                    Tested_Email = email,
                    Has_Account = exists,
                    Points = points,
                    Tested_At = testedAt.ToString("dd/MM/yyyy HH:mm"),
                    Web_Driver_Wait_Until_Timed_Out = timedOut,
                }
            };

            return platformModel;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetDriverLocationofPhantomJSVersion2_1_1()
        {
            var currentDirectory = HostingEnvironment.ApplicationPhysicalPath;
            string phantomJSDriverDirectory = currentDirectory + "Resources//PhantomJS Driver//2.1.1//";

            return phantomJSDriverDirectory;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetDriverLocationofPhantomJSVersion1_9_8()
        {
            var currentDirectory = HostingEnvironment.ApplicationPhysicalPath;
            string phantomJSDriverDirectory = currentDirectory + "Resources//PhantomJS Driver//1.9.8//";

            return phantomJSDriverDirectory;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static PhantomJSDriverService GetPhantomJSDriverService(bool driverOld)
        {
            if(driverOld == true)
            {
                var service = PhantomJSDriverService.CreateDefaultService(GetDriverLocationofPhantomJSVersion1_9_8());
                service.IgnoreSslErrors = true;
                service.LoadImages = false;

                return service;
            }
            else
            {
                var service = PhantomJSDriverService.CreateDefaultService(GetDriverLocationofPhantomJSVersion2_1_1());
                service.IgnoreSslErrors = true;
                service.LoadImages = false;

                return service;
            }
            
        }
    }
}