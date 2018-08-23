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
using hostapi.Models.Groups.GroupsUsers;
using hostapi.Models.Others.GroupsUsers;
using hostapi.Classes.Configuracion;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using hostapi.Models;

namespace hostapi.Controllers.Configuracion
{
    [Authorize]
    [RoutePrefix("api")]
    public class GroupsController : ApiController
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        String connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        [Route("groups")]
        public async Task<HttpResponseMessage> Groups(GroupsDataModel model)
        {
            Thread.CurrentThread.CurrentCulture = culture;

            List<GroupsQueryModel> resultSeachModel = null;
            GroupsByIdModel resultSeachByIdModel = null;
            Grupos proxy = new Grupos();

            try
            {
                if (!await Authentication.isAdmin(User, Request))
                {
                    Authentication auth = new Authentication();

                    if (!await auth.AccesRights(User.Identity.GetUserId(), "groups", model.type))
                    {
                        return Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                    }
                    auth = null;
                }
               
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    using (SqlConnection connection1 = new SqlConnection(connetionString))
                    {
                        await connection1.OpenAsync();
                        
                        if (model.type == 1)
                        {
                            resultSeachModel = new List<GroupsQueryModel>();
                            await proxy.SearchQuery(model.search, resultSeachModel, connection1);
                        }
                        else if (model.type == 2)
                        {
                            resultSeachByIdModel = new GroupsByIdModel();
                            await proxy.ById(model.byId, connection1, resultSeachByIdModel);
                        }
                        else if (model.type == 3)
                        {
                            await proxy.New(connection1, model.update);
                        }
                        else if (model.type == 4)
                        {
                            await proxy.Update(connection1, model.update);
                        }
                        else if (model.type == 5)
                        {
                            await proxy.UpdateIsActive(connection1, model.isActive);
                        }
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                ErrorModel _errors = new ErrorModel();
                _errors.message = ex.Message;
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, _errors);
            }
            catch (Exception ex)
            {
                ErrorModel _errors = new ErrorModel();
                _errors.message = ex.Message;
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, _errors);

            }

            if (model.type == 1)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, resultSeachModel);
            }
            else if (model.type == 2)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, resultSeachByIdModel);
            }

            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            
        }
  
    }

}
