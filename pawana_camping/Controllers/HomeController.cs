﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pawana_camping.Models;
using System.Windows.Forms;
using System.Configuration;
using System.Text;


using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Net;
using System.IO;


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
            HttpContext.Session.Add("offset", 0);

            var obj = new db_connect();
            List<string>[] list = new List<string>[3];
            list = obj.events_show(offset);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

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

        public ActionResult PaymentSuccess()
        {
            try
            {

                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

                if (Request.Form["status"] == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + Request.Form["status"];


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "");

                    }
                    Response.Write(merc_hash_string); //exit;
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != Request.Form["hash"])
                    {
                        Response.Write("Hash value did not matched");

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        Response.Write("value matched");

                        //Hash value did not matched
                    }

                }

                else
                {

                    Response.Write("Hash value did not matched");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }
                return View();
            }

            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
                return View();
            }
            
        }

        public ActionResult PaymentFailure()
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

                return View("Events");
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
                
        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;
        private object exit;

        public void PayNow(string name, string email, string phone, string total_cost)
        {
            try
            {
                string[] hashVarsSeq;
                string hash_string = string.Empty;
                
                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                txnid1 = strHash.ToString().Substring(0, 20);

                hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
                hash_string = "";
                foreach (string hash_var in hashVarsSeq)
                {
                    if (hash_var == "key")
                    {
                        hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "txnid")
                    {
                        hash_string = hash_string + txnid1;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "amount")
                    {
                        hash_string = hash_string + 10;//Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "productinfo")
                    {
                        hash_string = hash_string + "pawnacamp";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "firstname")
                    {
                        hash_string = hash_string + "dharma";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf1")
                    {
                        hash_string = hash_string + "udf1";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf2")
                    {
                        hash_string = hash_string + "udf2";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf3")
                    {
                        hash_string = hash_string + "udf3";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf4")
                    {
                        hash_string = hash_string + "udf4";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf5")
                    {
                        hash_string = hash_string + "udf5";
                        hash_string = hash_string + '|';
                    }
                    else
                    {
                        hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                        hash_string = hash_string + '|';
                    }
                }

                hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT
                hash1 = Generatehash512(hash_string).ToLower();         //generating hash
                action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment"; // setting URL
                
                if (!string.IsNullOrEmpty(hash1))
                {
                    System.Collections.Hashtable data = new System.Collections.Hashtable(); // adding values in gash table for data post
                    data.Add("hash", hash1);
                    data.Add("txnid", txnid1);
                    data.Add("key", ConfigurationManager.AppSettings["MERCHANT_KEY"]);
                    string AmountForm = "10";// eliminating trailing zeros
                    data.Add("amount", AmountForm);
                    data.Add("firstname", "dharma");
                    data.Add("email", "dharma9191@gmail.com");
                    data.Add("phone", "9960044802");
                    data.Add("productinfo", "pawnacamp");
                    data.Add("surl", "http://localhost:64707/Home/PaymentSuccess");
                    data.Add("furl", "http://localhost:64707/Home/PaymentFailure");
                    data.Add("lastname", "thakar");
                    data.Add("curl", "http://www.pawnaheritagecamping.com");
                    data.Add("address1", "pune");
                    data.Add("address2", "pune");
                    data.Add("city", "pune");
                    data.Add("state", "maharashtra");
                    data.Add("country", "india");
                    data.Add("zipcode", "419406");
                    data.Add("udf1", "udf1");
                    data.Add("udf2", "udf2");
                    data.Add("udf3", "udf3");
                    data.Add("udf4", "udf4");
                    data.Add("udf5", "udf5");
                    data.Add("pg", "");
                    data.Add("service_provider", "payu_paisa");
                    MessageBox.Show(action1);
                    string strForm = PreparePOSTForm(action1, data);
                    MessageBox.Show(strForm);
                    Response.Write(strForm);
                }
                else
                {
                    //no hash
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }

        private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (System.Collections.DictionaryEntry key in data)
            {

                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }

            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }
                
        public string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }
}