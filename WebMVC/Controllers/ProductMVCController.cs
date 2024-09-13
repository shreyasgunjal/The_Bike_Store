using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class ProductMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";
        string baseURL1 = "https://localhost:44352/";
        // GET: ProductMVC
        //public ActionResult Index()
        //{
        //    IEnumerable<Product> prodList = null;
        //    using (var client = new HttpClient())
        //    {
        //        // Url of Webapi project
        //        client.BaseAddress = new Uri(baseURL);
        //        //HTTP GET
        //        var responseTask = client.GetAsync("products");  // Categories is the WebApi controller name
        //        // wait for task to complete
        //        responseTask.Wait();
        //        // retrieve the result
        //        var result = responseTask.Result;
        //        // check the status code for success
        //        if (result.IsSuccessStatusCode)
        //        {
        //            // read the result
        //            var readTask = result.Content.ReadAsAsync<IList<Product>>();
        //            readTask.Wait();
        //            // fill the list vairable created above with the returned result
        //            prodList = readTask.Result;
        //        }
        //        else //web api sent error response 
        //        {
        //            prodList = Enumerable.Empty<Product>();
        //            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
        //        }
        //    }
        //    return View(prodList);
        //}


        #region INDEX

        // GET: CustomerMVC
        [Authorize(Roles = "Admin,Store,Staff")]
        public ActionResult Index(string searchTerm, string catName, string modelYear, int? custId, string brandName, string storeId, int? filterOption)
        {
            IEnumerable<Product> prodList = Enumerable.Empty<Product>();
            var message = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                HttpResponseMessage response;

                try
                {

                    if (filterOption.HasValue)
                    {
                        switch (filterOption.Value)
                        {
                            case 1: // By Category Name
                                if (!string.IsNullOrEmpty(catName))
                                {
                                    response = client.GetAsync($"products/bycategoryname/categoryname?categoryName={catName}").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        prodList = response.Content.ReadAsAsync<IList<Product>>().Result;
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No details exist.";
                                        prodList = Enumerable.Empty<Product>();
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                    }
                                }
                                else
                                {
                                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                }
                                break;
                            case 2: // By Model Year
                                if (!string.IsNullOrEmpty(modelYear))
                                {
                                    response = client.GetAsync($"products/bymodelyear/modelyear?modelYear={modelYear}").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        prodList = response.Content.ReadAsAsync<IList<Product>>().Result;
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No details exist.";
                                        prodList = Enumerable.Empty<Product>();
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                    }
                                }
                                else
                                {
                                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                }
                                break;

                            case 3: // By BrandName
                                if (!string.IsNullOrEmpty(brandName))
                                {
                                    response = client.GetAsync($"products/bybrandname/bybrandname?brandName={brandName}").Result;

                                    if (response.IsSuccessStatusCode)
                                    {
                                        prodList = response.Content.ReadAsAsync<IList<Product>>().Result;
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No details exist.";
                                        prodList = Enumerable.Empty<Product>();
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                    }
                                }
                                else
                                {
                                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                }
                                break;


                            case 4: // products based on custID
                                if (custId.HasValue)
                                {
                                    response = client.GetAsync($"products/purchasedbycustomer/customerid?customerid={custId.Value}").Result; ;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var salesByStaff = response.Content.ReadAsAsync<IList<PurchaseByCustId>>().Result;
                                        ViewBag.PurchaseByCustId = salesByStaff;
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No sales data found.";
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                    }
                                }
                                else
                                {
                                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                }
                                break;

                            case 5: // Products sale by each store
                                response = client.GetAsync($"products/NumberOfProductsSoldByEachStore").Result; ;
                                if (response.IsSuccessStatusCode)
                                {
                                    var productsByStores = response.Content.ReadAsAsync<IList<ProductsByStore>>().Result;
                                    ViewBag.ProductsByStore = productsByStores;
                                }
                                else if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    message = "No sales data found.";
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                }
                                break;

                            case 6:  // Products purchased by maximum customer
                                response = client.GetAsync($"products/ProductPurchasedByMaxiumCustomer").Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var prodDetails = response.Content.ReadAsAsync<BestSeller>().Result;

                                    if (prodDetails != null)
                                    {
                                        ViewBag.BestSeller = new List<BestSeller> { prodDetails }; // Wrap in a list
                                    }
                                    else
                                    {
                                        ViewBag.BestSeller = Enumerable.Empty<BestSeller>(); // Empty collection
                                    }
                                }
                                else if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    message = "No sales data found.";
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                }
                                break;

                            case 7:  // product with min stock
                                client.BaseAddress = new Uri(baseURL1);
                                response = client.GetAsync($"stock/GetProductWithMiniumStock").Result; ;
                                if (response.IsSuccessStatusCode)
                                {


                                    var productsByStock = response.Content.ReadAsAsync<IList<ProductsWithMinStock>>().Result;


                                    ViewBag.ProductsWithMinStock = productsByStock;
                                }
                                else if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    message = "No sales data found.";
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                }
                                break;

                            default:
                                response = client.GetAsync("products").Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    prodList = response.Content.ReadAsAsync<IList<Product>>().Result;
                                }
                                //else 
                                if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    message = "No details exist.";
                                    prodList = Enumerable.Empty<Product>();
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                }
                                break;
                        }

                    }
                    else
                    {
                        response = client.GetAsync("products").Result;
                        //if (response.IsSuccessStatusCode)
                        //{
                        prodList = response.Content.ReadAsAsync<IList<Product>>().Result;
                        //}
                        //else if (response.StatusCode == HttpStatusCode.NotFound)
                        //{
                        //    message = "No details exist.";
                        //    prodList = Enumerable.Empty<Product>();
                    }
                    //}
                }


                catch (Exception ex)
                {
                    // Log the exception details for debugging
                    // For example: Log.Error(ex, "Error occurred while processing the request");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                }

            }

            ViewBag.Message = message;
            return View(prodList);
        }

        #endregion




        // GET: ProductMVC/Details/5
        [Authorize(Roles = "Admin,Store,Staff")]
        public ActionResult Details(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Product prodObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("products/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    prodObj = readTask.Result;
                }
            }
            return View(prodObj);
        }



        // GET: ProductMVC/Create
        public ActionResult Create()
        {
            return View();
        }



        // POST: ProductMVC/Create
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Create(Product prodObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Product>("products", prodObj);
                // wait for task to complete
                postTask.Wait();
                // retrieve the result
                var result = postTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            // Add model error
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error
            return View(prodObj);
        }

        // GET: ProductMVC/Edit/5
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Product prodObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("products/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    prodObj = readTask.Result;
                }
            }
            return View(prodObj);
        }

        // POST: ProductMVC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Product prodObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Product>($"products/edit/productid?prodId={id}", prodObj);
                // wait for task to complete
                putTask.Wait();
                // retrieve the result
                var result = putTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // Return to Index
                    return RedirectToAction("Index");
                }
            }
            // Add model error
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error 
            return View(prodObj);
        }

        // GET: ProductMVC/Delete/5
        public ActionResult Delete(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Product prodObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("products/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    prodObj = readTask.Result;
                }
            }
            return View(prodObj);
        }

        // POST: ProductMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Product prodObj)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("products?prodId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var deleteTask = responseTask.Result;
                // check the status code for success
                if (deleteTask.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
