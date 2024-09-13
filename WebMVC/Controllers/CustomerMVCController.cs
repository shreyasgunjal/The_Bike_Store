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
    public class CustomerMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        #region INDEX

        // GET: CustomerMVC
        [Authorize(Roles = "Store")]
        public ActionResult Index(string searchTerm, string city, string zipcode, DateTime? orderdate, int? filterOption)
        {
            IEnumerable<Customer> custList = null;
            var message = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                HttpResponseMessage response;

                switch (filterOption)
                {
                    case 1: // By City
                        if (!string.IsNullOrEmpty(city))
                        {
                            response = client.GetAsync($"customers/bycity/city?city={city}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 2: // By Order Date
                        if (orderdate.HasValue)
                        {
                            response = client.GetAsync($"customers/placeorderondate/orderdate?orderDate={orderdate.Value.ToString("yyyy-MM-dd")}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 3: // By ZIP Code
                        if (!string.IsNullOrEmpty(zipcode))
                        {
                            response = client.GetAsync($"customers/zipcode/zipcode?zipcode={zipcode}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 4: // Highest Order
                        response = client.GetAsync("customers/placedhighestorder").Result;
                        break;
                    default:
                        response = client.GetAsync("customers").Result;
                        break;
                }

                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<IList<Customer>>();
                    readTask.Wait();
                    custList = readTask.Result;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    message = "No details exist.";
                    custList = Enumerable.Empty<Customer>();
                }
                else
                {
                    custList = Enumerable.Empty<Customer>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Message = message;
            return View(custList);
        }

        #endregion

        // GET: CustomerMVC/Details/5
        [Authorize(Roles = "Store")]
        public ActionResult Details(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Customer customObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("customers/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Customer>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    customObj = readTask.Result;
                }
            }
            return View(customObj);
            
        }

        // GET: CustomerMVC/Create
        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerMVC/Create
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Create(Customer customObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Customer>("customers", customObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {customObj.Customer_Id} already exists. Please contact administrator.");
            // send the view back with model error
            return View(customObj);
        }

        // GET: CustomerMVC/Edit/5
        [Authorize(Roles ="Staff")]
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Customer customObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("customers/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Customer>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    customObj = readTask.Result;
                }
            }
            return View(customObj);
        }

        // POST: CustomerMVC/Edit/5
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Edit(int id, Customer customObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Customer>($"customers/edit/{id}", customObj);
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
            return View(customObj);
        }

        // GET: CustomerMVC/Delete/5
        public ActionResult Delete(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Customer customObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("customers/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Customer>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    customObj = readTask.Result;
                }
            }
            return View(customObj);
        }

        // POST: CustomerMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("customers?custId=" + id.ToString());
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
