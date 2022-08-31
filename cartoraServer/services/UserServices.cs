using System;
using cartoraServer.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cartoraServer.Data;
using Microsoft.Extensions.Options;

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
    }
}

