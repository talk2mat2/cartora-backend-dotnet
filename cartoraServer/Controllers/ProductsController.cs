using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cartoraServer.Data;
using cartoraServer.Helpers;
using cartoraServer.models;
using cartoraServer.services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cartoraServer.Controllers
{
    [Route("api/v1/[controller]")]
    public class ProductsController : Controller
    {
        private readonly AppSettings _appSettings;
        public AContext db;

        public ProductsController(AContext _db, IOptions<AppSettings> appSettings)
        {
            db = _db;
            _appSettings = appSettings.Value;
        }
        // GET: api/values
        [HttpGet]
        public ActionResult<ResData<Product>> Get()
        {
            var data = db.Products.Select(ppp =>
            new UsersProducts
            {
                id = ppp.id,
                UserId = ppp.UserId,
                Price = ppp.Price,
                description = ppp.description,
                Media = ppp.Media,
                stock=ppp.stock,
                Iscollection=ppp.Iscollection,
                Snapshot=ppp.Snapshot,
                frameColors=ppp.frameColors,
                Mediatype=ppp.Mediatype,
                User = new { userName = ppp.User!.userName,brand=ppp.User.brand }
            }).ToList();
            return Ok(new ResData<UsersProducts>() { data = data, message = "successfully retrieved", status = true });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResData<Product>>> Post([FromForm] ProductsDto request)
        {
            var user = (Users)HttpContext.Items["User"]!;
            var currentUser = await db.Users.FindAsync(user.id);
            if (currentUser == null)
            {
                return NotFound( new ResData<string> { message="user not found",status=false});
            }

            string colors = "";

            //List<string> ii = new List<string> { "dd","ffff" };
            if (request.frameColors.Count() > 0)
            {
                colors = String.Join(",", request.frameColors);
            }

            string Snapshotpath = "";
            List<ImgeUrll> MediaList = new List<ImgeUrll>();

            if (request.File!=null&&request.File.Count > 0)
            {
                
                foreach (var file in request.File)
                {

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    string fileExt = file.FileName.Split(".").Last();
                    string FileName = myuuidAsString + file.FileName.Split(".")[0];
                    string newFilename = myuuidAsString + FileName + "." + fileExt;
                    string fileNameWithPath = Path.Combine(path, newFilename);
                    if (file.FileName == "snapshot.jpg")
                    {
                        Snapshotpath = _appSettings.ServerUrl + "/images/" + newFilename;

                    }
                    else
                    {
                       
                        MediaList.Add(new ImgeUrll { url= _appSettings.ServerUrl + "/images/" + newFilename });
                    }

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                //model.IsSuccess = true;
                //model.Message = "Files upload successfully";
            }



            var newProduct = new Product
            {
                description = request.description,
                Mediatype = request.Mediatype,
                User = currentUser,
                Snapshot = Snapshotpath,
                frameColors = colors,
                Media=MediaList

            };
            db.Products.Add(newProduct);
            db.SaveChanges();
            return Ok(new ResData<Product> { status=true,data=new List<Product> { newProduct},message="successfully Created" });

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

