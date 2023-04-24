﻿using System;
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
        public ActionResult<ResData<Product>> Get([FromQuery] int userId)
        {
            var data = db.Products.Where(ppp=>ppp.Iscollection==false).OrderByDescending(p => p.createdAt).Select(ppp =>
            new UsersProducts()
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
                Title=ppp.Title,
                User = new { userName = ppp.User!.userName,brand=ppp.User.brand , phoneNo =ppp.User.phoneNo},
                isKnigted=db.KnightModel.Any(ss=>ss.brandId==ppp.UserId && ss.folowerId==userId),
                isLiked = db.LikeModel.Any(ss => ss.userId == userId && ss.productId== ppp.id)

            }).ToList();
            return Ok(new ResData<UsersProducts>() { data = data, message = "successfully retrieved", status = true });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

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
                Price=request.Price,
                frameColors = colors,
                Media=MediaList,
                stock=request.stock,
                Iscollection=request.Iscollection,
                Title=request.Title
                

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

        //[HttpPost("likeProduct")]
        //public async Task<ActionResult<ResData<string>>> LikeProduct([FromBody] LikeProductDto rq)
        //{
        //    var products = await db.Products.FindAsync(rq.productId);
        //  if(products!=null)
        //    {
        //        string likes = products.likes;
        //        var likeList = likes.Split(",");
        //       if(likeList.Contains(rq.userId))
        //        {

        //        }
        //        var response = new ResData<string>();
        //        response.message = "Liked";
        //        response.data = null!;
        //        response.status = true;
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpGet("getUserProducst/{id}")]
        public async Task<ActionResult<ResData<List<Product>>>> getUserProducst(int id)
        {
            var productsList = await db.Products.Where(c => c.UserId == id).OrderByDescending(p => p.createdAt).ToListAsync();

            return Ok(new ResData<Product>() { data = productsList, message = "successfull", status = true });

        }
        [HttpGet("getMyCollections/{id}")]
        public async Task<ActionResult<ResData<List<Product>>>> getMyCollections(int id)
        {
            //var productsList = await db.Products.Where(c => c.UserId == id && c.Iscollection==true).Select(ppp =>
            //new UsersProducts
            //{
            //    id = ppp.id,
            //    UserId = ppp.UserId,
            //    Price = ppp.Price,
            //    description = ppp.description,
            //    Media = ppp.Media,
            //    stock = ppp.stock,
            //    Iscollection = ppp.Iscollection,
            //    Snapshot = ppp.Snapshot,
            //    frameColors = ppp.frameColors,
            //    Mediatype = ppp.Mediatype,
            //    User = new { userName = ppp.User!.userName, brand = ppp.User.brand }
            //}).ToListAsync();

            //return Ok(new ResData<Product>() { data = productsList, message = "successfull", status = true });


            var data = await db.Products.Where(ppp => ppp.Iscollection == true &&ppp.UserId == id).OrderByDescending(p => p.createdAt).Select(ppp =>
   new UsersProducts
   {
       id = ppp.id,
       UserId = ppp.UserId,
       Price = ppp.Price,
       description = ppp.description,
       Media = ppp.Media,
       stock = ppp.stock,
       Iscollection = ppp.Iscollection,
       Snapshot = ppp.Snapshot,
       frameColors = ppp.frameColors,
       Mediatype = ppp.Mediatype,
       User = new { userName = ppp.User!.userName, brand = ppp.User.brand }
   }).ToListAsync();
            return Ok(new ResData<UsersProducts>() { data = data, message = "successfully retrieved", status = true });
        }


        [HttpPost("deleteProduct/{id}")]
        public async Task<ActionResult<ResData<string>>> DeleteProduct(int id)
        {
            var user = db.Products.Find(id);
            if (user != null)
            {
                db.Products.Remove(user);
                await db.SaveChangesAsync();
                return Ok(new ResData<string>() { message = "successfully deleted", status = true });
            }
            else
            {
                return StatusCode(504, new ResData<string>() { message = "not implemented", status = true });
            }
        }

        [Authorize]
        [HttpGet("kintit/{id}")]
        public async Task<ActionResult<ResData<string>>> knitit(int id)
        {
            try
            {
           

                var user = HttpContext.Items["User"] as Users;
                if (user != null)
                {
                    var isExist=db.KnightModel.Where(pp => pp.folowerId == user.id && pp.brandId == id).FirstOrDefault();
                    if (isExist!= null)
                    {
                        db.KnightModel.Remove(isExist);
                        await db.SaveChangesAsync();
                        return Ok(new ResData<string> { message = "Unknighted", status = true });
                    }
                    else
                    {
                        var newUser = new KnightModel();
                        newUser.folowerId = user.id;
                        newUser.brandId = id;
                        db.KnightModel.Add(newUser);
                        await db.SaveChangesAsync();
                        return Ok(new ResData<string> { message = "knighted", status = true });
                    }
                   
                }

                else
                {
                    return StatusCode(505, new ResData<string>() { message = $"User must be logged in to knit a product", status = false });
                }
            }
            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"failure{e}", status = false });

            }
        }


        [HttpPost("likeProduct")]
        public async Task<ActionResult<ResData<dynamic>>> likeProduct([FromBody] LikeDto likeDto)
        {
            try
            {


                var user = HttpContext.Items["User"] as Users;
                if (user != null)
                {
                    var isExist = db.LikeModel.Where(pp => pp.userId == user.id && pp.productId == likeDto.productId).FirstOrDefault();
                    if (isExist != null)
                    {
                        db.LikeModel.Remove(isExist);
                        await db.SaveChangesAsync();
                        return Ok(new ResData<string> { message = "Unliked", status = true });
                    }
                    else
                    {
                        var newUser = new LikeModel();
                        newUser.userId = user.id;
                        newUser.productId = likeDto.productId;
                        db.LikeModel.Add(newUser);
                        await db.SaveChangesAsync();
                        return Ok(new ResData<string> { message = "liked", status = true });
                    }

                }

                else
                {
                    return StatusCode(505, new ResData<string>() { message = $"User must be logged in to knit a product", status = false });
                }
            }
            catch (Exception e)
            {
                return StatusCode(505, new ResData<string>() { message = $"failure{e}", status = false });

            }
        }
    }


}

