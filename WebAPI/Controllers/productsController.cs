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
    public class productsController : ApiController
    {
        private BikeStores_Team3Entities db = new BikeStores_Team3Entities();

        #region GET: api/products

        // GET: api/products
        public IQueryable<product> Getproducts()
        {
            return db.products;
        }

        #endregion

        #region GET: api/products/5

        // GET: api/products/5
        [ResponseType(typeof(product))]
        [Route("api/products/{prodId}")]
        public IHttpActionResult Getproduct(int prodId)
        {
            product product = db.products.Find(prodId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        #endregion

        #region GET: api/products/bycategoryname/{categoryname}

        // GET: api/products/bycategoryname/categoryname
        [Route("api/products/bycategoryname/categoryname")]
        [ResponseType(typeof(product))]
        public IHttpActionResult GetproductbyCategoryName(string categoryName)
        {
            // Find the category with the given name
            var fetchedcategory = db.categories.FirstOrDefault(c => c.category_name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (fetchedcategory == null)
            {
                return NotFound();
            }

            // Retrieve products associated with the found category
            var product = db.products.Where(c => c.category_id==fetchedcategory.category_id).ToList();

            if (!product.Any())
            {
                return NotFound();
            }
            return Ok(product);
        }

        #endregion

        #region GET: api/products/bybrandname/{bybrandname}

        //GET: api/products/bybrandname/bybrandname
        [Route("api/products/bybrandname/bybrandname")]
        [ResponseType(typeof(product))]
        public IHttpActionResult GetproductbyBrandName(string brandName)
        {
            // Find the category with the given name
            var fetchedbrand = db.brands.FirstOrDefault(c => c.brand_name.Equals(brandName, StringComparison.OrdinalIgnoreCase));
            if (fetchedbrand == null)
            {
                return NotFound();
            }

            // Retrieve products associated with the found category
            var product = db.products.Where(c => c.brand_id == fetchedbrand.brand_id).ToList();

            if (!product.Any())
            {
                return NotFound();
            }
            return Ok(product);
        }


        #endregion

        #region GET: api/products/bymodelyear/{modelyear}

        // GET: api/products/bymodelyear/modelyear
        [Route("api/products/bymodelyear/modelyear")]
        [ResponseType(typeof(product))]
        public IHttpActionResult GetproductbyModelYear(short modelYear)
        {
            var fetchedprod = db.products.Where(c=>c.model_year==modelYear);
            if (!fetchedprod.Any())
            {
                return NotFound();
            }
            return Ok(fetchedprod);
        }

        #endregion

        #region GET: api/products/ProductDetails

        // GET: api/products/ProductDetails
        [HttpGet]
        [Route("api/products/ProductDetails")]
        [ResponseType(typeof(IQueryable<object>))]
        public IHttpActionResult GetProductDetails()
        {
            var productDetails = from p in db.products
                                 join c in db.categories on p.category_id equals c.category_id
                                 join b in db.brands on p.brand_id equals b.brand_id
                                 select new
                                 {
                                     CategoryName = c.category_name,
                                     ProductName = p.product_name,
                                     BrandName = b.brand_name
                                 };

            var productDetailsList = productDetails.ToList();

            if (!productDetailsList.Any())
            {
                return NotFound();
            }

            return Ok(productDetailsList);
        }

        #endregion

        #region GET: api/products/purchasedbycustomer/{customerid}

        // GET: api/products/purchasedbycustomer/customerid
        [HttpGet]
        [Route("api/products/purchasedbycustomer/customerid")]
        [ResponseType(typeof(IQueryable<object>))]
        public IHttpActionResult GetProductsPurchasedByCustomer(int customerid)
        {
            var productDetails = from o in db.orders
                                 join od in db.order_items on o.order_id equals od.order_id
                                 join p in db.products on od.product_id equals p.product_id
                                 join c in db.categories on p.category_id equals c.category_id
                                 join b in db.brands on p.brand_id equals b.brand_id
                                 where o.customer_id == customerid
                                 select new
                                 {
                                     ProductName = p.product_name,
                                     CategoryName = c.category_name,
                                     BrandName = b.brand_name,
                                     Quantity = od.quantity
                                 };

            var productDetailsList = productDetails.ToList();

            if (!productDetailsList.Any())
            {
                return NotFound();
            }

            return Ok(productDetailsList);
        }

        #endregion

        #region GET: api/products/NumberOfProductsSoldByEachStore

        [HttpGet]
        [Route("api/products/NumberOfProductsSoldByEachStore/")]
        [ResponseType(typeof(product))]
        public IHttpActionResult NumberOfProductsSoldByEachStore()
        {
            var productsSoldByEachStore = from o in db.orders
                                          join oi in db.order_items on o.order_id equals oi.order_id
                                          join s in db.stores on o.store_id equals s.store_id
                                          join p in db.products on oi.product_id equals p.product_id
                                          group oi by new { s.store_id, s.store_name } into g
                                          select new
                                          {
                                              StoreId = g.Key.store_id,
                                              StoreName = g.Key.store_name,
                                              NumberOfProductsSold = g.Count()
                                          };

            var result = productsSoldByEachStore.ToList();

            if (!result.Any())
            {
                return NotFound(); // No products sold by any store
            }

            return Ok(result);
        }

        #endregion

        #region GET: api/products/ProductPurchasedByMaxiumCustomer

        [HttpGet]
        [Route("api/products/ProductPurchasedByMaxiumCustomer")]
        [ResponseType(typeof(product))]
        public IHttpActionResult ProductPurchasedByMaxiumCustomer()
        {
            var productPurchasedByMaxCustomer = from o in db.orders
                                                join oi in db.order_items on o.order_id equals oi.order_id
                                                join p in db.products on oi.product_id equals p.product_id
                                                group new { p, o.customer_id } by new { p.product_id, p.product_name } into g
                                                let customerCount = g.Select(x => x.customer_id).Distinct().Count()
                                                orderby customerCount descending
                                                select new
                                                {
                                                    ProductId = g.Key.product_id,
                                                    ProductName = g.Key.product_name,
                                                    CustomerCount = customerCount
                                                };

            // Get the product with the maximum number of unique customers
            var result = productPurchasedByMaxCustomer.FirstOrDefault();

            if (result == null)
            {
                return NotFound(); // No products found
            }

            return Ok(result);
        }

        #endregion

        #region GET: api/stock/GetProductWithMiniumStock 

        [HttpGet]
        [Route("stock/GetProductWithMiniumStock")]
        [ResponseType(typeof(product))]
        public IHttpActionResult GetProductWithMiniumStock()
        {
            var productsWithMinStock = from s in db.stocks
                                       join p in db.products on s.product_id equals p.product_id
                                       group s by new { s.product_id, p.product_name } into g
                                       select new
                                       {
                                           ProductId = g.Key.product_id,
                                           ProductName = g.Key.product_name,
                                           MinimumStock = g.Min(s => s.quantity)
                                       };

            var result = productsWithMinStock.ToList()
                                     .OrderBy(p => p.MinimumStock)
                                     .ToList();


            if (!result.Any())
            {
                return NotFound(); // No stocks found for any product
            }

            return Ok(result);
        }

        #endregion



        #region PUT: api/products/edit/{productid} 

        // PUT: api/products/edit/productid 
        [Route("api/products/edit/productid")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putproduct(int prodId, product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (prodId != product.product_id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!productExists(prodId))
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

        #region POST: api/products

        // POST: api/products
        [ResponseType(typeof(product))]
        public IHttpActionResult Postproduct(product product)
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
                    message = "Validation Failed!!!"
                };

                // Check if the entity already exists or other specific constraints
                if (productExists(product.product_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                else
                {
                    db.products.Add(product);
                    db.SaveChanges();
                }
                
            }
            catch (DbUpdateException)
            {
                throw;
            }

            //return CreatedAtRoute("DefaultApi", new { id = product.product_id }, product);
            return Ok("Record Added Successfully!!");
        }

        #endregion

        #region PATCH: /api/products/edit/{productid}

        //PATCH: /api/products/editproductid
        [Route("api/products/edit/productid")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Patchproduct(int prodid, product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var product1 = db.products.Find(prodid);
            if (product1 == null)
            {
                return NotFound();
            }

            // Apply updates
            if (!string.IsNullOrEmpty(product.product_name))
            {
                product1.product_name = product.product_name;
            }

            if (product.list_price != 0)
            {
                product1.list_price = product.list_price;
            }

            db.Entry(product1).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!productExists(prodid))
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

        #region DELETE: api/products/5

        // DELETE: api/products/5
        [ResponseType(typeof(product))]
        public IHttpActionResult Deleteproduct(int prodId)
        {
            product product = db.products.Find(prodId);
            if (product == null)
            {
                return NotFound();
            }

            db.products.Remove(product);
            db.SaveChanges();

            return Ok(product);
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

        private bool productExists(int id)
        {
            return db.products.Count(e => e.product_id == id) > 0;
        }
    }
}