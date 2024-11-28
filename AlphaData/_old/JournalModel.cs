using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace alphaData.Models
{
    public class JournalModel
    {
        public LKModel lkModel { get; set; }
        public List<TaskModel> myTaskList { get; set; }
    }
}
