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

namespace DTransAPI.Controllers
{
    public class AgentsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Agents
        public IQueryable<Agent> GetAgents()
        {
            List<Agent> agnts = new List<Agent>();
            foreach(Agent a in db.Agents.ToList())
            {
                agnts.Add(new Agent { 
                AgentId=a.AgentId,
                CompanyAdress=a.CompanyAdress,
                CompanyLogo=a.CompanyLogo,
                CompanyName= a.CompanyName,
                BankName = a.BankName,
                Bank_Adress = a.Bank_Adress,
                Account_Number = a.Account_Number,
                CompanyTel=a.CompanyTel,
              Mode_Of_Transport=a.Mode_Of_Transport,
              IDpic=a.IDpic,
              ProofRes=a.IDpic,
                
                SubscriberId=a.SubscriberId
               
                
                });
            }
            return agnts.AsQueryable();
        }

        // GET: api/Agents/5
        [ResponseType(typeof(Agent))]
        public IHttpActionResult GetAgent(int id)
        {
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return NotFound();
            }
            Agent a = new Agent
            {
                AgentId = agent.AgentId,
                CompanyAdress = agent.CompanyAdress,
                CompanyLogo = agent.CompanyLogo,
                CompanyName = agent.CompanyName,
                BankName = agent.BankName,
                Bank_Adress = agent.Bank_Adress,
                Account_Number = agent.Account_Number,
                CompanyTel = agent.CompanyTel,
              Mode_Of_Transport=agent.Mode_Of_Transport,
                SubscriberId = agent.SubscriberId,
                IDpic = agent.IDpic,
                ProofRes = agent.IDpic,

            };
            return Json(a);
        }
        [HttpPost, Route("api/agentProfile")]
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
        // PUT: api/Agents/5
        [ResponseType(typeof(void))]
        public String PutAgent(int id, Agent agent)
        {
            String mess = "";
            
            Agent a = new Agent
            {

                AgentId = agent.AgentId,
                CompanyAdress = agent.CompanyAdress,
                CompanyLogo = agent.CompanyLogo,
                CompanyName = agent.CompanyName,
                BankName = agent.BankName,
                Bank_Adress = agent.Bank_Adress,
                Account_Number = agent.Account_Number,
                CompanyTel = agent.CompanyTel,
                Mode_Of_Transport = agent.Mode_Of_Transport,

                SubscriberId = agent.SubscriberId,
                IDpic = agent.IDpic,
                ProofRes = agent.IDpic,

            };
            db.Entry(a).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                mess = "Succeeded";
            }
            catch (Exception ex)
            {
                mess = ex.Message;
            }

            return mess;
        }

        // POST: api/Agents
        [ResponseType(typeof(Agent))]
        public IHttpActionResult PostAgent(Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            agent.IDpic = PostProfile(agent.upload1);
            agent.ProofRes = PostProfile(agent.upload2);
            Agent a = new Agent
            {
                AgentId = agent.AgentId,
                CompanyAdress = agent.CompanyAdress,
                CompanyLogo = agent.CompanyLogo,
                CompanyName = agent.CompanyName,
              Mode_Of_Transport=agent.Mode_Of_Transport,
                CompanyTel = agent.CompanyTel,
                BankName = agent.BankName,
                Bank_Adress = agent.Bank_Adress,
                Account_Number = agent.Account_Number,
                IDpic=agent.IDpic,
                ProofRes=agent.ProofRes,
                SubscriberId = agent.SubscriberId,

               

            };
            db.Agents.Add(a);
            db.SaveChanges();
           // DtransEntities dp = new DtransEntities();
           //// dp.
           // db.Wallets.Add(new Wallet
           // {
               
           //     Overal = "0",
           //     AgentID =a.AgentId,
           //     Available = "0",
           //     Drawable = "0",
           //     Pending = "0",
           //     Total = "0"
           // });
           // db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = a.AgentId }, a);
           

        }

        // DELETE: api/Agents/5
        [ResponseType(typeof(Agent))]
        public IHttpActionResult DeleteAgent(int id)
        {
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return NotFound();
            }
            Agent a = new Agent
            {
                AgentId = agent.AgentId,
                CompanyAdress = agent.CompanyAdress,
                CompanyLogo = agent.CompanyLogo,
                CompanyName = agent.CompanyName,
           Mode_Of_Transport=agent.Mode_Of_Transport,
                CompanyTel = agent.CompanyTel,
                BankName = agent.BankName,
                Bank_Adress = agent.Bank_Adress,
                Account_Number = agent.Account_Number,
               
                SubscriberId = agent.SubscriberId,
                //Agent_Post=agent.Agent_Post

            };
            db.Agents.Remove(a);
            db.SaveChanges();

            return Json(a);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AgentExists(int id)
        {
            return db.Agents.Count(e => e.AgentId == id) > 0;
        }
    }
}