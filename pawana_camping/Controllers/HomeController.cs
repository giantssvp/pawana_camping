using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pawana_camping.Models;
using System.Configuration;
using System.Text;
using System.Net.Mail;

using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace pawana_camping.Controllers
{
    public class HomeController : Controller
    {
        public int event_page_size = 10;
        public int feedback_page_size = 3;
        public int booking_page_size = 10;
        
        public static string mailServer = "relay-hosting.secureserver.net";
        public static string mailFrom = "info@pawnaheritagecamping.com";
        public static string mailTo = "info@pawnaheritagecamping.com";
        public static string mailCc = "dharma9191@gmail.com";
        public static string mailPassword = "Pawna@123";
        /*
        public static string mailServer = "smtp.gmail.com";
        public static string mailFrom = "abcdtes26@gmail.com";
        public static string mailTo = "abcdtes26@gmail.com";
        public static string mailBCC = "abcdtes26@gmail.com";
        public static string mailPassword = "9921642540";
        */

        public static db_connect obj = new db_connect();
        public ActionResult Index()
        {
            HttpContext.Session.Add("offset_event", 0);
            List<string>[] list = new List<string>[3];
            list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), 3);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            ViewBag.total = 0;
            HttpContext.Session.Add("offset_dashboard", 0);
            List<string>[] list = new List<string>[14];
            list = obj.bookings_show(Int32.Parse(HttpContext.Session["offset_dashboard"].ToString()), booking_page_size);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();
            
            return View();
        }

        [Authorize]
        public ActionResult ChangeRate()
        {
            return View();
        }

        public ActionResult Events()
        {
            HttpContext.Session.Add("offset_event", 0);

            List<string>[] list = new List<string>[4];
            list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), event_page_size);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            HttpContext.Session.Add("offset_feedback", 0);

            List<string>[] list = new List<string>[3];
            list = obj.feedback_show(Int32.Parse(HttpContext.Session["offset_feedback"].ToString()), feedback_page_size);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.UserName, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(user);            
        }

        [Authorize]
        public ActionResult Eventfeed()
        {
            return View();
        }

        [Authorize]
        public ActionResult DeleteEvent()
        {
            HttpContext.Session.Add("offset_event", 0);

            List<string>[] list = new List<string>[4];
            list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), event_page_size);
            ViewBag.list = list;
            ViewBag.total = list[0].Count();

            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult PaymentStatus(System.Web.Mvc.FormCollection form)
        {
            try
            {
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

                
                merc_hash_vars_seq = hash_seq.Split('|');
                Array.Reverse(merc_hash_vars_seq);
                merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + Request.Form["status"];

                foreach (string merc_hash_var in merc_hash_vars_seq)
                {
                    merc_hash_string += "|";
                    merc_hash_string = merc_hash_string + (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "");
                }
                //Response.Write(merc_hash_string); //exit;
                merc_hash = Generatehash512(merc_hash_string).ToLower();

                if (merc_hash != Request.Form["hash"])
                {
                    //Response.Write("Hash value did not matched");
                    order_id = Request.Form["txnid"];
                }
                else
                {
                    order_id = Request.Form["txnid"];
                }

                ViewBag.name = Request.Form["firstname"];
                ViewBag.status = Request.Form["status"];
                ViewBag.email = Request.Form["email"];
                ViewBag.phone = Request.Form["phone"];
                ViewBag.tid = Request.Form["txnid"];
                ViewBag.tr_amt = Request.Form["amount"];
                ViewBag.tr_date_time = Request.Form["udf1"];
                ViewBag.bk_date_time = Request.Form["udf2"];
                ViewBag.adults = Request.Form["udf3"];
                ViewBag.children = Request.Form["udf4"];
                ViewBag.prod_info = Request.Form["productinfo"];
                ViewBag.part_payment = Request.Form["udf5"];
                ViewBag.package = Request.Form["udf6"];

                obj.Insert_Booking(ViewBag.tid, ViewBag.status, ViewBag.tr_date_time, ViewBag.prod_info,
                ViewBag.name, ViewBag.email, ViewBag.phone, ViewBag.bk_date_time, Int32.Parse(ViewBag.adults), Int32.Parse(ViewBag.children),
                Int32.Parse(ViewBag.part_payment), (int)(Convert.ToDouble(ViewBag.tr_amt)), ViewBag.package);
                
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(mailServer);

                mail.From = new MailAddress(mailFrom);
                mail.To.Add(mailCc);
                mail.CC.Add(mailTo);
                mail.Bcc.Add(ViewBag.email);
                mail.Subject = "Booking Details For : " + ViewBag.name;
                mail.IsBodyHtml = true;

                string htmlBody;

                htmlBody = "<html> <head>  </head> <body>" +
                            "<div><img src=\"cid:icon_01\"> </div>" +
                            "<table border=\"1\" style=\"font - family:Georgia, Garamond, Serif; width: 100 %; color: blue; font - style:italic; \"> <tr bgcolor=\"#00FFFF\" align=\"center\"> <th> Transaction Id </th> <th> Status </th> <th> Name </th> <th> Email </th> <th> Phone </th>  <th>Booking Date</th>  " +
                            "<th>Adults</th> <th>Children</th> <th>Part Payment</th> <th>Paid Amount</th></tr> <tr align=\"center\"> " +
                            "<td>" + ViewBag.tid + "</td>" +
                            "<td>" + ViewBag.status + "</td>" +
                            "<td>" + ViewBag.name + "</td>" +
                            "<td>" + ViewBag.email + "</td>" +
                            "<td>" + ViewBag.phone + "</td>" +
                            "<td>" + ViewBag.bk_date_time + "</td>" +
                            "<td>" + ViewBag.adults + "</td>" +
                            "<td>" + ViewBag.children + "</td>" +
                            "<td>" + ViewBag.part_payment + "</td>" +
                            "<td>" + ViewBag.tr_amt + "</td>" +
                            "</tr> </table> </body> </html> ";


                mail.Body = htmlBody;
                //SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(mailFrom, mailPassword);
                //SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                               
                return View();
            }
            catch (Exception ex)
            {
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");
                return View();
            }
        }        
        
        public ActionResult UpdateNow(string p1_base_adult, string p1_base_child, string p2_base_adult, string p2_base_child, string p3_base_adult, string p3_base_child)
        {
            try
            {
                obj.update_rates(p1_base_adult, p1_base_child, 1);
                obj.update_rates(p2_base_adult, p2_base_child, 2);
                obj.update_rates(p3_base_adult, p3_base_child, 3);
                return RedirectToAction("ChangeRate", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("ChangeRate", "Home");
            }
        }

        public ActionResult submit_event_btn(string heading, string description)
        {
            try
            {
                obj.Insert_Event(heading, description);
                return RedirectToAction("Eventfeed", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Eventfeed", "Home");
            }
        }

        public ActionResult delete_event_btn(int data)
        {
            try
            {
                obj.Delete_Event(data);
                return RedirectToAction("DeleteEvent", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("DeleteEvent", "Home");
            }
        }

        public ActionResult add_feedback(string name, string email, string phone, string subject, string message)
        {
            try
            {
                obj.insert_feedback(name, email, phone, subject, message);
                return RedirectToAction("Feedback", "Home");
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('There is some issue while saving the details, please try again, Thanks.')</script>");
                return RedirectToAction("Feedback", "Home");
            }
        }

        public ActionResult F_Events(int data)
        {
            try
            {
                List<string>[] list = new List<string>[4];
                list = obj.events_show(0, event_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                if (data == 0)
                {
                    return View("Events");
                }
                else {
                    return View("DeleteEvent");
                }
            }
            catch (Exception ex)
            {
                return View("Events");
            }
        }

        public ActionResult P_Events(int data)
        {
            try
            {
                HttpContext.Session.Add("offset_event", (Int32.Parse(HttpContext.Session["offset_event"].ToString()) - event_page_size));
                if (Int32.Parse(HttpContext.Session["offset_event"].ToString()) <= (event_page_size-1))
                {
                    HttpContext.Session.Add("offset_event", 0);
                }

                List<string>[] list = new List<string>[4];
                list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), event_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                if (data == 0)
                {
                    return View("Events");
                }
                else
                {
                    return View("DeleteEvent");
                }
            }
            catch (Exception ex)
            {
                return View("Events");
            }
        }

        public ActionResult N_Events(int data)
        {
            try
            {
                int cnt = obj.event_count();
                HttpContext.Session.Add("offset_event", (Int32.Parse(HttpContext.Session["offset_event"].ToString()) + event_page_size));
                if (Int32.Parse(HttpContext.Session["offset_event"].ToString()) > cnt)
                {
                    HttpContext.Session.Add("offset_event", (cnt - (cnt % event_page_size)));
                }
                List<string>[] list = new List<string>[4];
                list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), event_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                if (data == 0)
                {
                    return View("Events");
                }
                else
                {
                    return View("DeleteEvent");
                }
            }
            catch (Exception ex)
            {
                return View("Events");
            }
        }

        public ActionResult L_Events(int data)
        {
            try
            {
                int cnt = obj.event_count();
                if (cnt > 0)
                {
                    if (cnt % event_page_size == 0)
                    {
                        HttpContext.Session.Add("offset_event", (cnt - event_page_size));
                    }
                    else
                    {
                        HttpContext.Session.Add("offset_event", (cnt - (cnt % event_page_size)));
                    }                    
                }
                
                List<string>[] list = new List<string>[4];
                list = obj.events_show(Int32.Parse(HttpContext.Session["offset_event"].ToString()), event_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                if (data == 0)
                {
                    return View("Events");
                }
                else
                {
                    return View("DeleteEvent");
                }
            }
            catch (Exception ex)
            {
                return View("Events");
            }
        }

        public ActionResult F_Feedback()
        {
            try
            {
                List<string>[] list = new List<string>[3];
                list = obj.feedback_show(0, feedback_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Feedback");
            }
            catch (Exception ex)
            {
                return View("Feedback");
            }
        }

        public ActionResult P_Feedback()
        {
            try
            {
                HttpContext.Session.Add("offset_feedback", (Int32.Parse(HttpContext.Session["offset_feedback"].ToString()) - feedback_page_size));
                if (Int32.Parse(HttpContext.Session["offset_feedback"].ToString()) <= (feedback_page_size - 1))
                {
                    HttpContext.Session.Add("offset_feedback", 0);
                }

                List<string>[] list = new List<string>[3];
                list = obj.feedback_show(Int32.Parse(HttpContext.Session["offset_feedback"].ToString()), feedback_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Feedback");
            }
            catch (Exception ex)
            {
                return View("Feedback");
            }
        }

        public ActionResult N_Feedback()
        {
            try
            {
                int cnt = obj.feedback_count();
                HttpContext.Session.Add("offset_feedback", (Int32.Parse(HttpContext.Session["offset_feedback"].ToString()) + feedback_page_size));
                if (Int32.Parse(HttpContext.Session["offset_feedback"].ToString()) > cnt)
                {
                    HttpContext.Session.Add("offset_feedback", (cnt - (cnt % feedback_page_size)));
                }
                List<string>[] list = new List<string>[3];
                list = obj.feedback_show(Int32.Parse(HttpContext.Session["offset_feedback"].ToString()), feedback_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Feedback");
            }
            catch (Exception ex)
            {
                return View("Feedback");
            }
        }

        public ActionResult L_Feedback()
        {
            try
            {
                int cnt = obj.feedback_count();
                if (cnt > 0)
                {
                    if (cnt % feedback_page_size == 0)
                    {
                        HttpContext.Session.Add("offset_feedback", (cnt - feedback_page_size));
                    }
                    else
                    {
                        HttpContext.Session.Add("offset_feedback", (cnt - (cnt % feedback_page_size)));
                    }
                }

                List<string>[] list = new List<string>[3];
                list = obj.feedback_show(Int32.Parse(HttpContext.Session["offset_feedback"].ToString()), feedback_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Feedback");
            }
            catch (Exception ex)
            {
                return View("Feedback");
            }
        }

        public ActionResult F_Booking()
        {
            try
            {
                List<string>[] list = new List<string>[14];
                list = obj.bookings_show(0, booking_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Dashboard");
            }
            catch (Exception ex)
            {
                return View("Dashboard");
            }
        }

        public ActionResult P_Booking()
        {
            try
            {
                HttpContext.Session.Add("offset_booking", (Int32.Parse(HttpContext.Session["offset_booking"].ToString()) - booking_page_size));
                if (Int32.Parse(HttpContext.Session["offset_booking"].ToString()) <= (booking_page_size - 1))
                {
                    HttpContext.Session.Add("offset_booking", 0);
                }

                List<string>[] list = new List<string>[14];
                list = obj.bookings_show(Int32.Parse(HttpContext.Session["offset_booking"].ToString()), booking_page_size);
                ViewBag.list = list;
                ViewBag.total = 0;
                ViewBag.total = list[0].Count();

                return View("Dashboard");
            }
            catch (Exception ex)
            {
                return View("Dashboard");
            }
        }

        public ActionResult N_Booking()
        {
            try
            {
                int cnt1 = obj.booking_count();
                HttpContext.Session.Add("offset_booking", (Int32.Parse(HttpContext.Session["offset_booking"].ToString()) + booking_page_size));
                if (Int32.Parse(HttpContext.Session["offset_booking"].ToString()) > cnt1)
                {
                    HttpContext.Session.Add("offset_booking", (cnt1 - (cnt1 % booking_page_size)));
                }
                List<string>[] list = new List<string>[14];
                list = obj.bookings_show(Int32.Parse(HttpContext.Session["offset_booking"].ToString()), booking_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Dashboard");
            }
            catch (Exception ex)
            {
                return View("Dashboard");
            }
        }

        public ActionResult L_Booking()
        {
            try
            {
                int cnt = obj.booking_count();
                if (cnt > 0)
                {
                    if (cnt % booking_page_size == 0)
                    {
                        HttpContext.Session.Add("offset_booking", (cnt - booking_page_size));
                    }
                    else
                    {
                        HttpContext.Session.Add("offset_booking", (cnt - (cnt % booking_page_size)));
                    }
                }

                List<string>[] list = new List<string>[14];
                list = obj.bookings_show(Int32.Parse(HttpContext.Session["offset_booking"].ToString()), booking_page_size);
                ViewBag.list = list;
                ViewBag.total = list[0].Count();

                return View("Dashboard");
            }
            catch (Exception ex)
            {
                return View("Dashboard");
            }
        }

        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;
        
        [HttpPost]
        public void Index(string name, string email, string phone, string total_cost, string bk_date, string package, string adult, string child, string partial_payment)
        {
            try
            {
                string part_payment = "0";
                if (partial_payment == "on")
                {
                    part_payment = "1";
                }                
                string[] hashVarsSeq;
                string hash_string = string.Empty;

                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                txnid1 = strHash.ToString().Substring(0, 20);
                DateTime dateTime = DateTime.UtcNow.Date;

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
                        hash_string = hash_string + total_cost;//Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "productinfo")
                    {
                        hash_string = hash_string + "Tent Booking";
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "firstname")
                    {
                        hash_string = hash_string + name;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf1")
                    {
                        hash_string = hash_string + dateTime;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf2")
                    {
                        hash_string = hash_string + bk_date;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf3")
                    {
                        hash_string = hash_string + adult;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf4")
                    {
                        hash_string = hash_string + child;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf5")
                    {
                        hash_string = hash_string + part_payment;
                        hash_string = hash_string + '|';
                    }
                    else if (hash_var == "udf6")
                    {
                        hash_string = hash_string + package;
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
                    string AmountForm = total_cost;// eliminating trailing zeros
                    data.Add("amount", AmountForm);
                    data.Add("firstname", name);
                    data.Add("email", email);
                    data.Add("phone", phone);
                    data.Add("productinfo", "Tent Booking");
                    data.Add("surl", "http://pawnaheritagecamping.com/Home/PaymentStatus");
                    data.Add("furl", "http://pawnaheritagecamping.com/Home/PaymentStatus");
                    data.Add("lastname", "thakar");
                    data.Add("curl", "http://www.pawnaheritagecamping.com");
                    data.Add("address1", "pune");
                    data.Add("address2", "pune");
                    data.Add("city", "pune");
                    data.Add("state", "maharashtra");
                    data.Add("country", "india");
                    data.Add("zipcode", "419406");
                    data.Add("udf1", dateTime);
                    data.Add("udf2", bk_date);
                    data.Add("udf3", adult);
                    data.Add("udf4", child);
                    data.Add("udf5", part_payment);
                    data.Add("udf6", package);
                    data.Add("pg", "");
                    data.Add("service_provider", "payu_paisa");
                    string strForm = PreparePOSTForm(action1, data);
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

        public ActionResult calculate_amount(string adult, string child, string package, string part_pay)
        {
            //var obj = new db_connect();
            int base_adult = obj.get_rates("adult", package);
            int base_child = obj.get_rates("child", package);
            double total_cost = 0;            
            if(Int32.Parse(part_pay) == 1)
            {
                total_cost = (((Int32.Parse(adult) * base_adult) + (Int32.Parse(child) * base_child)) * 0.5);
            }
            else
            {
                total_cost = ((Int32.Parse(adult) * base_adult) + (Int32.Parse(child) * base_child));
            }
            List<int> c = new List<int> {(int)(total_cost)};
            return Json(new { c=c},JsonRequestBehavior.AllowGet);
        }
    }
}