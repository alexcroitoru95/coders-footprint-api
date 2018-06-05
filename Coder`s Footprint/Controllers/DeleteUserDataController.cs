using Coder_s_Footprint.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Coder_s_Footprint.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/DeleteUserData")]
    public class DeleteUserDataController : ApiController
    {
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.Route("DeleteUser/")]
        public void DeleteUserData([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;

            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM dbo.Log WHERE [E-mail]='" + value + "'")
                {
                    Connection = connection
                };

                connection.Open();
                command.ExecuteNonQuery();
            };
        }
    }
}