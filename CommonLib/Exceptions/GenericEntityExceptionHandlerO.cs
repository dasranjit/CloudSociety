using System;
using System.Text;
using System.Web;
using System.Data.SqlClient;

namespace CommonLib.Exceptions
{
    public static class GenericExceptionHandler
    {
        const string _sessionkey = "Error";

        //public GenericExceptionHandler() { }

        public static void HandleException(Exception ex, string contextstring = "")
        {
            var sb = new StringBuilder();

            if (contextstring != "")
                sb.AppendLine(contextstring);
            sb.AppendLine(ex.Message);
            if (ex.InnerException != null)
                sb = sb.AppendLine(ex.InnerException.Message);
            if(ex.InnerException.InnerException  !=null)
                sb = sb.AppendLine(ex.InnerException.InnerException.Message);
            sb = sb.AppendLine(GetErrorMessageForSQLException(ex));
            HttpContext.Current.Session[_sessionkey] = sb.ToString();
        }

        public static String GetErrorMessageForSQLException(Exception ex)
        {
            string message = "";
            if (ex != null)
            {
                if (ex is SqlException)
                {
                    SqlException sqlex = (SqlException)ex;
                    switch (sqlex.Number)
                    {
                        case 4060:
                            message = "Invalid Database.Check Databse Name";
                            break;
                        case 18456:
                            message = "Login Failed.Check Databse Credentials";
                            break;
                        case 547:
                            message = "Foregin Key violation.Check Databse Schema";
                            break;
                        case 10054:
                            message = "Connection To Databse Refused";
                            break;
                        case 214:
                            message = sqlex.Message.ToString(); ;
                            break;
                        case 20:
                            message = sqlex.Message.ToString(); ;
                            break;
                        case 229:
                            message = "Permission Denied On Object. Contact DBA";
                            break;
                        case 230:
                            message = "Permission denied On A Column. Check permissions";
                            break;
                        case 235:
                            message = "Cannot Convert A Char Value To Money. The Char Value Has Incorrect Syntax.";
                            break;
                        case 236:
                            message = "The Conversion From Char Data Type To Money Resulted In A Money Overflow Error.";
                            break;
                        case 241:
                            message = "Conversion Failed When Converting Datetime From Character String.";
                            break;
                        case 262:
                            message = "Permission Denied In Database.";
                            break;
                        case 297:
                            message = "User Does Not Have Permissions To Perform This Action";
                            break;
                        case 313:
                            message = sqlex.Message.ToString();
                            break;
                        case 8144:
                            message = sqlex.Message.ToString();//"To Many Arguments Supplied For Procedure/Funtion ";
                            break;
                        case 8146:
                            message = sqlex.Message.ToString();//"Procedure Has No Parameters And Arguments Were Supplied ";
                            break;
                        case 10004:
                            message = "One Or More Invlaid Arguments ";
                            break;
                        case 18452:
                            message = "Login Failed For User. User Not Associated With A Trusted SQL Server Connection";
                            break;
                        case 21670:
                            message = "Connection To Server Failed.";
                            break;
                        case 2812:
                            message = "Could Not Find Stored Procedue.Check Name Of Stored Procedure";
                            break;
                        case 14043:
                            message = sqlex.Message.ToString();//Null Parameter Passed To Procedure
                            break;
                        case 15003:
                            message = sqlex.Message.ToString();//Role Specific SP
                            break;
                        case 16903:
                            message = sqlex.Message.ToString();//Incorrect Number Of Parameters
                            break;
                        case 16914:
                            message = sqlex.Message.ToString();//To Many Parameters
                            break;
                        case 18751:
                            message = sqlex.Message.ToString();//Wrong Number Of Parameters
                            break;
                        case 20587:
                            message = sqlex.Message.ToString();//Invalid Value For Procedure
                            break;
                        case 20624:
                            message = sqlex.Message.ToString();//User Not In Database
                            break;
                        case 21234:
                            message = sqlex.Message.ToString();//Cannot Insert as Table Has Idenitiy Column
                            break;
                        case 21343:
                            message = sqlex.Message.ToString();//Cannot Find Stored Procedure
                            break;
                        default:
                            message = sqlex.Message.ToString() + Environment.NewLine + "SQL ERROR CODE : " + sqlex.Number + Environment.NewLine + "Run Query For SysMessages To Check Error Details";
                            break;

                    }
                }
            }

            return message;
        }

        public static string ExceptionMessage()
        {
            return HttpContext.Current.Session[_sessionkey].ToString();
        }
       
    }
}
