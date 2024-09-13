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
    public class customersController : ApiController
    {
        private BikeStores_Team3Entities db = new BikeStores_Team3Entities();


        #region GET: api/customers

        // GET: api/customers
        public IQueryable<customer> Getcustomers()
        {
            return db.customers;
        }

        #endregion 

        #region GET: api/customers/bycity/{city}

        // GET: api/customers/bycity/city
        [HttpGet]
        [Route("api/customers/bycity/city")]
        public IHttpActionResult GetCustomersByCity(string city)
        {
            var customers = db.customers
                .Where(c => c.city.ToLower() == city.ToLower())
                .OrderBy(c => c.first_name)
                .ThenBy(c => c.last_name)
                .ToList();

            if (!customers.Any())
            {
                return NotFound();
            }

            return Ok(customers);
        }

        #endregion

        #region GET: api/customers/5

        // GET: api/customers/5
        [Route("api/customers/{custId}")]
        [ResponseType(typeof(customer))]
        public IHttpActionResult Getcustomer(int custId)
        {
            customer customer = db.customers.Find(custId);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        #endregion

        #region GET: api/customers/placeorderondate/{orderdate}

        // GET: api/customers/placeorderondate/orderdate
        [HttpGet]
        [Route("api/customers/placeorderondate/orderdate")]
        public IHttpActionResult GetCustomersByOrderDate(DateTime orderDate)
        {
            var customers = db.customers
                            .Join(db.orders, c => c.customer_id, o => o.customer_id, (c, o) => new { Customer = c, Order = o })
                            .Where(x => x.Order.order_date == orderDate)
                            .Select(x => x.Customer)
                            .Distinct()
                            .OrderBy(c => c.first_name)
                            .ThenBy(c => c.last_name)
                            .ToList();

            if (!customers.Any())
            {
                return NotFound();
            }

            return Ok(customers);
        }

        #endregion

        #region GET: api/customers/placedhighestorder

        // GET: api/customers/placedhighestorder
        [HttpGet]
        [Route("api/customers/placedhighestorder")]
        public IHttpActionResult GetCustomerWithHighestOrder()
        {
            var maxOrderDate = db.orders.Max(o => o.order_date);
            var customers = db.orders
                .Where(o => o.order_date == maxOrderDate)
                .Join(db.customers, o => o.customer_id, c => c.customer_id, (o, c) => c)
                .Distinct()
                .ToList();

            if (customers.Count == 0)
            {
                return NotFound();
            }
            return Ok(customers);
        }

        #endregion

        #region GET: api/customers/zipcode/{zipcode}

        // GET: api/customers/zipcode/zipcode
        [HttpGet]
        [Route("api/customers/zipcode/zipcode")]
        public IHttpActionResult GetCustomersByZipCode(string zipcode)
        {
            var customers = db.customers
                .Where(c => c.zip_code == zipcode)
                .OrderBy(c => c.first_name)
                .ThenBy(c => c.last_name)
                .ToList();

            if (!customers.Any())
            {
                return NotFound();
            }
            return Ok(customers);
        }

        #endregion

        #region PUT: api/customers/edit/{customerid}

        // PUT: api/customers/edit/customerid
        [ResponseType(typeof(void))]
        [Route("api/customers/edit/{custId}")]
        public IHttpActionResult Putcustomer(int custId, customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (custId != customer.customer_id)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(custId))
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

        #region POST: api/customers

        // POST: api/customers
        [ResponseType(typeof(customer))]
        public IHttpActionResult Postcustomer(customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                bool emailExists = db.customers.Any(c => c.email == customer.email);
                bool phoneExists = db.customers.Any(c => c.phone == customer.phone);

                if (emailExists || phoneExists)
                {
                //    if (CustomerExists(customer.customer_id) ) // Adjust condition based on your entity
                //{
                            var errorResponse = new
                            {
                                TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                Message = "Validation Failed!!!"
                            };
                            return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }

                else
                {
                    db.customers.Add(customer);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw;
            }


            //return CreatedAtRoute("DefaultApi", new { id = product.product_id }, product);
            return Ok("Record Added Successfully!!");
        }

        #endregion

        #region PATCH: api/customers/edit/{customerid}

        // PATCH: api/customers/edit/customerid
        [HttpPatch]
        [Route("api/customers/edit/customerid")]
        public IHttpActionResult PatchCustomer(int customerid, customer updateCustomer)
        {
            if (updateCustomer == null)
            {
                return BadRequest();
            }

            var customer = db.customers.Find(customerid);
            if (customer == null)
            {
                return NotFound();
            }

            // Apply updates
            if (!string.IsNullOrEmpty(updateCustomer.street))
            {
                customer.street = updateCustomer.street;
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent); // 204 No Content
        }

        #endregion

        #region DELETE: api/customers/5

        // DELETE: api/customers/5
        [ResponseType(typeof(customer))]
        public IHttpActionResult Deletecustomer(int custId)
        {
            customer customer = db.customers.Find(custId);
            if (customer == null)
            {
                return NotFound();
            }

            db.customers.Remove(customer);
            db.SaveChanges();

            return Ok(customer);
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

        //private bool customerExists(int id)
        //{
        //    return db.customers.Count(e => e.customer_id == id) > 0;
        //}

        private bool CustomerExists(int id)
        {
            return db.customers.Any(e => e.customer_id == id);
        }
    }
}