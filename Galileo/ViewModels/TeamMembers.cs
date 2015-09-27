using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class TeamMembers
    {
        public Module projectManager { get; set; }
        public Module teamLeader { get; set; }
        public List<Module> teamMembers { get; set; }
    }
}