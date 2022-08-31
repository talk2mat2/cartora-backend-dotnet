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
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get([FromHeader(Name = "Authorization")] string request)
        {
            var user = (Users)HttpContext.Items["User"]!;
            return new string[] { "value1", $"{user.email}" };
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
        public async Task<ActionResult<Users>> Post([FromBody]Users request)
        {
            if (!ModelState.IsValid)
            {
                ResData Errorresponse = new ResData();
                Errorresponse.message = "all required is not supplied";
                Errorresponse.status = false;
                return BadRequest(Errorresponse);
            }

            var IsexistUser = db.Users.Where(P => P.email == request.email.ToLower()).FirstOrDefault();
            Console.WriteLine(IsexistUser);
            if (IsexistUser!=null)
            { 
                ResData Errorresponse = new ResData();
                Errorresponse.message = $"A user with the email {request.email} already exist";
                Errorresponse.status = false;
                return BadRequest(Errorresponse);
            }
            var hash = SecurePasswordHasher.Hash(request.password);
            var newUser = new Users { email = request.email.ToLower(), password = hash, userName = request.userName, brand = request.brand, country = request.country };
            await db.Users.AddAsync(newUser);
            db.SaveChanges();
            ResData response = new ResData();
                response.message = "Registration was successfull";
                response.status = true;
            //var item = new List<dynamic>(){ "hello"};
            Console.WriteLine(ModelState.IsValid);
            response.data.Add(request);
                return Ok(response);
           
        }

        [HttpPost("Login")]
        public ActionResult<ResData> Login(LoginModel request)
        {
            if (!ModelState.IsValid)
            {
                ResData Errorresponse1 = new ResData();
                Errorresponse1.message = "all required is not supplied";
                Errorresponse1.status = false;
                return BadRequest(Errorresponse1);
            }
            var Isexist = db.Users.Where(p => p.email == request.email).FirstOrDefault();

            if (Isexist == null)
            {
                ResData Errorresponse2 = new ResData();
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
                    ResData response = new ResData();
                    response.message = "Logged In successfull";
                    response.status = true; 
                   Console.WriteLine(ModelState.IsValid);
                    response.data.Add(userSuccess);
                    return Ok(response);
                }
                else
                {
                    ResData Errorresponses = new ResData();
                    Errorresponses.message = "Inavalid email Or password combination";
                    Errorresponses.status = false;
                    return BadRequest(Errorresponses);
                }
            }
            ResData Errorresponse = new ResData();
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

