using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class TeamMembers
    {
        public User projectManager { get; set; }
        public User teamLeader { get; set; }
        public List<User> teamMembers { get; set; }
    }
}