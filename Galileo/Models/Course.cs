using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.Models
{
    public class Course
    {
        public int course_id { get; set; }
        public string course_name { get; set; }
        public int course_submit_day { get; set; }
        public DateTime course_date_created { get; set; }
        public DateTime course_begin_date { get; set; }
        public DateTime course_end_date { get; set; }
        public bool course_is_enabled { get; set; }
        public double course_ref_grade { get; set; }
        public double course_ref_hours { get; set; }
        public int course_total_time { get; set; }
    }
}