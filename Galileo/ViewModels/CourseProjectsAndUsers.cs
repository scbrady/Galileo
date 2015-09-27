using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class CourseProjectsAndUsers
    {
        public List<Module> projects { get; set; }
        public List<Module> teamsView { get; set; }
        public List<Module> users { get; set; }
    }
}