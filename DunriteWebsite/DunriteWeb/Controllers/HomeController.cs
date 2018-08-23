using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DunriteWeb.Controllers
{
    public class HomeController : Controller
    {
        List<string> pets = new List<string>();
        List<string> outdor = new List<string>();
        List<string> passion = new List<string>();

        // GET: Home
        public async Task<ActionResult> Index()
        {
            //await GetImages();
            return View();
        }


        #region Email
        public bool SendEmail(string subject, string email, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient();
                server.Host = "smtp.gmail.com";

                mail.IsBodyHtml = true;
                mail.From = new MailAddress(email.ToString());
                mail.To.Add(new MailAddress("info@dunritegames.gg"));
                mail.Subject = subject.ToString();
                mail.Body = "Email: " + email.ToString() + "<br/><br/>" + message.ToString();

                server.Port = 587;
                server.DeliveryMethod = SmtpDeliveryMethod.Network;
                server.UseDefaultCredentials = false;
                server.Credentials = new System.Net.NetworkCredential("info@dunritegames.gg", "!info#2018@");
                server.EnableSsl = true;

                server.Send(mail);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion

        private async Task GetImages()
        {
            pets.Clear();
            outdor.Clear();
            passion.Clear();

            HttpClient clientHttp = new HttpClient();
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            clientHttp.BaseAddress = new Uri("https://ofj7hyky9i.execute-api.us-east-2.amazonaws.com/prod/");

            var task = await clientHttp.GetAsync("dunritegetimage");

            if (task.StatusCode == HttpStatusCode.Unauthorized)
            {
            }
            else
            {
                if (task.StatusCode == HttpStatusCode.OK)
                {

                    task.EnsureSuccessStatusCode();

                    object dec = JsonConvert.DeserializeObject(task.Content.ReadAsStringAsync().Result); // deserializing Json string (it will deserialize Json string)
                    JObject obj = JObject.Parse(dec.ToString());
                    JObject obj2 = JObject.Parse(obj["body"].ToString());
                    var items =  obj2["Contents"];
                    for (int x = 0; x < items.Count(); x++)
                    {
                        string key = obj2["Contents"][x]["Key"].ToString();

                        var temp = key.Split('/');
                        if (temp.Count()  > 1 && temp[1] != "")
                        {
                            if (temp[0] == "Pets")
                            {
                                pets.Add(key);
                            }
                            else if (temp[0] == "Outdoor")
                            {
                                outdor.Add(key);
                            }
                            else if (temp[0] == "Passion")
                            {
                                passion.Add(key);
                            }

                        }

                    }



                }
                else
                {
                }
            }

        }
    }

    
}