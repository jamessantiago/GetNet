using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace getnet.Model
{
    public class SnackMessage
    {
        public string message { get; set; }
        public int timeout = 10000;
        public string actionHandler { get; set; }
        public string actionText { get; set; }
        public string timestamp = DateTime.UtcNow.ToString("o");
        public string AsJson()
        {
            if (actionHandler.HasValue())
                return JsonConvert.SerializeObject(this);
            else
                return JsonConvert.SerializeObject(new ToastMessage() { message = this.message, timeout = this.timeout, timestamp = this.timestamp });
        }
    }

    public class ToastMessage
    {
        public string message { get; set; }
        public int timeout { get; set; }
        public string timestamp = DateTime.UtcNow.ToString("o");
    }
}
