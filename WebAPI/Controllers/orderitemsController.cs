using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Models;

namespace WebAPI.Controllers
{
   // [RoutePrefix("api/orderitems")]
    public class orderitemsController : ApiController
    {
        public BikeStores_Team3Entities db = new BikeStores_Team3Entities();


        #region GET: api/orderitems

        // GET: api/orderitems
        [Route("api/orderitems")]
        public IEnumerable<order_items> Getorder_items()
        {
            // Fetch all items from the database and return
            return db.order_items.ToList();
        }

        #endregion

        #region GET: api/orderitems/{orderId}   //Get BY ID

        // GET: api/category/5
        [ResponseType(typeof(decimal))]
        [Route("api/orderitems/{orderId}")]
        public IHttpActionResult Getorderitem(int orderId)
        {
            List<order_items> orderItem = db.order_items.Where(x => x.order_id == orderId).ToList();
            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        #endregion

        #region GET: api/orderitems/{orderId}/{itemId}   //Get By  OrderId & itemId

        // GET: api/category/5
        [ResponseType(typeof(decimal))]
        [Route("api/orderitemsBy/Order_ItemID")]
        public IHttpActionResult GetExactItem(int orderId, int itemId)
        {
            var orderItem = db.order_items.FirstOrDefault(x => x.order_id == orderId && x.item_id == itemId);
            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        #endregion              

        #region GET: api/orderDetails/{orderId}    // Get the bill for an ordered without discount

        // GET: api/orderitems/5
        //Display the bill for an ordered without discount
        [HttpGet]
        [Route("api/orderDetails/{orderId}")]
        [ResponseType(typeof(decimal))]
        public IHttpActionResult Getorder_itemswith(int orderId)
        {
            var ot = db.order_items
                        .Where(o => o.order_id == orderId).Sum(o => o.quantity * o.list_price);

            if (ot == null)
            {
                return NotFound();
            }

            return Ok(ot);
        }

        #endregion

        #region GET: api/orderitem/{orderId}     // Get the bill amount for an Specific OrderId with Discount

        // GET: api/orderitem/5           Get the bill amount for an Specific OrderId with Discount
        [HttpGet]
        [Route("api/orderitem/{orderId}")]
        [ResponseType(typeof(decimal))]
        public IHttpActionResult Getorder_itemswithout(int orderId)
        {
            var ot = db.order_items.Where(o => o.order_id == orderId).Sum(o => o.quantity * o.list_price * (1 - o.discount));

            if (ot == null)
            {
                return NotFound();
            }

            return Ok(ot);
        }

        #endregion

        #region PUT: api/orderitems/{orderId}

        // PUT: api/orderitems/5
        [Route("api/orderitems/{orderId}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putorder_items(int orderId, int itemId, order_items updatedItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingItem = db.order_items
                .FirstOrDefault(o => o.order_id == orderId && o.item_id == itemId);
            
            if(existingItem != null)
            {
                // Update the fields if the item exists
                existingItem.list_price = updatedItem.list_price;
                existingItem.quantity = updatedItem.quantity;
                existingItem.discount = updatedItem.discount;
                existingItem.product_id = updatedItem.product_id;
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
                if (!order_itemsExists(orderId))
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

        #region POST: api/orderitems

        // POST: api/orderitems
        [ResponseType(typeof(order_items))]
        [HttpPost]
        [Route("api/orderitems")]
        public IHttpActionResult Postorder_items(order_items order_item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if the entity already exists
                //if (order_itemsExists(order_item.order_id))
                //{
                //    var errorResponse = new
                //    {
                //        timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                //        message = "Conflict: An item with this ID already exists."
                //    };

                //    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                //}
                db.order_items.Add(order_item);
                db.SaveChanges();

                // Return Created response with the URI of the newly created resource
                //return CreatedAtRoute("DefaultApi", new { id = order_item.order_id }, order_item);
            }
            catch (DbUpdateException)
            {

                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "An error occurred while processing your request. The entity might already exist or there was a database issue."
                };
                if (order_itemsExists(order_item.order_id))
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
            return Ok(order_item);
            // return CreatedAtRoute("DefaultApi", new { id = order_item.order_id }, order_item);
        }

        #endregion

        #region DELETE: api/orderitems/{orderId}/{itemId}

        // DELETE: api/orderitems/5
        [ResponseType(typeof(order_items))]
        [Route("api/orderitems/{orderId}/{itemId}")]
        public IHttpActionResult Deleteorder_items(int orderId, int itemId)
        {
            order_items order_items = db.order_items.FirstOrDefault(o => o.order_id == orderId && o.item_id == itemId);
            if (order_items == null)
            {
                return NotFound();
            }

            db.order_items.Remove(order_items);
            db.SaveChanges();

            return Ok(order_items);
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

        private bool order_itemsExists(int id)
        {
            return db.order_items.Count(e => e.order_id == id) > 0;
        }
    }
}