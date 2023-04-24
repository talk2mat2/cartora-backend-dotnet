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
using static cartoraServer.models.dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cartoraServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {

        private AContext db;
        private UserServices _userServe;

        public UsersController(AContext _db, UserServices userSer)
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
            var users = db.Users.Include(pp => pp.Products).ToList();
            return Ok(new ResData<Users> { status = true, data = users, message = " request successfully" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResData<usersStoreDto>>> Get(int id)
        {
            var getUser = await db.Users.FindAsync(id);
            if (getUser != null)
            { return Ok(new ResData<usersStoreDto>() { message = "successfully retrieved user", status = true, data = new List<usersStoreDto>() { new usersStoreDto() { brand = getUser.brand, userName = getUser.userName, aboutMe= getUser.AboutMe! } } }); }

            else
            {
                return Ok(new ResData<string>() { message = "Unable to get user", status = true });

            }

        }

        // POST api/values
        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<ResData<UsersDto>>> Post([FromBody] UsersDto request)
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
            if (IsexistUser != null)
            {
                ResData<string> Errorresponse = new ResData<string>();
                Errorresponse.message = $"A user with the email {request.email} already exist";
                Errorresponse.status = false;
                return BadRequest(Errorresponse);
            }
            var hash = SecurePasswordHasher.Hash(request.password);
            var newUser = new Users { email = request.email.ToLower(), password = hash, userName = request.userName, brand = request.brand, country = request.country,phoneNo=request.phoneNo };
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
                bool verifyPass = SecurePasswordHasher.Verify(request.password, Isexist.password);
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



        [HttpGet("fetchKniters/{userId}")]
        public async Task<ActionResult<string>> fetchKniters(int userId)
        {

            try
            {
                //var user = HttpContext.Items["User"] as Users;
                if (ModelState.IsValid)
                {
                    var kniters = await db.KnightModel.Where(pp => pp.brandId == userId).CountAsync();
                    var knited = await db.KnightModel.Where(pp => pp.folowerId == userId).CountAsync();
                    KnightIt2 resItems = new KnightIt2(kniters, knited);
                    ResData<KnightIt2> jj = new ResData<KnightIt2>() { message = "successful", status = true, };
                    jj.data.Add(resItems);
                    return Ok(jj);
                }
                else
                {
                    return StatusCode(505, new ResData<string>() { message = $"User id must be provided", status = false });
                }
            }
            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"failure{e}", status = false });
            }

        }

        [HttpGet("fetchUserKniters/{userId}")]
        public async Task<ActionResult<KnightModel>> fetchUserKniters(int userId)
        {

            try
            {
                //var user = HttpContext.Items["User"] as Users;
                if (ModelState.IsValid)
                {

                    var knitedList = await db.KnightModel.Where(pp => pp.folowerId == userId).ToListAsync();

                    ResData<KnightModel> jj = new ResData<KnightModel>() { message = "successful", status = true, data = knitedList };

                    return Ok(jj);
                }
                else
                {
                    return StatusCode(505, new ResData<string>() { message = $"User id must be provided", status = false });
                }
            }
            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"failure{e}", status = false });
            }

        }

        [Authorize]
        [HttpPost("updateAbout")]
        public async Task<ActionResult<ResData<dynamic>>> updateAbout([FromBody] aboutmeDto rq)
        {
            try
            {
                var user = HttpContext.Items["User"] as Users;
                var usersData = db.Users.Where(xx => xx.id == user!.id).FirstOrDefault();
                if (usersData != null)
                {
                    usersData.AboutMe = rq.aboutme;

                    db.Users.Update(usersData);
                    await db.SaveChangesAsync();

                    return Ok(new ResData<Users>() { message = "Successfull Updated", status = true, data = new List<Users> { usersData } });
                    
                }
                else
                {
                    return StatusCode(505,new ResData<string>() { message = "user not found", status = true });

                }
            

            }

            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"An error occured{e}", status = false });

            }

        }

    }
}

