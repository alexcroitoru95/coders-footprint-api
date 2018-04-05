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
        public string Issued_At { get; set; }
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
        public bool Web_Driver_Timed_Out { get; set; }
    }

    public class ModelLogDatabase
    {
        public string Email { get; set; }
        public string TestedAt { get; set; }
        public int TotalPoints { get; set; }
    }
}