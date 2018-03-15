using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pawana_camping.Models;
using System.Windows.Forms;

namespace pawana_camping.Controllers
{
    public class HomeController : Controller
    {
        private int offset = 0; 

        public ActionResult Index()
        {
            HttpContext.Session.Add("offset", 0);

            var obj = new db_connect();
            List<string>[] list = new List<string>[3];
            list = obj.events_show(offset);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult login_btn(string username, string password)
        {
            try
            {
                var obj = new db_connect();

                if (obj.Login(username, password))
                {
                    TempData["AlertMessage"] = "Logged in successfully";
                    HttpContext.Session.Add("user", 1);
                    return RedirectToAction("Eventfeed", "Home");

                }
                else
                {
                    TempData["AlertMessage"] = "Your username or password is not correct";
                    HttpContext.Session.Add("user", 0);
                    return RedirectToAction("Login", "Home");
                }

            }
            catch (Exception ex)
            {
                HttpContext.Session.Add("user", 0);
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Eventfeed()
        {
            try
            {
                string session_status = HttpContext.Session["user"].ToString();
                if (Int32.Parse(session_status) == 0)
                    return RedirectToAction("Login", "Home");
                else
                    return View();
            }
            catch (Exception ex)
            {
                HttpContext.Session.Add("user", 0);
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult submit_event_btn(string heading, string description)
        {
            try
            {
                var obj = new db_connect();
                obj.Insert_Event(heading, description);
                return RedirectToAction("Eventfeed", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Eventfeed", "Home");
            }
        }

        public ActionResult logout_btn(string heading, string description)
        {
            try
            {
                HttpContext.Session.Add("user", 0);
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult first_btn()
        {
            try
            {
                var obj = new db_connect();
                List<string>[] list = new List<string>[3];
                list = obj.events_show(0);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult previous_btn()
        {
            try
            {
                if (offset <= 1)
                {
                    offset = 0;
                }
                else
                {
                    offset = offset - 2;
                }

                var obj = new db_connect();
                List<string>[] list = new List<string>[3];
                list = obj.events_show(offset);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult next_btn()
        {
            try
            {   
                var obj = new db_connect();
                int cnt = obj.event_count();
                offset = offset + 2;
                if (offset > cnt)
                {
                    offset = cnt-2;
                }
                offset = 2;
                List<string>[] list = new List<string>[3];
                list = obj.events_show(1);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();
                return PartialView("event_show");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult last_btn()
        {
            try
            {
                offset = offset + 2;
                var obj = new db_connect();
                int cnt = obj.event_count();
                if (cnt != 0)
                {
                    if (cnt > 0)
                    {
                        if (cnt % 2 == 0)
                            offset = cnt - 2;
                        else
                            offset = cnt - 1;
                    }
                }
                List<string>[] list = new List<string>[3];
                list = obj.events_show(offset);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}