using AlphaData.Models.Statistics.Account;
using System.Collections.Generic;

namespace AlphaData.Models.Statistics.Journal
{
    public class JournalModel
    {
        public string idLk { get; set; }
        public List<MyTaskModel> myTaskModels { get; set; }
        public List<string> typeTables { get; set; }
    }
}
