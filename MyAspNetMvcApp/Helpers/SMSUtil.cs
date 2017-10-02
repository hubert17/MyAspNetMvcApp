using Plivo.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Gabs.Helpers
{
    public static class SMSUtil
    {
        // https://manage.plivo.com/dashboard/
        private const string AUTH_ID = "";
        private const string AUTH_TOKEN = "";
        private const string SENDER_NO = "+639999999999";

        public static string Send(string mobileNo, string smsMessage)
        {
            RestAPI plivo = new RestAPI(AUTH_ID, AUTH_TOKEN);

            IRestResponse<MessageResponse> resp = plivo.send_message(new Dictionary<string, string>()
            {
                { "src", SENDER_NO }, // Sender's phone number with country code
                { "dst", mobileNo }, // Receiver's phone number with country code
                { "text", smsMessage }, // Your SMS text message
                { "url", "http://dotnettest.apphb.com/delivery_report"}, // The URL to which with the status of the message is sent
                { "method", "POST"} // Method to invoke the url
            });

            //Prints the message details
            return resp.Content;

            // Print the message_uuid
            //return  resp.Data.message_uuid[0];

            // Print the api_id
            //return resp.Data.api_id;
        }

        public static string GenerateCode(string mobileNo = "")
        {
            int maxSize = 6;
            char[] chars = new char[30];
            string a;
            a = string.IsNullOrEmpty(mobileNo) ? "0123456789" : mobileNo;
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data) { result.Append(chars[b % (chars.Length)]); }
            return result.ToString();
        }
    }
}