using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using alphaData.Models;
using alphaData.Works.SQLWork;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

using AlphaData.Models.Statistics;
using AlphaData.Models.Statistics.Management;

namespace alphaData.Controllers
{
    public class StatisticsController : Controller
    {
        StatisticsSQL statisticsSQL = new StatisticsSQL();

        /// <summary>
        /// Journal
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Journal_old(string login, string password)
        {
            if (login == null || password == null)
                return Content("Ошибка в обращении. Проверьте правильность данных.");//View(dataModel);

            JournalModel journalModel = new JournalModel();
            journalModel.lkModel = await statisticsSQL.Get_LK(login, password);
            journalModel.myTaskList = await statisticsSQL.GetTaskList(journalModel.lkModel.idLK);
            return View(journalModel);
        }
        public async Task<IActionResult> _JournalTable(string idLK, string idTask, string period)
        {
            try
            {
                TaskModel myTask = await statisticsSQL.GetTask(idLK, idTask);
                DataTable dataTable = await GetTable(myTask.nameTable, myTask.idTask, myTask.poleList, period);
                return View("_JournalTable", dataTable);
            } catch
            {
                return Content("Ошибка в загрузке таблицы");
            }
        }
        private async Task<DataTable> GetTable(string nameTable, string idTask, List<string> poleList, string period)
        {
            DateTime aDtime;
            DateTime bDtime;
            string poleComma = "";
            foreach (string pole in poleList)
            {
                if (poleComma != "")
                    poleComma = poleComma + "," + pole;
                else
                    poleComma = pole;
            }

            try
            {
                string[] periodArray = period.Split('-');
                aDtime = DateTime.Parse(periodArray[0]);
                bDtime = DateTime.Parse(periodArray[1]);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Произошла ошибка в парсинге дат");
                return new DataTable();
            }

            List<ConModel> connectionDataModels = await statisticsSQL.GetConModels(idTask, aDtime, bDtime);
            DataTable dataTable = await statisticsSQL.GetAbonentTable(nameTable, poleComma, aDtime, bDtime);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    string phone = dataRow["Phone"].ToString();
                    if (dataRow.Table.Columns.Contains("chainid"))
                    {
                        string chainId = dataRow["chainid"].ToString();
                        if (chainId.Length > 10)
                        {
                            foreach (ConModel conModel in connectionDataModels)
                                if (chainId == conModel.IdChain)
                                    foreach (ConModel redirectCdm in connectionDataModels)
                                        if (conModel.IdChain == redirectCdm.IdChain)
                                        {
                                            redirectCdm.search = true;
                                            string urlPlayer = Url.Action("Download", "Statistics", new { mixPath = redirectCdm.pathURL }, null);
                                            string divPlayer = $"<div><audio class='player' src='{urlPlayer}'>Браузер устарел</audio></div>";
                                            if (redirectCdm.ConnectionType == "5")// Разговор
                                            {
                                                dataRow["audioPath"] = dataRow["audioPath"].ToString() + divPlayer;
                                            }
                                            if (redirectCdm.ConnectionType == "1") // Переадресация | исх
                                            {
                                                if (redirectCdm.IsOutput)
                                                    dataRow["audioPath"] = dataRow["audioPath"].ToString() + divPlayer;
                                                else
                                                    dataRow["redirectPath"] = dataRow["redirectPath"].ToString() + divPlayer;
                                            }
                                        }
                        }
                        else
                            goto PhoneDtime;
                    }

                PhoneDtime:
                    DateTime dtime = (DateTime)dataRow["Dtime"];
                    foreach (ConModel conModel in connectionDataModels)
                        if (conModel.AbonentNumber.Contains(phone.Remove(0, 1)))
                            if ((conModel.DateTimeStart <= dtime.AddSeconds(30) && conModel.DateTimeStart >= dtime.AddSeconds(-30)) ||
                                (conModel.DateTimeStop <= dtime.AddSeconds(60) && conModel.DateTimeStop >= dtime.AddSeconds(-60)))
                                foreach (ConModel redirectCdm in connectionDataModels)
                                    if (conModel.IdChain == redirectCdm.IdChain)
                                    {
                                        redirectCdm.search = true;
                                        string urlPlayer = Url.Action("Download", "Statistics", new { mixPath = redirectCdm.pathURL }, null);
                                        string divPlayer = $"<div><audio class='player' src='{urlPlayer}'>Браузер устарел</audio></div>";
                                        if (redirectCdm.ConnectionType == "5")// Разговор
                                        {
                                            dataRow["audioPath"] = dataRow["audioPath"].ToString() + divPlayer;
                                        }
                                        if (redirectCdm.ConnectionType == "1") // Переадресация | исх
                                        {
                                            if (redirectCdm.IsOutput)
                                                dataRow["audioPath"] = dataRow["audioPath"].ToString() + divPlayer;
                                            else
                                                dataRow["redirectPath"] = dataRow["redirectPath"].ToString() + divPlayer;
                                        }
                                    }
                }
                catch
                {
                    continue;
                }
            }

            return dataTable;
        }


        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        ///  M A N A G E M E N T
        public async Task<IActionResult> Management()
        {
            return View();
        }
        public async Task<IActionResult> _LkList()
        {
            List<LkModel> lkModel = await statisticsSQL.GetLkList();
            return View("ModalViews/Management/_LkList", lkModel);
        }
        public async Task<IActionResult> DeleteLk(string idLk)
        {
            await statisticsSQL.DeleteLk(idLk);
            return Ok();
        }
        public async Task<IActionResult> InsertLk(string idProject, string login, string password)
        {
            await statisticsSQL.InsertLk(idProject, login, password);
            return Redirect(Url.Action("Management","Statistics"));
        }
        public async Task<IActionResult> OpenJournal(string idLk)
        {
            return Redirect(Url.Action("Account"));
        }


        /// A C C O U N T
        /// A C C O U N T
        /// A C C O U N T
        public async Task<IActionResult> Account(string idLK)
        {
            return View();
        }

        /// J O U R N A L
        /// J O U R N A L
        /// J O U R N A L
        public async Task<IActionResult> Journal()
        {
            return View();
        }





        //===============
        public async Task<IActionResult> _AddLk()
        {
            AddLkModel addLkModel = new AddLkModel();
            addLkModel.projectModels = await statisticsSQL.GetProjectList();
            return View("ModalViews/Management/_AddLk", addLkModel);
        }

        public async Task<IActionResult> AddLK(string IdProjectSelect_AddLK, string login_AddLK, string pass_AddLk)
        {
            //await statisticsSQL.Insert_Lk(IdProjectSelect_AddLK, login_AddLK, pass_AddLk);
            return Redirect(Url.Action("Management", "Statistics"));
        }
        /// <summary>
        /// Account
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Account_old(string login, string password)
        {
            if (login == null || password == null)
                return Content("Произошла ошибка. Неверно введены данные.");

            AccountModel accountModel = new AccountModel();
            accountModel.lKModel = await statisticsSQL.Get_LK(login, password);
            accountModel.systemTaskList = await statisticsSQL.GetFullTaskList();
            accountModel.myTaskList = await statisticsSQL.GetTaskList(accountModel.lKModel.idLK);
            foreach(TaskModel myTask in accountModel.myTaskList)
            {
                myTask.fullPoleList= await statisticsSQL.GetFullPoleList(myTask.nameTable);
                List<string> clearPole = myTask.poleList;
                myTask.poleList = new List<string>();
                foreach (string pole in clearPole)
                {
                    if (myTask.fullPoleList.Contains(pole))
                        myTask.poleList.Add(pole);
                }
            }

            return View(accountModel);
        }


        public async Task<string> SavePole(string idLK, string idTask, string pole)
        {
            if(idLK == null || idTask == null || pole == null)
                return "Нет данных";

            await statisticsSQL.Update_Pole(idLK, idTask, pole);
            return "Сохранено";
        }


        public async Task<IActionResult> AddTask(string taskInfo, string idLK, string login, string password)
        {
            string idTask = taskInfo.Split('|')[0];
            string nameTable = taskInfo.Split('|')[1];
            List<string> fullPoleList = await statisticsSQL.GetFullPoleList(nameTable);
            string fullPole = "";
            foreach (string pole in fullPoleList)
            {
                if (fullPole == "")
                    fullPole = pole;
                else
                    fullPole = fullPole + "," + pole;
            }
            await statisticsSQL.Insert_Task(idLK, idTask, fullPole);
            return Redirect(Url.Action("Account", "Statistics", new LKModel { idLK = idLK, login = login, password = password }));
        }



        /// <summary>
        ///  Д О П О Л Н И Т Е Л Ь Н Ы Й    Ф У Н К Ц И О Н А Л
        /// </summary>
        
        // Получить аудио-файл
        public FileResult Download(string mixPath)
        {
            if (mixPath == null)
                return null;

            string fullPath = @"\\192.168.0.98\RecordedFiles\" + mixPath;
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            string fileName = System.IO.Path.GetFileName(fullPath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public async Task<IActionResult> ExcelDownload(string idLK, string idTask, string period)
        {
            if (idLK == null || idTask == null || period == null)
                return Content("Выберите задачу и период");
            try
            {
                TaskModel myTask = await statisticsSQL.GetTask(idLK, idTask);
                DataTable dataTable = await GetTable(myTask.nameTable, myTask.idTask, myTask.poleList, period);
                dataTable.Columns.Remove("audioPath");
                dataTable.Columns.Remove("redirectPath");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable, myTask.nameTask);
                    using (MemoryStream myMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(myMemoryStream);
                        myMemoryStream.Position = 0;
                        var content = myMemoryStream.ToArray();
                        myMemoryStream.Close();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{ myTask.nameTask}_{period}.xlsx");
                    }
                }
            }
            catch
            {
                return Content("Произошла ошибка в построении файла.");
            }
        }
    }
}
