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
    public class WalletsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Wallets
        public IQueryable<Wallet> GetWallets()
        {
        List<Wallet> wallets= new List<Wallet>();
            foreach(Wallet wallet in db.Wallets)
            {
                wallets.Add(new Wallet {
            ID=wallet.ID,Overal=wallet.Overal,AgentID=wallet.AgentID,Available=wallet.Available,Drawable=wallet.Drawable,Pending=wallet.Pending,Total=wallet.Total
            });
            }
            return wallets.AsQueryable();
        }

        // GET: api/Wallets/5
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult GetWallet(int id)
        {
           
            Wallet wallet = db.Wallets.Find(id);
            if (wallet == null)
            {
                return NotFound();
            }
            Wallet walls = new Wallet {
            ID=wallet.ID,Overal=wallet.Overal,AgentID=wallet.AgentID,Available=wallet.Available,Drawable=wallet.Drawable,Pending=wallet.Pending,Total=wallet.Total
            };
            return Json(walls);
        }

        [Route("api/AgentWallet")]
        public IHttpActionResult GetAgentWallet(int id)
        {
            Wallet wallet = db.Wallets.FirstOrDefault(s=>s.AgentID==id);
            if (wallet == null)
            {
                return NotFound();
            }
            Wallet walls = new Wallet
            {
                ID = wallet.ID,
                Overal = wallet.Overal,
                AgentID = wallet.AgentID,
                Available = wallet.Available,
                Drawable = wallet.Drawable,
                Pending = wallet.Pending,
                Total = wallet.Total
            };
            return Json(walls);
        }



        // PUT: api/Wallets/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWallet(int id, Wallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wallet.ID)
            {
                return BadRequest();
            }

            db.Entry(wallet).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalletExists(id))
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

        // POST: api/Wallets
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult PostWallet(Wallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wallets.Add(wallet);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = wallet.ID }, wallet);
        }

        // DELETE: api/Wallets/5
        [ResponseType(typeof(Wallet))]
        public IHttpActionResult DeleteWallet(int id)
        {
            Wallet wallet = db.Wallets.Find(id);
            if (wallet == null)
            {
                return NotFound();
            }

            db.Wallets.Remove(wallet);
            db.SaveChanges();

            return Ok(wallet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WalletExists(int id)
        {
            return db.Wallets.Count(e => e.ID == id) > 0;
        }
    }
}