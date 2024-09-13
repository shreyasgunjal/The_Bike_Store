using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ordersController : ApiController
    {
        public BikeStores_Team3Entities db = new BikeStores_Team3Entities();
        #region  GET: api/orders
        // GET: api/orders
        [Route("api/orders")]
        public IQueryable<order> Getorders()
        {
            return db.orders;
        }
        #endregion

        #region GET: api/orders/5

        // GET: api/orders/5
        [Route("api/orders/{orderId}")]
        [ResponseType(typeof(order))]
        public IHttpActionResult Getorder(int orderID)
        {
            order orderObj = db.orders.Find(orderID);
            if (orderObj == null)
            {
                return NotFound();
            }

            return Ok(orderObj);
        }

        #endregion

        #region PUT: api/orders/edit/{orderid}
        // PUT: api/orders/5
        [ResponseType(typeof(void))]
        [Route("api/orders/edit/{orderid}")]
        public IHttpActionResult Putorder(int orderid, order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderid != order.order_id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!orderExists(orderid))
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

        #region POST: api/orders
        // POST: api/orders
        [ResponseType(typeof(order))]
        [Route("api/orders")]
        public IHttpActionResult Postorder(order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            try
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed!!!"
                };

                if (orderExists(order.order_id)) // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                else
                {
                    db.orders.Add(order);
                    db.SaveChanges();
                }
                
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            // Ensure the route name matches the one defined in your WebApiConfig
            //return CreatedAtRoute("DefaultApi", new { id = order.order_id }, order);
            return Ok(order);
        }

        #endregion

        #region DELETE: api/orders/5
        // DELETE: api/orders/5
        [ResponseType(typeof(order))]
        [Route("api/orders/delete/{orderId}")]
        public IHttpActionResult Deleteorder(int orderId)
        {
            order order = db.orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            db.orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }
        #endregion

        #region PATCH: api/orders/edit/{orderid}
        //PATCH: api/orders/edit/{orderid}
        [ResponseType(typeof(order))]
        [Route("api/orders/edit/{orderid}")]
        public IHttpActionResult PatchOrder(int orderid, order updatedOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = db.orders.Find(orderid);

            if (order == null)
            {
                return NotFound();
            }

            if (orderid != updatedOrder.order_id)
            {
                return BadRequest("Order ID mismatch.");
            }

            // Update the properties of the existing order entity
            order.order_status = updatedOrder.order_status;
            // Update other properties as needed

            // No need to set the state to Modified; Entity Framework will detect the changes
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!orderExists(orderid))
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

        #region GET: api/orders/{customerid}

        [ResponseType(typeof(order))]
        [Route("api/orders/customerid/{customerid}")]
        public IHttpActionResult GetAllOrderByCustomerId(int customerid) {
            List<order> custOrderList = db.orders.Where(o=>o.customer_id == customerid).ToList();
            if (custOrderList == null)
            {
                return NotFound();
            }

            return Ok(custOrderList);
        }

        #endregion

        #region GET: api/orders/customername/{customername}

        [ResponseType(typeof(order))]
        [Route("api/orders/customername/{customername}")]
        public IHttpActionResult GetAllOrderByCustomerId(string customername)
        {
            List<order> custOrderList = db.customers
                .Where(c => c.first_name == customername)
                .Join(db.orders, c => c.customer_id, o => o.customer_id, (c, o) => o)
                .ToList();
            if (custOrderList == null)
            {
                return NotFound();
            }

            return Ok(custOrderList);
        }
        #endregion

        #region GET: api/orders/orderdate/{orderdate}
        // GET: api/orders/orderdate/{orderdate}
        [ResponseType(typeof(IEnumerable<order>))]
        [Route("api/orders/orderdate/{orderdate}")]
        public IHttpActionResult GetOrdersByOrderDate(String orderdate)
        {
            DateTime parsedDate;
            // Attempt to parse the date string in the format YYYY-MM-DD
            if (!DateTime.TryParseExact(orderdate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            }

            var orders = db.orders
                            .Where(o => DbFunctions.TruncateTime(o.order_date) == parsedDate.Date)
                            .ToList();
            if (orders.Count == 0)
            {
                return NotFound();
            }

            return Ok(orders);
        }
        #endregion

        #region GET: api/orders/status/{status}
        // GET: api/orders/status/{status}
        [ResponseType(typeof(IEnumerable<order>))]
        [Route("api/orders/status/{status}")]
        public IHttpActionResult GetOrdersByStatus(int status)
        {
            var orders = db.orders.Where(o => o.order_status == status).ToList();
            if (orders.Count == 0)
            {
                return NotFound();
            }

            return Ok(orders);
        }
        #endregion

        #region GET: api/orders/numberoforderbydate
        // GET: api/orders/numberoforderbydate
        [Route("api/orders/numberoforderbydate")]
        public IHttpActionResult GetNumberOfOrdersByDate()
        {
            var orderCounts = db.orders
        .AsEnumerable() // Switch to client-side processing
        .GroupBy(o => o.order_date.Date) // Group by date
        .Select(g => new
        {
            OrderDate = g.Key,
            NumberOfOrders = g.Count()
        })
        .OrderBy(x => x.OrderDate)
        .ToList();

            return Ok(orderCounts);
        }
        #endregion

        #region GET: api/orders/maximumorderplaceonparticulardate
        // GET: api/orders/maximumorderplaceonparticulardate
        [Route("api/orders/maximumorderplaceonparticulardate")]
        public IHttpActionResult GetDateWithMaximumOrders()
        {
            var maxOrderDate = db.orders
        .AsEnumerable() // Switch to client-side processing
        .GroupBy(o => o.order_date.Date) // Group by date
        .Select(g => new
        {
            OrderDate = g.Key,
            NumberOfOrders = g.Count()
        })
        .OrderByDescending(x => x.NumberOfOrders)
        .FirstOrDefault();

            if (maxOrderDate == null)
            {
                return NotFound();
            }

            return Ok(maxOrderDate.OrderDate);
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

        private bool orderExists(int id)
        {
            return db.orders.Count(e => e.order_id == id) > 0;
        }
    }
}