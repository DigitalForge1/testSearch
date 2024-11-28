using alphaData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AlphaData.Models.Statistics;
using AlphaData.Models.Statistics.Management;

// https://www.aspsnippets.com/Articles/ASPNet-Core-MVC-Using-DataSet-DataTable-as-Model-in-View.aspx
namespace alphaData.Works.SQLWork
{
    public class StatisticsSQL
    {
        /// <summary>
        /// Journal
        /// </summary>
        /// <returns></returns>
        public async Task<List<ConModel>> GetConModels(string idTask, DateTime aDtime, DateTime bDtime)
        {
            Debug.WriteLine("Делаем запрос GetConModels");

            List<ConModel> connectionDataModels = new List<ConModel>();
            string zapros = $"EXECUTE ConnectionTask @idTask = '{idTask}', @ADtime = '{aDtime.ToString("dd.MM.yyyy")}', @BDtime = '{bDtime.ToString("dd.MM.yyyy")}';";
            Debug.WriteLine("GetConModels: " + zapros);
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
                            // Получаем данные
                            ConModel connectionDataModel = new ConModel();
                            connectionDataModel.IdChain = dr["IdChain"].ToString();
                            connectionDataModel.IdConn = dr["IdConn"].ToString();
                            connectionDataModel.LenTime = dr["LenTime"].ToString();
                            connectionDataModel.IsOutput = (Boolean)dr["IsOutput"];
                            connectionDataModel.AbonentNumber = dr["AbonentNumber"].ToString();
                            connectionDataModel.DateTimeStart = (DateTime)dr["DateTimeStart"];
                            connectionDataModel.DateTimeStop = (DateTime)dr["DateTimeStop"];
                            connectionDataModel.TimeStart = (DateTime)dr["TimeStart"];
                            connectionDataModel.IdPrev = dr["IdPrev"].ToString();
                            connectionDataModel.alinenum = dr["alinenum"].ToString();
                            connectionDataModel.blinenum = dr["blinenum"].ToString();
                            connectionDataModel.astr = dr["astr"].ToString();
                            connectionDataModel.bstr = dr["bstr"].ToString();
                            connectionDataModel.ConnectionType = dr["ConnectionType"].ToString();
                            connectionDataModels.Add(connectionDataModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return connectionDataModels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetConModels: " + ex.Message);
                return connectionDataModels;
            }
        }

        public async Task<DataTable> GetAbonentTable(string nameTable,string poleList, DateTime aDtime, DateTime bDtime)
        {
            Debug.WriteLine("Делаем запрос GetAbonentTable");
            DataTable dataTable = new DataTable();
            string zapros = $"select {poleList} from oktell.dbo.{nameTable} where (Dtime BETWEEN convert(datetime, '{aDtime.ToString("dd.MM.yyyy")} 00:00:01',104) AND convert(datetime, '{bDtime.ToString("dd.MM.yyyy")} 23:59:59',104)) order by id desc;";
            Debug.WriteLine(zapros);
            try
            {
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
                dataTable.Columns.Add("audioPath");
                dataTable.Columns.Add("redirectPath");

                return dataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetAbonentTable: " + ex.Message);
                return new DataTable();
            }
        }



        public async Task<LKModel> Get_LK(string login, string password)
        {
            LKModel lKModel = new LKModel();
            string zapros = $"select idLK, idProject, loginLK, passLK from Alphadata_lk where loginLK = '{login}' and passLK = '{password}';";
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
                            lKModel.idLK = dr["idLK"].ToString();wq
                            lKModel.login = dr["loginLK"].ToString();
                            lKModel.password = dr["passLK"].ToString();
                            lKModel.idProject = dr["idProject"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return lKModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка Get_LKList: " + ex.Message);
                return lKModel;
            }
        }
        public async Task<List<TaskModel>> GetTaskList(string idLK)
        {
            Debug.WriteLine("Делаем запрос GetTaskList");
            List<TaskModel> taskModelList = new List<TaskModel>();
            string zapros = $"EXEC GetMyTask @idLK = '{idLK}';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            TaskModel task = new TaskModel();
                            task.idTask = dr["idTask"].ToString();
                            task.nameTask = dr["nameTask"].ToString();
                            task.nameTable = dr["nameTable"].ToString();
                            task.poleList = dr["poleList"].ToString().Split(',').ToList();
                            task.sendRep = dr["sendRep"].ToString();
                            task.mailRep = dr["mailRep"].ToString();
                            taskModelList.Add(task);
                        }
                    }
                    await con.CloseAsync();
                }
                return taskModelList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetTaskList: " + ex.Message);
                return taskModelList;
            }
        }
        public async Task<TaskModel> GetTask(string idLK, string idTask)
        {
            Debug.WriteLine("Делаем запрос GetTask");
            TaskModel taskLKModel = new TaskModel();
            string zapros = $"EXEC GetMyTask @idLK = '{idLK}', @idTask = '{idTask}';";
            Debug.WriteLine(zapros);
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionPathSQL.alphacentre_db))
                using (SqlCommand cmd = new SqlCommand(zapros, con))
                {
                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            taskLKModel.idTask = dr["idTask"].ToString();
                            taskLKModel.nameTask = dr["nameTask"].ToString();
                            taskLKModel.nameTable = dr["nameTable"].ToString();
                            taskLKModel.poleList = dr["poleList"].ToString().Split(',').ToList();
                            taskLKModel.sendRep = dr["sendRep"].ToString();
                            taskLKModel.mailRep = dr["mailRep"].ToString();
                        }
                    }
                    await con.CloseAsync();
                }
                return taskLKModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetTask: " + ex.Message);
                return taskLKModel;
            }
        }


        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        public async Task<List<ProjectModel>> GetProjectList()
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
        public async Task<List<LkModel>> GetLkList()
        {
            Debug.WriteLine("Делаем запрос GetLkList");
            List<LkModel> lkModels = new List<LkModel>();
            string zapros = $"select Lk.idLK, lk.loginLK, Lk.passLK ,Projects.[Name], Lk.idProject from Alphadata_lk as Lk left join oktell_settings.dbo.A_TaskManager_Projects as Projects ON Projects.Id = Lk.idProject order by Projects.[Name]";
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
        public async Task DeleteLk(string idLk)
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
        public async Task InsertLk(string idProject, string login, string pass)
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

        



        public async Task Update_Pole(string idLK, string idTask, string pole)
        {
            Debug.WriteLine("Делаем запрос Update_Pole");
            string zapros = $"update Alphadata_task set poleList = '{pole}' where idLK = '{idLK}' and idTask = '{idTask}';";
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
                Debug.WriteLine("Произошла ошибка Update_Pole: " + ex.Message);
            }
        }


        /// <summary>
        /// Account
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetFullPoleList(string nameTable)
        {
            Debug.WriteLine("Делаем запрос GetFullPoleList");
            List<string> fullPoleList = new List<string>();
            string zapros = $"Exec GetFullPoleList @nameTable = '{nameTable}';";
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
                            fullPoleList = dr["FullPoleList"].ToString().Replace("[", "").Replace("]", "").Replace(" ", "").Split(',').ToList();
                        }
                    }
                    await con.CloseAsync();
                }
                return fullPoleList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetFullPoleList: " + ex.Message);
                return fullPoleList;
            }
        }

        public async Task<List<TaskModel>> GetFullTaskList()
        {
            Debug.WriteLine("Делаем запрос GetFullTaskList");
            List<TaskModel> fullTasks = new List<TaskModel>();
            string zapros = $"EXEC GetSystemTask;";
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
                            TaskModel taskModel = new TaskModel();
                            taskModel.idTask = dr["idTask"].ToString();
                            taskModel.nameTask = dr["Name"].ToString();
                            taskModel.nameTable = dr["nameTable"].ToString();
                            fullTasks.Add(taskModel);
                        }
                    }
                    await con.CloseAsync();
                }
                return fullTasks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка GetFullTaskList: " + ex.Message);
                return fullTasks;
            }
        }

        public async Task Insert_Task(string idLk, string idTask, string poleList)
        {
            Debug.WriteLine("Делаем запрос Insert_Task");
            string zapros = $"insert into Alphadata_task (idLK,idTask,poleList) values ('{idLk}','{idTask}','{poleList}');";
            Debug.WriteLine(zapros);
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
                Debug.WriteLine("Произошла ошибка Insert_Task: " + ex.Message);
            }
        }

    }
}
