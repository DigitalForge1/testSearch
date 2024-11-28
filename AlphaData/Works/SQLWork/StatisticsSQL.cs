using AlphaData.Models.Statistics;
using AlphaData.Models.Statistics.Account;
using AlphaData.Models.Statistics.Journal;
using AlphaData.Models.Statistics.Management;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// https://www.aspsnippets.com/Articles/ASPNet-Core-MVC-Using-DataSet-DataTable-as-Model-in-View.aspx
namespace alphaData.Works.SQLWork
{
    public class StatisticsSQL
    {
        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        public async Task<List<ProjectModel>> GetProjectListAsync()
        {
            Debug.WriteLine("Делаем запрос GetProjectList");
            List<ProjectModel> projectModels = new List<ProjectModel>();
            string zapros = $"select Id[idProject], [Name][nameProject] from oktell_settings.dbo.A_TaskManager_Projects order by Name";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            ProjectModel projectModel = new ProjectModel();
                            projectModel.id = dr["idProject"].ToString();
                            projectModel.name = dr["nameProject"].ToString();
                            projectModels.Add(projectModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return projectModels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetProjectList: " + ex.Message);
                return projectModels;
            }
        }
        public async Task<List<LkModel>> GetLkListAsync()
        {
            Debug.WriteLine("Делаем запрос GetLkList");
            List<LkModel> lkModels = new List<LkModel>();
            string zapros = @"select 
Lk.idLK,
lk.loginLK,
Lk.passLK,
ISNULL(Projects.[Name], N'[Имя проекта не найденно]') as name,
Lk.idProject
from Alphadata_lk as Lk
	left join oktell_settings.dbo.A_TaskManager_Projects as Projects ON Projects.Id = Lk.idProject
order by ISNULL(Projects.[Name], N'Ω[Имя проекта не найденно, скорее всего проект был удалён]')";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            LkModel lKModel = new LkModel();
                            lKModel.id = dr["idLK"].ToString();
                            lKModel.name = dr["Name"].ToString();
                            lKModel.login = dr["loginLK"].ToString();
                            lKModel.pass = dr["passLK"].ToString();
                            lkModels.Add(lKModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return lkModels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetLkList: " + ex.Message);
                return lkModels;
            }
        }
        public async Task DeleteLkAsync(string idLk)
        {
            Debug.WriteLine("Делаем запрос DeleteLk");
            string zapros = $"delete Alphadata_lk where idLK = '{idLk}'; delete Alphadata_task where idLK = '{idLk}';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(zapros, con))
                        await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка DeleteLk: " + ex.Message);
            }
        }
        public async Task InsertLkAsync(string idProject, string login, string pass)
        {
            Debug.WriteLine("Делаем запрос InsertLk");
            string zapros = $"insert into Alphadata_lk (idLK,idProject,loginLK,passLK) values ('{Guid.NewGuid().ToString().ToUpper()}','{idProject}','{login}','{pass}');";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(zapros, con))
                        await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка InsertLk: " + ex.Message);
            }
        }

        /// A C C O U N T
        /// A C C O U N T
        /// A C C O U N T
        public async Task InsertTaskAsync(string idLk, string idTask)
        {
            Debug.WriteLine("Делаем запрос InsertTask");

            List<string> fullPoles = await GetFullPoleAsync(idTask);
            string fullPolesString = "";
            if (fullPoles.Count > 0)
            {
                foreach (string pole in fullPoles)
                    fullPolesString = fullPolesString + "," + pole;
                if (fullPolesString.Substring(0, 1) == ",")
                    fullPolesString = fullPolesString.Substring(1);
                fullPolesString = fullPolesString + ",";
            }

            string zapros = $"insert into Alphadata_task (idLK,idTask,poleList) values ('{idLk}','{idTask}','{fullPolesString}');";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(zapros, con))
                        await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка InsertTask: " + ex.Message);
            }
        }
        public async Task<List<string>> GetFullPoleAsync(string idTask)
        {
            string nameTable = await GetNameTableAsync(idTask);
            string zapros = $"EXEC AlphaData_GetFullPole @nameTable = '{nameTable}';";
            try
            {
                List<string> fullPoles = new List<string>();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            fullPoles.Add(dr["pole"].ToString().Replace("[", "").Replace("]", ""));
                        }
                    }
                    await con.CloseAsync();
                }

                if (fullPoles.Contains("chainid"))
                    fullPoles.Remove("chainid");
                if (fullPoles.Contains("Chainid"))
                    fullPoles.Remove("Chainid");
                if (fullPoles.Contains("Comid"))
                    fullPoles.Remove("Comid");
                return fullPoles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetFullPole: " + ex.Message);
                return null;
            }
        }
        public async Task<List<string>> GetMyPoleAsync(string idLk, string idTask)
        {
            string zapros = $"select poleList from Alphadata_task where idLK = '{idLk}' and idTask = '{idTask}';";
            try
            {
                List<string> myPoles = new List<string>();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            myPoles = dr["poleList"].ToString().Split(',').ToList();
                        }
                    }
                    await con.CloseAsync();
                }
                return myPoles.Where(pole => pole.Length > 1).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetFullPole: " + ex.Message);
                return null;
            }
        }
        public async Task UpdatePolesAsync(string idLk, string idTask, string poles)
        {
            poles = poles + ","; // Обязательно должны быть запятая в конце
            string zapros = $"update Alphadata_task set poleList = '{poles}' where idLK = '{idLk}' and idTask = '{idTask}';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(zapros, con))
                        await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка InsertTask: " + ex.Message);
            }
        }
        public async Task<List<MyTaskModel>> GetMyTaskListAsync(string idLk)
        {
            List<MyTaskModel> myTaskModels = new List<MyTaskModel>();
            string zapros = $"EXEC AlphaData_GetMyTask @idLk = '{idLk}';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            MyTaskModel myTaskModel = new MyTaskModel();
                            myTaskModel.idTask = dr["idTask"].ToString();
                            myTaskModel.nameTask = dr["nameTask"].ToString();
                            myTaskModel.nameTable = dr["nameTable"].ToString();
                            myTaskModel.myPoles = await GetMyPoleAsync(idLk, myTaskModel.idTask);
                            myTaskModel.fullPoles = await GetFullPoleAsync(myTaskModel.idTask);
                            myTaskModels.Add(myTaskModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return myTaskModels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetMyTasksAsync: " + ex.Message);
                return myTaskModels;
            }
        }
        public async Task<List<SystemTaskModel>> GetSystemTaskListAsync(string idTask = null)
        {
            Debug.WriteLine("Делаем запрос GetSystemTaskListAsync");
            List<SystemTaskModel> fullSystemTasks = new List<SystemTaskModel>();
            string zapros = $"EXEC AlphaData_GetSystemTask;";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            SystemTaskModel systemTaskModel = new SystemTaskModel();
                            systemTaskModel.idTask = dr["idTask"].ToString();
                            systemTaskModel.nameTask = dr["Name"].ToString();
                            systemTaskModel.nameTable = dr["nameTable"].ToString();
                            fullSystemTasks.Add(systemTaskModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return fullSystemTasks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetSystemTaskListAsync: " + ex.Message);
                return fullSystemTasks;
            }
        }
        public async Task<List<string>> GetSettingsTypeTable(string idLk)
        {
            string zapros = $"select typeTable from Alphadata_lk where idLk = '{idLk}';";
            try
            {
                List<string> typeTables = new List<string>();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            typeTables = dr["typeTable"].ToString().Split(',').ToList();
                        }
                    }
                    await con.CloseAsync();
                }
                return typeTables;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetSettingsTypeTable: " + ex.Message);
                return null;
            }
        }
        public async Task UpdateSettingsTypeTable(string idLk, string typeTable)
        {
            typeTable = typeTable + ","; // Обязательно должны быть запятая в конце
            string zapros = $"update Alphadata_lk set typeTable = '{typeTable}' where idLk = '{idLk}';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(zapros, con))
                        await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка InsertTask: " + ex.Message);
            }
        }

        /// J O U R N A L
        /// J O U R N A L
        /// J O U R N A L
        /// 
        public async Task<DataTable> GetRenameTable(string nameTable)
        {
            string zapros = $@"Select * from {nameTable}";
            try
            {
                DataTable Table = new DataTable();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    SqlDataAdapter adapter = new SqlDataAdapter(zapros, con);
                    adapter.Fill(Table);
                    cmd.ExecuteNonQuery();
                }
                return Table;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка " + ex.Message);
                return null;
            }
        }



        public async Task<string> GetIdLkLogin(string login, string password)
        {
            string zapros = $"select idLK from Alphadata_lk where loginLk = '{login}' and passLK = '{password}';";
            try
            {
                string idLk = "";
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            idLk = dr["idLK"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return idLk;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetIdLkLogin: " + ex.Message);
                return null;
            }
        }
        public async Task<LkModel> GetLoginLk(string idLk)
        {
            string zapros = $"select loginLK,passLK from Alphadata_lk where idLK = '{idLk}';";
            try
            {
                LkModel lkModel = new LkModel();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lkModel.login = dr["loginLK"].ToString();
                            lkModel.pass = dr["passLK"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return lkModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetLoginLk: " + ex.Message);
                return null;
            }
        }
        public async Task<List<TaskModel>> GetMyTaskAsync(string idLk)
        {
            string zapros = $"EXEC GetMyTask @idLk = '{idLk}';";
            try
            {
                List<TaskModel> taskModels = new List<TaskModel>();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            TaskModel taskModel = new TaskModel();
                            taskModel.id = dr["idTask"].ToString();
                            taskModel.name = dr["JoinedTable"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return taskModels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetNameTable: " + ex.Message);
                return null;
            }
        }
        public async Task<string> GetNameTableAsync(string idTask)
        {
            string zapros = $"EXEC AlphaData_GetSystemTask @idTask = '{idTask}';";
            try
            {
                string nameTable = null;
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            nameTable = dr["nameTable"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return nameTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetNameTable: " + ex.Message);
                return null;
            }
        }
        public async Task<DataTable> GetAbonentTableAsync(string idTask, List<string> poles, DateTime minDtime, DateTime maxDtime)
        {
            Debug.WriteLine("Делаем запрос GetAbonentTable");
            string polesString = "id, ";
            poles.Remove("id");
            poles.Remove("Id");
            foreach (string pole in poles.Select(x=> $"[{x}]"))
                polesString = polesString + pole + ",";
            try
            {
                string nameTable = await GetNameTableAsync(idTask);
                string zapros = $"select {polesString} Chainid from oktell.dbo.{nameTable} where (Dtime BETWEEN convert(datetime, '{minDtime.ToString("dd.MM.yyyy")} 00:00:01',104) AND convert(datetime, '{maxDtime.ToString("dd.MM.yyyy")} 23:59:59',104)) order by id desc;";
                Debug.WriteLine(zapros);
                DataTable dataTable = new DataTable();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dataTable);
                    }
                    await con.CloseAsync();
                }

                if (dataTable.Columns.Contains("chainid"))
                    dataTable.Columns["chainid"].ColumnName = "Chainid";
                return dataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetAbonentTable: " + ex.Message);
                return new DataTable();
            }
        }
        public async Task<Dictionary<string, List<ConnModel>>> GetConnPairsAsync(string idTask, DateTime minDtime, DateTime maxDtime, bool necessarilyRecorded = true)
        {
            Debug.WriteLine("Делаем запрос GetConnPairs");
            //if (idTask == "")
            {
                necessarilyRecorded = false;
            }
            string zapros = $"EXECUTE AlphaData_CallInfo @idTask = '{idTask}', @ADtime = '{minDtime.ToString("dd.MM.yyyy")}', @BDtime = '{maxDtime.ToString("dd.MM.yyyy")}', @Recorded={(necessarilyRecorded ? "'1'" : "'0'")};";
            Debug.WriteLine(zapros);
            try
            {
                Dictionary<string, List<ConnModel>> connPairs = new Dictionary<string, List<ConnModel>>();
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    // cmd.CommandTimeout = 3600;
                    await con.OpenAsync();

                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            ConnModel connModel = new ConnModel();
                            connModel.IdChain = dr["IdChain"].ToString();
                            connModel.IdConn = dr["IdConn"].ToString();
                            connModel.LenTime = dr["LenTime"].ToString();
                            connModel.IsOutput = (Boolean)dr["IsOutput"];
                            connModel.AbonentNumber = dr["AbonentNumber"].ToString();
                            connModel.DateTimeStart = dr["DateTimeStart"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)dr["DateTimeStart"];
                            connModel.DateTimeStop = dr["DateTimeStop"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)dr["DateTimeStop"];
                            connModel.TimeStart = dr["TimeStart"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)dr["TimeStart"];
                            connModel.IdPrev = dr["IdPrev"].ToString();
                            connModel.alinenum = dr["alinenum"].ToString();
                            connModel.blinenum = dr["blinenum"].ToString();
                            connModel.astr = dr["astr"].ToString();
                            connModel.bstr = dr["bstr"].ToString();
                            connModel.ConnectionType = dr["ConnectionType"].ToString();
                            connModel.IdInList = dr.GetInt32("IdInList");
                            if (connPairs.ContainsKey(connModel.IdChain))
                                connPairs[connModel.IdChain].Add(connModel);
                            else
                                connPairs.Add(connModel.IdChain, new List<ConnModel> { connModel });
                        }
                    }
                    await con.CloseAsync();
                }

                return connPairs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetConnPairs: " + ex.Message);
                return null;
            }
        }

        public Task<DataTable> GetCallReportDataAsync(Guid taskId, DateTime minDateTime, DateTime maxDateTime)
        {
            DataTable dataTable = new DataTable();
            minDateTime = minDateTime.Date;
            maxDateTime = maxDateTime.Date.Add(TimeSpan.FromDays(1) - TimeSpan.FromTicks(1));

            using (SqlConnection connection = new SqlConnection(ConnectionPathSQL.alphacentre_db))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[GetCallReportAndInfoFromTableAbonent]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdTask", taskId);
                    command.Parameters.AddWithValue("@minDtime", minDateTime);
                    command.Parameters.AddWithValue("@maxDtime", maxDateTime);

                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return Task.FromResult(dataTable);
        }
    }
}
