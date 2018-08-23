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
using hostapi.Models.Users;

namespace hostapi.Controllers.Configuracion
{
    [Authorize]
    [RoutePrefix("api")]
    public class MenuRighsController : ApiController
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        String connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        [Route("menurightstoken")]
        public async Task<HttpResponseMessage> MenuRightsToken(GroupsDataModel model)
        {
            Thread.CurrentThread.CurrentCulture = culture;

            List<MenuRightsModel> resultRightsModel = null;
            
            try
            {
                bool isAdmin = true;
                if (!await Authentication.isAdmin(User, Request))
                {
                    isAdmin = false;
                }
               
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    using (SqlConnection connection1 = new SqlConnection(connetionString))
                    {
                        await connection1.OpenAsync();

                        resultRightsModel = new List<MenuRightsModel>();
                        if (User.IsInRole("Administrador"))
                        {
                            AddRight("Configuracion/Compañias", resultRightsModel);
                            AddRight("Configuracion/Menu", resultRightsModel);
                            AddRight("Configuracion/Grupos", resultRightsModel);
                            AddRight("Configuracion/Usuarios", resultRightsModel);
                        }
                        else
                        {
                            await GetMenuRights(connection1, User.Identity.GetUserId(), resultRightsModel, isAdmin);
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

           
             return Request.CreateResponse(System.Net.HttpStatusCode.OK, resultRightsModel);
            
        }

        private void AddRight(string menu, List<MenuRightsModel> model)
        {

            MenuRightsModel modelRight = new MenuRightsModel();
            modelRight.menu = menu;
            modelRight.isquery = 1;
            modelRight.isedit = 1;
            modelRight.isnew = 1;
            modelRight.isdelete = 1;
            model.Add(modelRight);

        }

        private async Task GetMenuRights(SqlConnection connection1, string userId, List<MenuRightsModel> model, bool isAdmin)
        {


            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT m.menu, u.isquery, u.isedit, u.isnew, u.isdelete FROM UserRights u " +
                               "left join menu m on u.idmenu = m.id " +
                               "where iduser = @iduser and m.status = 1 " +
                               "order by m.menu";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32(1) == 1)
                {
                    MenuRightsModel menu = new MenuRightsModel();
                    menu.menu = reader.GetString(0);
                    menu.isquery = reader.GetInt32(1);
                    menu.isedit = reader.GetInt32(2);
                    menu.isnew = reader.GetInt32(3);
                    menu.isdelete = reader.GetInt32(4);

                    model.Add(menu);

                }
                
            }

            reader.Close();

        }


    }

}
