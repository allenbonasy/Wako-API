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
    public class Agent_PostController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Agent_Post
        //public IQueryable<Agent_Post> GetAgent_Post()
        //{
        //    List<Agent_Post> apost = new List<Agent_Post>();
        //    foreach(Agent_Post agent in db.Agent_Post.ToList())
        //    {
        //        apost.Add(new Agent_Post { 
        //        AgentId=agent.AgentId,
        //        Date=agent.Date,
        //        Eng_PostId=agent.Eng_PostId,
        //        PostId=agent.PostId,
        //        Rec_Date=agent.Rec_Date,
                
                
                
        //        });
        //    }
        //    return db.Agent_Post.AsQueryable();
        //}

        // GET: api/Agent_Post/5
        //[ResponseType(typeof(Agent_Post))]
        //public IHttpActionResult GetAgent_Post(int id)
        //{
        //    Agent_Post agent_Post = db.Agent_Post.Find(id);
        //    if (agent_Post == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(agent_Post);
        //}

        // PUT: api/Agent_Post/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAgent_Post(int id, Agent_Post agent_Post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agent_Post.Eng_PostId)
            {
                return BadRequest();
            }
            Agent_Post agent_post = new Agent_Post
            {
                AgentId = agent_Post.AgentId,
                Date = agent_Post.Date,
                Eng_PostId = agent_Post.Eng_PostId,
                PostId = agent_Post.PostId,
                Rec_Date = agent_Post.Rec_Date



            };
            db.Entry(agent_post).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Agent_PostExists(id))
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

        // POST: api/Agent_Post
        [ResponseType(typeof(Agent_Post))]
        public IHttpActionResult PostAgent_Post(Agent_Post agent_Post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Agent_Post agent_post = new Agent_Post
            {
                AgentId = agent_Post.AgentId,
                Date = agent_Post.Date,
                Eng_PostId = agent_Post.Eng_PostId,
                PostId = agent_Post.PostId,
                Rec_Date = agent_Post.Rec_Date



            };
            db.Agent_Post.Add(agent_post);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = agent_post.Eng_PostId }, agent_post);
        }

        // DELETE: api/Agent_Post/5
        [ResponseType(typeof(Agent_Post))]
        public IHttpActionResult DeleteAgent_Post(int id)
        {
            Agent_Post agent_Post = db.Agent_Post.Find(id);
            if (agent_Post == null)
            {
                return NotFound();
            }
            Agent_Post agent_post = new Agent_Post
            {
                AgentId = agent_Post.AgentId,
                Date = agent_Post.Date,
                Eng_PostId = agent_Post.Eng_PostId,
                PostId = agent_Post.PostId,
                Rec_Date = agent_Post.Rec_Date



            };
            db.Agent_Post.Remove(agent_post);
            db.SaveChanges();

            return Json(agent_Post);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Agent_PostExists(int id)
        {
            return db.Agent_Post.Count(e => e.Eng_PostId == id) > 0;
        }
    }
}