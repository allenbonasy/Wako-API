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
    public class TransactionFeesController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/TransactionFees
        public IQueryable<TransactionFee> GetTransactionFees()
        {
            List<TransactionFee> posts = new List<TransactionFee>();
            foreach (TransactionFee t in db.TransactionFees.OrderByDescending(id => id.transID).ToList())
            {
                posts.Add(new TransactionFee
                {
                 AgentID =t.AgentID,
                 amount=t.amount,
                 Date=t.Date,
                 postID=t.postID,
                 reff_Number=t.reff_Number,
                 Status=t.Status,
                 Time=t.Time,
                 transID=t.transID,
                 paymentReff=t.paymentReff,
                 PollURL=t.PollURL,Hash=t.Hash,
                 Submitted=t.Submitted

                });
            }
            return posts.AsQueryable();
        }



         [Route("api/UserTransactions")]
        public IQueryable<TransactionFee> GetUserTransactions(int ID)
        {
           
            List<TransactionFee> posts = new List<TransactionFee>();
            foreach (TransactionFee t in db.TransactionFees.Where(s=>s.AgentID==ID||s.reff_Number.Substring(10).Contains(ID.ToString())).OrderByDescending(id => id.transID).ToList())
            {
                posts.Add(new TransactionFee
                {
                    AgentID = t.AgentID,
                    amount = t.amount,
                    Date = t.Date,
                    postID = t.postID,
                    reff_Number = t.reff_Number,
                    Status = t.Status,
                    Time = t.Time,
                    transID = t.transID,
                    paymentReff = t.paymentReff,
                    PollURL = t.PollURL,
                    Hash = t.Hash,
                    Submitted = t.Submitted

                });
            }
            return posts.AsQueryable();
        }



        // GET: api/TransactionFees/5
        [ResponseType(typeof(TransactionFee))]
        public IHttpActionResult GetTransactionFee(int id)
        {
            TransactionFee transactionFee = db.TransactionFees.Find(id);
            if (transactionFee == null)
            {
                return null;
            }

            return Json(transactionFee);
        }

        // PUT: api/TransactionFees/5
        [ResponseType(typeof(void))]
        public String PutTransactionFee(int id, TransactionFee transactionFee)
        {
            String mess = "";

            db.Entry(transactionFee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                mess = "saved";
            }
            catch (Exception ex)
            {
                mess = ex.Message;
            }

            return mess;
        }

        // POST: api/TransactionFees
        [ResponseType(typeof(TransactionFee))]
        public IHttpActionResult PostTransactionFee([FromBody]TransactionFee transactionFee)
        {
            if (transactionFee != null)
            {
                db.TransactionFees.Add(transactionFee);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = transactionFee.transID }, transactionFee);
            }
            return Ok("Null Refrence");
        }

        // DELETE: api/TransactionFees/5
        [ResponseType(typeof(TransactionFee))]
        public IHttpActionResult DeleteTransactionFee(int id)
        {
            TransactionFee transactionFee = db.TransactionFees.Find(id);
            if (transactionFee == null)
            {
                return NotFound();
            }

            db.TransactionFees.Remove(transactionFee);
            db.SaveChanges();

            return Ok(transactionFee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionFeeExists(int id)
        {
            return db.TransactionFees.Count(e => e.transID == id) > 0;
        }
    }
}