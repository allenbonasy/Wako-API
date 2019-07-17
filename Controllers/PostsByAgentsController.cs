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
    public class PostsByAgentsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/PostsByAgents
        public IQueryable<PostsByAgent> GetPostsByAgents()
        {
            List<PostsByAgent> posts = new List<PostsByAgent>();
            foreach(PostsByAgent pos in db.PostsByAgents.ToList())
            {

                posts.Add(new PostsByAgent
                {
                    AgentId = pos.AgentId,
                    DatePosted = pos.DatePosted,
                    Id = pos.Id,
                    DepatureDate = pos.DepatureDate,
                    PickUp = pos.PickUp,
                    TransPort = pos.TransPort,
                    LocationFrom = pos.LocationFrom,
                    LocationTo = pos.LocationTo,
                    Weight = pos.Weight,
                    Fragility = pos.Fragility,
                    ETA = pos.ETA,
                    
                    Agent = new Agent {AgentId=pos.Agent.AgentId,CompanyAdress=pos.Agent.CompanyAdress,
                    CompanyLogo=pos.Agent.CompanyLogo,CompanyName=pos.Agent.CompanyName,CompanyTel=pos.Agent.CompanyTel,Mode_Of_Transport=pos.Agent.Mode_Of_Transport,
                    SubscriberId=pos.Agent.SubscriberId

                    }

                  //  Agent=pos.Agent
                });
            
            }
            return posts.AsQueryable();
        }

        // GET: api/PostsByAgents/5
        [ResponseType(typeof(PostsByAgent))]
        public IHttpActionResult GetPostsByAgent(int id)
        {
            PostsByAgent postsByAgent = db.PostsByAgents.Find(id);
            if (postsByAgent == null)
            {
                return NotFound();
            }

            return Ok(postsByAgent);
        }

        // PUT: api/PostsByAgents/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPostsByAgent(int id, PostsByAgent postsByAgent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != postsByAgent.Id)
            {
                return BadRequest();
            }

            db.Entry(postsByAgent).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostsByAgentExists(id))
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

        // POST: api/PostsByAgents
        [ResponseType(typeof(PostsByAgent))]
        public IHttpActionResult PostPostsByAgent(PostsByAgent postsByAgent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PostsByAgents.Add(postsByAgent);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = postsByAgent.Id }, postsByAgent);
        }

        // DELETE: api/PostsByAgents/5
        [ResponseType(typeof(PostsByAgent))]
        public IHttpActionResult DeletePostsByAgent(int id)
        {
            PostsByAgent postsByAgent = db.PostsByAgents.Find(id);
            if (postsByAgent == null)
            {
                return NotFound();
            }

            db.PostsByAgents.Remove(postsByAgent);
            db.SaveChanges();

            return Ok(postsByAgent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostsByAgentExists(int id)
        {
            return db.PostsByAgents.Count(e => e.Id == id) > 0;
        }
    }
}