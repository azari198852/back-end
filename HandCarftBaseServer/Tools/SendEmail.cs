﻿using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MimeKit.Text;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace HandCarftBaseServer.Tools
{
   public class SendEmail
    {

        public void SendRegisterEmail(string password, string emaill)
        {

            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("info@tabrizhandicrafts.com");
            email.To.Add(MailboxAddress.Parse("Sahand.farshbaf@gmail.com"));

            email.Subject = " ثبت نام در بازارچه اینترنتی صنایع دستی";
            var body = "نام کاربری و کلمه عبور شما در بازارچه اینترنتی صنایع دستی به شرح زیر می باشد:";
            body += System.Environment.NewLine;
            body += "نام کاربری: ";
            body += emaill;
            body += System.Environment.NewLine;
            body += "کلمه عبور: ";
            body += password;
            body += System.Environment.NewLine;
            body += "لطفا به منظور ورود به فروشگاه ، به آدرس زیر مراجعه نمایید:";
            body += System.Environment.NewLine;
            body += "tabrizhandicrafts.com";
            email.Body = new TextPart(TextFormat.Text) { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                smtp.Connect("tabrizhandicrafts.com", 587, SecureSocketOptions.Auto);

                // hotmail
                //smtp.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);

                // office 365
                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("info@tabrizhandicrafts.com", "123456qQ");
                smtp.Send(email);
                smtp.Disconnect(true);

      
            }
            catch (Exception ex)
            {

              
            }
            // send email

        }

        public void SendSuccessOrderPayment(string emaill, string orderNo, long orderId)
        {

            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emaill);
            email.To.Add(MailboxAddress.Parse(emaill));

            email.Subject = "ثبت سفارش";
            var body = "سفارش شما با شماره ";
            body += orderNo;
            body += "در بازارچه اینترنتی صنایع دستی در حال آماده سازی می باشد. ";
            body += System.Environment.NewLine;
            body += "tabrizhandicrafts.com/Home/CoustomerOrderTrace/";
            body += orderId;
            body += System.Environment.NewLine;
            email.Body = new TextPart(TextFormat.Text) { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // hotmail
                //smtp.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);

                // office 365
                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("cama.eenergy@gmail.com", "cama1390");
                smtp.Send(email);
                smtp.Disconnect(true);


            }
            catch (Exception ex)
            {


            }
            // send email

        }

        public void SendLoginEmail(string emaill, long code)
        {

            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emaill);
            email.To.Add(MailboxAddress.Parse(emaill));

            email.Subject = "کد تایید";
            var body = " کد تایید و رمز عبور شما برای ورود به ";
            body += "tabrizhandicrafts.com";
            body += System.Environment.NewLine;
            body += "";
            body += code.ToString();
            body += " می باشد.";
            email.Body = new TextPart(TextFormat.Text) { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // hotmail
                //smtp.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);

                // office 365
                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("sahand.farshbaf@gmail.com", "sa19881213");
                smtp.Send(email);
                smtp.Disconnect(true);


            }
            catch (Exception ex)
            {


            }
            // send email

        }

        public void SendResetPassEmail(string emaill, long code)
        {

            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emaill);
            email.To.Add(MailboxAddress.Parse(emaill));

            email.Subject = "بازیابی رمز عبور";
            var body = " کد تایید شما برای بازیابی رمز عبور در  ";
            body += "tabrizhandicrafts.com";
            body += System.Environment.NewLine;
            body += "";
            body += code.ToString();
            body += " می باشد.";
            email.Body = new TextPart(TextFormat.Text) { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // hotmail
                //smtp.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);

                // office 365
                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("sahand.farshbaf@gmail.com", "sa19881213");
                smtp.Send(email);
                smtp.Disconnect(true);


            }
            catch (Exception ex)
            {


            }
            // send email

        }

        public void SendChangeComissionEmail(string emaill, string sellerName, string productName, int comission)
        {

            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("info@tabrizhandicrafts.com");
            email.To.Add(MailboxAddress.Parse(emaill));

            email.Subject = " ثبت نام در بازارچه اینترنتی صنایع دستی";
            var body = "با سلام صنعتگر گرامی ";
            body += sellerName;
            body += System.Environment.NewLine;
            body += "کمیسیون محصول ";
            body += productName;
            body += " ";
            body += "شما به ";
            body += comission.ToString();
            body += " ";
            body += "درصد تغییر یافت.";
            body += System.Environment.NewLine;
            body += "بازارچه صنایع دستی سارای";
            body += System.Environment.NewLine;
            body += "بازارچه صنایع دستی سارای";
            body += System.Environment.NewLine;
            body += "http://www.saraycrafts.com/";
            email.Body = new TextPart(TextFormat.Text) { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                smtp.Connect("tabrizhandicrafts.com", 587, SecureSocketOptions.Auto);

                // hotmail
                //smtp.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);

                // office 365
                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("info@tabrizhandicrafts.com", "123456qQ");
                smtp.Send(email);
                smtp.Disconnect(true);


            }
            catch (Exception ex)
            {


            }
            // send email

        }


    }
}
