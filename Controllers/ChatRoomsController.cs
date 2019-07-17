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
using System.Threading.Tasks;

namespace DTransAPI.Controllers
{
    public class ChatRoomsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/ChatRooms
        public IQueryable<ChatRoom> GetChatRooms()
        {
            List<ChatRoom> chats = new List<ChatRoom>();

  

 

          foreach(ChatRoom chatRoom in db.ChatRooms.ToList())
            {
                chats.Add(new ChatRoom { 
                ChatRoomId=chatRoom.ChatRoomId,
                CreatorId=chatRoom.CreatorId,
                CreatorIP=chatRoom.CreatorIP,
                DateCreated= chatRoom.DateCreated,
                RecipientID=chatRoom.RecipientID,
                Status=chatRoom.Status,
                TimeCreated=chatRoom.TimeCreated,
              

                });
            }
            return chats.AsQueryable();
        }

        // GET: api/ChatRooms/5
        [ResponseType(typeof(ChatRoom))]
        public IHttpActionResult GetChatRoom(int id)
        {
            ChatRoom chatRoom = db.ChatRooms.Find(id);
            if (chatRoom == null)
            {
                return NotFound();
            }
            ChatRoom chat = new ChatRoom
            {
                ChatRoomId = chatRoom.ChatRoomId,
                CreatorId = chatRoom.CreatorId,
                CreatorIP = chatRoom.CreatorIP,
                DateCreated = chatRoom.DateCreated,
                RecipientID = chatRoom.RecipientID,
                Status = chatRoom.Status,
                TimeCreated = chatRoom.TimeCreated

            };
            return Json(chat);
        }

        // PUT: api/ChatRooms/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutChatRoom(int id, ChatRoom chatRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chatRoom.ChatRoomId)
            {
                return BadRequest();
            }
            ChatRoom chat = new ChatRoom
            {
                ChatRoomId = chatRoom.ChatRoomId,
                CreatorId = chatRoom.CreatorId,
                CreatorIP = chatRoom.CreatorIP,
                DateCreated = chatRoom.DateCreated,
                RecipientID = chatRoom.RecipientID,
                Status = chatRoom.Status,
                TimeCreated = chatRoom.TimeCreated

            };
            db.Entry(chat).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatRoomExists(id))
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

        // POST: api/ChatRooms
        [ResponseType(typeof(ChatRoom))]
        public IHttpActionResult PostChatRoom([FromBody]ChatRoom chatRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ChatRoom chat = new ChatRoom
            {
                ChatRoomId = chatRoom.ChatRoomId,
                CreatorId = chatRoom.CreatorId,
                CreatorIP = chatRoom.CreatorIP,
                DateCreated = chatRoom.DateCreated,
                RecipientID = chatRoom.RecipientID,
                Status = chatRoom.Status,
                TimeCreated = chatRoom.TimeCreated

            };
            db.ChatRooms.Add(chat);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = chat.ChatRoomId }, chat);
        }

        // DELETE: api/ChatRooms/5
        [ResponseType(typeof(ChatRoom))]
        public IHttpActionResult DeleteChatRoom(int id)
        {
            ChatRoom chatRoom = db.ChatRooms.Find(id);
            if (chatRoom == null)
            {
                return NotFound();
            }
            ChatRoom chat = new ChatRoom
            {
                ChatRoomId = chatRoom.ChatRoomId,
                CreatorId = chatRoom.CreatorId,
                CreatorIP = chatRoom.CreatorIP,
                DateCreated = chatRoom.DateCreated,
                RecipientID = chatRoom.RecipientID,
                Status = chatRoom.Status,
                TimeCreated = chatRoom.TimeCreated

            };
            db.ChatRooms.Remove(chat);
            db.SaveChanges();

            return Json(chatRoom);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChatRoomExists(int id)
        {
            return db.ChatRooms.Count(e => e.ChatRoomId == id) > 0;
        }
    }
}