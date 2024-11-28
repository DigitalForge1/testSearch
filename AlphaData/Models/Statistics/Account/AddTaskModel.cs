using System.Collections.Generic;

namespace AlphaData.Models.Statistics.Account
{
    public class AddTaskModel
    {
        public string idLk { get; set; }
        public List<SystemTaskModel> systemTaskModels { get; set; }
    }
}
