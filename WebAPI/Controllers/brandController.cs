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
    public class brandController : ApiController
    {
        public BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        // GET: api/brand
        #region GET /api/brand
        public IQueryable<brand> Getbrands()
        {
            return db.brands;
        }
        #endregion

        // GET: api/brand/5
        #region GET api/brand/id
        [Route("api/brand/id")]
        [ResponseType(typeof(brand))]
        public IHttpActionResult Getbrand(int brandId)
        {
            brand brand = db.brands.Find(brandId);
            if (brand == null)
            {
                return NotFound();
            }

            return Ok(brand);
        }
        #endregion

        // PUT: api/brand/5
        #region PUT /api/brand/edit/{id}
        [HttpPut]
        [Route("api/brand/edit/id")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putbrand(int brandId, brand brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (brandId != brand.brand_id)
            {
                return BadRequest();
            }

            db.Entry(brand).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!brandExists(brandId))
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

        // POST: api/brand
        #region POST api/brand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [ResponseType(typeof(brand))]
        public IHttpActionResult Postbrand(brand brand) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                
                if (!brandExists(brand.brand_name))
                {
                    db.brands.Add(brand);
                    db.SaveChanges();
                }
                else
                {
                    var errorResponse = new
                    {
                        timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        message = "Brand name already exists"
                    };
                    return Content(HttpStatusCode.Conflict, errorResponse); ;
                }
            }

            catch (DbUpdateException ex)

            {
                throw;
            
            }

            // return CreatedAtRoute("DefaultApi", new { id = brand.brand_id }, brand);
            return Ok("Record Added Successfully!!");
        }
        #endregion

        // DELETE: api/brand/5
        #region DELETE api/brand{brandid}
        [ResponseType(typeof(brand))]
        public IHttpActionResult Deletebrand(int brandId)
        {
            brand brand = db.brands.Find(brandId);
            if (brand == null)
            {
                return NotFound();
            }

            db.brands.Remove(brand);
            db.SaveChanges();

            return Ok(brand);
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

        private bool brandExists(string brandName)
        {
            //
            return db.brands.Any(x => x.brand_name.Contains(brandName));
        }

        private bool brandExists(int id)
        {
            return db.brands.Count(e => e.brand_id == id) > 0;
        }
    }
}