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
    public class HiddenChatsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/HiddenChats
        //public IQueryable<HiddenChat> GetHiddenChats()
        //{
        //    return db.HiddenChats;
        //}

        // GET: api/HiddenChats/5
        //[ResponseType(typeof(HiddenChat))]
        //public IHttpActionResult GetHiddenChat(int id)
        //{
        //    HiddenChat hiddenChat = db.HiddenChats.Find(id);
        //    if (hiddenChat == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(hiddenChat);
        //}

        // PUT: api/HiddenChats/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutHiddenChat(int id, HiddenChat hiddenChat)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != hiddenChat.HiddenId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(hiddenChat).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!HiddenChatExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/HiddenChats
        [ResponseType(typeof(HiddenChat))]
        public IHttpActionResult PostHiddenChat(HiddenChat hiddenChat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            HiddenChat hidden = new HiddenChat {

                ChatId = hiddenChat.ChatId,
            Date=DateTime.Now.Date,
                HiddenId = hiddenChat.HiddenId,
                SubsciberId = hiddenChat.SubsciberId
        

            
            };
            db.HiddenChats.Add(hidden);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = hidden.HiddenId }, hidden);
        }

      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HiddenChatExists(int id)
        {
            return db.HiddenChats.Count(e => e.HiddenId == id) > 0;
        }
    }
}