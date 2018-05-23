using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Coder_s_Footprint.Models
{
    public class Models
    {
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailRequestModel
    {
        public string Email { get; set; } 
    }

    public class DisplayNameRequestModel
    {
        public string DisplayName { get; set; }
    }

    public class EmailRequestCode
    {
        public EmailRequestCode()
        {
            this.ResponseMsg = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.UnsupportedMediaType };
        }
        public HttpResponseMessage ResponseMsg { get; set; }
    }

    public class LoginResponseCode
    {
        public LoginResponseCode()
        {
            this.ResponseMsg = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized };
        }
        public HttpResponseMessage ResponseMsg { get; set; }
    }

    public class GeneratedTokenResponse
    {
        public DateTime Issued_At { get; set; }
        public string Expires_At { get; set; }
        public string Requesting_Token { get; set; }
        public string Token { get; set; }
    }

    public class PlatformApiCollection
    {
        public List<PlatformApi> Platforms { get; set; }
    }

    public class PlatformApi
    {
        public string Platform_Name { get; set; }
        public bool Has_Account { get; set; }
        public int Points { get; set; }
        public string Tested_Email { get; set; }
        public string Tested_At { get; set; }
        public bool Web_Driver_Wait_Until_Timed_Out { get; set; }
    }

    public class ModelLogDatabase
    {
        public string Email { get; set; }
        public string TestedAt { get; set; }
        public int TotalPoints { get; set; }
    }

    public class GitHubAPIRequestModel
    {
        public string cUrlSearchUserByEmail = "https://api.github.com/search/users?q=";
        public string cUrlGetRepositories = "https://api.github.com/users/";
        public string cUrlGetFollowers = "https://api.github.com/users/";
        public string cUrlGetOrganizations = "https://api.github.com/users/";
        public string cUrlGetSubscriptions = "https://api.github.com/users/";
        public string OAuthApplicationClient_Id = "ba7cd06f283f82df0bde";
        public string OAuthApplicationClient_Secret = "dbe777200c04b36a17a885a2e8961af359a1bcb1";
    }

    public class StackExchangeAPIRequestModel
    {
        public string cUrlGetUsernameId = "https://api.stackexchange.com/2.2/users?inname=";
        public string cUrlGetQuestions = "https://api.stackexchange.com/2.2/users/";
        public string cUrlGetAnswers = "https://api.stackexchange.com/2.2/users/";
        public string cUrlGetComments = "https://api.stackexchange.com/2.2/users/";
    }

        public class GitHubAPIUserRepository
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GitHubAPIUserProfile
    {
        public string Username { get; set; }
        public int Organizations { get; set; }
        public int Followers { get; set; }
        public int Subscriptions { get; set; }
        public int TotalRepositories { get; set; }
        public List<GitHubAPIUserRepository> Repositories { get; set; }
        public string Tested_At { get; set; }
    }

    public class StackExchangeAPIUserProfile
    {
        public string DisplayName { get; set; }
        public int UserId { get; set; }
        public int Questions { get; set; }
        public int Answers { get; set; }
        public int Comments { get; set; }
        public string Tested_At { get; set; }
    }
}