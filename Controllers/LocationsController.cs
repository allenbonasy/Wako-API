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
    public class LocationsController : ApiController
    {
        private DtransEntities db = new DtransEntities();

        // GET: api/Locations
        public IQueryable<Location> GetLocations()
        {
            List<Location> loc = new List<Location>();
            foreach(Location location in db.Locations.ToList())
            {
                loc.Add(new Location { 
                LocationId=location.LocationId,
                City=location.City,
                Country=location.Country,
                mapUrl=location.mapUrl
                });
            }
            return loc.AsQueryable();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocationExists(int id)
        {
            return db.Locations.Count(e => e.LocationId==id) > 0;
        }
    }
}