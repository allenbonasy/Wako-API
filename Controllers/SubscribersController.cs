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
using System.IO;
using System.Net.Mail;
using System.Web;

namespace DTransAPI.Controllers
{

    public class SubscribersController : ApiController
    {
        private DtransEntities db = new DtransEntities();
        PasswordHasher passwordHasher = new PasswordHasher();

        // GET: api/Subscribers
        public IQueryable<Subscriber> GetSubscribers()
        {
            List<Subscriber> allSubs = new List<Subscriber>();
            foreach (Subscriber subscriber in db.Subscribers.ToList())
            {
                new Subscriber {
                    Address = subscriber.Address,
                    DateRegistered = subscriber.DateRegistered,
                    Email = subscriber.Email,
                    IDNumber = subscriber.IDNumber,
                    Name = subscriber.Name,
                    Password = subscriber.Password,
                    Phone = subscriber.Phone,
                    RepeatPassword = subscriber.RepeatPassword,
                    ProfilePic = subscriber.ProfilePic,
                    Status = subscriber.Status,
                    SubscriberId = subscriber.SubscriberId,
                    Surname = subscriber.Surname,
                    upload = subscriber.upload,
                    VerificationCode= subscriber.VerificationCode
                };
                allSubs.Add(subscriber);
            }
            return allSubs.AsQueryable();
        }
       
        // GET: api/Subscribers/5
        [ResponseType(typeof(Subscriber))]
        public IHttpActionResult GetSubscriber(int id)
        {
            Subscriber item = db.Subscribers.Find(id);
            if (item == null)
            {
                return NotFound();
            }


           return Json(item);
        }


          [HttpPost, Route("api/profile")]
    [AllowAnonymous]
          public String PostProfile([FromBody]ImageUp pic)
          {
              string path = "~/Pros/";

              string message = "";

              if (pic.image != null)
              {
                  string finalPath = path + pic.Name;
                  var filePath = HttpContext.Current.Server.MapPath(finalPath);
                  File.WriteAllBytes(filePath, Convert.FromBase64String(pic.image));
                  message = finalPath;
                  return message;
              }
              return "/Pros/Default.png";
          }

[HttpPost, Route("api/email")]
          public String postEmail(Subscriber sub)
          {
              string check = "";



              var message = new MailMessage();

              message.To.Add(new MailAddress(sub.Email));
              // message.To.Add(new MailAddress("allenbonasy@gmail.com"));
              // replace with valid value 
              message.From = new MailAddress("wakho.notifications@gmail.com");  // replace with valid value
              message.Subject = "Wakho Verification Code";

              var body = "<div><p> Hello " + sub.Name + " " + sub.Surname + "</p><br /><p>Your Wakho verification code is :" + sub.VerificationCode + ". " + "</p></div>";
              //message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
              message.Body = string.Format(body);

              message.IsBodyHtml = true;
              try
              {



                  using (var smtp = new SmtpClient())
                  {
                        var credential = new NetworkCredential
                        {
                            UserName = "wakho.notifications@gmail.com",
                            Password = "tongayisithole"
                        };
                        smtp.Credentials = credential;
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                        smtp.Send(message);
                        check = sub.VerificationCode;

                  }
              }
              catch (Exception e)
              {

                  check = e.Message;
              }

              return check;
          }

        // PUT: api/Subscribers/5
        [ResponseType(typeof(void))]
        public string PutSubscriber(int id, Subscriber subscriber)
        {
            DtransEntities dbP = new DtransEntities();
            if (subscriber.Password != subscriber.RepeatPassword)
            {
                return "passwords are not matching";
            }

            subscriber.Password = passwordHasher.base64Encode(subscriber.Password);
            subscriber.RepeatPassword = subscriber.Password;

            if (subscriber!=null && subscriber.upload!=null)
       {
           subscriber.ProfilePic = PostProfile(subscriber.upload);
       }



       String message = "";
            Subscriber sub = dbP.Subscribers.Find(id);
            if (sub == null)
            {
                return "User not found";
            }
           
       try
       {
           // subscriber.SubscriberId = sub.SubscriberId;
           subscriber.DateRegistered = sub.DateRegistered;
           db.Entry(subscriber).State = EntityState.Modified;


           db.SaveChanges();
           message = "Updated_" + subscriber.ProfilePic;
       }
       catch (Exception e)
       {
           message = e.Message;
       }


       return message;
        }
      
        // POST: api/Subscribers

   [ResponseType(typeof(Subscriber))]
        public string PostSubscriber([FromBody]Subscriber obj)
        {
            if (obj==null)
            {
                return "failed";
            }

            int subscribers = db.Subscribers.Where(s => s.Email == obj.Email).Count();
            if (subscribers > 0)
            {
                return  "email already exists";
            }
            int numberCount = db.Subscribers.Where(s => s.Phone == obj.Phone).Count();
            if (numberCount > 0)
            {
                return "phone number already exists";
            }
            if (obj.Password != obj.RepeatPassword)
            {
                return "passwords are not matching";
            }


            Random r = new Random();
           
        
            obj.DateRegistered = DateTime.Now.Date.ToString();
            obj.Password = passwordHasher.base64Encode(obj.Password);
            obj.RepeatPassword = obj.Password;
            obj.VerificationCode = "" +DateTime.Now.Year+r.Next(400);
          
            string message = "";
            db.Subscribers.Add(obj);
            try {
                db.SaveChanges();
                String ver=postEmail(obj);
                message = "Suceeded,"+ver;
            }
            catch(Exception e) 
            {
                message = message + e.InnerException;
            }

            return message;
        }

    //DELETE: api/Subscribers/5
   [ResponseType(typeof(Subscriber))]
   public IHttpActionResult DeleteSubscriber(int id)
   {
       Subscriber subscriber = db.Subscribers.Find(id);
       if (subscriber == null)
       {
           return NotFound();
       }

       db.Subscribers.Remove(subscriber);
       db.SaveChanges();

       return Ok(subscriber);
   }


   [HttpPost, Route("api/fogotpassword")]
   public String postFogotEmail(String email)

   {
       Subscriber sub = db.Subscribers.FirstOrDefault(s => s.Email == email);

       string check = "";



       var message = new MailMessage();

       message.To.Add(new MailAddress(sub.Email));
       // message.To.Add(new MailAddress("allenbonasy@gmail.com"));
       // replace with valid value 
       message.From = new MailAddress("wakho.notifications@gmail.com");  // replace with valid value
       message.Subject = "Wakho Password Recovery";

       var body = "<div><p> Hie " + sub.Name + " " + sub.Surname + "</p><br /><p>Your Wakho password  is : " + sub.Password + " .  We advise you to change your password after logging in for security reasons.Thank you" + "</p></div>";
       // message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
       message.Body = string.Format(body);

       message.IsBodyHtml = true;
       try
       {



           using (var smtp = new SmtpClient())
           {
               var credential = new NetworkCredential
               {
                   UserName = "wakho.notifications@gmail.com",
                   Password = "tongayisithole"
               };
               smtp.Credentials = credential;
                    //if (email.Contains("@outlook.com"))
                    //{
                    //    smtp.Host = "smtp-mail.outlook.com";
                    //    smtp.Port = 587;
                    //}
              
                    //else if (email.Contains("@yahoo.com"))
                    //{
                    //    smtp.Host = "smtp.mail.yahoo.com";
                    //    smtp.Port = 465;
                    //}
                    //else if (email.Contains("@gmail.com"))
                    //{
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                    //}
                    //else if (email.Contains("@afrosoft.co.zw"))
                    //{
                       
                    //    smtp.Host = "smtp.office365.com";
                    //    smtp.Port = 587;

                    //    smtp.EnableSsl = true;
                    //}



                    smtp.Send(message);
               check = "OK";

           }
       }
       catch (Exception e)
       {

           check = e.InnerException.ToString();
       }

       return check;
   }

        [HttpPost, Route("api/login")]
        [ResponseType(typeof(Subscriber))]
        public IHttpActionResult LoginSubscriber(LoginObject loginObject)
        {
            loginObject.password = passwordHasher.base64Encode(loginObject.password);
            Subscriber subscriber = db.Subscribers.FirstOrDefault(s=>s.Password==loginObject.password && s.Email==loginObject.email);
            if (subscriber == null)
            {
                return NotFound();
            }
            return Ok(subscriber);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubscriberExists(int id)
        {
            return db.Subscribers.Count(e => e.SubscriberId == id) > 0;
        }
    }




}