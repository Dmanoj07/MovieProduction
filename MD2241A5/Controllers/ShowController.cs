using MD2241A5.Data;
using MD2241A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MD2241A5.Controllers
{
    public class ShowController : Controller
    {
        // GET: Show
        private Manager m = new Manager();
        public ActionResult Index()
        {
            var shows = m.GetAllShows();

            if (shows == null || !shows.Any())
            {
                
                return HttpNotFound();
            }

            return View(shows);

        }

        // GET: Show/Details/5
        public ActionResult Details(int id)
        {
            var show = m.GetShowsById(id); 

            if (show == null)
            {
                return HttpNotFound();
            }

            return View(show);
        }


        [HttpGet]
        [Authorize(Roles = "Coordinator")]
        public ActionResult Create(int? actorId)
        {
            
            string knownActorName = null;

            if (actorId.HasValue)
            {
                var actor = m.GetActorById(actorId.Value);
                if (actor == null)
                {
                    return HttpNotFound("Actor not found.");
                }
                knownActorName = actor.Name;
            }

            var allActors = m.GetAllActors().Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name,
               
                Selected = actorId.HasValue && a.Id == actorId.Value
            }).ToList();

            var viewModel = new ShowAddFormViewModel
            {
                KnownActorName = knownActorName,
                Actors = new MultiSelectList(allActors, "Value", "Text"),
                Genres = new SelectList(m.GetAllGenres(), "Name", "Name"),
                SelectedActorIds = actorId.HasValue ? new[] { actorId.Value } : new int[] { }
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public ActionResult Create(ShowAddFormViewModel viewModel)
        {
           
            viewModel.Actors = new MultiSelectList(m.GetAllActors(), "Id", "Name");

      
            if (!ModelState.IsValid)
            {
                viewModel.Genres = new SelectList(m.GetAllGenres(), "Name", "Name");
                return View(viewModel);
            }

         
            var showModel = new ShowAddViewModel
            {
                Name = viewModel.Name,
                Genre = viewModel.Genre,
                ReleaseDate = viewModel.ReleaseDate,
                ImageUrl = viewModel.ImageUrl,
                SelectedActorIds = viewModel.SelectedActorIds
            };

          
            var result = m.AddShow(showModel);
            if (result != null)
            {
         
                return RedirectToAction("Details", new { id = result.Id });
            }
            else
            {
        
                ModelState.AddModelError("", "Unable to save the show, please try again.");
                return View(viewModel);
            }
        }
        [Authorize(Roles = "Clerk")]
        // GET: Shows/{id}/AddEpisode
        [Route("Shows/{id}/AddEpisode")]
        public ActionResult AddEpisode(int id)
        {
    
            var showDetails = m.GetShowsById(id);
            if (showDetails == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EpisodeAddFormViewModel
            {
                ShowId = id,
                ShowName = showDetails.Name,
                GenreList = new SelectList(m.GetAllGenres(), "Id", "Name")
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Clerk")]
        [Route("Shows/{showId}/AddEpisode")]  
        [ValidateAntiForgeryToken]
        public ActionResult AddEpisode(int showId, EpisodeAddFormViewModel model)
        {
           
            if (model.ShowId != showId)
            {
                ModelState.AddModelError("", "There was an error with the show information. Please try again.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.GenreList = new SelectList(m.GetAllGenres(), "Id", "Name", model.Genre);
                return View(model);
            }

           
            model.Clerk = User.Identity.Name;
            var addedEpisode = m.AddEpisode(model);
            if (addedEpisode != null)
            {
                return RedirectToAction("EpisodeDetails", "Episode", new { id = addedEpisode.Id });  
            }
            else
            {
    
                ModelState.AddModelError("", "Unable to add the episode. Please check your data and try again.");
                model.GenreList = new SelectList(m.GetAllGenres(), "Id", "Name", model.Genre);
                return View(model);
            }
        }

        // GET: Show/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Show/Edit/5
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

        // GET: Show/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Show/Delete/5
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
