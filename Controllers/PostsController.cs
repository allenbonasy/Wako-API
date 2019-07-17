using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DTransAPI.Models;
using System.Web;
using System.IO;
using System.Web.Mvc.Async;
using System.Web.Mvc.Filters;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Drawing;
using System.Web.Hosting;
namespace DTransAPI.Controllers
{
   
    public class PostsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Posts
        public IQueryable<Post> GetPosts()
        {
            List<Post> posts = new List<Post>();
            foreach (Post post in db.Posts.OrderByDescending(id => id.PostId).Where(s => s.Status == "Pending" ||s.Status.Contains("Pay Initiated")).ToList())
            {
                posts.Add(new Post { 
                DatePosted=post.DatePosted,
                DeliveryDate=post.DeliveryDate,
                Description=post.Description,
                Fragility=post.Fragility,
                LocationFromId=post.LocationFromId,
                LocationToId=post.LocationToId,
                ParcelPic=post.ParcelPic,
                PickUpPoint=post.PickUpPoint,
                PostId=post.PostId,
                ProposedFee=post.ProposedFee,
                Status=post.Status,
                SubscriberId=post.SubscriberId,
                TimePosted=post.TimePosted,
                Title=post.Title,
              
                  Weight = post.Weight,
                AddressTo=post.AddressTo

                });
            }
            return posts.AsQueryable();
        }


       [Route("api/pendingPost")]
        public IQueryable<Post> GetPending()
        {
            List<Post> posts = new List<Post>();
            foreach (Post post in db.Posts.OrderByDescending(id => id.PostId).Where(s => s.Status.Contains("Paid")).ToList())
            {
                posts.Add(new Post
                {
                    DatePosted = post.DatePosted,
                    DeliveryDate = post.DeliveryDate,
                    Description = post.Description,
                    Fragility = post.Fragility,
                    LocationFromId = post.LocationFromId,
                    LocationToId = post.LocationToId,
                    ParcelPic = post.ParcelPic,
                    PickUpPoint = post.PickUpPoint,
                    PostId = post.PostId,
                    ProposedFee = post.ProposedFee,
                    Status = post.Status,
                    SubscriberId = post.SubscriberId,
                    TimePosted = post.TimePosted,
                    Title = post.Title,
                    AgentID = post.AgentID,
                    Weight = post.Weight,
                    AddressTo = post.AddressTo

                });
            }
            return posts.AsQueryable();
        }

        // GET: api/Posts/5
        [ResponseType(typeof(Post))]
        public IHttpActionResult GetPost(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }
            Post p = new Post
            {
                DatePosted = post.DatePosted,
                DeliveryDate = post.DeliveryDate,
                Description = post.Description,
                Fragility = post.Fragility,
                LocationFromId = post.LocationFromId,
                LocationToId = post.LocationToId,
                ParcelPic = post.ParcelPic,
                PickUpPoint = post.PickUpPoint,
                PostId = post.PostId,
                ProposedFee = post.ProposedFee,
                Status = post.Status,
                SubscriberId = post.SubscriberId,
                TimePosted = post.TimePosted,
                Title = post.Title,
                Weight = post.Weight,
                AddressTo=post.AddressTo

            };
            return Json(p);
        }

    
        // POST: api/Posts
        [ResponseType(typeof(Post))]
        public String PostPost([FromBody]Post post)
        {

            String message = "";

            try {
              post.DatePosted = DateTime.Now.Date;
        
          
            if (post.upload != null)
            {
                post.ParcelPic = PostFile(post.upload);
            }
            else
            {
                post.ParcelPic = "/Parcel/Default.jpg";
            }
            post.TimePosted = DateTime.Now.TimeOfDay; 
            db.Posts.Add(post);
            db.SaveChanges();
            message = "Succeeded_"+post.PostId;
            }catch(Exception e){
            message=e.Message;
            }


            return message;
        }


         
        public String PutPost(int id,Post post)
        {

            DtransEntities dbP = new DtransEntities();
            String message = "";

            try
            {
             


                dbP.Entry(post).State = EntityState.Modified;
            
              
                dbP.SaveChanges();
                message = ""+post.PostId;
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return message;
        }


        [Route("api/upload")]

        [AllowAnonymous]
        public String PostFile([FromBody]ImageUp pic)
        {
       
           
                   

                    string path = "~/Parcel/";

                    string message = "";

                    if (pic.image != null)
                    {
                        string finalPath = path + pic.Name;
                        var filePath = HttpContext.Current.Server.MapPath(finalPath);
                        File.WriteAllBytes(filePath, Convert.FromBase64String(pic.image));
                        message = finalPath;
                        return message;
                    }
                    return "/Parcel/Default.jpg";
        }

        [Route("api/HireRequests")]
        public IQueryable<Post> GetHireRequests()
        {
            List<Post> posts = new List<Post>();
            foreach (Post post in db.Posts.OrderByDescending(id => id.PostId).Where(s => s.Status == "Hire Request").ToList())
            {
                posts.Add(new Post
                {
                    DatePosted = post.DatePosted,
                    DeliveryDate = post.DeliveryDate,
                    Description = post.Description,
                    Fragility = post.Fragility,
                    LocationFromId = post.LocationFromId,
                    LocationToId = post.LocationToId,
                    ParcelPic = post.ParcelPic,
                    PickUpPoint = post.PickUpPoint,
                    PostId = post.PostId,
                    ProposedFee = post.ProposedFee,
                    Status = post.Status,
                    SubscriberId = post.SubscriberId,
                    TimePosted = post.TimePosted,
                    Title = post.Title,
                    AgentID=post.AgentID,
                    Weight = post.Weight,
                    AddressTo = post.AddressTo

                });
            }
            return posts.AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostExists(int id)
        {
            return db.Posts.Count(e => e.PostId == id) > 0;
        }
    }
}