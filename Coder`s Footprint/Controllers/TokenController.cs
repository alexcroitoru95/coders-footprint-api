using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Coder_s_Footprint.Models;

namespace Coder_s_Footprint.Controllers
{
    public class TokenController : ApiController
    {
        [RequireHttps]
        [HttpPost]
        public IHttpActionResult Authenticate([FromBody] LoginRequest login)
        {
            LoginRequest loginrequest = new LoginRequest { };

            loginrequest.Username = login.Username;
            loginrequest.Password = login.Password;
            
            HttpResponseMessage responseMsg = new HttpResponseMessage();
            bool isUsernamePasswordValid = false;

            if (login != null)
            {
                if (loginrequest.Username == "!S3Lvmue1uhjaT4vr#6fyVIiMO1AijD4hKn2gRObRPGzbF4TSe"
                      && loginrequest.Password == "13NzgZpJT8Unk#q^jz!b9R1Da#h8sit9XgAXsuwEb&Fa&I3L89Rj5^Qp$d^MKVtpHDZ@M6JomuBHsvI2uMWY7GNL@gJ*ATmX1u6MMuF77GvHB9BzD66nJcimn1thhpb$u1$xt256pb&iMTutt#U#d8Q@gR6MwRlx6iD3M*fzZe5WIhy3K@DRMofsD6d#X@MMHOWiv*w2jpdGdSVPoGu2*GOT!E$Mk7S#44eef4xwFBT%VyKo16m6&25rhfP551Oh")
                {
                    isUsernamePasswordValid = true;
                }
                else
                {
                    isUsernamePasswordValid = false;
                }
            }

            // if credentials are valid
            if (isUsernamePasswordValid)
            {
                GeneratedTokenResponse GeneratedTokenResponse = CreateToken(loginrequest.Username);

                //return the token json
                return Ok(GeneratedTokenResponse);
            }
            else
            {
                LoginResponseCode loginResponse = new LoginResponseCode { };

                // if credentials are not valid send unauthorized status code in response
                loginResponse.ResponseMsg.StatusCode = HttpStatusCode.Unauthorized;
                return Content(loginResponse.ResponseMsg.StatusCode, "Incorrect username or password! Please try again.", Configuration.Formatters.JsonFormatter);
            }
        }

        private GeneratedTokenResponse CreateToken(string username)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.Now;
            //set the time when it expires
            DateTime expires = DateTime.Now.AddDays(7);

            //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            const string sec = "6vfkszunr3d3p46kh778aszjqkr9466m5b5m6rx3ea3g3382veg7b2pgb9j93qqdt2heds8aw7f9yvjyzzk6cfxur6nmaf8sgrsqkt546nsn3f3kazg9gqga9pt586wqjb8wvtzwsb76htrfj2afesp8ksfgnmrnt27h5j5884av6a8kfqaeqcfestzd4dp4288z2dwaucgy8hegtxjuryhvsts89nd7fmmes9445mejdh8588wev85tacs2e2eq";

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(issuer: "http://localhost:60483", audience: "http://localhost:60483",
                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);

            GeneratedTokenResponse GeneratedTokenResponse = new GeneratedTokenResponse
            {
                Issued_At = issuedAt.ToString("dd/MM/yyyy HH:mm"),
                Expires_At = expires.ToString("dd/MM/yyyy HH:mm"),
                Requesting_Token = username,
                Token = "Bearer " + tokenString,
            };

            return GeneratedTokenResponse;
        }
    }
}