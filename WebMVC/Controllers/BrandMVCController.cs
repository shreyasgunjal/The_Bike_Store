using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class BrandMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        // GET: BrandMVC
        [Authorize]
        public ActionResult Index()
        {
            IEnumerable<Brand> brandList = null;
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("Brand");  // Brand is the WebApi controller name
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<IList<Brand>>();
                    readTask.Wait();
                    // fill the list vairable created above with the returned result
                    brandList = readTask.Result;
                }
                else //web api sent error response 
                {
                    brandList = Enumerable.Empty<Brand>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(brandList);

        }



        // GET: BrandMVC/Details/5
        public ActionResult Details(int id)
        {
            // variable to hold the brand details retrieved from WebApi
            Brand bObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("brand/id?brandId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Brand>();
                    readTask.Wait();
           
                    bObj = readTask.Result;
                }
            }
            return View(bObj);
        }

        // GET: BrandMVC/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BrandyMVC/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Brand bObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Brand>("brand", bObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {bObj.Brand_Name} already exists. Please contact administrator.");
            // send the view back with model error
            return View(bObj);
        }

        // GET: BrandMVC/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            // variable to hold the brand details retrieved from WebApi
            Brand bObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("brand/id?brandId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Brand>();
                    readTask.Wait();
                    // fill the brand vairable created above with the returned result
                    bObj = readTask.Result;
                }
            }
            return View(bObj);
        }

        // POST: CategoryMVC/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(int id, Brand bObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Brand>($"brand/edit/id?brandId={id}", bObj);
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
            return View(bObj);
        }

        // GET: BrandMVC/Delete/5
        public ActionResult Delete(int id)
        {
            Brand bObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("brand/id?brandId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Brand>();
                    readTask.Wait();
                    // fill the brand vairable created above with the returned result
                    bObj = readTask.Result;
                }
            }
            return View(bObj);
        }

        // POST: BrandMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // variable to hold the brand details retrieved from WebApi

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("brand?brandId=" + id.ToString());
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