using hostapi.Models.Menu;
using hostapi.Models.Others;
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
    public class Menu
    {
        String rowsQuery = ConfigurationManager.AppSettings["rowsQuery"];

        internal async Task SearchQuery(SearchQueryModel model, List<MenuQueryModel> resultSeachModel, SqlConnection connection1)
        {
            await GetMenus(connection1, model, resultSeachModel);
        }

        internal async Task ById(SearchByIdModel model, SqlConnection connection1, MenuByIdModel resultSeachByIdModel)
        {
            await GetById(connection1, model, resultSeachByIdModel);
        }

        internal async Task New(SqlConnection connection1, MenuUpdateModel model)
        {
           await SaveMenu(connection1, model);
        }

        internal async Task Update(SqlConnection connection1, MenuUpdateModel model)
        {
            await UpdateMenu(connection1, model);
        }

        internal async Task UpdateIsActive(SqlConnection connection1, UpdateIsActiveModel model)
        {
            String commandText1 = "update menu set status = @status " +
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

        private async Task GetMenus(SqlConnection connection1, SearchQueryModel model, List<MenuQueryModel> lst)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT top " + rowsQuery.ToString() + " id, menu, status FROM menu ";

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
                    cmdString += "where menu like @search ";
                }
                else
                {
                    cmdString += "and menu like @search ";
                }

                parameter = new SqlParameter("@search", SqlDbType.VarChar);
                parameter.Value = "%" + model.filter + "%";
                cmd.Parameters.Add(parameter);
            }

            cmdString += "order by menu";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                MenuQueryModel menuModel = new MenuQueryModel();
                menuModel.id = reader.GetString(0);
                menuModel.menu = reader.GetString(1);
                menuModel.status = reader.GetInt32(2);

                lst.Add(menuModel);
            }
            reader.Close();
        }

        private async Task GetById(SqlConnection connection1, SearchByIdModel model, MenuByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT menu, entitie FROM menu m " +
                   "where id = @id";

            cmd.CommandText = cmdString;
            cmd.Connection = connection1;

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            cmd.Parameters.Add(parameter);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.menu = reader.GetString(0);
                data.entitie = reader.GetString(1);
            }
            reader.Close();
        }

        private async Task SaveMenu(SqlConnection connection1, MenuUpdateModel model)
        {
            String commandText1 = "INSERT INTO menu " +
                                "(id,menu,entitie,status) " +
                                "values (@id,@menu,@entitie,1)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@menu", SqlDbType.VarChar);
            parameter.Value = model.menu;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@entitie", SqlDbType.VarChar);
            parameter.Value = model.entitie;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        private async Task UpdateMenu(SqlConnection connection1, MenuUpdateModel model)
        {
            String commandText1 = "update menu set menu = @menu, entitie = @entitie " +
                                  "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@menu", SqlDbType.VarChar);
            parameter.Value = model.menu;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@entitie", SqlDbType.VarChar);
            parameter.Value = model.entitie;
            command1.Parameters.Add(parameter);

            await command1.ExecuteNonQueryAsync();
        }

        
    }
}