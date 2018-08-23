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
using hostapi.Models.Menu;
using hostapi.Models.Users;
using Microsoft.AspNet.Identity.Owin;
using hostapi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using hostapi.Models.Others.GroupsUsers;
using hostapi.Models.Groups.GroupsUsers;
using hostapi.Classes.Configuracion;

namespace hostapi.Controllers.Configuracion
{
    [Authorize]
    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        //date    locked      
        //null    1		  Reload rigths -> "user locked" util next logon
        //1801    1		  Reload rigths -> bloquedo administrador
        //1800    1		  bloquedo administrador
        //> 1900  1       user lock 3 timmes password tries
        //null    0		  user active

        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        String connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
       
        [Route("users")]
        public async Task<HttpResponseMessage> Users(UsersDataModel model)
        {
            Thread.CurrentThread.CurrentCulture = culture;

            List<UsersQueryModel> resultSeachModel = null;
            UsersByIdModel resultSeachByIdModel = null;
            Users proxy = new Users();
            try
            {
                bool isAdmin = false;
                if (!await Authentication.isAdmin(User, Request))
                {
                    Authentication auth = new Authentication();

                    if (!await auth.AccesRights(User.Identity.GetUserId(), "users", model.type))
                    {
                        return Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                    }
                    auth = null;
                }
                else
                {
                    isAdmin = true;
                }

                string idAdminrole = "";
                if (isAdmin && (model.type == 3 || model.type == 4))
                {
                    var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    idAdminrole = rm.FindByName("Administrador").Id;
                }

                if (model.type == 3)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, 
                        await proxy.New(model.update, Request, connetionString, isAdmin, idAdminrole));
                }
                else if (model.type == 5)
                {
                    await proxy.UpdateIsActive(model.isActive, Request);
                }
                else if (model.type == 6)
                {
                    await proxy.ChangePassword(model.changePassword, Request);
                }
                else
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {

                        using (SqlConnection connection1 = new SqlConnection(connetionString))
                        {
                            await connection1.OpenAsync();

                            if (model.type == 1)
                            {
                                resultSeachModel = new List<UsersQueryModel>();
                                await proxy.SearchQuery(model.search, resultSeachModel, connection1, isAdmin);
                            }
                            else if (model.type == 2)
                            {
                                resultSeachByIdModel = new UsersByIdModel();
                                await proxy.ById(model.byId, connection1, resultSeachByIdModel, isAdmin);
                            }
                            else if (model.type == 4)
                            {
                                await proxy.Update(model.update, connection1, isAdmin, idAdminrole);
                            }
                           

                        }
                        scope.Complete();
                    }
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
