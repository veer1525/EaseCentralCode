using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditSolution.Models
{
    public class RedditList
    {
        public string reddit_id { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
        public string author { get; set; }
    }
}