using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.Models
{
    public class Comment
    {
        public string recipient_id { get; set; }
        public string commenter_id { get; set; }
        public DateTime created_at { get; set; }
        public string comment_text { get; set; }
    }
}