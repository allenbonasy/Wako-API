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

namespace DTransAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Notifications
        public IQueryable<Notification> GetNotifications()
        {
           List<Notification> notList= new List<Notification>();
            foreach(var item in db.Notifications)
            {

                notList.Add(new Notification { 
                ID=item.ID,
                PostID=item.PostID,SenderID=item.SenderID,Recipient=item.Recipient,Status=item.Status,Date=item.Date,Time=item.Time,Message=item.Message
                
                });
             
            }
               return notList.AsQueryable();
        }

        // GET: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public IHttpActionResult GetNotification(int id)
        {
            Notification item = db.Notifications.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            Notification a = new Notification
            {
                ID = item.ID,
                PostID = item.PostID,
                SenderID = item.SenderID,
                Recipient = item.Recipient,
                Status = item.Status,
                Date = item.Date,
                Time = item.Time,
                Message = item.Message

            };
            return Json(a);
        }

        // PUT: api/Notifications/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNotification([FromBody]int id, Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notification.ID)
            {
                return BadRequest();
            }

            db.Entry(notification).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Notifications
        [ResponseType(typeof(Notification))]
        public IHttpActionResult PostNotification(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Notifications.Add(notification);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = notification.ID }, notification);
        }

        // DELETE: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public IHttpActionResult DeleteNotification(int id)
        {
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            db.Notifications.Remove(notification);
            db.SaveChanges();
            return Ok(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationExists(int id)
        {
            return db.Notifications.Count(e => e.ID == id) > 0;
        }
    }
}