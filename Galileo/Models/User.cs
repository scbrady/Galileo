﻿namespace Galileo.Models
{
    public class User
    {
        public string user_id { get; set; }
        public bool user_is_enabled { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public bool user_is_student { get; set; }
        public bool user_is_teacher { get; set; }
        public bool user_is_manager { get; set; }
    }
}