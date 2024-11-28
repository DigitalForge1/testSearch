using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaData.Models.Statistics.Account
{
    public class AccountModel
    {
        public string idLk { get; set; }
        public List<MyTaskModel> myTaskModels { get; set; }
    }
}
