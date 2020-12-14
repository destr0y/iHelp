using System;
namespace iHelp.Models
{
    public class ResponseModel
    {
        public System.Net.HttpStatusCode Code { get; set; }

        public string Body { get; set; }
    }
}
