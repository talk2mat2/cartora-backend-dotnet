using System;
using cartoraServer.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cartoraServer.Data;
using Microsoft.Extensions.Options;
using System.Net;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace cartoraServer.services
{

    //public interface IUserService
    //{
    //    Users GetById(int id);
    //}

    public class UserServices
    {
        public AContext _db;

        private readonly AppSettings _appSettings;
        public UserServices(IOptions<AppSettings> appSettings, AContext db)
        {
            _appSettings = appSettings.Value;
            _db = db;

        }


        public Users GetById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.id == id)!;
            //return _users.FirstOrDefault(x => x.Id == id);
        }


        public string generateJwtToken(Users user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }


        public dynamic Sendmail(string subject,string useremail,string body)
        {

            try
            {

                var host = "mail.cartoraapp.lat";
                var port = 8889;

                var username = "support@cartoraapp.lat";
                var password = "Chibuzo1234@"; 

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Cartora Support", "support@cartoraapp.lat"));
                message.To.Add(new MailboxAddress(useremail, useremail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                var client = new SmtpClient();

                client.Connect(host, port, SecureSocketOptions.None);
                client.Authenticate(username, password);

                client.Send(message);
                client.Disconnect(true);

                return true;
            }

            catch (Exception e)
            {
                var error = e;
                return e;
            }
        }
    }
}

