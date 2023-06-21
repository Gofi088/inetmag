using System.Net.Mail;
using System.Text.RegularExpressions;

namespace NotebookShop
{
    public class EmailLogic
    {
        public bool IsEmailValid(string emailaddress)
        {
            bool isValid = true;

            try
            {
                MailAddress m = new MailAddress(emailaddress);
                isValid = true;
            }
            catch
            {
                isValid = false;
            }

            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (!regexEmail.IsMatch(emailaddress))
                isValid = false;

            return isValid;
        }
    }
}