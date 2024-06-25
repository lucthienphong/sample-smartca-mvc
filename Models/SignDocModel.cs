using System.Web;

namespace Sample.WebApp.SmartCAv4.Models
{
    public class SignDocModel
    {
        public HttpPostedFileBase RefDoc { get; set; }
        public string ErrMsg { get; set; }
    }
}