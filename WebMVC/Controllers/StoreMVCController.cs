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
    public class StoreMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        // GET: StoreMVC
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string searchTerm, string city,int? filterOption)
        {
            IEnumerable<Store> StoreList = null;
            var message = string.Empty;
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
            

                switch(filterOption)
                {
                    case 1: //By City
                       // string city = "";
                        if(!string.IsNullOrEmpty(city))
                        {
                            response = client.GetAsync($"stores/city?city={city}").Result;
                        }
                        else
                        {
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        break;


                    case 2:
                        {
                            //Display Number of store in each state
                            response = client.GetAsync("stores/totalstoreineachstate").Result;
                            if (response.IsSuccessStatusCode)
                            {

                                // read the result
                                var readTask = response.Content.ReadAsAsync<IList<StoreInStateVM>>();
                                readTask.Wait();
                                // fill the list vairable created above with the returned result
                                ViewBag.TotalStoresInState = readTask.Result;
                            }
                            else if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                message = "No details exist.";
                                StoreList = Enumerable.Empty<Store>();
                            }
                            else //web api sent error response
                            {
                                StoreList = Enumerable.Empty<Store>();
                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            }
                            //ViewBag.TotalStoresInState
                            break;
                        }

                    case 3:
                        { //Display the  store that has maximum customer
                            response = client.GetAsync("stores/maxiumcustomers").Result;
                            if (response.IsSuccessStatusCode)
                            {

                                // read the result
                                var readTask = response.Content.ReadAsAsync<string>();
                                readTask.Wait();
                                // fill the list vairable created above with the returned result
                                ViewBag.StoreWithMaxCustomer = readTask.Result;
                            }
                            else if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                message = "No details exist.";
                                StoreList = Enumerable.Empty<Store>();
                            }
                            else //web api sent error response
                            {
                                StoreList = Enumerable.Empty<Store>();
                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            }
                            //ViewBag.TotalStoresInState
                            break;
                      
                        }

                    case 4:
                        { //Display the store that made highest sale
                            response = client.GetAsync("stores/highestsale").Result;
                            if (response.IsSuccessStatusCode)
                            {

                                // read the result
                                var readTask = response.Content.ReadAsAsync<string>();
                                readTask.Wait();
                                // fill the list vairable created above with the returned result
                                ViewBag.StoreWithHighestSale = readTask.Result;
                            }
                            else if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                message = "No details exist.";
                                StoreList = Enumerable.Empty<Store>();
                            }
                            else //web api sent error response
                            {
                                StoreList = Enumerable.Empty<Store>();
                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            }
                            break;
                        }

                    default:
                        response = client.GetAsync("stores").Result;
                        break;

                }
                if (response.IsSuccessStatusCode)
                {

                    // read the result
                    var readTask = response.Content.ReadAsAsync<IList<Store>>();
                    readTask.Wait();
                    // fill the list vairable created above with the returned result
                    StoreList = readTask.Result;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    message = "No details exist.";
                    StoreList = Enumerable.Empty<Store>();
                }
                else //web api sent error response
                {
                    StoreList = Enumerable.Empty<Store>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.Message = message;
            return View(StoreList);
        }

        // GET: StoreMVC/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Store storeObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("stores/id?storeID=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Store>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    storeObj = readTask.Result;
                }
            }
            return View(storeObj);
        }

        // GET: StoreMVC/Create
        [Authorize(Roles = "Store")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoreMVC/Create
        [Authorize(Roles = "Store")]
        [HttpPost]
        public ActionResult Create(Store storeObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Store>("stores", storeObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {storeObj.Store_Name} already exists. Please contact administrator.");
            // send the view back with model error
            return View(storeObj);
        }

        // GET: StoreMVC/Edit/5
        [Authorize(Roles ="Store")]
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Store storeObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("stores/id?storeID=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Store>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    storeObj = readTask.Result;
                }
            }
            return View(storeObj);
        }

        // POST: StoreMVC/Edit/5
        [Authorize(Roles = "Store")]
        [HttpPost]
        public ActionResult Edit(int id, Store obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Store>($"stores/edit/{id}", obj);
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

        // GET: StoreMVC/Delete/5
        public ActionResult Delete(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Store storeObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("stores/id?storeID=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Store>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    storeObj = readTask.Result;
                }
            }
            return View(storeObj);
        }

        // POST: StoreMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id,Store obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync($"stores/{id}");
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
