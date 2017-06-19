using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CloudSocietyLib.MessagingService
{
    public class TextMessagingService
    {
        public bool SendSMS(string urlSMS)
        {
            string responseString = "";
            bool status = false;
            try
            {
                // creating web request to send sms
                HttpWebRequest _createRequest = (HttpWebRequest)WebRequest.Create(urlSMS);
                // getting response of sms
                HttpWebResponse myResp = (HttpWebResponse)_createRequest.GetResponse();
                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                responseString = _responseStreamReader.ReadToEnd();
                _responseStreamReader.Close();
                myResp.Close();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
    }
}
