using alphaData.Works.SQLWork;

using AlphaData.Models.Statistics.Account;
using AlphaData.Models.Statistics.Journal;
using AlphaData.Models.Statistics.Management;

using ClosedXML.Excel;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace alphaData.Controllers
{
    public class StatisticsController : Controller
    {
        StatisticsSQL statisticsSQL = new StatisticsSQL();

        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        #region M A N A G E M E N T
        public IActionResult Management()
        {
            return View();
        }
        public async Task<IActionResult> _LkList()
        {
            List<LkModel> lkModel = await statisticsSQL.GetLkListAsync();
            return View("ModalViews/Management/_LkList", lkModel);
        }
        public async Task<IActionResult> _AddLk()
        {
            AddLkModel addLkModel = new AddLkModel();
            addLkModel.projectModels = await statisticsSQL.GetProjectListAsync();
            return View("ModalViews/Management/_AddLk", addLkModel);
        }
        public async Task<IActionResult> DeleteLk(string idLk)
        {
            await statisticsSQL.DeleteLkAsync(idLk);
            return Ok();
        }
        public async Task<IActionResult> InsertLk(string idProject, string login, string password)
        {
            await statisticsSQL.InsertLkAsync(idProject, login, password);
            return Redirect(Url.Action("Management", "Statistics"));
        }
        public async Task<IActionResult> OpenJournal(string idLk)
        {
            LkModel lkModel = await statisticsSQL.GetLoginLk(idLk);
            if (lkModel.login != null && lkModel.pass != null)
                return Redirect(Url.Action("Journal", new { login = lkModel.login, password = lkModel.pass }));
            else
                return Redirect(Url.Action("Journal"));
        }
        #endregion


        /// A C C O U N T
        /// A C C O U N T
        /// A C C O U N T
        #region A C C O U N T
        public async Task<IActionResult> Account(string idLK)
        {
            if (idLK == null)
                return Content("Не указан id кабинета");
            AccountModel accountModel = new AccountModel();
            accountModel.idLk = idLK;
            accountModel.myTaskModels = await statisticsSQL.GetMyTaskListAsync(idLK);
            return View(accountModel);
        }
        public async Task<IActionResult> _TaskAdd(string idLk)
        {
            AddTaskModel addTaskModel = new AddTaskModel();
            addTaskModel.idLk = idLk;
            addTaskModel.systemTaskModels = await statisticsSQL.GetSystemTaskListAsync();

            List<MyTaskModel> myTaskModels = await statisticsSQL.GetMyTaskListAsync(idLk);
            addTaskModel.systemTaskModels = addTaskModel.systemTaskModels.Where(task =>
            {
                foreach (MyTaskModel myTaskModel in myTaskModels)
                    if (myTaskModel.idTask == task.idTask)
                        return false;
                return true;
            }).ToList();
            return View("ModalViews/Account/_TaskAdd", addTaskModel);
        }
        public async Task<IActionResult> InsertTask(string idLk, string idTask)
        {
            System.Diagnostics.Debug.WriteLine(idLk + "    " + idTask);
            await statisticsSQL.InsertTaskAsync(idLk, idTask);
            return Redirect(Url.Action("Account", "Statistics", new { idLk }));
        }
        public async Task<string> UpdatePoles(string idLk, string idTask, string poles)
        {
            await statisticsSQL.UpdatePolesAsync(idLk, idTask, poles);
            return "Обновлено";
        }
        public async Task<IActionResult> _Settings(string idLk)
        {
            SettingsModel settingsModel = new SettingsModel();
            settingsModel.idLk = idLk;
            settingsModel.typeTables = await statisticsSQL.GetSettingsTypeTable(idLk);
            return View("ModalViews/Account/_settings", settingsModel);
        }
        public async Task<IActionResult> UpdateSettingsTypeTable(string idLk, string[] typeTable)
        {
            string type = "";
            foreach (string typeTableString in typeTable)
            {
                type = type + "," + typeTableString;
            }
            await statisticsSQL.UpdateSettingsTypeTable(idLk, type);
            return Redirect(Url.Action("Account", "Statistics", new { idLk }));
        }
        #endregion


        /// J O U R N A L
        /// J O U R N A L
        /// J O U R N A L
        #region J O U R N A L
        public async Task<IActionResult> Journal(string login, string password)
        {
            if (login == null || password == null)
                return Content("Неверно указаны данные");

            JournalModel journalModel = new JournalModel();
            journalModel.idLk = await statisticsSQL.GetIdLkLogin(login, password);
            if (journalModel.idLk == null)
                return Content("Неверно указаны данные");

            journalModel.typeTables = await statisticsSQL.GetSettingsTypeTable(journalModel.idLk);
            journalModel.myTaskModels = await statisticsSQL.GetMyTaskListAsync(journalModel.idLk);
            return View(journalModel);
        }
        /// <summary>
        /// Частичное представление
        /// </summary>
        /// <param name="typeTable">abonent - Отчет; call - Звонки; full - Отчет + Звонки</param>
        /// <returns></returns>
        public async Task<IActionResult> _JournalTable(string typeTable, string idLk, string idTask, string period)
        {
            if (idLk == null)
                return Content("Ошибка");
            if (idTask == null)
                return Content("Выберите проект");
            if (period == null || period.Length != 21)
                return Content("Выберите период");


            DateTime minDtime = DateTime.Parse(period.Split('-')[0]);
            DateTime maxDtime = DateTime.Parse(period.Split('-')[1]);
            DataTable dataTable;
            switch (typeTable)
            {
                case "abonent":
                    dataTable = await AbonentTable(idLk, idTask, minDtime, maxDtime);
                    break;
                case "call":
                    dataTable = await CallTable(idTask, minDtime, maxDtime);
                    break;
                case "full":
                    dataTable = await FullTable(idLk, idTask, minDtime, maxDtime);
                    break;
                case "reportData": //todo
                    dataTable = await GetCallReportData(idLk, Guid.Parse(idTask), minDtime, maxDtime);
                    break;
                default:
                    return Content("Ошибка в типе отчета.");
            }


            //специальные настройки
            var idTaskUpper = idTask.ToUpper();
            if (idTaskUpper == "94CF0D42-1174-40FF-9922-E4471F2DC5B1")
            {

                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["operator"].ToString().Contains("opit011") || row["operator"].ToString().Contains("opit024"))
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // Теперь удалите строки из DataTable
                foreach (DataRow row in rowsToDelete)
                {
                    dataTable.Rows.Remove(row);
                }
            }
            else if (idTaskUpper == "0F4AC382-6719-4462-9C59-6C86BDD8ADEF" || idTaskUpper  == "43F371CA-CE55-4705-AA90-3C33E4A0CD0C")
            {
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (DataRow row in dataTable.Rows)
                {
                    if (DateTime.Parse(row["Dtime"].ToString()) < DateTime.Parse("29.01.2024 14:24") 
                        || row["Phone"].ToString() == "79288722666"
                        || row["Phone"].ToString() == "79870681226")
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // Теперь удалите строки из DataTable
                foreach (DataRow row in rowsToDelete)
                {
                    dataTable.Rows.Remove(row);
                }
            }
            else if (idTaskUpper == "A7C7A569-0E28-4D71-800F-C05892D95765")
            {
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["Dtime"].ToString().Contains("17.05.2024") && row["Dtime"].ToString().Contains("12:31") && row["Phone"].ToString() == "79516001166")
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // Теперь удалите строки из DataTable
                foreach (DataRow row in rowsToDelete)
                {
                    dataTable.Rows.Remove(row);
                }
            }

            if (dataTable.Columns.Contains("Chainid"))
                dataTable.Columns.Remove("Chainid");
            if (dataTable.Columns.Contains("Comid"))
                dataTable.Columns.Remove("Comid");
            return View("ModalViews/Journal/_JournalTable", dataTable);
        }
        public async Task<DataTable> AbonentTable(string idLk, string idTask, DateTime minDtime, DateTime maxDtime)
        {
            List<string> poles = await statisticsSQL.GetMyPoleAsync(idLk, idTask);
            return await statisticsSQL.GetAbonentTableAsync(idTask, poles, minDtime, maxDtime);
        }

        public async Task<DataTable> CallTable(string idTask, DateTime minDtime, DateTime maxDtime)
        {
            Dictionary<string, List<ConnModel>> connPairs = await statisticsSQL.GetConnPairsAsync(idTask, minDtime, maxDtime);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Operator");
            dataTable.Columns.Add("Phone");
            dataTable.Columns.Add("Dtime");
            dataTable.Columns.Add("AudioCall");
            dataTable.Columns.Add("AudioRedirection");
            dataTable.Columns.Add("Chainid");
            dataTable.Columns.Add("IdInList");
            int AudElCount = 0;
            TimeSpan FullTimeAllCall = TimeSpan.Zero;
            if (connPairs != null)
            {
                AudElCount = connPairs.Count();

            }

            foreach (List<ConnModel> connModels in connPairs.Values)
            {
                DataRow dataRow = dataTable.NewRow();

                foreach (ConnModel connModel in connModels)
                {
                    FullTimeAllCall += connModel.DateTimeStop - connModel.DateTimeStart;
                    dataRow["Chainid"] = connModels[0].IdChain;
                    dataRow["Dtime"] = connModels[0].DateTimeStart;
                    dataRow["IdInList"] = connModels[0].IdInList;
                    string urlPlayer = Url.Action("Download", "Statistics", new { mixPath = connModel.pathURL });
                    string divPlayer;
                    //  @$"<div> <button onclick='NewAud('{urlPlayer}')'> </button></div>";
                    //<button onclick="goToPage()">Go to Page</button>
                    if (AudElCount > 9999)//~30 See crbug.com/1144736#c27
                    {
                        //to do  <button onclick="$(this).parent().append('<div>123</div>')">Для прослушивания нажмите на кнопку</button>
                        //  <audio  disabled=""disabled"" class=""player"" id=""{connModels[0].IdChain}_aud"" src=""{urlPlayer}"">Браузер устарел</audio>

                        divPlayer = @$" <div>                                           
                                            <button onclick='CreateAudElement(this,""{connModels[0].IdChain}_aud"",""{urlPlayer}"");'>Для прослушивания нажмите на кнопку</button> 
                                        </div>";
                    }
                    else
                    {
                        divPlayer = @$"<div>    <audio class='player' id='{connModels[0].IdChain}_aud' src='{urlPlayer}'>Браузер устарел</audio></div>";
                    }

                    if (connModel.IsOutput) // исх задача
                    {
                        dataRow["Operator"] = connModels[0].astr;
                        dataRow["Phone"] = connModels[0].bstr;
                        if (connModel.ConnectionType == "1")
                            dataRow["AudioCall"] = dataRow["AudioCall"].ToString() + divPlayer;

                        //нет обычного разговора
                        bool onlyDialing = connModels.FirstOrDefault(x => x.ConnectionType != "-1") == null;
                        if (connModel.ConnectionType == "-1" 
                            && onlyDialing
                            //&& idTask == "24f6803b-1ffc-4825-9349-49d96d3753cd"
                            )//попытка дозвона
                        {
                            dataRow["AudioCall"] = dataRow["AudioCall"].ToString() + divPlayer;
                        }
                    }
                    if (!connModel.IsOutput) // вх задача
                    {
                        dataRow["Operator"] = connModels[0].bstr;
                        dataRow["Phone"] = connModels[0].astr;
                        if (connModel.ConnectionType == "5") // Переадресация
                            dataRow["AudioCall"] = dataRow["AudioCall"].ToString() + divPlayer;
                        if (connModel.ConnectionType == "1")
                            dataRow["AudioRedirection"] = dataRow["AudioRedirection"].ToString() + divPlayer;
                    }
                }
                dataTable.Rows.Add(dataRow);
            }
            Debug.WriteLine("Full Time In Table second" + FullTimeAllCall.TotalSeconds);
            return dataTable;
        }

        public async Task<DataTable> FullTable(string idLk, string idTask, DateTime minDtime, DateTime maxDtime)
        {
            DataTable callTable = await CallTable(idTask, minDtime, maxDtime); //~ effort
            DataTable abonentTable = await AbonentTable(idLk, idTask, minDtime, maxDtime); // ~TA

            if (abonentTable.Columns.Contains("Dtime"))
            {
                foreach (DataRow item in abonentTable.Rows)
                {
                    try
                    {
                        if (item["Chainid"] == null || item["Chainid"].ToString() == "")
                        {
                            var mb = callTable.AsEnumerable()
                                .Where(x => x["Phone"] == item["Phone"])
                                    .Select(x => (x["Chainid"].ToString(), agr: Math.Abs((DateTime.Parse(x["Dtime"].ToString()) - (DateTime)item["Dtime"]).TotalMilliseconds)));
                            item["Chainid"] = mb.OrderBy(x => x.agr).FirstOrDefault().Item1;
                            if (item["Chainid"] == null)
                            {
                                //todo Warning 
                                item["Chainid"] = Guid.NewGuid().ToString();
                            }
                            //  item["Chainid"] = mb.FirstOrDefault()?["Chainid"];
                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

            if (abonentTable.Columns.Contains("Dtime"))
                abonentTable.Columns.Remove("Dtime");

            if (abonentTable.Columns.Contains("Operator"))
                abonentTable.Columns.Remove("Operator");
            foreach (DataColumn abonentColumn in abonentTable.Columns)
                if (!callTable.Columns.Contains(abonentColumn.ColumnName))
                    callTable.Columns.Add(abonentColumn.ColumnName);

            List<object[]> dubli = new List<object[]>();
            foreach (DataRow callRow in callTable.Rows)
            {
                try
                {
                    if (callRow.Table.Columns.Contains("IdInList"))
                    {
                        DataRow[] abonentRows = abonentTable.Select($"id = '{callRow["IdInList"]}'");

                        if (abonentRows.Count() >= 1)
                            foreach (DataColumn abonentColumn in abonentTable.Columns)
                            {
                                callRow[abonentColumn.ColumnName] = abonentRows[0][abonentColumn.ColumnName];
                            }
                        if (abonentRows.Count() > 1)
                        {
                            dubli.Add(callRow.ItemArray);
                        }
                    }
                    else if (callRow.Table.Columns.Contains("Chainid"))
                    {
                        DataRow[] abonentRows = abonentTable.Select($"Chainid = '{callRow["Chainid"]}'");

                        if (abonentRows.Count() >= 1)
                            foreach (DataColumn abonentColumn in abonentTable.Columns)
                            {
                                callRow[abonentColumn.ColumnName] = abonentRows[0][abonentColumn.ColumnName];
                            }
                        if (abonentRows.Count() > 1)
                        {
                            dubli.Add(callRow.ItemArray);
                        }
                    }
                }
                catch (Exception ex)
                {

                    //throw;
                }
            }

            callTable.Columns.Remove("IdInList");
            if (callTable.Columns.Contains("id"))
            {
                callTable.Columns.Remove("id");
            }
            


            return RemoverDubleDtimePhone(callTable);
        }

        public async Task<DataTable> GetCallReportData(string idLk, Guid taskId, DateTime minDateTime, DateTime maxDateTime)
        {
            DataTable dataTable = await statisticsSQL.GetCallReportDataAsync(taskId,minDateTime,maxDateTime);

            return dataTable;
        }

        private DataTable RemoverDubleDtimePhone(DataTable dataTable)
        {
            DataView view = new DataView(dataTable);
            DataTable distinctValues = view.ToTable(true);
            return distinctValues;
        }
        #endregion


        /// O T H E R
        /// O T H E R
        /// O T H E R
        #region
        private DataTable RenameCol(DataTable dataTable)
        {
            return dataTable;
        }


        public FileResult Download(string mixPath)
        {
            if (mixPath == null)
                return null;
            string fullPath;
            if (mixPath.Contains("mix_0_0__"))
                fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aud", "mix_1m00s.mp3");
            else
                fullPath = @"\\192.168.0.98\RecordedFiles\" + mixPath;


            byte[] fileBytes;// = System.IO.File.ReadAllBytes(fullPath);
            string fileName;//= System.IO.Path.GetFileName(fullPath);
            try
            {
                fileBytes = System.IO.File.ReadAllBytes(fullPath);
                fileName = System.IO.Path.GetFileName(fullPath);
                //if (!mixPath.Contains("mix_0_0__"))
                //    throw new Exception();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, /*fileName*/ Guid.NewGuid().ToString() + ".mp3");
            }
            catch (Exception)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential("WebAPI", "ZuQ1S57R123");
                        var pathDownloadOktellApi = @"http://192.168.0.98:4055/download/byscript?name=Echo&startparam1=..\Server\RecordedFiles\";
                        fullPath = pathDownloadOktellApi + mixPath;
                        fileBytes = client.DownloadData(fullPath);                        
                    }
                
                    //fileName = System.IO.Path.GetFileName(fullPath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, /*fileName*/ Guid.NewGuid().ToString() + ".mp3");

                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        public IActionResult DownloadChainId(string idChain)
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("oktell", "pF23nz#x8G04sATIN3DV%e%IOlE!");
                var pathAudHTML = @"http://192.168.0.27/RecordingСallNotification/api/Audio/GetAudioPlayerFromChain/" + idChain;
                var html = client.DownloadString(pathAudHTML);
                return Content(html, "text/html");
            }
        }
        public async Task<IActionResult> ExcelDownload(string typeTable, string idLk, string idTask, string period)
        {
            if (idLk == null || typeTable == null || idTask == null || period == null)
                return Content("Выберите задачу и период");
            try
            {
                DateTime minDtime = DateTime.Parse(period.Split('-')[0]);
                DateTime maxDtime = DateTime.Parse(period.Split('-')[1]);
                DataTable dataTable;
                switch (typeTable)
                {
                    case "abonent":
                        dataTable = await AbonentTable(idLk, idTask, minDtime, maxDtime);
                        break;
                    case "call":
                        dataTable = await CallTable(idTask, minDtime, maxDtime);
                        break;
                    case "full":
                        dataTable = await FullTable(idLk, idTask, minDtime, maxDtime);
                        break;
                    default:
                        return Content("Ошибка в типе отчета.");
                }

                if (dataTable.Columns.Contains("Chainid"))
                    dataTable.Columns.Remove("Chainid");
                if (dataTable.Columns.Contains("Comid"))
                    dataTable.Columns.Remove("Comid");
                if (dataTable.Columns.Contains("AudioCall"))
                    dataTable.Columns.Remove("AudioCall");
                if (dataTable.Columns.Contains("AudioRedirection"))
                    dataTable.Columns.Remove("AudioRedirection");

                dataTable = await RenameTable(dataTable);

                string nameTable = await statisticsSQL.GetNameTableAsync(idTask);
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable, nameTable);
                    using (MemoryStream myMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(myMemoryStream);
                        myMemoryStream.Position = 0;
                        var content = myMemoryStream.ToArray();
                        myMemoryStream.Close();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{nameTable}_{period}.xlsx");
                    }
                }
            }
            catch
            {
                return Content("Произошла ошибка в построении файла.");
            }
        }

        private static async Task<DataTable> RenameTable(DataTable dataTable)
        {
            StatisticsSQL statisticsSQL = new StatisticsSQL();
            var renameTable = await statisticsSQL.GetRenameTable("RenamedTable");

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                //переименовование столбцов
                var rowsName = renameTable.Select($"original = '{dataTable.Columns[i].ColumnName}'");
                if (rowsName.Length > 0)
                {
                    string name = rowsName[0]["renamed"].ToString();
                    if (name != null)
                    {
                        if (name != "")
                        {
                            dataTable.Columns[i].Caption = name;
                        }

                    }
                }
            }

            return dataTable;
        }
        #endregion

    }
}
