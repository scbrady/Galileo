using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class Module
    {
        public string id { get; set; }
        public string name { get; set; }
        public float total_hours { get; set; }
        public float hours_per_day { get; set; }
        public float hours_per_week { get; set; }
    }
}