using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class StaffMVCController : Controller
    {
        string baseURL = "https://localhost:44352/api/";

        #region GET Staff Details
        [Authorize(Roles = "Store,Staff")]
        public ActionResult Index(string searchTerm, int? id, string storeName, int? filterOption)
        {
            IEnumerable<Staff> staffList = Enumerable.Empty<Staff>();
            var message = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                HttpResponseMessage response = null;

                try
                {
                    if (filterOption.HasValue)
                    {
                        switch (filterOption.Value)
                        {
                            case 1: // By Staff Id
                                if (id.HasValue)
                                {
                                    response = client.GetAsync($"staff/managerdetails/staffid?staffId={id.Value}").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var staffDetails = response.Content.ReadAsAsync<Staff>().Result;
                                        staffList = staffDetails != null ? new List<Staff> { staffDetails } : Enumerable.Empty<Staff>();
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No details exist.";
                                        staffList = Enumerable.Empty<Staff>();
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

                            case 2: // By Sales Made by Staff
                                if (id.HasValue)
                                {
                                    response = client.GetAsync($"staff/salesmadebystaff/staffid?staffId={id.Value}").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var salesByStaff = response.Content.ReadAsAsync<IList<SalesByStaff>>().Result;
                                        ViewBag.SalesByStaff = salesByStaff;
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

                            case 3: // By Store Name
                                if (!string.IsNullOrEmpty(storeName))
                                {
                                    response = client.GetAsync($"staff/storename?storeName={storeName}").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        staffList = response.Content.ReadAsAsync<IList<Staff>>().Result;
                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        message = "No details exist.";
                                        staffList = Enumerable.Empty<Staff>();
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

                            default:
                                response = client.GetAsync("staff").Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    staffList = response.Content.ReadAsAsync<IList<Staff>>().Result;
                                }
                                else if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    message = "No details exist.";
                                    staffList = Enumerable.Empty<Staff>();
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
                        response = client.GetAsync("staff").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            staffList = response.Content.ReadAsAsync<IList<Staff>>().Result;
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            message = "No details exist.";
                            staffList = Enumerable.Empty<Staff>();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details for debugging
                    // For example: Log.Error(ex, "Error occurred while processing the request");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                }
            }

            ViewBag.Message = message;
            return View(staffList);
        }

        #endregion

        // GET: StaffMVC/Details/5
        #region GET Staff/Details/5
        [Authorize(Roles = "Store")]
        public ActionResult Details(int id)
        {
            // variable to hold the staff details retrieved from WebApi
            Staff staffObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("staff/edit/id?staffId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Staff>();
                    readTask.Wait();
                    // fill the person vairable created above with the returned result
                    staffObj = readTask.Result;
                }
            }
            return View(staffObj);
        }
        #endregion

        // GET: StaffMVC/Create
        #region GET staff/create
        [Authorize(Roles = "Store")]
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        // POST: StaffMVC/Create
        #region POST Staff/Create
        [Authorize(Roles = "Store")]
        [HttpPost]
        public ActionResult Create(Staff staffObj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Staff>("staff", staffObj);
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
            ModelState.AddModelError(string.Empty, $"Server Error, {staffObj.First_Name} already exists. Please contact administrator.");
            // send the view back with model error
            return View(staffObj);
        }
        #endregion

        // GET: StaffMVC/Edit/5
        #region Staff/edit/5
        [Authorize(Roles = "Store")]
        public ActionResult Edit(int id)
        {
            // variable to hold the staff details retrieved from WebApi
            Staff staffObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("staff/edit/id?staffId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Staff>();
                    readTask.Wait();
                    // fill the staff created above with the returned result
                    staffObj = readTask.Result;
                }
            }
            return View(staffObj);
        }
        #endregion

        // POST: StaffMVC/Edit/5
        #region POST staff/edit/5
        [Authorize(Roles = "Store")]
        [HttpPost]
        public ActionResult Edit(int id, Staff obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Staff>($"staff/edit/staffid?staffId={id}", obj);
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

        // GET: StaffMVC/Delete/5
        #region GET staff/delete/5
        public ActionResult Delete(int id)
        {
            Staff staffObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("staff/edit/id?staffId=" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<Staff>();
                    readTask.Wait();
                    // fill the staff created above with the returned result
                    staffObj = readTask.Result;
                }
            }
            return View(staffObj);
        }
        #endregion

        // POST: StaffMVC/Delete/5
        #region POST staff/delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // variable to hold the staff details retrieved from WebApi

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("staff?staffId=" + id.ToString());
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
