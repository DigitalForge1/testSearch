using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alphaData.Models
{
    public class TaskModel
    {
        // Инфо по задаче
        public string idTask { get; set; }
        public string nameTask { get; set; }
        public string nameTable { get; set; }

        // Настройка полей
        public List<string> poleList { get; set; }
        public List<string> fullPoleList { get; set; }

        // Для отчетов
        public string mailRep { get; set; }
        public string sendRep { get; set; }
    }
}
