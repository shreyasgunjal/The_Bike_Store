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
using System.Web.Routing;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class stockController : ApiController
    {
        private BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        #region GET: api/stock
        // GET: api/stock
        public IQueryable<stock> Getstocks()
        {
            return db.stocks;
        }
        #endregion

        //#region PUT: api/stock/edit/{id}
        //// PUT: api/stock/edit/{id}
        //[ResponseType(typeof(void))]
        //public IHttpActionResult Putstock(int stockId, stock stock)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (stockId != stock.store_id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(stock).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!stockExists(stockId))
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
        //#endregion


        #region PUT: api/orderitems/{orderId}

        // PUT: api/stock/edit/{id}
        [Route("api/stock/edit/{storeId}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putorder_items(int storeId, int prodId, stock updatedItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingItem = db.stocks
                .FirstOrDefault(o => o.store_id == storeId && o.product_id == prodId);
            if (existingItem != null)
            {
                // Update the fields if the item exists
                existingItem.quantity = updatedItem.quantity;
            }
            else
            {
                // If an item to be updated does not exist, return NotFound
                return NotFound();
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!stockExists(storeId))
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

        #region POST: api/stock
        //POST: api/stock
        [ResponseType(typeof(stock))]
        public IHttpActionResult Poststock(stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the store_id exists in the stores table
            var storeExists = db.stores.Any(s => s.store_id == stock.store_id);

            if (!storeExists)
            {
                return BadRequest("The store_id provided does not exist in the stores table.");
            }

            db.stocks.Add(stock);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "An error occurred while processing your request. The entity might already exist or there was a database issue."
                };

                if (stockExists(stock.store_id)) // Adjust condition based on your entity
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

            return CreatedAtRoute("DefaultApi", new { id = stock.store_id }, stock);
        }
        #endregion

        #region GET api/stock/id
        // GET: api/stock/5
        [ResponseType(typeof(stock))]
        [Route("api/stock/{stockId}")]
        public IHttpActionResult Getstock(int stockId)
        {
            var stock = db.stocks.Where(x => x.store_id == stockId)
                                   .ToList();
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }
        #endregion

        #region GET api/stock/{stockId}/{productId}
        // GET : api/stock/{stockId}/{productId}
        [ResponseType(typeof(decimal))]
        [Route("api/stockByStore")]
        public IHttpActionResult Getstock(int stockId, int productId)
        {
            var stock = db.stocks.FirstOrDefault(x => x.store_id == stockId && x.product_id == productId);
                                   
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }
        #endregion

        #region DELETE api/stock/{id}
        // DELETE: api/stock/5
        [ResponseType(typeof(stock))]
        [Route("api/stock/{storeId}/{productId}")]
        public IHttpActionResult Deletestock(int storeId, int productId)
        {
            stock stock = db.stocks.FirstOrDefault(x => x.store_id == storeId && x.product_id == productId);
            if (stock == null)
            {
                return NotFound();
            }

            db.stocks.Remove(stock);
            db.SaveChanges();

            return Ok(stock);
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

        private bool stockExists(int id)
        {
            return db.stocks.Count(e => e.store_id == id) > 0;
        }
    }
}