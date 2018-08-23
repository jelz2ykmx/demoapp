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
    public class Companies
    {
        String rowsQuery = ConfigurationManager.AppSettings["rowsQuery"];

        internal async Task SearchQuery(SearchQueryModel model, List<CompaniesQueryModel> resultSeachModel, SqlConnection connection1)
        {
           await GetCompanies(connection1, model, resultSeachModel);
        }

        internal async Task ById(SearchByIdModel model, SqlConnection connection1, CompaniesByIdModel resultSeachByIdModel)
        {
            resultSeachByIdModel.name = "";
            resultSeachByIdModel.users = new List<CompaniesUsersByIdDetailsModel>();
            if (model.id != "0")
            {
                await GetById(connection1, model, resultSeachByIdModel);
            }
            await GetByUsers(connection1, model, resultSeachByIdModel);
        }

        internal async Task New(SqlConnection connection1, CompaniesUpdateModel model)
        {
            await SaveCompany(connection1, model);
            List<string> users = new List<string>();
            foreach (var item in model.users)
            {
                await SaveCompaniesUsers(connection1, model.id, item);
            }
        }

        internal async Task Update(SqlConnection connection1, CompaniesUpdateModel model)
        {
            await UpdateCompany(connection1, model);
            List<string> users = new List<string>();
            foreach (var item in model.users)
            {
                if (item.isedit == 0)
                {
                    await SaveCompaniesUsers(connection1, model.id, item);
                }
                else
                {
                    await UpdateCompaniesUsers(connection1, model.id, item);
                }
            }
        }

        internal async Task UpdateIsActive(SqlConnection connection1, UpdateIsActiveModel model)
        {
            String commandText1 = "update companies set status = @status " +
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

        private async Task GetCompanies(SqlConnection connection1, SearchQueryModel model, List<CompaniesQueryModel> lst)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT top " + rowsQuery.ToString() + " id, name, status FROM companies ";

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
                CompaniesQueryModel menuModel = new CompaniesQueryModel();
                menuModel.id = reader.GetString(0);
                menuModel.name = reader.GetString(1);
                menuModel.status = reader.GetInt32(2);

                lst.Add(menuModel);
            }
            reader.Close();
        }

        private async Task GetById(SqlConnection connection1, SearchByIdModel model, CompaniesByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "SELECT name FROM Companies " +
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

        private async Task GetByUsers(SqlConnection connection1, SearchByIdModel model, CompaniesByIdModel data)
        {
            SqlCommand cmd = new SqlCommand();

            string cmdString = "select a.id, a.email, u.firstName + ' ' + u.LastName as name, g.ischecked from AspNetUsers a " +
                "left join users u on a.id = u.id " +
                "left join companiesusers g on g.idcompany = @id and g.iduser = u.id " +
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
                CompaniesUsersByIdDetailsModel details = new CompaniesUsersByIdDetailsModel();
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

        private async Task SaveCompany(SqlConnection connection1, CompaniesUpdateModel model)
        {
            String commandText1 = "INSERT INTO companies " +
                                "(id,name,rfc,direccion,colonia,ciudad,tel,photo,status) " +
                                "values (@id,@name,@rfc,@direccion,@colonia,@ciudad,@tel,@photo,1)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@name", SqlDbType.VarChar);
            parameter.Value = model.name;
            command1.Parameters.Add(parameter);
            
            parameter = new SqlParameter("@rfc", SqlDbType.VarChar);
            parameter.Value = model.rfc;
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
        
        private async Task SaveCompaniesUsers(SqlConnection connection1, string id, CompaniesUsersUpdateModel model)
        {
            String commandText1 = "INSERT INTO CompaniesUsers " +
                                "(idcompany,iduser,ischecked) " +
                                "values (@idcompany,@iduser,@ischecked)";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idcompany", SqlDbType.VarChar);
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

        private async Task UpdateCompany(SqlConnection connection1, CompaniesUpdateModel model)
        {
            String commandText1 = "update companies set name = @name " +
                                  "rfc = @rfc, direccion = @direccion, colonia = @colonia, ciudad = @ciudad, " +
                                  "tel = @tel, photo = @photo " +
                                  "where id = @id";

            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@id", SqlDbType.VarChar);
            parameter.Value = model.id;
            command1.Parameters.Add(parameter);

            parameter = new SqlParameter("@name", SqlDbType.VarChar);
            parameter.Value = model.name;
            command1.Parameters.Add(parameter);
            
            parameter = new SqlParameter("@rfc", SqlDbType.VarChar);
            parameter.Value = model.rfc;
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
      
        private async Task UpdateCompaniesUsers(SqlConnection connection1, string id, CompaniesUsersUpdateModel model)
        {
            String commandText1 = "update companiesusers " +
                                "set ischecked = @ischecked " +
                                "where idcompany = @idcompany and iduser = @iduser";
            
            SqlCommand command1 = new SqlCommand(commandText1, connection1);

            SqlParameter parameter = new SqlParameter("@idcompany", SqlDbType.VarChar);
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
        
    }
}