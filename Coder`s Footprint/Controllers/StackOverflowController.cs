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

namespace Coder_s_Footprint.Controllers
{
    [Authorize]
    [RoutePrefix("api/StackOverflow")]
    public class StackOverflowController : ApiController
    {
        [AcceptVerbs("POST")]
        [Route("GetUserProfile/")]
        public StackExchangeAPIUserProfile GetUserProfile([FromBody] DisplayNameRequestModel displayNameRequest)
        {
            DateTime TestedAt = DateTime.Now;

            string displayName = displayNameRequest.DisplayName;

            if (displayName == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            int userId = GetUsernameID(displayName);

            int questions = GetUserQuestions(userId);

            int answers = GetUserAnswers(userId);

            int comments = GetUserComments(userId);

            StackExchangeAPIUserProfile stackOverflowAPIUserProfile = new StackExchangeAPIUserProfile
            {
                DisplayName = displayName,
                UserId = userId,
                Questions = questions,
                Answers = answers,
                Comments = comments,
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
    }
}