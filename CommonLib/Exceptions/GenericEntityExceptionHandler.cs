using System;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace CommonLib.Exceptions
{
    public static class GenericExceptionHandler
    {
        const string _sessionkey = "Error";
        public static Dictionary<string, string> dictionary;
        static GenericExceptionHandler()
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("DELETE statement conflicted with the REFERENCE constraint", "Cannot DELETE! This entry is refered in other tables. Please remove the references from resp. entries first before deleting.");
            dictionary.Add("Violation of PRIMARY KEY", "Cannot Insert duplicate records");
        }

        public static void HandleException(Exception ex, string contextstring = "")
        {
            StringBuilder sb = new StringBuilder();
            if (contextstring != "")
                sb.AppendLine(contextstring);
            sb.AppendLine(ex.Message);
            if (ex.InnerException != null)
                sb = sb.AppendLine(ex.InnerException.Message);
            if (ex.InnerException.InnerException != null)
                sb = sb.AppendLine(ex.InnerException.InnerException.Message);
            HttpContext.Current.Session[_sessionkey] = GetMessageForException(sb.ToString());
        }
        public static String GetMessageForException(string Message)
        {
            try
            {
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    if (Message.Contains(pair.Key))
                        Message = pair.Value;
                }
                return Message;
            }
            catch
            {
                return Message;
            }
        }
        public static string ExceptionMessage()
        {
            return HttpContext.Current.Session[_sessionkey].ToString();
        }

    }
}
