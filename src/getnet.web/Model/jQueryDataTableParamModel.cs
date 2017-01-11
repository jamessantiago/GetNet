using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.Model
{
    public class jQueryDataTableParamModel
    {
        public Dictionary<string,Dictionary<string,string>>[] columns { get; set; }
        public Dictionary<string, string>[] order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Dictionary<string,string> search { get; set; }
        public bool draw { get; set; }
    }
}
