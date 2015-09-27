using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class TeamsProjectsAndUsers
    {
        public List<Project> projects { get; set; }
        public List<Project> teamsView { get; set; }
        public List<User> users { get; set; }
        public string projectManager { get; set; }
        public IEnumerable<Team> teams { get; set; }
    }
}