using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class staffController : ApiController
    {
        public BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        //public BikeStores_Team3Entities Db { get; set; }

        // GET: api/staff
        #region GET api/staff/
        [HttpGet]
        public IEnumerable<staff> Getstaffs()
        {
            return db.staffs.ToList();
        }
        #endregion

        // GET: api/staff/5

        #region GET api/staff/id
        [ResponseType(typeof(staff))]
        [Route("api/staff/edit/id")]
        public IHttpActionResult GetStaffById(int staffId)
        {
            staff staff = db.staffs.Find(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }
        #endregion

        //GET: api/staff/5
        #region GET api/staff/managerdetails/{staffid}/
        [HttpGet]
        [Route("api/staff/managerdetails/staffid")]
        [ResponseType(typeof(staff))]
        public IHttpActionResult GetManagerByStaffId(int staffId)
        {
            staff staff = db.staffs.Find(staffId);
            if (staff == null)
            {
                return NotFound();
            }
            if (staff.manager_id.HasValue)
            {
                var manager = db.staffs.Find(staff.manager_id.Value);
                if (manager != null)
                {
                    return Ok(manager);
                }
                // return Ok(manager);
            }
            return NotFound();
        }
        #endregion


        //GET: api/staff/5
        #region GET api/staff/salesmadebystaff/{staffid}/
        [HttpGet]
        [Route("api/staff/salesmadebystaff/staffid")]
        [ResponseType(typeof(staff))]
        public IHttpActionResult GetOrdersByStaffId(int staffId)
        {
            var orders = db.orders
                           .Where(x => x.staff_id == staffId).Join(db.customers, o => o.customer_id, c => c.customer_id, (o, c) => new
                           {
                               OrderId = o.order_id,
                               FirstName = c.first_name,
                               LastName = c.last_name
                           }).ToList();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);

        }
        #endregion

        #region GET api/staff/storename/{storename}/
        [HttpGet]
        [Route("api/staff/storename/")]
        [ResponseType(typeof(staff))]
        public IHttpActionResult GetstaffByName(string storeName)
        {
            var staffList = from s in db.staffs
                            join st in db.stores on s.store_id equals st.store_id
                            where st.store_name.Contains(storeName)
                            select new { s.staff_id, s.first_name, s.last_name, s.email, s.phone, s.active, s.store_id, s.manager_id };

            if (staffList == null || !staffList.Any())
            {
                return NotFound();
            }

            return Ok(staffList);
        }
        #endregion


        // PUT: api/staff/5
        #region PUT api/staff/edit/{staffid}/
        [HttpPut]
        [Route("api/staff/edit/staffid")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putstaff(int staffId, staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (staffId != staff.staff_id)
            {
                return BadRequest();
            }

            db.Entry(staff).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(staffId))
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


        // edit email and phone number

        #region PATCH api/staff/edit/{staffid}/
        [HttpPatch]
        [Route("api/staff/edit/id")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Patchstaff(int staffId, staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sta = db.staffs.Find(staffId);
            if (sta == null)
            {
                return NotFound();
            }

            // Only update the properties that are provided
            if (!string.IsNullOrEmpty(staff.email))
            {
                sta.email = staff.email;
            }

            if (!string.IsNullOrEmpty(staff.phone))
            {
                sta.phone = staff.phone;
            }

            try
            {
                db.Entry(sta).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(staffId))
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
        private bool StaffExists(int staffId)
        {
            return db.staffs.Any(e => e.staff_id == staffId);
        }

        // POST: api/staff
        #region POST api/staff
        [HttpPost]
        [ResponseType(typeof(staff))]
        public IHttpActionResult Poststaff(staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var storeExists = db.stores.Any(s => s.store_id == staff.store_id);

            if (!storeExists)
            {
                return BadRequest("The store_id provided does not exist in the stores table.");
            }

            try
            {
                db.staffs.Add(staff);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Return a Conflict status code if the store_id is known to exist but an update conflict occurred
                var errorResponse = new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = "Validation Failed"
                };
                if (StaffExists(staff.store_id))
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                else
                {
                    // Rethrow the exception if it's not a known conflict
                    var detailedErrorResponse = new
                    {
                        TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        Message = "Validation Failed",
                    };
                    return Content(HttpStatusCode.InternalServerError, detailedErrorResponse); // HTTP 500 Internal Server Error
                }
            }
            // return CreatedAtRoute("DefaultApi", new { id = staff.staff_id }, staff);
            return Ok("Record Created Successfully");
        }
        #endregion

        // DELETE: api/staff/5
        #region DELETE api/staff/{staffid}/
        [ResponseType(typeof(staff))]
        public IHttpActionResult Deletestaff(int staffId)
        {
            staff staff = db.staffs.Find(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            db.staffs.Remove(staff);
            db.SaveChanges();

            return Ok(staff);
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

        //private bool staffExists(int id)
        //{
        //    return db.staffs.Count(e => e.staff_id == id) > 0;
        //}
    }
}