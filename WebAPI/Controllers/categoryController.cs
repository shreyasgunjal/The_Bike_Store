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
    public class categoryController : ApiController
    {         
        public BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        #region GET: api/category

        // GET: api/category
        public IQueryable<category> Getcategories()
        {
            return db.categories;
        }

        #endregion

        #region GET: api/category/5

        // GET: api/category/5
        [ResponseType(typeof(category))]
        [Route("api/category/catId")]
        public IHttpActionResult Getcategory(int catId)
        {
            category category = db.categories.Find(catId);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        #endregion

        #region POST: api/category

        // POST: api/category
        [ResponseType(typeof(category))]
        public IHttpActionResult Postcategory(category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoryExists(category.category_name))  // Adjust condition based on your entity
            {
                var errorResponse = new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = "Validation Failed!!!"
                };
                return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
            }
            else
            {
                db.categories.Add(category);
            }


            try
            {

                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }


            //return CreatedAtRoute("DefaultApi", new { id = product.product_id }, product);
            return Ok("Record Added Successfully!!");
        }

        #endregion

        #region Put/api/category/edit/{id}
        // PUT: api/brand/5
        [HttpPut]
        [Route("api/category/edit/id")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putcategory(int catId, category catObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (catId != catObj.category_id)
            {
                return BadRequest();
            }

            db.Entry(catObj).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!categoryExists(catId))
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

        #region DELETE: api/category/5
        // DELETE: api/category/5
        [ResponseType(typeof(category))]
        public IHttpActionResult Deletecategory(int catId)
        {
            category category = db.categories.Find(catId);
            if (category == null)
            {
                return NotFound();
            }

            db.categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
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

        private bool categoryExists(int id)
        {
            return db.categories.Count(e => e.category_id == id) > 0;
        }

        private bool categoryExists(string catName)
        {
            return db.categories.Any(x => x.category_name.Contains(catName));
        }
    }
}