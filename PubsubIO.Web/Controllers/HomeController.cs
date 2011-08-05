using System.Web.Mvc;
using PubsubIO.Client;

namespace PubsubIO.Web.Controllers
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
            string result = PubsubIoClient.Publish("http://matcctst09.net.dr.dk:9999", "dr", new {hello = "world"});

            return Content(result);
        }
    }
}