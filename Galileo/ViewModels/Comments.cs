using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galileo.ViewModels
{
    public class Comments
    {
        public List<Comment> comments { get; set; }
        public string recipients { get; set; }
        public string comment { get; set; }
    }
}