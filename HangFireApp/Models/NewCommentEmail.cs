using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFireApp.Models
{
    public class NewCommentEmail : Email
    {
        public string Id { get; set; }
        public string To { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
    }
}