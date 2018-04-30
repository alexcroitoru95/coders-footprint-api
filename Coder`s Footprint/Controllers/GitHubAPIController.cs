using Coder_s_Footprint.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;

namespace Coder_s_Footprint.Controllers
{
    [Authorize]
    [RoutePrefix("api/GitHub")]
    public class GitHubAPIController : ApiController
    {
        [AcceptVerbs("POST")]
        [Route("GetUserProfile/")]
        public GitHubAPIUserProfile GetUserProfile([FromBody] EmailRequestModel emailRequest)
        {
            GitHubAPIRequestModel gitHubAPIRequestModel = new GitHubAPIRequestModel();
            DateTime TestedAt = DateTime.Now;

            string user_email = emailRequest.Email;

            if (user_email == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, String.Format("Request Body Error! Empty key/value pair or wrong content type, please use x-www-form-urlencoded.")));
            }

            string cURL = gitHubAPIRequestModel.cUrlSearchUserByEmail + user_email + "&client_id=" + gitHubAPIRequestModel.OAuthApplicationClient_Id + "&client_secret=" + gitHubAPIRequestModel.OAuthApplicationClient_Secret;

            string username = GetUsername(cURL);

            if(username == "0")
            {
                username = "none";

                List<GitHubAPIUserRepository> gitHubAPINoUserRepositories = new List<GitHubAPIUserRepository>();
                gitHubAPINoUserRepositories.Clear();

                GitHubAPIUserProfile gitHubAPINoUserProfile = new GitHubAPIUserProfile
                {
                    Username = username,
                    Organizations = 0,
                    Followers = 0,
                    Subscriptions = 0,
                    TotalRepositories = 0,
                    Repositories = gitHubAPINoUserRepositories,
                    Tested_At = TestedAt.ToString("dd/MM/yyyy HH:mm")
                };

                return gitHubAPINoUserProfile;
            }
            else
            {
                List<GitHubAPIUserRepository> gitHubAPIUserRepositoriesCollection = GetUserRepositories(username);

                int total_number_of_repositories = gitHubAPIUserRepositoriesCollection.Count;

                int followers = GetFollowers(username);

                int subscriptions = GetSubscriptions(username);

                int organizations = GetOrganizations(username);

                GitHubAPIUserProfile gitHubAPIUserProfile = new GitHubAPIUserProfile
                {
                    Username = username,
                    Organizations = organizations,
                    Followers = followers,
                    Subscriptions = subscriptions,
                    TotalRepositories = total_number_of_repositories,
                    Repositories = gitHubAPIUserRepositoriesCollection,
                    Tested_At = TestedAt.ToString("dd/MM/yyyy HH:mm")
                };

                return gitHubAPIUserProfile;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetUsername(string cURL)
        {
            string user_exists = string.Empty;
            string username = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Coder's Footprint";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JToken json_response_object = JObject.Parse(reader.ReadToEnd());

                user_exists = (string)json_response_object.First;

                if(user_exists == "0")
                {
                    return user_exists;
                }

                username = (string)json_response_object.SelectToken("items[0].login");

                return username;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static List<GitHubAPIUserRepository> GetUserRepositories(string username)
        {
            GitHubAPIRequestModel gitHubAPIRequestModel = new GitHubAPIRequestModel();

            List<GitHubAPIUserRepository> gitHubAPIUserRepositories = new List<GitHubAPIUserRepository>();
            List<int> repositoryId = new List<int>();
            List<string> repositoryName = new List<string>();

            int totalRepositories = 0;
            string cUrl = gitHubAPIRequestModel.cUrlGetRepositories + username + "/repos?client_id=" + gitHubAPIRequestModel.OAuthApplicationClient_Id + "&client_secret=" + gitHubAPIRequestModel.OAuthApplicationClient_Secret;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cUrl);
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Coder's Footprint";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JArray repositoriesArray = JArray.Parse(reader.ReadToEnd());

                foreach (JObject repository in repositoriesArray.Children<JObject>())
                {
                    foreach (JProperty repositoryProperties in repository.Properties())
                    {
                        if (repositoryProperties.Name == "id")
                        {
                            repositoryId.Add((int)repositoryProperties.Value);
                            totalRepositories++;
                        }
                        if (repositoryProperties.Name == "name")
                        {
                            repositoryName.Add((string)repositoryProperties.Value);
                        }
                    }
                }
            }

            for (int i = 0; i < repositoryId.Count; i++)
            {
                GitHubAPIUserRepository newGitHubAPIUserRepository = new GitHubAPIUserRepository()
                {
                    Id = repositoryId[i],
                    Name = repositoryName[i]
                };

                gitHubAPIUserRepositories.Add(newGitHubAPIUserRepository);
            }

            return gitHubAPIUserRepositories;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetFollowers(string username)
        {
            GitHubAPIRequestModel gitHubAPIRequestModel = new GitHubAPIRequestModel();

            string cUrl = gitHubAPIRequestModel.cUrlGetFollowers + username + "/followers?client_id=" + gitHubAPIRequestModel.OAuthApplicationClient_Id + "&client_secret=" + gitHubAPIRequestModel.OAuthApplicationClient_Secret;
            int total_number_of_followers = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cUrl);
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Coder's Footprint";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JArray followersArray = JArray.Parse(reader.ReadToEnd());

                foreach (JObject follower in followersArray.Children<JObject>())
                {
                    foreach (JProperty followerProperties in follower.Properties())
                    {
                        if (followerProperties.Name == "login")
                        {
                            total_number_of_followers++;
                        }
                    }
                }
            }

            return total_number_of_followers;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetOrganizations(string username)
        {
            GitHubAPIRequestModel gitHubAPIRequestModel = new GitHubAPIRequestModel();

            string cUrl = gitHubAPIRequestModel.cUrlGetOrganizations + username + "/orgs?client_id=" + gitHubAPIRequestModel.OAuthApplicationClient_Id + "&client_secret=" + gitHubAPIRequestModel.OAuthApplicationClient_Secret;
            int total_number_of_organizations = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cUrl);
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Coder's Footprint";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JArray organizationsArray = JArray.Parse(reader.ReadToEnd());

                foreach (JObject organization in organizationsArray.Children<JObject>())
                {
                    foreach (JProperty organizationProperties in organization.Properties())
                    {
                        if (organizationProperties.Name == "login")
                        {
                            total_number_of_organizations++;
                        }
                    }
                }
            }

            return total_number_of_organizations;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static int GetSubscriptions(string username)
        {
            GitHubAPIRequestModel gitHubAPIRequestModel = new GitHubAPIRequestModel();

            string cUrl = gitHubAPIRequestModel.cUrlGetSubscriptions + username + "/subscriptions?client_id=" + gitHubAPIRequestModel.OAuthApplicationClient_Id + "&client_secret=" + gitHubAPIRequestModel.OAuthApplicationClient_Secret;
            int total_number_of_subscriptions = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cUrl);
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Coder's Footprint";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JArray subscriptionsArray = JArray.Parse(reader.ReadToEnd());

                foreach (JObject subscription in subscriptionsArray.Children<JObject>())
                {
                    foreach (JProperty subscriptionProperties in subscription.Properties())
                    {
                        if (subscriptionProperties.Name == "id")
                        {
                            total_number_of_subscriptions++;
                        }
                    }
                }
            }

            return total_number_of_subscriptions;
        }
    }
}