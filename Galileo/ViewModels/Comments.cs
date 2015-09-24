using Galileo.Models;
using System.Collections.Generic;

namespace Galileo.ViewModels
{
    public class Comments
    {
        public List<Comment> comments { get; set; }
        public string recipients { get; set; }
        public string comment { get; set; }
        public string commenter_id { get; set; }
    }
}