using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundXWebBlzr.Models
{

    public class Val
    {
        public string Company { get; set; }
        public string Standard_Tag { get; set; }
        public string Tag { get; set; }
        public int Filing_Year { get; set; }
        public string Period_Date { get; set; }
        public int Period_Days { get; set; }
        public float Value { get; set; }
    }

}
