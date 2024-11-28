using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alphaData.Models
{
    public class AccountModel
    {
        public LKModel lKModel { get; set; }
        public List<TaskModel> systemTaskList { get; set; }
        public List<TaskModel> myTaskList { get; set; }
    }
}
