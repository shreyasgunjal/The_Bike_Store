using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles ="Staff,Store")]
    public class StockMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        // GET: orderitemsBy/Order_ItemID ? orderId = {orderId} & itemId = {itemId}
        [HttpGet]
        public ActionResult EditStoreByStoreId(int storeId, int prodId)

        {
            // variable to hold the person details retrieved from WebApi
            Stock stockObj = null;

            using (var client = new HttpClient())

            {

                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"stockByStore?stockId={storeId}&productId={prodId}");

                // wait for task to complete
                responseTask.Wait();

                // retrieve the result
                var result = responseTask.Result;

                // check the status code for success
                if (result.IsSuccessStatusCode)

                {

                    // read the result
                    var readTask = result.Content.ReadAsAsync<Stock>();

                    readTask.Wait();

                    // fill the person vairable created above with the returned result
                    stockObj = readTask.Result;

                }

            }

            return View(stockObj);

        }
        
        // POST: Order_ItemsMVC/Delete/5
        [HttpPost]
        public ActionResult EditStoreByStoreId(int storeId, int prodId, Stock stockObj)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                
                var responseTask = client.PutAsJsonAsync<Stock>($"stock/edit/{storeId}?prodId={prodId}", stockObj);
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
            return View(stockObj);
        }



        // GET: StockMVC
        public ActionResult Index()
        {
            IEnumerable<Stock> StockList = null;
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("Stock");  // Categories is the WebApi controller name
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<IList<Stock>>();
                    readTask.Wait();
                    // fill the list vairable created above with the returned result
                    StockList = readTask.Result;
                }
                else //web api sent error response 
                {
                    StockList = Enumerable.Empty<Stock>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(StockList);
        }

        // GET: StockMVC/Details/5
        public ActionResult Details(int storeId, int prodId)
        {
            // variable to hold the person details retrieved from WebApi
            Stock stockObj = null;

            using (var client = new HttpClient())

            {

                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);

                //HTTP Delete
                var responseTask = client.GetAsync($"stockByStore?stockId={storeId}&productId={prodId}");

                // wait for task to complete
                responseTask.Wait();

                // retrieve the result
                var result = responseTask.Result;

                // check the status code for success
                if (result.IsSuccessStatusCode)

                {

                    // read the result
                    var readTask = result.Content.ReadAsAsync<Stock>();

                    readTask.Wait();

                    // fill the person vairable created above with the returned result
                    stockObj = readTask.Result;

                }

            }

            return View(stockObj);
        }

        // GET: StockMVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockMVC/Create
        [HttpPost]
        public ActionResult Create(Stock stockObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Stock>("stock", stockObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {stockObj.Product_Id} already exists. Please contact administrator.");
            // send the view back with model error
            return View(stockObj);
        }

        // GET: StockMVC/Edit/5
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            List<Stock> stockObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("stock/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<List<Stock>>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    stockObj = readTask.Result;
                }
            }
            return View(stockObj);
        }

        // POST: StockMVC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id,Stock obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Stock>($"stock?stockId={id}", obj);
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

        // GET: StockMVC/Delete/5
        public ActionResult Delete(int storeId, int prodId)
        {
            // variable to hold the person details retrieved from WebApi
            Stock stockObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync($"stockByStore?stockId={storeId}&productId={prodId}");
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Stock>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    stockObj = readTask.Result;
                }
            }
            return View(stockObj);
        }

        // POST: StockMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int storeId, int prodId, Stock stockobj)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync($"stock/{storeId}/{prodId}");
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
