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
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class storesController : ApiController
    {
        private BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        #region GET: api/stores
        // GET: api/stores
        [Route("api/Stores")]
        public IQueryable<store> Getstores()
        {
            return db.stores;
        }
        #endregion

        #region api/stores/id
        // GET: api/stores/5
        [Route("api/stores/id")]
        [ResponseType(typeof(store))]
        public IHttpActionResult Getbrand(int storeID)
        {
            store storeObj = db.stores.Find(storeID);

            if (storeObj == null)
            {
                return NotFound();
            }

            return Ok(storeObj);
        }
        #endregion

        # region GET: api/stores/city
        // GET: api/stores/city
        [Route("api/stores/city")]
        [ResponseType(typeof(store))]
        public IHttpActionResult Getstore(string city)
        {
            var storesInCity = db.stores.Where(s => s.city.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();
            if (storesInCity == null)
            {
                return NotFound();
            }

            //return Ok(store);
            return Ok(storesInCity);
        }
        #endregion

        #region GET: api/stores/totalstoresineachstate
        // GET: api/stores/total stores
        [Route("api/stores/totalstoreineachstate")]
        [HttpGet]
        [ResponseType(typeof(object))]
        public IHttpActionResult Totalstoreineachstate()
        {
            var totalStore = db.stores.GroupBy(s => s.state).Select
                (g => new
                {
                    state = g.Key,
                    TotalStores = g.Count()
                }).ToList();
            //return Ok(store);
            return Ok(totalStore);
        }
        #endregion

        #region GET: api/stores/maximum customers
        // GET: api/stores/maximum customers
        [Route("api/stores/maxiumcustomers")]
        [HttpGet]
        [ResponseType(typeof(store))]
        public IHttpActionResult maxiumcustomers()
        {
            var storesInCity = db.orders.GroupBy(s => s.store_id).Select(g => new
            {
                StoreId = g.Key,
                UniqueCust = g.Select(c => c.customer_id).Distinct().Count()
            })
            .OrderByDescending(g => g.UniqueCust)
            .FirstOrDefault();

            var res = storesInCity.StoreId;

            var storeInfo = db.stores.Where(e => e.store_id == res).FirstOrDefault();

            var storeName = storeInfo.store_name;

            //return Ok(store);
            return Ok(storeName);
        }
        #endregion

        #region GET:api/stores/highestsale
        // GET: api/stores/highestsale
        [HttpGet]
        [Route("api/stores/highestsale")]
        [ResponseType(typeof(store))]
        public IHttpActionResult GetStoreWithHighestOrders()
        {
            // Query to find the store with the highest number of orders
            var highestOrderStore = db.orders
                .GroupBy(o => o.store_id)
                .Select(g => new
                {
                    StoreId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(s => s.OrderCount)
                .FirstOrDefault();

            if (highestOrderStore == null)
            {
                return NotFound();
            }

            // Retrieve the store details based on the StoreId
            var storeDetails = db.stores
                .Where(s => s.store_id == highestOrderStore.StoreId)
                .Select(s => new
                {
                    s.store_name
                })
                .FirstOrDefault();

            if (storeDetails == null)
            {
                return NotFound();
            }

            return Ok(storeDetails.store_name);
        }
        #endregion

        #region PUT: api/stores/edit/{storeid}
        // PUT: api/stores/edit/{storeid}
        [Route ("api/stores/edit/{storeid}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putstore(int storeId, store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (storeId != store.store_id)
            {
                return BadRequest();
            }

            db.Entry(store).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!storeExists(storeId))
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
        #endregion

        #region POST: api/stores
        [HttpPost]
        [Route ("api/stores/")]
        // POST: api/stores
        [ResponseType(typeof(store))]
        public IHttpActionResult Poststore(store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            

            try
            {
                db.stores.Add(store);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "An error occurred while processing your request. The entity might already exist or there was a database issue."
                };

                if (storeExists(store.store_id)) // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                else
                {
                    var detailedErrorResponse = new
                    {
                        timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        message = "Validation Failed",
                    };

                    return Content(HttpStatusCode.InternalServerError, detailedErrorResponse); // HTTP 500 Internal Server Error
                }
            }
            return Ok(store);//CreatedAtRoute("DefaultApi", new { id = store.store_id }, store);
        }
        #endregion

        #region PATCH: api/stores/edit/{storeId}
        //PATCH: api/stores/edit/{storeId}
        [Route("api/stores/edit/{storeId}")]
        [ResponseType(typeof(void))]
        [HttpPatch]
        public IHttpActionResult PatchStoreDetails(int storeId, string phoneNo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store1 = db.stores.Where(x => x.store_id == storeId).FirstOrDefault();

            if (storeId != store1.store_id)
            {
                return BadRequest();
            }

            store1.phone = phoneNo;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!storeExists(storeId))
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
        #endregion


        // DELETE: api/stores/5
        #region Get api/stores/id
        [HttpDelete]
        [ResponseType(typeof(store))]
        public IHttpActionResult Deletestore(int id)
        {
            store store = db.stores.Find(id);
            if (store == null)
            {
                return NotFound();
            }

            db.stores.Remove(store);
            db.SaveChanges();

            return Ok(store);
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool storeExists(int id)
        {
            return db.stores.Count(e => e.store_id == id) > 0;
        }
    }
}