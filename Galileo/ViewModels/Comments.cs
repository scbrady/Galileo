using Galileo.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Galileo.ViewModels
{
    public class Comments
    {
        public List<Comment> comments { get; set; }

        [DisplayName("To:")]
        public string recipients { get; set; }

        [DisplayName("Comment:")]
        public string comment { get; set; }

        [DisplayName("Hidden:")]
        public bool hidden { get; set; }
        public string commenter_id { get; set; }
    }
}