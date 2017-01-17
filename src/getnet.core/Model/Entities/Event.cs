using System;

namespace getnet.core.Model.Entities
{
    public class Event
    {
        public string Details { get; set; }
        public int EventId { get; set; }
        public string Host { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public int SiteId { get; set; }
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
    }
}