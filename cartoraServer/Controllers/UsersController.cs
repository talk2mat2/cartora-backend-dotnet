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
using static cartoraServer.models.deleteAcctDto;
using Microsoft.AspNetCore.Hosting.Server;
using Org.BouncyCastle.Asn1.Ocsp;
using static System.Net.WebRequestMethods;

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
            { return Ok(new ResData<usersStoreDto>() { message = "successfully retrieved user", status = true, data = new List<usersStoreDto>() { new usersStoreDto() { brand = getUser.brand, userName = getUser.userName, aboutMe= getUser.AboutMe!, profileImage=getUser.profileImage } } }); }

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
            string body = $"<body>\n    <h2>Hi {request.userName}</h2>\n    <p>\n      Thanks for registering with Cartora!:) <br />We are excited to Have you on\n      board.\n    </p>\n\n    <small>\n      Kind regards,<br />\n      Cartora Team</small\n    >\n  </body>";
            var sendMail = _userServe.Sendmail("Password Change", request.email, body);
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
            //var getUser = db.Users.Where(TT => TT.email == request.email).FirstOrDefault();
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
                    var userRes = new { Token=Token,data=new List<dynamic> {} };
                    ResData<dynamic> response = new ResData<dynamic>( );
                    
                    response.message = "Logged In successfull";
                    response.status = true;
                    var newItem = new { Token = Token,
                        id= Isexist.id,
                        userName= Isexist.userName,
                        brand=Isexist.brand,
                        email =Isexist.email,
                        country= Isexist.country,
                        createdAt= Isexist.createdAt,
                        aboutMe= Isexist.AboutMe,
                        profileImage= Isexist.profileImage,
                        phoneNo=Isexist.phoneNo,
                        tags=Isexist.tags
                        };
                    response.data = new List<dynamic> { newItem };
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


        [Authorize]
        [HttpPost("updatetags")]
        public async Task<ActionResult<ResData<dynamic>>> updatetags(tagsdto rq)
        {

            var user = HttpContext.Items["User"] as Users;

            //check if userstag already exist
            var exist = db.Tags.Where(item => item.UserId == user!.id).FirstOrDefault();

            if (exist != null)
            {
                //we update herew
                foreach (var prop in rq.GetType().GetProperties())
                {
                    var propname = prop.Name;
                    var value = prop.GetValue(rq);
                    if (value != null)
                    {
                        var toProperty = exist.GetType().GetProperty(propname);
                        toProperty!.SetValue(exist, value, null);
                    }
             
                }
                await db.SaveChangesAsync();
                return Ok(new ResData<tags>() { message = "Successfull Updated", status = true, data = new List<tags> { exist } });

            }
            else
            {
                rq.UserId = user!.id;

                tags newTag = new tags { };
                foreach (var prop in rq.GetType().GetProperties())
                {
                    var propname = prop.Name;
                    var value = prop.GetValue(rq);
                    //tags newTag = new tags { };
                    var toProperty = newTag.GetType().GetProperty(propname);
                    toProperty!.SetValue(newTag, value, null);

                }
                //await db.Tags.AddAsync(newTag);
                await db.SaveChangesAsync();
                return Ok(new ResData<tags>() { message = "Successfull Created", status = true, data = new List<tags> { newTag } });
            }
            
          


        }

        [Authorize]
        [HttpPost("UpdateUserProfile")]
        public async Task<ActionResult<ResData<dynamic>>> UpdateUserProfile(ProfileDto rq)
        {
            var user = HttpContext.Items["User"] as Users;
            var usersData = db.Users.Where(xx => xx.id == user!.id).FirstOrDefault();

            if (usersData != null)
            {
                foreach(var prop in  rq.GetType().GetProperties())
                {
                    var propname = prop.Name;
                    var value= prop.GetValue(rq);
                    if (value != null) 
                    {
                        var toProperty = usersData.GetType().GetProperty(propname);
                        toProperty!.SetValue(usersData,value,null);
                    }
                   
                }
                await db.SaveChangesAsync();
                return Ok(new ResData<Users>() { message = "Successfull Updated", status = true, data = new List<Users> { usersData } });
            }


            return StatusCode(505, new ResData<string>() { message = $"failure", status = false });

        }




        [HttpGet("fetchUserTags/{userId}")]
        public ActionResult<string> fetchUserTags(int userId)
        {

            try
            {
                //var user = HttpContext.Items["User"] as Users;
                if (ModelState.IsValid)
                {
                    var UsersTags =  db.Tags.Where(pp => pp.UserId == userId).FirstOrDefault();

                    if (UsersTags != null)
                    {
                        ResData<tags> jjJ = new ResData<tags>() { message = "successful", status = true,data= new List<tags> { UsersTags } };
                        return Ok(jjJ);
                    }
                    else
                    {
                        ResData<tags> jj = new ResData<tags>() { message = "successful", status = true, };
                        jj.data.Add(new tags { });

                        return Ok(jj);
                    }
               
                 
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






        [HttpGet("fetchUserKniteD/{userId}")]
        public async Task<ActionResult<dynamic>> fetchUserKniteD(int userId)
        {
            var user = HttpContext.Items["User"] as Users;
            try
            {
                //var user = HttpContext.Items["User"] as Users;
                if (ModelState.IsValid)
                {

                    var knitedList = await db.KnightModel.Where(pp => pp.folowerId == userId).ToListAsync();

                    if (user != null)
                    {
                        var knitedLists = db.KnightModel.Where(pp => pp.folowerId == userId).Select(xxx => new knitedUsersList(db.Users.Where(item => item.id == xxx.brandId).FirstOrDefault()!) { isKnigted = db.KnightModel.Any(xp => xp.brandId == xxx.folowerId && xp.folowerId == user!.id) }).ToList();
                        ResData<knitedUsersList> jj = new ResData<knitedUsersList>() { message = "successful", status = true, data = knitedLists };
                        return Ok(jj);
                    }
                    else
                    {
                        var knitedLists = db.KnightModel.Where(pp => pp.folowerId == userId).Select(xxx => new knitedUsersList(db.Users.Where(item => item.id == xxx.brandId).FirstOrDefault()!)).ToList();
                        ResData<knitedUsersList> jj = new ResData<knitedUsersList>() { message = "successful", status = true, data = knitedLists };
                        return Ok(jj);
                    }
       

                   

              
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


        
        [HttpGet("fetchUserKnitersList/{userId}")]
        public async Task<ActionResult<dynamic>> fetchUserKnitersList(int userId)
        {
            var user = HttpContext.Items["User"] as Users;

           

            try
            {
                if (user != null)
                {
                    if (ModelState.IsValid)
                    {

                        var knitedList = await db.KnightModel.Where(pp => pp.brandId == userId).ToListAsync();

                        var knitedLists = db.KnightModel.Where(pp => pp.brandId == userId).Select(xxx => new knitedUsersList(db.Users.Where(item => item.id == xxx.folowerId).FirstOrDefault()!) { isKnigted = db.KnightModel.Any(xp => xp.brandId == xxx.folowerId && xp.folowerId == user!.id) }).ToList();

                        ResData<knitedUsersList> jj = new ResData<knitedUsersList>() { message = "successful", status = true, data = knitedLists };
                        //check where brand id is flowwerid and follower is is userid
                        return Ok(jj);
                    }
                    else
                    {
                        return StatusCode(505, new ResData<string>() { message = $"User id must be provided", status = false });
                    }

                }
                else
                {
                    if (ModelState.IsValid)
                    {

                        var knitedList = await db.KnightModel.Where(pp => pp.brandId == userId).ToListAsync();

                        var knitedLists = db.KnightModel.Where(pp => pp.brandId == userId).Select(xxx => new knitedUsersList(db.Users.Where(item => item.id == xxx.folowerId).FirstOrDefault()!)).ToList();

                        ResData<knitedUsersList> jj = new ResData<knitedUsersList>() { message = "successful", status = true, data = knitedLists };
                        //check where brand id is flowwerid and follower is is userid
                        return Ok(jj);
                    }
                    else
                    {
                        return StatusCode(505, new ResData<string>() { message = $"User id must be provided", status = false });
                    }
                }

                //var user = HttpContext.Items["User"] as Users;
              
            }
            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"failure{e}", status = false });
            }

        }


        [HttpGet("autoSearch")]
        public ActionResult<ResData<dynamic>> autoSearch([FromQuery] string query)
        {
            if (query == null)
            {
                return StatusCode(200, new ResData<string>() { message = $"Empty Response", status = true,data= new List<string> { } });
            }
            else
            {

                var resp1 = db.Users.Where(ty => ty.brand.Contains(query) || ty.userName.Contains(query)).Select(po => new Users {
 userName=po.userName,
 id=po.id,
 brand=po.brand,
 email=po.email,
 profileImage=po.profileImage,
 phoneNo=po.phoneNo
                }).ToList();
                //var resp1 = from b in db.Users
                //            where b.brand.StartsWith(query) || b.userName.StartsWith(query)
                //            select b;

                //var data = new List<IQueryable> { };
                var data = new List<Users> { };
                return StatusCode(200, new ResData<Users>() { message = $"Success", status = true, data = resp1 });
            }

        }

        [HttpPost("DeletemyAccount")]
        public async Task<ActionResult<ResData<string>>> DeletemyAccount(deleteDto rq)
        {

            var Isexist = db.Users.Where(p => p.id == rq.id).FirstOrDefault();
            if (Isexist != null)
            {
                bool verifyPass = SecurePasswordHasher.Verify(rq.password, Isexist.password);
                if (verifyPass)
                {
                    db.Users.Remove(Isexist);
                    await db.SaveChangesAsync();
                    return Ok(new ResData<string> { message = "Successfully Deleted", status = true });
                }
                else
                {
                    return BadRequest(new ResData<string> { message = "Invalid Password Supplied", status = true });
                }
            }
            else
            {
                return NotFound(new ResData<string> { message = "User Not Found", status = true });
            }


        }
        [HttpGet("privacy_policy")]
        public ActionResult<string> GetHtml()
        {
            var html = System.IO.File.ReadAllText(@"./Helpers/p2.html");
            return base.Content(html, "text/html");
            //return File(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/") + "privacy.html", "text/html");
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResData<dynamic>>> ForgotPassword([FromQuery]string email)
        {
            var IsexistUser = db.Users.Where(P => P.email == email.ToLower()).FirstOrDefault();
        
            if (IsexistUser != null)
            {
                int otp = _userServe.GenerateRandomNo();
                var newUserOtp = new OtpModel { Expires =  DateTime.Now + new TimeSpan(0, 5, 0) ,Otp=otp,email=IsexistUser.email};
                await db.OtpModel.AddAsync(newUserOtp);
                db.SaveChanges();

                string body = $"<div>hi {IsexistUser.userName}<h6></h6><br/><p>We received a request to reset your Cartora app password.\n<br/></p><h4>Your otp is {otp}</h4> </div>";
               var sendMail= _userServe.Sendmail("Reset Password otp", IsexistUser.email, body);

                return Ok(new ResData<string> { message = "Reset Otp Send to your registered email", status = true ,data= new List<string> {$"{sendMail}" } });

            }
            return BadRequest(new ResData<string> { message = "User not found", status = false });
        }

        [HttpPost("ValidateOtp")]
        public ActionResult<ResData<dynamic>> ValidateOtp(validateotpdto rq )
        {
            var isOtp = db.OtpModel.Where(xx => xx.Otp == rq.otp && xx.email == rq.email).FirstOrDefault();
            if (isOtp != null)
            {
                var Res = new ResData<bool> { message = "otp validated", status = true  };
                Res.data.Add(true);
                return Ok(Res);
            }

            else
            {
                return NotFound(new ResData<string> { message = "Invalid Otp", status = false });
            }
        }


        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ResData<string>>> ChangePassword(changepassDto rq)
        {
            var isOtp = db.OtpModel.Where(xx => xx.Otp == rq.otp && xx.email == rq.email).FirstOrDefault();
            if (isOtp != null)
            {
              
               var user= db.Users.Where(xxx => xxx.email == rq.email).FirstOrDefault();
                if (user != null)
                {
                    var hash = SecurePasswordHasher.Hash(rq.newPsdd);
                    user.password = hash;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    db.OtpModel.Remove(isOtp);
                    string body = $"<div>hi {user.userName}<h6></h6><br/><p>Your Cartora Password was changed successfully .\n<br/></p> </div>";
                    var sendMail = _userServe.Sendmail("Password Change", user.email, body);
                    return Ok(new ResData<string> { message = "Password change was successfull", status = true });
                }
                else
                {
                    return NotFound(new ResData<string> { message = "Unable to update Password", status = false });
                }
            }
            else
            {
                return NotFound(new ResData<string> { message = "Unable to update Password", status = false });
            }
        }


    }
}

