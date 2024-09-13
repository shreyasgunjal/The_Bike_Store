using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles ="Store")]
    public class Order_ItemsMVCController : Controller
    {
        // GET: Order_ItemsMVC
        string baseURL = "https://localhost:44352/api/";
        public ActionResult Index()
        {
            IEnumerable<Order_Items> order_itemList = null;
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("orderitems");  // Categories is the WebApi controller name

                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<IList<Order_Items>>();
                    readTask.Wait();
                    // fill the list vairable created above with the returned result
                    order_itemList = readTask.Result;
                }
                else //web api sent error response 
                {
                    order_itemList = Enumerable.Empty<Order_Items>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(order_itemList);
        }

        // GET: Order_ItemsMVC/Details/5
        public ActionResult Details(int orderId, int itemId)
        {
            // variable to hold the person details retrieved from WebApi
            Order_Items ordObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"orderitemsBy/Order_ItemID?orderId={orderId}&itemId={itemId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order_Items>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    ordObj = readTask.Result;
                }
            }
            return View(ordObj);
        }

        // GET: Order_ItemsMVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_ItemsMVC/Create
        [HttpPost]
        public ActionResult Create(Order_Items OrderItemObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Order_Items>("orderitems", OrderItemObj);
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
            return View(OrderItemObj);
        }

        // GET: Order_ItemsMVC/Edit/5
        public ActionResult ShowAllOrderByOrderId(int id)
        {
            // variable to hold the person details retrieved from WebApi
            List<Order_Items> OrderItemObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("orderitems/" + id.ToString());

                ViewBag.DiscountedPrice = withDiscount(id).ToString() ;
                ViewBag.withoutDiscountPrice = withoutDiscount(id).ToString() ;

                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<List<Order_Items>>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    OrderItemObj = readTask.Result;
                }
            }
            return View(OrderItemObj);
        }

        // POST: Order_ItemsMVC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Order_Items OrderItemObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Order_Items>($"", OrderItemObj);
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
            return View(OrderItemObj);
        }

        // GET: Order_ItemsMVC/Delete/5
        public ActionResult Delete(int orderId, int itemId)
        {
            // variable to hold the person details retrieved from WebApi
            Order_Items ordObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"orderitemsBy/Order_ItemID?orderId={orderId}&itemId={itemId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order_Items>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    ordObj = readTask.Result;
                }
            }
            return View(ordObj);
        }

        // POST: Order_ItemsMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int orderId, int itemId, Order_Items obj)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync($"orderitems/{orderId}/{itemId}");
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

        // GET: orderitemsBy/Order_ItemID ? orderId = {orderId} & itemId = {itemId}
        [HttpGet]
        public ActionResult GETEditOrderByItemId(int orderId, int itemId)
        {
            // variable to hold the person details retrieved from WebApi
            Order_Items ordObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"orderitemsBy/Order_ItemID?orderId={orderId}&itemId={itemId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Order_Items>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    ordObj = readTask.Result;
                }
            }
            return View(ordObj);
        }

        // POST: Order_ItemsMVC/Delete/5
        [HttpPost]
        public ActionResult GETEditOrderByItemId(int orderId, int itemId, Order_Items OrderItemObj)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete

                var responseTask = client.PutAsJsonAsync<Order_Items>($"orderitems/{orderId}?itemId={itemId}", OrderItemObj);
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
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
            return View(OrderItemObj);
        }

        //GET : orderDetails/{orderId}
        public double withoutDiscount(int orderId)
        {
            // variable to hold the person details retrieved from WebApi
            double ordObj = 0.0;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"orderDetails/{orderId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<double>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    ordObj = readTask.Result;
                }
            }
            return ordObj;
        }


        //GET : orderitem/{orderId}
        public double withDiscount(int orderId)
        {
            // variable to hold the person details retrieved from WebApi
            double ordObj=0.0;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"orderitem/{orderId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<double>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    ordObj = readTask.Result;
                }
            }
            return ordObj;
        }
    }
}