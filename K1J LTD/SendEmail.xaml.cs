using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using System.Net.Http;

namespace K1J_LTD
{
    /// <summary>
    /// Interaction logic for SendEmail.xaml
    /// </summary>
    public partial class SendEmail : Window
    {
        public SendEmail()
        {
            InitializeComponent();
        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("S2200844@ndonline.ac.uk");
            mailMessage.To.Add("S2200844@ndonline.ac.uk");
            mailMessage.Subject = "Sent by C#";
            mailMessage.Body = "This can only end well.";

            using (SmtpClient smtp = new SmtpClient("Smtp.office365.com", 587))
            {
                smtp.Credentials = new NetworkCredential(Details.username, Details.password);
                smtp.EnableSsl = true;
                smtp.Send(mailMessage);
            }
            
        }
    }
}
