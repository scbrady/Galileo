using System;

namespace Galileo.Models
{
    public class Entry
    {
        public int entry_id { get; set; }
        public DateTime entry_begin_time { get; set; }
        public DateTime entry_end_time { get; set; }
        public int entry_total_time { get; set; }
        public string entry_work_accomplished { get; set; }
        public string entry_comment { get; set; }
        public string entry_user_id { get; set; }
        public int entry_project_id { get; set; }
        public int entry_location_id { get; set; }
        public int entry_category_id { get; set; }
        public string course_name { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public string project_name { get; set; }
        public int entry_total_hours { get; set; }
    }
}