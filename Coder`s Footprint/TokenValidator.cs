using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Coder_s_Footprint
{
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            HttpContent content;

            //determine whether a jwt exists or not
            if (!TryRetrieveToken(request, out string token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                //allow requests with no token - whether a action method needs an authentication can be set with the claimsauthorization attribute
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                const string sec = "6vfkszunr3d3p46kh778aszjqkr9466m5b5m6rx3ea3g3382veg7b2pgb9j93qqdt2heds8aw7f9yvjyzzk6cfxur6nmaf8sgrsqkt546nsn3f3kazg9gqga9pt586wqjb8wvtzwsb76htrfj2afesp8ksfgnmrnt27h5j5884av6a8kfqaeqcfestzd4dp4288z2dwaucgy8hegtxjuryhvsts89nd7fmmes9445mejdh8588wev85tacs2e2eq";
                var now = DateTime.UtcNow;
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));


                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = "http://localhost:60483",
                    ValidIssuer = "http://localhost:60483",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };
                //extract and assign the user of the jwt
                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                var errorMessage = "Invalid token authentication! Please try again.";
                var json = JsonConvert.SerializeObject(errorMessage);
                content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.InternalServerError;
                var errorMessage = "There seems to be some server issues! Please try again later.";
                var json = JsonConvert.SerializeObject(errorMessage);
                content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { Content = content });
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    }
}