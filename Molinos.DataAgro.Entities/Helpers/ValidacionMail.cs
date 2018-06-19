using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Helpers
{
    class ValidacionMail
    {
        public static bool IsValidEmail(string email)
        {
            bool invalid = false;
            invalid = false;
            if (!String.IsNullOrEmpty(email))
            {

                // Use IdnMapping class to convert Unicode domain names.
                try
                {
                    email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                          RegexOptions.None, TimeSpan.FromMilliseconds(200));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }

                if (invalid)
                    return false;

                // Return true if email is in valid e-mail format.
                try
                {
                    return Regex.IsMatch(email,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
            return true;
        }

        private static string DomainMapper(Match match)
        {
            bool invalid = false;
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public static bool IsValidNumber(string numero)
        {
            if (string.IsNullOrEmpty(numero))
            {
                return true;
            }
            else
            {            
            try
            {
                return Regex.IsMatch(numero,
                      @"^[0-9]+$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            }
        }
    }
}
