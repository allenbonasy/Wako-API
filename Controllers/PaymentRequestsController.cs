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
    public class PaymentRequestsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/PaymentRequests
        public IQueryable<payment_requests> Getpayment_requests()
        {
            return db.payment_requests.AsQueryable();
        }

        // GET: api/PaymentRequests/5
        [ResponseType(typeof(payment_requests))]
        public IHttpActionResult Getpayment_requests(int id)
        {
            payment_requests payment_requests = db.payment_requests.Find(id);
            if (payment_requests == null)
            {
                return NotFound();
            }

            return Ok(payment_requests);
        }

        [ResponseType(typeof(payment_requests))]
        public IHttpActionResult GetRequestPayment(int agent_id)

        {
            payment_requests pay = new payment_requests();
            pay.agent_id = agent_id;
            pay.amount_paid = 0;
            pay.authorised_by = null;
            pay.date_authorised = null;
            pay.date_requested = DateTime.Now;
            db.payment_requests.Add(pay);
            db.SaveChanges();
         

            return Ok(pay);
        }

        // PUT: api/PaymentRequests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putpayment_requests(int id, payment_requests payment_requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payment_requests.id)
            {
                return BadRequest();
            }

            db.Entry(payment_requests).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!payment_requestsExists(id))
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

        // POST: api/PaymentRequests
        [ResponseType(typeof(payment_requests))]
        public IHttpActionResult Postpayment_requests(payment_requests payment_requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.payment_requests.Add(payment_requests);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = payment_requests.id }, payment_requests);
        }

        // DELETE: api/PaymentRequests/5
        [ResponseType(typeof(payment_requests))]
        public IHttpActionResult Deletepayment_requests(int id)
        {
            payment_requests payment_requests = db.payment_requests.Find(id);
            if (payment_requests == null)
            {
                return NotFound();
            }

            db.payment_requests.Remove(payment_requests);
            db.SaveChanges();

            return Ok(payment_requests);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool payment_requestsExists(int id)
        {
            return db.payment_requests.Count(e => e.id == id) > 0;
        }
    }
}