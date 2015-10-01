using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class TeamMembers
    {
        public List<Module> projectManager { get; set; }
        public List<Module> teamLeader { get; set; }
        public List<Module> teamMembers { get; set; }
    }
}