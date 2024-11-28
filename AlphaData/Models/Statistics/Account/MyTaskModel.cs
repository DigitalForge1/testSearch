using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaData.Models.Statistics.Account
{
    public class MyTaskModel
    {
        public string idTask { get; set; }
        public string nameTask { get; set; }
        public string nameTable { get; set; }
        public List<string> fullPoles { get; set; }
        public List<string> myPoles { get; set; }
    }
}
