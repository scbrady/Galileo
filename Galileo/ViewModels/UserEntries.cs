using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class UserEntries
    {
        public User user { get; set; }
        public List<Entry> entries { get; set; }
    }
}