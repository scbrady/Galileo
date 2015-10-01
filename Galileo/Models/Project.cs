using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.Models
{
    public class Project
    {
        public int project_id { get; set; }
        public int project_course_id { get; set; }
        public string project_name { get; set; }
        public string project_created_by { get; set; }
        public DateTime project_date_created { get; set; }
        public string project_description { get; set; }
        public DateTime project_begin_date { get; set; }
        public DateTime project_end_date { get; set; }
        public bool project_is_enabled { get; set; }
        public int project_total_time { get; set; }
        public bool project_is_team { get; set; }
    }
}