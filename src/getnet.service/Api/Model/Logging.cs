using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.service.Api
{
    public class Logging
    {
        public string fileenabled { get; set; }
        public string filelayout { get; set; }
        public string filename { get; set; }
        public string smtpenabled { get; set; }
        public string smtpserver { get; set; }
        public string smtpfrom { get; set; }
        public string smtpsubject { get; set; }
        public string databaseenabled { get; set; }
        
    }
}
