using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gabs.Helpers
{
    public class Formatter
    {
        public static string FormatPhoneNumber(string phoneNumber, string countryCode = "63")
        {
            if (phoneNumber != null)
            {
                phoneNumber = phoneNumber.TrimStart('0');

                if (phoneNumber.Length.Equals(10) && string.IsNullOrEmpty(countryCode))
                    countryCode = "63";
                else if (phoneNumber.Length > 10 && !string.IsNullOrEmpty(countryCode))
                    countryCode = string.Empty;                

                var phoneNumberCC = countryCode + phoneNumber;
                if (phoneNumberCC.Length.Equals(12))
                    return string.Format("{0:(+##) ###-###-####}", Convert.ToInt64(phoneNumberCC));
                else if (phoneNumberCC.Length.Equals(11))
                    return string.Format("{0:(+#) ###-###-####}", Convert.ToInt64(phoneNumberCC));
                else if (phoneNumberCC.Length.Equals(13))
                    return string.Format("{0:(+###) ###-###-####}", Convert.ToInt64(phoneNumberCC));
                else if (phoneNumber.Length.Equals(7))
                    return string.Format("{0:###-####}", Convert.ToInt64(phoneNumber));
            }

            return phoneNumber;
        }

        public static string ConstructEllipticalPhrase(string description)
        {
            var phraseDescription = "";

            foreach (var desc in description.Split().Take(10))
            {
                phraseDescription += desc + " ";
            }

            return phraseDescription + "...";
        }

    }
}