using hostapi.Models.Groups;
using hostapi.Models.Others;
using hostapi.Models.Others.GroupsUsers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace hostapi.Classes.Configuracion
{
    public class Grupos
    {
        String rowsQuery = ConfigurationManager.AppSettings["rowsQuery"];

        internal async Task SearchQuery(SearchQueryModel model, List<GroupsQueryModel> resultSeachModel, SqlConnection connection1)
        {
           await GetGroups(connection1, model, resultSeachModel);
        }

        internal async Task ById(SearchByIdModel model, SqlConnection connection1, GroupsByIdModel resultSeachByIdModel)
        {
            resultSeachByIdModel.name = "";
            resultSeachByIdModel.menu = new List<GroupsUsersByIdMenuDetailsModel>();
            resultSeachByIdModel.users = new List<GroupsUsersByIdGroupsDetailsModel>();
            if (model.id != "0")
            {
                await GetById(connection1, model, resultSeachByIdModel);
            }
            await GetByMenu(connection1, model, resultSeachByIdModel);
            await GetByUsers(connection1, model, resultSeachByIdModel);
        }

        internal async Task New(SqlConnection connection1, GroupsUpdateModel model)
        {
            await SaveGroup(connection1, model);
            foreach (var item in model.menu)
            {
                await SaveGroupMenu(connection1, model.id, item);
            }
            List<string> users = new List<string>();
            int counter = model.menu.Count;
            foreach (var item in model.users)
            {
                await SaveGroupsUsers(connection1, model.id, item);
                if (counter > 0)
                {
                    users.Add(item.iduser);
                }
            }
            if (users.Count > 0)
            {
                await UpdateAspNetUser(connection1, users);
            }
        }

        internal async Task Update(SqlConnection connection1, GroupsUpdateModel model)
        {
            await UpdateGroup(connection1, model);
            foreach (var item in model.menu)
            {
                if (item.isedit == 0)
                {
                    await SaveGroupMenu(connection1, model.id, item);
                }
                else
                {
                    await UpdateGroupMenu(connection1, model.id, item);
                }
            }
            List<string> users = new List<string>();
            int counter = model.menu.Count;
            foreach (var item in model.users)
            {
                if (item.isedit == 0)
                {
                    await SaveGroupsUsers(connection1, model.id, item);
                }
                else
                {
                    await UpdateGroupsUsers(connection1, model.id, item);
                }
                if (counter > 0)
                {
                    users.Add(item.iduser);
                }
            }

            if (users.Count > 0)
            {
                await UpdateAspNetUser(connection1, users);
            }
            if (counter > 0)
            {
                users = new List<string>();
                await GetUsersByGroup(connection1, model.id, users);
                await UpdateAspNetUser(connection1, users);
            }

        }

        internal async Task UpdateIsActive(SqlConnection connection1, UpdateIsActiveModel model)
        {
            String commandText1 = "update Groups set status = @status " +
                                  "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@status", SqlDbType.Int);
            parameter.Value = model.status;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task GetGroups(SqlConnection connection1, SearchQueryModel model, List<GroupsQueryModel> lst)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT top " + rowsQuery.ToString() + " id, name, status FROM Groups ";

            SqlParameter parameter = null;

            if (model.status != 2)
            {
                cmdString += "where status = @status ";
                parameter = new SqlParameter("@status", SqlDbType.Int);
                parameter.Value = model.status;
                cmd.Parameters.Add(parameter);
            }

            if (model.filter != "")
            {
                if (model.status == 2)
                {
                    cmdString += "where name like @search ";
                }
                else
                {
                    cmdString += "and name like @search ";
                }

                parameter = new SqlParameter("@search", SqlDbType.VarChar);
                parameter.Value = "%" + model.filter + "%";
                cmd.Parameters.Add(parameter);
            }

            cmdString += "order by name";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                GroupsQueryModel menuModel = new GroupsQueryModel();
                menuModel.id = reader.GetString(0);
                menuModel.name = reader.GetString(1);
                menuModel.status = reader.GetInt32(2);

                lst.Add(menuModel);
            }
            reader.Close();
        }

        private async Task GetById(SqlConnection connection1, SearchByIdModel model, GroupsByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT name FROM Groups " +
                   "where id = @id";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.name = reader.GetString(0);
            }
            reader.Close();
        }

        private async Task GetByMenu(SqlConnection connection1, SearchByIdModel model, GroupsByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "select m.id, m.menu, g.isquery, g.isedit, g.isnew, g.isdelete from menu m " +
                    "left join groupsmenu g on m.id = g.idmenu and g.idgroup = @id  " +
                    "left join groups gr on g.idgroup = gr.id and gr.id = @id " +
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

        private async Task GetByUsers(SqlConnection connection1, SearchByIdModel model, GroupsByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "select a.id, a.email, u.firstName + ' ' + u.LastName as name, g.ischecked from AspNetUsers a " +
                "left join users u on a.id = u.id " +
                "left join usersgroups g on g.idgroup = @id and g.iduser = u.id " +
                "left join AspNetUserRoles ur on a.id = ur.UserId " +
                "left join AspNetRoles r on ur.RoleId = r.id " +
                "where a.LockoutEnabled = 0 and r.name is null " +
                "order by name";

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
                if (!reader.IsDBNull(2))
                {
                    details.name = reader.GetString(2).Trim();
                }
                if (reader.IsDBNull(3))
                {
                    details.ischecked = false;
                    details.isEdit = 0;

                }
                else
                {
                    details.ischecked = reader.GetBoolean(3);
                    details.isEdit = 1;
                }
                data.users.Add(details);

            }
            reader.Close();
        }

        private async Task SaveGroup(SqlConnection connection1, GroupsUpdateModel model)
        {
            String commandText1 = "INSERT INTO Groups " +
                                "(id,name,status) " +
                                "values (@id,@name,1)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@name", SqlDbType.VarChar);
            parameter.Value = model.name;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task SaveGroupMenu(SqlConnection connection1, string id, GroupsUsersMenuUpdateModel model)
        {
            String commandText1 = "INSERT INTO GroupsMenu " +
                                "(idgroup,idmenu,isquery,isnew,isedit,isdelete) " +
                                "values (@idgroup,@idmenu,@isquery,@isnew,@isedit,@isdelete)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
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

        private async Task SaveGroupsUsers(SqlConnection connection1, string id, GroupsUsersUpdateModel model)
        {
            String commandText1 = "INSERT INTO Usersgroups " +
                                "(idgroup,iduser,ischecked) " +
                                "values (@idgroup,@iduser,@ischecked)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = model.iduser;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@ischecked", SqlDbType.Bit);
            parameter.Value = model.ischecked;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateGroup(SqlConnection connection1, GroupsUpdateModel model)
        {
            String commandText1 = "update Groups set name = @name " +
                                  "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@name", SqlDbType.VarChar);
            parameter.Value = model.name;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateGroupMenu(SqlConnection connection1, string id, GroupsUsersMenuUpdateModel model)
        {
            String commandText1 = "update GroupsMenu " +
                                "set isquery = @isquery, isnew = @isnew, isedit = @isedit, isdelete = @isdelete  " +
                                "where idgroup = @idgroup and idmenu = @idmenu";


            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
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

        private async Task UpdateGroupsUsers(SqlConnection connection1, string id, GroupsUsersUpdateModel model)
        {
            String commandText1 = "update Usersgroups " +
                                "set ischecked = @ischecked " +
                                "where idgroup = @idgroup and iduser = @iduser";
            
            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
            parameter.Value = id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@iduser", SqlDbType.VarChar);
            parameter.Value = model.iduser; 
            command1.Parameters.Add(parameter);
            
            parameter = new SqlParameter("@ischecked", SqlDbType.Bit);
            parameter.Value = model.ischecked;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateAspNetUser(SqlConnection connection1, List<string> users)
        {
            foreach (var id in users)
            {
                String commandText1 = "update AspNetUsers set PhoneNumberConfirmed = 1 " +
                                      "where id = @id and PhoneNumberConfirmed = 0";

                SqlCommand command1 = new SqlCommand(commandText1, connection1);

                SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
                parameter.Value = id;
                command1.Parameters.Add(parameter);

                await command1.ExecuteNonQueryAsync();
            }

        }

        private async Task GetUsersByGroup(SqlConnection connection1, string id, List<string> users)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT iduser FROM usersgroups " +
                               "where idgroup = @idgroup and isChecked = 1";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = parameter = new SqlParameter("@idgroup", SqlDbType.VarChar);
            parameter.Value = id;
            cmd.Parameters.Add(parameter);
            
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(reader.GetString(0));
            }
            reader.Close();
        }

    }
}