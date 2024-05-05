using MD2241A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MD2241A5.Controllers
{
    public class ActorController : Controller
    {
        // GET: Actor
        private Manager m = new Manager();
        public ActionResult Index()
        {
            var actor = m.GetAllActors();
            ViewBag.IsExecutive = m.User.HasRoleClaim("Executive");
            return View(actor);
        }

        // GET: Actor/Details/5
        public ActionResult Details(int id)
        {
            var actor = m.GetActorById(id);

            if (actor == null)
            {
                // Handle the case where the actor is null
                // For example, display an error message or redirect to another page
                return RedirectToAction("Index");
            }

            // Check if the user has the "Coordinator" role
            bool isCoordinator = m.User.HasRoleClaim("Coordinator");

            // Pass the actor details and the role information to the view
            ViewBag.IsCoordinator = isCoordinator;
            return View(actor);
        }


        // GET: Actor/Create
        [Authorize(Roles = "Executive")]
        public ActionResult Create()
        {
            var viewModel = new ActorAddViewModel();
            return View(viewModel);
        }

        // POST: Actor/Create
        // POST: Actor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActorAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                viewModel.Executive = User.Identity.Name;

                var addedActor = m.AddActor(viewModel);

                if (addedActor != null)
                {
                    return RedirectToAction("Details", new { id = addedActor.Id });
                }
            }

            List<string> errorMessages = new List<string>();

            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errorMessages.Add(error.ErrorMessage);
                }
            }

            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Coordinator")]
        [Route("Actors/{id}/AddShow")]
        public ActionResult AddShow(int id)
        {
            var actor = m.GetActorById(id);
            if (actor == null) return HttpNotFound();

            var form = new ShowAddFormViewModel
            {
                KnownActorName = actor.Name,
                Actors = new MultiSelectList(m.GetAllActors(), "Id", "Name", new List<int> { id }), // Ensuring actor is selected
                Genres = new SelectList(m.GetAllGenres(), "Name", "Name") // Assuming genres are displayed by name
            };

            return View(form);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        [Route("Actors/{id}/AddShow")]
        public ActionResult AddShow(int id, ShowAddFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Convert ShowAddFormViewModel to ShowAddViewModel
            var showModel = new ShowAddViewModel
            {
                Name = model.Name,
                Genre = model.Genre,
                ReleaseDate = model.ReleaseDate,
                ImageUrl = model.ImageUrl,
                SelectedActorIds = model.Actors.Items.Cast<SelectListItem>()
                                      .Where(item => item.Selected)
                                      .Select(item => int.Parse(item.Value))
                                      .ToList()
            };

            var addedShow = m.AddShow(showModel);

            if (addedShow == null) return View(model);

            return RedirectToAction("Details", "Show", new { id = addedShow.Id });
        }


        // GET: Actor/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Actor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Actor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Actor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
