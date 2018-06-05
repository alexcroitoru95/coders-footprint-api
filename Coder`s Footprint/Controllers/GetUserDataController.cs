using Coder_s_Footprint.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Coder_s_Footprint.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/GetUserData")]
    public class GetUserDataController : ApiController
    {
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.Route("SendEmail/")]
        public IHttpActionResult SendEmail([FromBody] EmailRequestModel emailRequest)
        {
            string value = emailRequest.Email;
            string pathToFile = GetPathOfServerDirectoryToSaveCSVFile() + value + ".csv";

            if (!File.Exists(pathToFile))
            {
                CreateCSVFile(value);
            }

            try
            {
                HttpStatusCode responseCode = HttpStatusCode.OK;

                SendEmail(value, pathToFile);

                return Content(responseCode, "The email containing all the user data has been sent successfully!", Configuration.Formatters.JsonFormatter);
            }
            catch (Exception e)
            {
                HttpStatusCode responseCode = HttpStatusCode.Forbidden;
                return Content(responseCode, e.ToString(), Configuration.Formatters.JsonFormatter);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string GetPathOfServerDirectoryToSaveCSVFile()
        {
            var currentDirectory = HostingEnvironment.ApplicationPhysicalPath;
            string pathOfServerDirectoryToSaveCSVFile = currentDirectory + "Resources//UsersCSVFiles//";

            return pathOfServerDirectoryToSaveCSVFile;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static void SendEmail(string email, string pathToFile)
        {
            HyperLink link = new HyperLink() { NavigateUrl = "https://codersfootprintapp.azurewebsites.net/", Text = "Coder's Footprint Official Webpage" };
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            link.RenderControl(htw);
            sw.Close();
            String linkWithHyperlink = sw.ToString();

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("codersfootprint@softstatetechnologies.com");
            mail.To.Add(email);
            mail.Subject = "Your Data on Coder's Footprint";
            mail.Body = "<b>You will find attached a .csv file containing all your data that has been processed on Coder's Footprint.</b><br /><br />" +
                "Visit our webpage for more information: " + linkWithHyperlink + "<br /><br />" +
                "<b>Have a nice day,</b><br />" +
                "<b>Coder's Footprint Team</b>";

            Attachment attachment;
            attachment = new Attachment(pathToFile)
            {
                Name = "Your Data on Coder's Footprint.csv",
            };
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("alexandru.croitoru@softstatetechnologies.com", "Meepefynu85p@");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static void CreateCSVFile(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString.ToString();
            var selectQuery = "SELECT * FROM dbo.Log WHERE [E-mail]='" + email + "'";

            string filePath = GetPathOfServerDirectoryToSaveCSVFile() + email + ".csv";

            var table = ReadTable(connectionString, selectQuery);
            WriteToFile(table, filePath, false, ",");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static DataTable ReadTable(string connectionString, string selectQuery)
        {
            var returnValue = new DataTable();

            var conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                var command = new SqlCommand(selectQuery, conn);

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(returnValue);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return returnValue;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = false, string seperator = ";")
        {
            var sw = new StreamWriter(fileOutputPath, false);

            int icolcount = dataSource.Columns.Count;

            if (!firstRowIsColumnHeader)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    sw.Write(dataSource.Columns[i]);
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }

                sw.Write(sw.NewLine);
            }

            foreach (DataRow drow in dataSource.Rows)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    if (!Convert.IsDBNull(drow[i]))
                        sw.Write(drow[i].ToString());
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}