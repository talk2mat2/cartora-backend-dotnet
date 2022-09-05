using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cartoraServer.Data;
using cartoraServer;
using cartoraServer.models;
using Newtonsoft;
using cartoraServer.Helpers;
using cartoraServer.services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cartoraServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {

        private AContext db;
        private UserServices _userServe;

        public UsersController(AContext _db,UserServices userSer)
        {
            db = _db;
            _userServe = userSer;
        }
        // GET: api/values
        //[Authorize]
        [HttpGet]
        public ActionResult<ResData<Users>> Get()
        {
            //var user = (Users)HttpContext.Items["User"]!;
            //return new string[] { "value1", $"{user.email}" };
            var users = db.Users.Include(pp=>pp.Products).ToList();
            return Ok(new ResData<Users> { status = true, data = users, message = "successfully Created" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<ResData<UsersDto>>> Post([FromBody]UsersDto request)
        {
            if (!ModelState.IsValid)
            {
                ResData<string> Errorresponse = new ResData<string>();
                Errorresponse.message = "all required is not supplied";
                Errorresponse.status = false;
                return BadRequest(Errorresponse);
            }

            var IsexistUser = db.Users.Where(P => P.email == request.email.ToLower()).FirstOrDefault();
            Console.WriteLine(IsexistUser);
            if (IsexistUser!=null)
            { 
                ResData<string> Errorresponse = new ResData<string>();
                Errorresponse.message = $"A user with the email {request.email} already exist";
                Errorresponse.status = false;
                return BadRequest(Errorresponse);
            }
            var hash = SecurePasswordHasher.Hash(request.password);
            var newUser = new Users { email = request.email.ToLower(), password = hash, userName = request.userName, brand = request.brand, country = request.country };
            await db.Users.AddAsync(newUser);
            db.SaveChanges();
            ResData<UsersDto> response = new ResData<UsersDto>();
                response.message = "Registration was successfull";
                response.status = true;
            //var item = new List<dynamic>(){ "hello"};
            Console.WriteLine(ModelState.IsValid);
            response.data.Add(request);
                return Ok(response);
           
        }

        [HttpPost("Login")]
        public ActionResult<ResData<AuthResponse>> Login(LoginModel request)
        {
            if (!ModelState.IsValid)
            {
                ResData<string> Errorresponse1 = new ResData<string>();
                Errorresponse1.message = "all required is not supplied";
                Errorresponse1.status = false;
                return BadRequest(Errorresponse1);
            }
            var Isexist = db.Users.Where(p => p.email == request.email).FirstOrDefault();

            if (Isexist == null)
            {
                ResData<string> Errorresponse2 = new ResData<string>();
                Errorresponse2.message = $"Account not found for {request.email} ";
                Errorresponse2.status = false;
                return BadRequest(Errorresponse2);
            }
            if (Isexist.email == request.email)
            {
                bool verifyPass= SecurePasswordHasher.Verify(request.password,Isexist.password);
                if (verifyPass)
                {

                    string Token = _userServe.generateJwtToken(Isexist);
                    AuthResponse userSuccess = new AuthResponse(Isexist, Token);
                    ResData<AuthResponse> response = new ResData<AuthResponse>();
                    response.message = "Logged In successfull";
                    response.status = true; 
                   Console.WriteLine(ModelState.IsValid);
                    response.data.Add(userSuccess);
                    return Ok(response);
                }
                else
                {
                    ResData<string> Errorresponses = new ResData<string>();
                    Errorresponses.message = "Inavalid email Or password combination";
                    Errorresponses.status = false;
                    return BadRequest(Errorresponses);
                }
            }
            ResData<string> Errorresponse = new ResData<string>();
            Errorresponse.message = $"Unable to process request try again ";
            Errorresponse.status = false;
            return BadRequest(Errorresponse);

        }
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

