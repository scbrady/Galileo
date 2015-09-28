using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.Models
{
    public class Comment
    {
        public int id { get; set; }
        public string recipients { get; set; }
        public string recipient_id { get; set; }
        public string recipient_first_name { get; set; }
        public string recipient_last_name { get; set; }
        public string commenter { get; set; }
        public string commenter_id { get; set; }
        public string commenter_first_name { get; set; }
        public string commenter_last_name { get; set; }
        public DateTime created_at { get; set; }
        public string comment_text { get; set; }
        public bool user_is_commenter { get; set; }
    }
}