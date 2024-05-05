using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MD2241A5.Controllers
{
    [Authorize(Roles = "Admin,Executive,Coordinator, Clerk")]
    public class LoadDataController : Controller
    {
        // Reference to the manager object
        Manager m = new Manager();

        // GET: LoadData
        public ActionResult Index()
        {
            if (m.LoadData())
            {
                return Content("data has been loaded");
            }
            else
            {
                return Content("data exists already");
            }
        }

        public ActionResult Remove()
        {
            if (m.RemoveData())
            {
                return Content("data has been removed");
            }
            else
            {
                return Content("could not remove data");
            }
        }

        public ActionResult RemoveDatabase()
        {
            if (m.RemoveDatabase())
            {
                return Content("database has been removed");
            }
            else
            {
                return Content("could not remove database");
            }
        }

        [AllowAnonymous]
        public ActionResult Roles()
        {
            m.LoadRoles();
            return Content("Roles loaded");
        }

      
        public ActionResult Genres()
        {
            m.LoadGenres();
            return RedirectToAction("Index", "Genre");
        }

        
        public ActionResult Actors()
        {
            m.LoadActors();
            return RedirectToAction("Index", "Actor");
        }

        
        public ActionResult Shows()
        {
            m.LoadShows();
            return RedirectToAction("Index", "Show");
        }

       
        public ActionResult Episodes()
        {
            m.LoadEpisodes();
            return RedirectToAction("Index", "Episode");
        }
       

    }
}