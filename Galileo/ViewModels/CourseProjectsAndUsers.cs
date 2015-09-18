using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class CourseProjectsAndUsers
    {
        public List<Project> projects { get; set; }
        public List<User> users { get; set; }
    }
}