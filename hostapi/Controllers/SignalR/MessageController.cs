using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Transactions;
using hostapi.Models.Others;
using System.Data;
using hostapi.Models.Groups;

namespace hostapi.Controllers.SignalR
{
    [Authorize(Roles = "Administrador")]
    [RoutePrefix("api/Message")]
    public class MessageController : ApiController
    {
        private Microsoft.AspNet.SignalR.IHubContext<NotifyHub> _hubContext;

        public MessageController(Microsoft.AspNet.SignalR.IHubContext<NotifyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public string Post([FromBody]MessageModel model)
        {
            string retMessage = string.Empty;

            try
            {
                _hubContext.Clients.All.Send(model.name, model.message);
                retMessage = "Success";
            }
            catch (Exception e)
            {
                retMessage = e.ToString();
            }

            return retMessage;
        }
    }



}
