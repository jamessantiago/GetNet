using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
        public int? SiteId { get; set; }
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }

        public static Expression<Func<Event, bool>> SearchPredicates(string text)
        {
            var predicates = PredicateBuilder.False<Event>();
            foreach (var term in text.Split(' '))
            {
                var st = term.ToLower();
                predicates = predicates.Or(d => d.Message.ToLower().Contains(st));
                predicates = predicates.Or(d => d.Type.ToLower().Contains(st));
                predicates = predicates.Or(d => d.Details.ToLower().Contains(st));
                predicates = predicates.Or(d => d.Source.ToLower().Contains(st));
            }
            return predicates;
        }
    }
}