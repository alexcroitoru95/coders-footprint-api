using Coder_s_Footprint.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Globalization;

namespace Coder_s_Footprint.Controllers
{
    [Authorize]
    [RoutePrefix("api/StackOverflow")]
    public class StackOverflowController : ApiController
    {
        private static int sameDisplayName;
        public static string locationStackOverflow;
        public static string displayNameStackOverflow;

        static StackOverflowController()
        {
            sameDisplayName = 0;
            locationStackOverflow = String.Empty;
            displayNameStackOverflow = String.Empty;
        }

        [AcceptVerbs("POST")]
        [Route("GetUserProfile/")]
        public StackExchangeAPIUserProfile GetUserProfile([FromBody] DisplayNameRequestModel displayNameRequest)
        {
            DateTime TestedAt = DateTime.Now;

            string displayName = displayNameRequest.DisplayName;
            string email = displayNameRequest.Email;
            displayNameStackOverflow = displayName;

            var total_points = PlatformsController.GetPoints(true, 2);
            var extra_points = 0;
            var multipleUsersWithSameName = false;

            if (displayName == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            int userId = GetUsernameID(displayName);

            if(sameDisplayName > 1)
            {
                multipleUsersWithSameName = true;
            }

            int questions = GetUserQuestions(userId);

            int answers = GetUserAnswers(userId);

            int comments = GetUserComments(userId);

            string location = GetUserLocation(userId);

            if (questions > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 15);
                total_points += extra_points;
            }

            if (answers > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 16);
                total_points += extra_points;
            }

            if (comments > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 17);
                total_points += extra_points;
            }

            PlatformsController.CalculateTotalPoints(true, email, "StackOverflow", total_points);

            locationStackOverflow = RemoveDiacritics(location);

            StackExchangeAPIUserProfile stackOverflowAPIUserProfile = new StackExchangeAPIUserProfile
            {
                DisplayName = displayName,
                MultipleUsersWithSameName = multipleUsersWithSameName,
                UserId = userId,
                Questions = questions,
                Answers = answers,
                Comments = comments,
                Location = location,
                Total_Points = total_points,
                Tested_At = TestedAt.ToString("dd/MM/yyyy HH:mm")
            };

            return stackOverflowAPIUserProfile;
        }

        [AcceptVerbs("POST")]
        [Route("GetUsersWithSameDisplayName/")]
        public JArray GetUsersWithSameDisplayName([FromBody] DisplayNameRequestModel displayNameRequest)
        {
            string displayName = displayNameRequest.DisplayName;

            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetUsernameId + displayName + "&site=stackoverflow&filter=!)RwcIFN0yiUg3RiQONI8nIpT";

            JArray userArray;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken json_response_object = JObject.Parse(reader.ReadToEnd());
                userArray = (JArray)json_response_object["items"];
            }

            return userArray;
        }

        [AcceptVerbs("POST")]
        [Route("SearchUserById/")]
        public StackExchangeAPIUserProfile SearchUserById([FromBody] StackOverflowUserIdRequestModel userIdRequest)
        {
            DateTime TestedAt = DateTime.Now;

            string email = userIdRequest.Email;
            int userId = userIdRequest.UserId;

            var total_points = PlatformsController.GetPoints(true, 2);
            var extra_points = 0;

            if (userId == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            string displayName = GetDisplayNameBySearchingId(userId);

            int questions = GetUserQuestions(userId);

            int answers = GetUserAnswers(userId);

            int comments = GetUserComments(userId);

            string location = GetUserLocation(userId);

            if (questions > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 15);
                total_points += extra_points;
            }

            if (answers > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 16);
                total_points += extra_points;
            }

            if (comments > 0)
            {
                extra_points = PlatformsController.GetPoints(true, 17);
                total_points += extra_points;
            }

            PlatformsController.CalculateTotalPoints(true, email, "StackOverflow", total_points);

            locationStackOverflow = RemoveDiacritics(location);

            StackExchangeAPIUserProfile stackOverflowAPIUserProfile = new StackExchangeAPIUserProfile
            {
                DisplayName = displayName,
                UserId = userId,
                Questions = questions,
                Answers = answers,
                Comments = comments,
                Location = location,
                Total_Points = total_points,
                Tested_At = TestedAt.ToString("dd/MM/yyyy HH:mm")
            };

            return stackOverflowAPIUserProfile;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetUsernameID(string displayName)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetUsernameId + displayName + "&site=stackoverflow&filter=!23IYXAR8SntfO)zqiR8*P";

            int userId = 0, json_response_user_id = 0;

            sameDisplayName = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken json_response_object = JObject.Parse(reader.ReadToEnd());

                JArray userArray = (JArray)json_response_object["items"];

                foreach (JObject user in userArray.Children<JObject>())
                {
                    sameDisplayName++;

                    foreach (JProperty userProperties in user.Properties())
                    {
                        if (userProperties.Name == "user_id")
                        {
                            json_response_user_id = (int)userProperties.Value;
                        }

                        if (userProperties.Name == "display_name")
                        {
                            if ((string)userProperties.Value == displayName)
                            {
                                userId = json_response_user_id;
                            }
                        }
                    }
                }
            }

            return userId;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetDisplayNameBySearchingId(int userId)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetNameBySearchingId + userId + "?order=desc&sort=reputation&site=stackoverflow&filter=!T6maJ-S1EnQur548Y5";

            string displayName = String.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken json_response_object = JObject.Parse(reader.ReadToEnd());

                JArray userArray = (JArray)json_response_object["items"];

                foreach (JObject user in userArray.Children<JObject>())
                {
                    foreach (JProperty userProperties in user.Properties())
                    {
                        if (userProperties.Name == "display_name")
                        {
                           displayName = (string)userProperties.Value;
                        }
                    }
                }
            }

            return displayName;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetUserQuestions(int userId)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetQuestions + userId + "/questions?site=stackoverflow&filter=total";

            int questions = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken userArray = JObject.Parse(reader.ReadToEnd());
                questions = (int)userArray["total"];
            }

          return questions;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetUserAnswers(int userId)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetAnswers + userId + "/answers?site=stackoverflow&filter=total";

            int answers = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken userArray = JObject.Parse(reader.ReadToEnd());
                answers = (int)userArray["total"];
            }

            return answers;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetUserComments(int userId)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetComments + userId + "/comments?site=stackoverflow&filter=total";

            int comments = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken userArray = JObject.Parse(reader.ReadToEnd());
                comments = (int)userArray["total"];
            }

            return comments;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetUserLocation(int userId)
        {
            StackExchangeAPIRequestModel stackOverflowAPIRequestModel = new StackExchangeAPIRequestModel();

            string cURL = stackOverflowAPIRequestModel.cUrlGetLocation + userId + "?site=stackoverflow&filter=!)RwcIFN0yiUg3RiQONI8nIpT";

            string location = String.Empty, json_response_location = String.Empty;

            bool sameUserId = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken json_response_object = JObject.Parse(reader.ReadToEnd());

                JArray userArray = (JArray)json_response_object["items"];

                foreach (JObject user in userArray.Children<JObject>())
                {
                    sameUserId = false;

                    foreach (JProperty userProperties in user.Properties())
                    {
                        if (userProperties.Name == "location")
                        {
                            json_response_location = (string)userProperties.Value;
                        }

                        if (userProperties.Name == "user_id")
                        {
                            if ((int)userProperties.Value == userId) 
                            {
                                sameUserId = true;
                            }
                        }

                        if(sameUserId == true)
                        {
                            location = json_response_location;
                        }
                    }
                }
            }

            return location;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static string RemoveDiacritics(string text)
        {
            if (null == text) return null;
            var chars = text
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}