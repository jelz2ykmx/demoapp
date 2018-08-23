using hostapi.Models;
using hostapi.Models.Others;
using hostapi.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace hostapi.Classes.Configuracion
{
    public class Authentication 
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        String connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        internal static async Task<Boolean> isAdmin(IPrincipal User, HttpRequestMessage Request)
        {
            string userId = User.Identity.GetUserId();
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            Boolean canDo = UserManager.IsInRole(userId, "Administrador");
            if (!canDo || user.LockoutEnabled)
            {
                return false;
            }

            return true;
        }

        internal async Task<ErrorModel> LoadRights(string userId)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            ErrorModel _errors = null;

            Dictionary<string,LoadRightsModel> rightModel = new Dictionary<string, LoadRightsModel>();

            try
            {
                Menu proxy = new Menu();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    using (SqlConnection connection1 = new SqlConnection(connetionString))
                    {
                        await connection1.OpenAsync();
                        await GetGroupsEntities(connection1, userId, rightModel);
                        await GetUsersEntities(connection1, userId, rightModel);
                        await ResetUsersEntities(connection1, userId);
                        foreach (var item in rightModel.Values)
                        {
                            if (item.isquery == 2)
                            {
                                item.isquery = 0;
                            }
                            if (item.isedit == 2)
                            {
                                item.isedit = 0;
                            }
                            if (item.isnew == 2)
                            {
                                item.isnew = 0;
                            }
                            if (item.isdelete == 2)
                            {
                                item.isdelete = 0;
                            }
                            int rows = await UpdateUsersEntities(connection1, userId, item);
                            if (rows == 0)
                            {
                                await AddUsersEntities(connection1, userId, item);
                            }
                        }
                        await ResetLoadRights(connection1, userId);
                    }
                    scope.Complete();
                }

            }
            catch (TransactionAbortedException ex)
            {
                _errors = new ErrorModel();
                _errors.message = ex.Message;
            }
            catch (Exception ex)
            {
                _errors = new ErrorModel();
                _errors.message = ex.Message;
            }

            return _errors;
        }

        internal async Task<bool> AccesRights(string userId, string entitie, int right)
        {
            bool rows = false;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                using (SqlConnection connection1 = new SqlConnection(connetionString))
                {
                    await connection1.OpenAsync();
                    if (!await CheckLoadRights(connection1, userId))
                    {
                       rows = await GetAccesRights(connection1, userId, entitie, right);
                    }
                }
                scope.Complete();
            }

            return rows;
        }
        
        private async Task GetGroupsEntities(SqlConnection connection1, String userId, Dictionary<string, LoadRightsModel> rightModel)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT m.entitie, gm.isquery, gm.isedit, gm.isnew, gm.isdelete, m.id FROM UsersGroups ug " +
                               "left join groups g on ug.idgroup = g.id " +
                               "left join groupsmenu gm on ug.idgroup = gm.idgroup " +
                               "left join menu m on gm.idmenu = m.id " +
                               "where ug.iduser = @iduser and ug.isChecked = 1 and g.status = 1 and m.status = 1";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);
            

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                LoadRightsModel model = new LoadRightsModel();
                model.identitie = reader.GetString(0);
                model.isquery = reader.GetInt32(1);
                model.isedit = reader.GetInt32(2);
                model.isnew = reader.GetInt32(3);
                model.isdelete = reader.GetInt32(4);
                model.idmenu = reader.GetString(5);

                if (rightModel.ContainsKey(model.identitie))
                {
                    if (rightModel[model.identitie].isquery < model.isquery)
                    {
                        rightModel[model.identitie].isquery = model.isquery;
                    }
                    if (rightModel[model.identitie].isedit < model.isedit)
                    {
                        rightModel[model.identitie].isedit = model.isedit;
                    }
                    if (rightModel[model.identitie].isnew < model.isnew)
                    {
                        rightModel[model.identitie].isnew = model.isnew;
                    }
                    if (rightModel[model.identitie].isdelete < model.isdelete)
                    {
                        rightModel[model.identitie].isdelete = model.isdelete;
                    }
                   
                }
                else
                {
                    rightModel.Add(model.identitie, model);
                }
            }
            reader.Close();
        }

        private async Task GetUsersEntities(SqlConnection connection1, String userId, Dictionary<string, LoadRightsModel> rightModel)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT m.entitie, um.isquery, um.isedit, um.isnew, um.isdelete, m.id FROM Usersmenu um " +
                               "left join menu m on um.idmenu = m.id " +
                               "where um.iduser = @iduser and m.status = 1 ";
        
            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);


            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                LoadRightsModel model = new LoadRightsModel();
                model.identitie = reader.GetString(0);
                model.isquery = reader.GetInt32(1);
                model.isedit = reader.GetInt32(2);
                model.isnew = reader.GetInt32(3);
                model.isdelete = reader.GetInt32(4);
                model.idmenu = reader.GetString(5);

                if (rightModel.ContainsKey(model.identitie))
                {
                    if (model.isquery > 0)
                    {
                        rightModel[model.identitie].isquery = model.isquery;
                    }
                    if (model.isedit > 0)
                    {
                        rightModel[model.identitie].isedit = model.isedit;
                    }
                    if (model.isnew > 0)
                    {
                        rightModel[model.identitie].isnew = model.isnew;
                    }
                    if (model.isdelete > 0)
                    {
                        rightModel[model.identitie].isdelete = model.isdelete;
                    }
                }
                else
                {
                    rightModel.Add(model.identitie, model);
                }
            }
            reader.Close();
        }

        private async Task ResetUsersEntities(SqlConnection connection1, String userId)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "update UserRights set isquery = 0, isedit = 0, isnew = 0, isdelete = 0 " +
                               "where iduser = @iduser";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<int> UpdateUsersEntities(SqlConnection connection1, String userId, LoadRightsModel model)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "update UserRights set isquery = @isquery, isedit = @isedit, isnew = @isnew, isdelete = @isdelete " +
                               "where iduser = @iduser and entitie = @entitie";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@entitie", SqlDbType.VarChar);
            parameter.Value = model.identitie;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isquery", SqlDbType.Int);
            parameter.Value = model.isquery;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isedit", SqlDbType.Int);
            parameter.Value = model.isedit;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isnew", SqlDbType.Int);
            parameter.Value = model.isnew;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isdelete", SqlDbType.Int);
            parameter.Value = model.isdelete;
            cmd.Parameters.Add(parameter);

            return await cmd.ExecuteNonQueryAsync();
        }

        private async Task<int> AddUsersEntities(SqlConnection connection1, String userId, LoadRightsModel model)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "insert into UserRights (iduser, idmenu, entitie, isquery, isedit, isnew, isdelete) " +
                               "Values(@iduser, @idmenu, @entitie, @isquery, @isedit, @isnew, @isdelete)";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@idmenu", SqlDbType.VarChar);
            parameter.Value = model.idmenu;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@entitie", SqlDbType.VarChar);
            parameter.Value = model.identitie;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isquery", SqlDbType.Int);
            parameter.Value = model.isquery;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isedit", SqlDbType.Int);
            parameter.Value = model.isedit;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isnew", SqlDbType.Int);
            parameter.Value = model.isnew;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@isdelete", SqlDbType.Int);
            parameter.Value = model.isdelete;
            cmd.Parameters.Add(parameter);

            return await cmd.ExecuteNonQueryAsync();
        }

        private async Task ResetLoadRights(SqlConnection connection1, String userId)
        {
           
            String commandText1 = "update AspNetUsers set PhoneNumberConfirmed = 0 " +
                                    "where id = @userId";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@userId", SqlDbType.VarChar);
            parameter.Value = userId;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
            
        }

        private async Task<bool> CheckLoadRights(SqlConnection connection1, string userId)
        {

            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT PhoneNumberConfirmed FROM aspnetusers " +
                               "where id = @iduser and PhoneNumberConfirmed = 1";
            
            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);
            
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            bool rows = reader.HasRows;

            reader.Close();

            return rows;
        }

        private async Task<bool> GetAccesRights(SqlConnection connection1, string userId, string entitie, int right)
        {

            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT u.entitie FROM UserRights u " +
                               "left join menu m on u.idmenu = m.id " +
                               "where u.iduser = @iduser and u.entitie = @entitie and m.status = 1 and ";

            if (right == 1 || right == 2)
            {
                cmdString += "u.isquery = 1";
            }
            else if (right == 3)
            {
                cmdString += "u.isnew = 1";
            }
            else if (right == 4 || right == 6)
            {
                cmdString += "u.isedit = 1";
            }
            else if (right == 5)
            {
                cmdString += "u.isdelete = 1";
            }

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = userId;
            cmd.Parameters.Add(parameter);

            parameter = new SqlParameter("@entitie", SqlDbType.VarChar);
            parameter.Value = entitie;
            cmd.Parameters.Add(parameter);


            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            bool rows = reader.HasRows;

            reader.Close();

            return rows;
        }

 
    }
}