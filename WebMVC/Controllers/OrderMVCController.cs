using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class OrderMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";
        #region Index
        // GET: OrderMVC
        [Authorize(Roles = "Admin,Store")]
        public ActionResult Index(string searchTerm, int? customerid, string customername, int? status, DateTime? orderdate, int? filterOption)
        
        {
            IEnumerable<Order> orderList = null;
            //IEnumerable<NumberOfOrderByOrderdate> numberOfOrderByDateList = null;
            var message = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                HttpResponseMessage response;

                switch (filterOption)
                {
                    case 1: // By Customer Id
                        if (!(customerid == 0))
                        {
                            response = client.GetAsync($"orders/customerid/{customerid}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 2: // By Customer Name
                        if (!string.IsNullOrEmpty(customername))
                        {
                            response = client.GetAsync($"orders/customername/{customername}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 3: // By Order ate
                        if (orderdate.HasValue)
                        {
                            response = client.GetAsync($"orders/orderdate/{orderdate.Value.ToString("yyyy-MM-dd")}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 4: // By Status
                        if (status > 0 && status < 5)
                        {
                            response = client.GetAsync($"orders/status/{status}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;
                    case 5:
                        { // By Number Of Orders By Date
                            response = client.GetAsync("orders/numberoforderbydate").Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsAsync<IList<NumberOfOrderByOrderdate>>();
                                readTask.Wait();
                                ViewBag.NoOfOrderByOrderDate = readTask.Result;
                            }
                            else if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                message = "No details exist.";
                                orderList = Enumerable.Empty<Order>();
                            }
                            else
                            {
                                orderList = Enumerable.Empty<Order>();
                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            }
                            break;
                        }
                    case 6: // By Maximum Order Place On Particular Date
                        response = client.GetAsync("orders/maximumorderplaceonparticulardate").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var readTask = response.Content.ReadAsAsync<string>();
                            readTask.Wait();
                            ViewBag.MaximumOrder = readTask.Result;
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            message = "No details exist.";
                            orderList = Enumerable.Empty<Order>();
                        }
                        else
                        {
                            orderList = Enumerable.Empty<Order>();
                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                        break;
                    default:
                        response = client.GetAsync("orders").Result;
                        break;
                }

                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    orderList = readTask.Result;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    message = "No details exist.";
                    orderList = Enumerable.Empty<Order>();
                }
                else
                {
                    orderList = Enumerable.Empty<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.Message = message;
            return View(orderList);

        }

        #endregion

        #region Details
        // GET: OrderMVC/Details/5
        [Authorize(Roles = "Admin,Store")]
        public ActionResult Details(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Order orderObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("orders/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    orderObj = readTask.Result;
                }
            }
            return View(orderObj);
        }

        #endregion

        #region Create GET
        // GET: OrderMVC/Create
        [Authorize(Roles ="Staff,Store,Admin")]
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Create POST
        // POST: OrderMVC/Create
        [Authorize(Roles = "Staff,Store,Admin")]
        [HttpPost]
        public ActionResult Create(Order orderObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Order>("orders", orderObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {orderObj.Order_id} already exists. Please contact administrator.");
            // send the view back with model error
            return View(orderObj);
        }
        #endregion

        #region Edit GET
        // GET: OrderMVC/Edit/5
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Order orderObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("orders/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    orderObj = readTask.Result;
                }
            }
            return View(orderObj);
        }
        #endregion

        #region Edit POST
        // POST: OrderMVC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Order obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Order>($"orders/edit/{id}", obj);
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
            return View(obj);
        }
        #endregion

        #region Delete GET
        // GET: OrderMVC/Delete/5
        public ActionResult Delete(int id)
        {
            Order orderObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("orders/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    orderObj = readTask.Result;
                }
            }
            return View(orderObj);
        }
        #endregion

        #region Delete POST
        // POST: OrderMVC/Delete/5
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
                var responseTask = client.DeleteAsync("orders/delete/" + id.ToString());
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
        #endregion
    }
}
