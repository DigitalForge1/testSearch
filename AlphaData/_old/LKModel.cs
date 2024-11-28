using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alphaData.Models
{
    public class LKModel
    {
        public string idLK { get; set; }
        public string nameProject { get; set; }
        public string idProject { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    }
}






/*
 
 // Список выбранных полей
        public string selectPole { get; set; }
        public List<string> selectPoles {
            get 
            {
                if (selectPole == null)
                    return new List<string>();
                else
                    return selectPole.Split(',').ToList();
            }
        }

        // Список всех полей
        public string fullPole { get; set; }
        public List<string> fullPoles
        {
            get
            {
                if (fullPole == null)
                    return new List<string>();
                else
                    return fullPole.Split(',').ToList();
            }
        }
 */