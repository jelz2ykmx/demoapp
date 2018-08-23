using hostapi.Controllers.Configuracion;
using hostapi.Models;
using hostapi.Models.Groups.GroupsUsers;
using hostapi.Models.Others;
using hostapi.Models.Others.GroupsUsers;
using hostapi.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace hostapi.Classes.Configuracion
{
    public class Users
    {
        String rowsQuery = ConfigurationManager.AppSettings["rowsQuery"];
       
        internal async Task SearchQuery(SearchQueryModel model, List<UsersQueryModel> resultSeachModel, SqlConnection connection1, bool isAdmin)
        {
            await GetUsuarios(connection1, model, resultSeachModel, isAdmin);
        }
        
        internal async Task ById(SearchByIdModel model, SqlConnection connection1, UsersByIdModel resultSeachByIdModel, bool isAdmin)
        {
            resultSeachByIdModel.email = "";
            resultSeachByIdModel.firstname = "";
            resultSeachByIdModel.lastname = "";
            resultSeachByIdModel.direccion = "";
            resultSeachByIdModel.colonia = "";
            resultSeachByIdModel.ciudad = "";
            resultSeachByIdModel.tel = "";
            resultSeachByIdModel.menu = new List<GroupsUsersByIdMenuDetailsModel>();
            resultSeachByIdModel.groups = new List<GroupsUsersByIdGroupsDetailsModel>();
            resultSeachByIdModel.isrootadmin = Convert.ToInt32(isAdmin);
            bool save = true;
            if (model.id != "0")
            {
                if (!isAdmin)
                {
                    save = await CheckisAdmin(connection1, model.id);
                }
                if (save)
                {
                    await GetById(connection1, model, resultSeachByIdModel);
                }
            }
            if (save)
            {
                await GetByMenu(connection1, model, resultSeachByIdModel);
                await GetByGroups(connection1, model, resultSeachByIdModel);
            }
        }
        
        internal async Task<SearchByIdModel> New(UsersUpdateModel model, HttpRequestMessage Request, string connetionString, bool isAdmin, string idAdminrole)
        {
            SearchByIdModel idUser = new SearchByIdModel();

            var appDbContext = Request.GetOwinContext().Get<ApplicationDbContext>();
            using (var identitydbContextTransaction = appDbContext.Database.BeginTransaction())
            {
                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var userByEmail = await UserManager.FindByEmailAsync(model.email);

                if (userByEmail == null)
                {

                    var user = new ApplicationUser() { UserName = model.email, Email = model.email };
                    if (model.menu.Count > 0 || model.groups.Count > 0)
                    {
                        user.PhoneNumberConfirmed = true;
                    }

                    IdentityResult result = await UserManager.CreateAsync(user, model.password);

                    if (!result.Succeeded)
                    {
                        ErrorModel _errors = new ErrorModel();
                        foreach (string error in result.Errors)
                        {
                            _errors.message += error;
                        }
                        throw new Exception(_errors.message);
                    }
                    else
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            using (SqlConnection connection1 = new SqlConnection(connetionString))
                            {
                                idUser.id = user.Id;
                                model.id = user.Id;
                                await connection1.OpenAsync();
                                await SaveUser(connection1, model);
                                if (model.isUserAdmin == 0)
                                {
                                    foreach (var item in model.menu)
                                    {
                                        await SaveUsersMenu(connection1, model.id, item);
                                    }
                                    foreach (var item in model.groups)
                                    {
                                        await SaveUsersGroups(connection1, model.id, item);
                                    }
                                }
                                else if (isAdmin && idAdminrole != null && idAdminrole != "")
                                {
                                    await AddAdminRole(connection1, model.id, idAdminrole);
                                }
                                scope.Complete();
                                identitydbContextTransaction.Commit();
                                return idUser;
                            }
                        }
                    }
                    
                }
                else
                {
                    throw new Exception("Correo ya existe");
                }
            }
        }

        internal async Task Update(UsersUpdateModel model, SqlConnection connection1, bool isAdmin, string idAdminrole)
        {
           
            await UpdateUser(connection1, model);
            await UpdateAspNetUser(connection1, model);
            if (model.isUserAdmin == 0)
            {
                foreach (var item in model.menu)
                {
                    if (item.isedit == 0)
                    {
                        await SaveUsersMenu(connection1, model.id, item);
                    }
                    else
                    {
                        await UpdateUsersMenu(connection1, model.id, item);
                    }
                }
                foreach (var item in model.groups)
                {
                    if (item.isedit == 0)
                    {
                        await SaveUsersGroups(connection1, model.id, item);
                    }
                    else
                    {
                        await UpdateUsersGroups(connection1, model.id, item);
                    }
                }
                await DeleteAdminRole(connection1, model.id, idAdminrole);
            }
            else if (isAdmin && idAdminrole != null && idAdminrole != "")
            {
               await AddAdminRole(connection1, model.id, idAdminrole);
            }
        }
        
        internal async Task UpdateIsActive(UpdateIsActiveModel model, HttpRequestMessage Request)
        {
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await UserManager.FindByIdAsync(model.id);

            if (model.status == 1)
            {
                user.LockoutEndDateUtc = DateTime.Now.AddYears(200);
                user.LockoutEnabled = true;
            }
            else
            {
                user.LockoutEndDateUtc = null;
                user.LockoutEnabled = false;
            }

            IdentityResult result = await UserManager.UpdateAsync(user);
            if (result.Errors == null)
            {
                ErrorModel _errors = new ErrorModel();
                foreach (string error in result.Errors)
                {
                    _errors.message += error;
                }
                throw new Exception(_errors.message);
            }
        }

        internal async Task ChangePassword(ChangePasswordModel model, HttpRequestMessage Request)
        {
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            IdentityResult result = await UserManager.RemovePasswordAsync(model.id);
            if (result.Succeeded)
            {
                result = await UserManager.AddPasswordAsync(model.id, model.password);
            }
            else
            {
                ErrorModel _errors = new ErrorModel();
                foreach (string error in result.Errors)
                {
                    _errors.message = error;
                }
                throw new Exception(_errors.message);
            }

        }
        
        private async Task GetUsuarios(SqlConnection connection1, SearchQueryModel model, List<UsersQueryModel> lst, bool isAdmin)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT top " + rowsQuery.ToString() + " a.Id, a.Email, a.LockoutEnabled, a.LockoutEndDateUtc, isnull(u.firstname,''), isnull(u.lastname,'') " +
                "FROM AspNetUsers as a " +
                "left join users u on a.id = u.id ";

            if (!isAdmin)
            {
                cmdString += "left join AspNetUserRoles ur on a.id = ur.UserId " +
                             "left join AspNetRoles r on ur.RoleId = r.id ";
            }
            
            if (model.status == 0)
            {
                cmdString += "where a.LockoutEnabled = 0 ";
            }
            else if (model.status == 1)
            {
                cmdString += "where a.LockoutEnabled = 1 ";
            }
            
            if (model.filter != "")
            {
                if (model.status == 2)
                {
                    cmdString += "where a.Email like @search or u.firstname like @search or u.lastname like @search ";
                }
                else
                {
                    cmdString += "and (a.Email like @search or u.firstname like @search or u.lastname like @search) ";
                }

                SqlParameter parameter = new SqlParameter("@search", SqlDbType.VarChar);
                parameter.Value = "%" + model.filter + "%";
                cmd.Parameters.Add(parameter);
            }

            if (!isAdmin)
            {
                cmdString += "and r.name is null ";
            }

            cmdString += "order by a.email";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                UsersQueryModel userModel = new UsersQueryModel();
                userModel.id = reader.GetString(0);
                userModel.email = reader.GetString(1);
                userModel.status = Convert.ToInt16(reader.GetBoolean(2));
                if (!reader.IsDBNull(3))
                {
                    reader.GetDateTime(3);
                }
                userModel.name = reader.GetString(4) + " ";
                userModel.name += reader.GetString(5);
                if (userModel.name.Trim() == "")
                {
                    userModel.name = userModel.email;
                }
                
                lst.Add(userModel);



            }
            reader.Close();
        }

        private async Task<bool> CheckisAdmin(SqlConnection connection1, string id)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT a.id " +
               "FROM AspNetUsers as a " +
               "left join users u on a.id = u.id " +
               "left join AspNetUserRoles ur on a.id = ur.UserId " +
               "left join AspNetRoles r on ur.RoleId = r.id " +
               "where a.id = @id and r.name is null ";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            bool rows = reader.HasRows;

            reader.Close();

            return rows;
        }

        private async Task GetById(SqlConnection connection1, SearchByIdModel model, UsersByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT a.email, isnull(u.firstname,''), isnull(u.lastname,''), isnull(u.direccion,''), isnull(u.colonia,''), isnull(u.ciudad,''), isnull(u.tel,''), u.photo, " +
                   "r.name FROM AspNetUsers as a " +
                   "left join users u on a.id = u.id " +
                   "left join AspNetUserRoles ur on a.id = ur.UserId " +
                   "left join AspNetRoles r on ur.RoleId = r.id " +
                   "where a.id = @id";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.email = reader.GetString(0);
                data.firstname = reader.GetString(1);
                data.lastname = reader.GetString(2);
                data.direccion = reader.GetString(3);
                data.colonia = reader.GetString(4);
                data.ciudad = reader.GetString(5);
                data.tel = reader.GetString(6);
                if (reader.IsDBNull(7))
                {
                    data.photo = new byte[0];
                }
                else
                {
                    data.photo = (byte[])reader["photo"];
                }
                if (!reader.IsDBNull(8))
                {
                    data.isadmin = 1;
                }
            }
            reader.Close();
        }

        private async Task GetByMenu(SqlConnection connection1, SearchByIdModel model, UsersByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "select m.id, m.menu, g.isquery, g.isedit, g.isnew, g.isdelete from menu m " +
                    "left join usersmenu g on m.id = g.idmenu and g.iduser = @id  " +
                    "left join users gr on g.iduser = gr.id and gr.id = @id " +
                    "where m.status = 1 " +
                    "order by m.menu";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                GroupsUsersByIdMenuDetailsModel details = new GroupsUsersByIdMenuDetailsModel();
                details.id = reader.GetString(0);
                details.name = reader.GetString(1);
                details.isEdit = 0;
                if (reader.IsDBNull(2))
                {
                    details.isquery = 0;
                }
                else
                {
                    details.isquery = reader.GetInt32(2);
                    details.isEdit = 1;
                }
                if (reader.IsDBNull(3))
                {
                    details.iseditField = 0;
                }
                else
                {
                    details.iseditField = reader.GetInt32(3);
                    details.isEdit = 1;
                }
                if (reader.IsDBNull(4))
                {
                    details.isnew = 0;
                }
                else
                {
                    details.isnew = reader.GetInt32(4);
                    details.isEdit = 1;
                }
                if (reader.IsDBNull(5))
                {
                    details.isdelete = 0;
                }
                else
                {
                    details.isdelete = reader.GetInt32(5);
                    details.isEdit = 1;
                }
                data.menu.Add(details);
            }
            reader.Close();
        }

        private async Task GetByGroups(SqlConnection connection1, SearchByIdModel model, UsersByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "select m.id, m.name, g.ischecked from groups m " +
                "left join usersgroups g on m.id = g.idgroup and g.iduser = @id  " +
                "left join users gr on g.iduser = gr.id and gr.id = @id " +
                "where m.status = 1 " +
                "order by m.name";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                GroupsUsersByIdGroupsDetailsModel details = new GroupsUsersByIdGroupsDetailsModel();
                details.id = reader.GetString(0);
                details.name = reader.GetString(1);
                if (reader.IsDBNull(2))
                {
                    details.ischecked = false;
                    details.isEdit = 0;

                }
                else
                {
                    details.ischecked = reader.GetBoolean(2);
                    details.isEdit = 1;
                }
                data.groups.Add(details);

            }
            reader.Close();
        }

        private async Task SaveUser(SqlConnection connection1, UsersUpdateModel model)
        {
            String commandText1 = "INSERT INTO users " +
                                "(id,firstName,LastName,direccion,colonia,ciudad,tel,photo) " +
                                "values (@id,@firstName,@LastName,@direccion,@colonia,@ciudad,@tel,@photo)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@firstName", SqlDbType.VarChar);
            parameter.Value = model.firstname;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@LastName", SqlDbType.VarChar);
            parameter.Value = model.lastname;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@direccion", SqlDbType.VarChar);
            parameter.Value = model.direccion;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@colonia", SqlDbType.VarChar);
            parameter.Value = model.colonia;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@ciudad", SqlDbType.VarChar);
            parameter.Value = model.ciudad;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@tel", SqlDbType.VarChar);
            parameter.Value = model.tel;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@photo", SqlDbType.Binary);
            parameter.Value = model.photo;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task SaveUsersMenu(SqlConnection connection1, string id, GroupsUsersMenuUpdateModel model)
        {
            String commandText1 = "INSERT INTO UsersMenu " +
                                "(iduser,idmenu,isquery,isnew,isedit,isdelete) " +
                                "values (@iduser,@idmenu,@isquery,@isnew,@isedit,@isdelete)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idmenu", SqlDbType.VarChar);
            parameter.Value = model.idmenu;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isquery", SqlDbType.Int);
            parameter.Value = model.isquery;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isnew", SqlDbType.Int);
            parameter.Value = model.isnew;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isedit", SqlDbType.Int);
            parameter.Value = model.iseditField;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isdelete", SqlDbType.Int);
            parameter.Value = model.isdelete;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task SaveUsersGroups(SqlConnection connection1, string id, GroupsUsersGroupsUpdateModel model)
        {
            String commandText1 = "INSERT INTO Usersgroups " +
                                "(iduser,idgroup,ischecked) " +
                                "values (@iduser,@idgroup,@ischecked)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
            parameter.Value = model.idgroup;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@ischecked", SqlDbType.Bit);
            parameter.Value = model.ischecked;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }
        
        private async Task UpdateUser(SqlConnection connection1, UsersUpdateModel model)
        {
            String commandText1 = "update users set firstName = @firstName, " +
                                  "LastName = @LastName, direccion = @direccion, colonia = @colonia, ciudad = @ciudad, " +
                                  "tel = @tel, photo = @photo ";
            
            commandText1 += "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@firstName", SqlDbType.VarChar);
            parameter.Value = model.firstname;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@LastName", SqlDbType.VarChar);
            parameter.Value = model.lastname;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@direccion", SqlDbType.VarChar);
            parameter.Value = model.direccion;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@colonia", SqlDbType.VarChar);
            parameter.Value = model.colonia;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@ciudad", SqlDbType.VarChar);
            parameter.Value = model.ciudad;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@tel", SqlDbType.VarChar);
            parameter.Value = model.tel;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@photo", SqlDbType.Binary);
            parameter.Value = model.photo;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateAspNetUser(SqlConnection connection1, UsersUpdateModel model)
        {
            String commandText1 = "update AspNetUsers set email = @email, username = @email ";

            if (model.menu.Count > 0 || model.groups.Count > 0)
            {
                commandText1 += ", PhoneNumberConfirmed = 1 ";
            }

            commandText1 += "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@email", SqlDbType.VarChar);
            parameter.Value = model.email;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@username", SqlDbType.VarChar);
            parameter.Value = model.email;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateUsersMenu(SqlConnection connection1, string id, GroupsUsersMenuUpdateModel model)
        {
            String commandText1 = "update UsersMenu " +
                                "set isquery = @isquery, isnew = @isnew, isedit = @isedit, isdelete = @isdelete  " +
                                "where iduser = @iduser and idmenu = @idmenu";


            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idmenu", SqlDbType.VarChar);
            parameter.Value = model.idmenu;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isquery", SqlDbType.Int);
            parameter.Value = model.isquery;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isnew", SqlDbType.Int);
            parameter.Value = model.isnew;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isedit", SqlDbType.Int);
            parameter.Value = model.iseditField;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@isdelete", SqlDbType.Int);
            parameter.Value = model.isdelete;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateUsersGroups(SqlConnection connection1, string id, GroupsUsersGroupsUpdateModel model)
        {
            String commandText1 = "update Usersgroups " +
                                "set ischecked = @ischecked " +
                                "where iduser = @iduser and idgroup = @idgroup";


            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
            parameter.Value = model.idgroup;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@ischecked", SqlDbType.Bit);
            parameter.Value = model.ischecked;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task AddAdminRole(SqlConnection connection1, string id, string idAdminrole)
        {
            String commandText1 = "insert into AspNetUserRoles (UserId, RoleId) Values(@id, @idAdminrole) ";
            
            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idAdminrole", SqlDbType.VarChar);
            parameter.Value = idAdminrole;
            command1.Parameters.Add(parameter);
            
            await command1.ExecuteNonQueryAsync();
        }

        private async Task DeleteAdminRole(SqlConnection connection1, string id, string idAdminrole)
        {
            String commandText1 = "delete AspNetUserRoles where UserId = @id and RoleId = @idAdminrole";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@idAdminrole", SqlDbType.VarChar);
            parameter.Value = idAdminrole;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }


    }
}