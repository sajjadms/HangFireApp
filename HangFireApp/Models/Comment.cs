using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFireApp.Models
{
    public class Comment
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public string To { get;set; }
    }
}