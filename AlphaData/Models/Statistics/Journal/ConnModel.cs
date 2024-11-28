using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaData.Models.Statistics.Journal
{
    public class ConnModel
    {
        public Boolean search { get; set; }
        public string IdConn { get; set; }
        public string IdPrev { get; set; }
        public string IdChain { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeStop { get; set; }
        public DateTime TimeStart { get; set; }
        public Boolean IsOutput { get; set; }
        public string AbonentNumber { get; set; }

        public string alinenum { get; set; }
        public string blinenum { get; set; }
        public string astr { get; set; }
        public string bstr { get; set; }
        public string ConnectionType { get; set; }

        public string LenTime { get; set; }
        public string pathURL
        {
            get
            {
                string aln = "";
                string bln = "";
                string ts = "";
                string path = "";
                if (string.IsNullOrWhiteSpace(alinenum))
                {
                    alinenum = "0";
                    blinenum = "0";
                }
                int aInt = (int)Convert.ToInt32(alinenum, 16);
                int bInt = (int)Convert.ToInt32(blinenum, 16);

                if (aInt < bInt)
                {
                    aln = alinenum;
                    bln = blinenum;
                }
                else
                {
                    aln = blinenum;
                    bln = alinenum;
                }
       
                ts = TimeStart.ToString("yyyy_MM_dd") + "__" + TimeStart.ToString("HH_mm_ss_fff");

                string filePath = ts.Substring(0, 10).Replace("_", "") + @"\" + ts.Substring(12, 2) + ts.Substring(15, 2) + @"\mix_" + aln + "_" + bln + "__" + ts;
                string exstansion = ".mp3";

                if (filePath.Contains(@"00010101\0000\mix____0001_01_01__00_00_00_000"))
                    return null;

                if (System.IO.File.Exists(@"\\192.168.0.98\RecordedFiles\" + filePath + ".mp3"))
                    exstansion = ".mp3";
                if (System.IO.File.Exists(@"\\192.168.0.98\RecordedFiles\" + filePath + ".wav"))
                    exstansion = ".wav";

                path = filePath + exstansion;
                return path;
            }
        }

        public int IdInList { get; internal set; }
    }
}
