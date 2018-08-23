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
using hostapi.Classes.Configuracion;

namespace hostapi.Controllers.Configuracion
{
    [Authorize(Roles = "Administrador")]
    [RoutePrefix("api")]
    public class MenuController : ApiController
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        String connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    
        [Route("menu")]
        public async Task<HttpResponseMessage> Menu(MenuDataModel model)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            List<MenuQueryModel> resultSeachModel = null;
            MenuByIdModel resultSeachByIdModel = null;

            try
            {
                Menu proxy = new Menu();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    using (SqlConnection connection1 = new SqlConnection(connetionString))
                    {
                        await connection1.OpenAsync();

                        if (model.type == 1)
                        {
                            resultSeachModel = new List<MenuQueryModel>();
                            await proxy.SearchQuery(model.search, resultSeachModel, connection1);
                        }
                        else if (model.type == 2)
                        {
                            resultSeachByIdModel = new MenuByIdModel();
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
