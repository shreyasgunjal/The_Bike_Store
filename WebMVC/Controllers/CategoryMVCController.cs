using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        // GET: CategoryMVC
        public ActionResult Index()
        {
            IEnumerable<Category> catList = null;
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("category");  // Categories is the WebApi controller name
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<IList<Category>>();
                    readTask.Wait();
                    // fill the list vairable created above with the returned result
                    catList = readTask.Result;
                }
                else //web api sent error response 
                {
                    catList = Enumerable.Empty<Category>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(catList);
        }

        // GET: CategoryMVC/Details/5
        public ActionResult Details(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("category/catId?catId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    catObj = readTask.Result;
                }
            }
            return View(catObj);
        }

        // GET: CategoryMVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryMVC/Create
        [HttpPost]
        public ActionResult Create(Category catObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Category>("category", catObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {catObj.Category_Name} already exists. Please contact administrator.");
            // send the view back with model error
            return View(catObj);
        }

        // GET: CategoryMVC/Edit/5
        public ActionResult Edit(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("category/catId?catId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    catObj = readTask.Result;
                }
            }
            return View(catObj);
        }

        // POST: CategoryMVC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Category obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Category>($"category/edit/id?catId={id}", obj);
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

        // GET: CategoryMVC/Delete/5
        public ActionResult Delete(int id)
        {
            // variable to hold the person details retrieved from WebApi
            Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("category/catId?catId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    catObj = readTask.Result;
                }
            }
            return View(catObj);
        }

        // POST: CategoryMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Category category)
        {
            // variable to hold the person details retrieved from WebApi
            //Category catObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("category?catId=" + id.ToString());
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
