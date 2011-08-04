using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PubsubIO.Client;

namespace pubsub.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(MvcApplication.Posts);
        }

        public ActionResult MakePost(string text)
        {
            return RedirectToAction("Index");
        }

        public ActionResult DoJab()
        {
            var result = PubsubIoClient.Publish("http://matcctst09.net.dr.dk:9999", "dr", new { hello = "world" });

            return Content(result);
        }
    }
}