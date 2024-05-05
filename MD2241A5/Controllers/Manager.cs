using AutoMapper;
using MD2241A5.Data;
using MD2241A5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Security.Claims;
using System.Web;
using static System.Net.WebRequestMethods;

// ************************************************************************************
// WEB524 Project Template V2 == 2231-1d09539b-8387-47fc-8850-8407604c42c0
//
// By submitting this assignment you agree to the following statement.
// I declare that this assignment is my own work in accordance with the Seneca Academic
// Policy. No part of this assignment has been copied manually or electronically from
// any other source (including web sites) or distributed to other students.
// ************************************************************************************

namespace MD2241A5.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper instance
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            var config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Product, ProductBaseViewModel>();

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();
                cfg.CreateMap<Genre,GenreBaseViewModel>();
                cfg.CreateMap<Actor, ActorBaseViewModel>();
                cfg.CreateMap<Actor, ActorWithDetailViewModel>();
                cfg.CreateMap<ActorAddViewModel, Actor>();
                cfg.CreateMap<Actor, ActorAddViewModel>();
                cfg.CreateMap<Show, ShowBaseViewModel>();
                cfg.CreateMap<Show, ShowWithDetailViewModel>()
                   .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors))
                   .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes))
                   .MaxDepth(2);  // Limit recursion depth
           
            cfg.CreateMap<ShowAddViewModel, Show>();
                cfg.CreateMap<ShowAddViewModel, ShowAddFormViewModel>();
                cfg.CreateMap<Episode, EpisodeBaseViewModel>();
                cfg.CreateMap<Episode, EpisodeWithDetailViewModel>()
                .ForMember(dest => dest.Show, opt => opt.MapFrom(src => new ShowBaseViewModel
                {
                    Id = src.Show.Id,
                    Name = src.Show.Name,
                    Genre = src.Show.Genre,
                    ReleaseDate = src.Show.ReleaseDate,
                    ImageUrl = src.Show.ImageUrl,
                    Coordinator = src.Show.Coordinator
                }));
                cfg.CreateMap<EpisodeAddViewModel,Episode >();
                cfg.CreateMap<EpisodeAddViewModel, EpisodeAddFormViewModel>();
                cfg.CreateMap<EpisodeAddFormViewModel, Episode>()
                    .ForMember(dest => dest.Show, opt => opt.Ignore())  
                    .ForMember(dest => dest.Clerk, opt => opt.Ignore());
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }


        // Add your methods below and call them from controllers. Ensure that your methods accept
        // and deliver ONLY view model objects and collections. When working with collections, the
        // return type is almost always IEnumerable<T>.
        //
        // Remember to use the suggested naming convention, for example:
        // ProductGetAll(), ProductGetById(), ProductAdd(), ProductEdit(), and ProductDelete().

        public IEnumerable<GenreBaseViewModel> GetAllGenres()
        {
            return mapper.Map<IEnumerable<Genre>,IEnumerable<GenreBaseViewModel>>(ds.Genres);
        }
        public IEnumerable<ActorBaseViewModel> GetAllActors()
        {
            return mapper.Map<IEnumerable<Actor>, IEnumerable<ActorBaseViewModel>>(ds.Actors);
        }

        // Method to get one actor with detail
        public ActorWithDetailViewModel GetActorById(int id)
        {
            var obj = ds.Actors.Find(id);
            return obj ==null?null : mapper.Map<Actor, ActorWithDetailViewModel>(obj);
        }

        // Method to add a new actor
        public ActorAddViewModel AddActor(ActorAddViewModel newActor)
        {
            newActor.Executive = HttpContext.Current.User.Identity.Name;
            var addedItem = ds.Actors.Add(mapper.Map<ActorAddViewModel, Actor>(newActor));
            ds.SaveChanges();
            // If successful, return the added item (mapped to a view model class).
            return addedItem == null ? null : mapper.Map<Actor, ActorAddViewModel>(addedItem);

        }
      
        public IEnumerable<ShowBaseViewModel> GetAllShows()
        {
            return mapper.Map<IEnumerable<Show>, IEnumerable<ShowBaseViewModel>>(ds.Shows);
        }


        public ShowWithDetailViewModel GetShowsById(int id)
{
    var show = ds.Shows
                 .Where(s => s.Id == id)
                 .Select(s => new ShowWithDetailViewModel
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Genre = s.Genre,
                     ReleaseDate = s.ReleaseDate,
                     ImageUrl = s.ImageUrl,
                     Coordinator = s.Coordinator,
                     Actors = s.Actors.Select(a => new ActorBaseViewModel {}),
                     Episodes = s.Episodes.Select(e => new EpisodeBaseViewModel {})
                 }).FirstOrDefault();
    
    return show;
}

  

        public ShowBaseViewModel AddShow(ShowAddViewModel showModel)
        {
            var show = mapper.Map<ShowAddViewModel, Show>(showModel);
            show.Coordinator = HttpContext.Current.User.Identity.Name;

            // Associate the show with the selected actors
            show.Actors = showModel.SelectedActorIds.Select(id => ds.Actors.Find(id)).ToList();

            ds.Shows.Add(show);
            ds.SaveChanges();

            return mapper.Map<Show, ShowBaseViewModel>(show);
        }

        public IEnumerable<EpisodeWithDetailViewModel> GetAllEpisodes()
        {
            return mapper.Map<IEnumerable<Episode>, IEnumerable<EpisodeWithDetailViewModel>>(ds.Episodes.Include("Show"));
        }

      
        public EpisodeWithDetailViewModel GetEpisodesById(int id)
        {
            var obj = ds.Episodes
                        .Include("Show")  
                        .SingleOrDefault(e => e.Id == id);

            return obj == null ? null : mapper.Map<Episode, EpisodeWithDetailViewModel>(obj);
        }


        public EpisodeBaseViewModel AddEpisode(EpisodeAddFormViewModel newEpisode)
        {
            var show = ds.Shows.Find(newEpisode.ShowId);  
            if (show == null)
            {
               
                throw new ArgumentException("No show found with the specified ID.");
            }

            var episode = mapper.Map<Episode>(newEpisode);

            episode.Clerk = HttpContext.Current.User.Identity.Name; 
            episode.ShowId = show.Id; 

            try
            {
                ds.Episodes.Add(episode);
                ds.SaveChanges();
                return mapper.Map<Episode, EpisodeBaseViewModel>(episode);
            }
            catch (DbEntityValidationException ex)
            {
                // Log detailed error messages
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine($"Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                    }
                }
                throw; // Re-throw to handle or log at a higher level
            }
        }


        // *** Add your methods above this line **


        #region Role Claims

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        #endregion

        #region Load Data Methods

        // Add some programmatically-generated objects to the data store
        // You can write one method or many methods but remember to
        // check for existing data first.  You will call this/these method(s)
        // from a controller action.

        public bool LoadData()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // *** Role claims ***
            if (ds.RoleClaims.Count() == 0)
            {
                // Add role claims here

                //ds.SaveChanges();
                //done = true;
            }

            return done;
        }

        public bool RemoveData()
        {
            try
            {
                // Remove episodes first
                foreach (var e in ds.Episodes)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }

                // Remove shows
                foreach (var s in ds.Shows)
                {
                    ds.Entry(s).State = System.Data.Entity.EntityState.Deleted;
                }

                // Remove actors
                foreach (var a in ds.Actors)
                {
                    ds.Entry(a).State = System.Data.Entity.EntityState.Deleted;
                }

                // Remove genres
                foreach (var g in ds.Genres)
                {
                    ds.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                }

                // Remove roles
                foreach (var r in ds.RoleClaims)
                {
                    ds.Entry(r).State = System.Data.Entity.EntityState.Deleted;
                }

                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void LoadRoles()
        {
            if (!ds.RoleClaims.Any())
            {
                ds.RoleClaims.Add(new RoleClaim { Name = "Admin" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Executive" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Clerk" });
                ds.SaveChanges();
            }
        }

        public void LoadGenres()
        {
            if (!ds.Genres.Any())
            {
                var genres = new List<Genre>
        {
            new Genre { Name = "Action" },
            new Genre { Name = "Comedy" },
            new Genre { Name = "Drama" },
            new Genre { Name = "Romance" },
            new Genre { Name = "Sci-Fi" },
            new Genre { Name = "Horror" },
            new Genre { Name = "Documentary" },
            new Genre { Name = "Animation" },
            new Genre { Name = "Musical" },
            new Genre { Name = "Adventure" }
        };

                ds.Genres.AddRange(genres);
                ds.SaveChanges();
            }
        }

        public void LoadActors()
        {
            if (!ds.Actors.Any())
            {
                var actors = new List<Actor>
        {
            new Actor
            {
                Name = "Dwayne Johnson",
                AlternateName = "The Rock",
                BirthDate = new DateTime(1972, 5, 2),
                Height = 1.96,
                ImageUrl = "https://i.imgur.com/M1rkS1F.jpeg",
                Executive = User.Name
            },
            new Actor
            {
                Name = "Emma Stone",
                BirthDate = new DateTime(1988, 11, 6),
                Height = 1.68,
                ImageUrl = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FEmma_Stone&psig=AOvVaw1rT05HIdnhfFfjUD0tzPWk&ust=1712683925536000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCMirk_uSs4UDFQAAAAAdAAAAABAE",
                Executive = User.Name
            },
            new Actor
            {
                Name = "Tom Hardy",
                BirthDate = new DateTime(1977, 9, 15),
                Height = 1.78,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/b/b4/Tom_Hardy_by_Gage_Skidmore.jpg",
                Executive = User.Name
            }
        };

                ds.Actors.AddRange(actors);
                ds.SaveChanges();
            }
        }

        public void LoadShows()
        {
            if (!ds.Shows.Any())
            {
                var theRock = ds.Actors.SingleOrDefault(a => a.Name == "Dwayne Johnson");

                var shows = new List<Show>
        {
            new Show
            {
                Name = "WWE Smackdown!",
                Genre = "Action",
                ReleaseDate = new DateTime(1999, 8, 26),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/a/a1/WWE_SmackDown_Logo.svg/1200px-WWE_SmackDown_Logo.svg.png",
                Coordinator = User.Name,
                Actors = new HashSet<Actor> { theRock }
            },
            new Show
            {
                Name = "Jumanji: Welcome to the Jungle",
                Genre = "Action",
                ReleaseDate = new DateTime(2017, 12, 20),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b9/Jumanji_Welcome_to_the_Jungle.png",
                Coordinator = User.Name,
                Actors = new HashSet<Actor> { theRock }
            }
        };

                ds.Shows.AddRange(shows);
                ds.SaveChanges();
            }
        }

        public void LoadEpisodes()
        {
            if (!ds.Episodes.Any())
            {
                var wweShow = ds.Shows.SingleOrDefault(s => s.Name == "WWE Smackdown!");
                var jumanjiShow = ds.Shows.SingleOrDefault(s => s.Name == "Jumanji: Welcome to the Jungle");

                var episodes = new List<Episode>
        {
            new Episode
            {
                Name = "The Final SmackDown! of the Year 2000",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                Genre = "Action",
                AirDate = new DateTime(2000, 12, 28),
                ImageUrl = "https://i.ytimg.com/vi/GM4uVld-6Uo/maxresdefault.jpg",
                Clerk = User.Name,
                Show = wweShow
            },
            new Episode
            {
                Name = "The Rock's Return to SmackDown!",
                SeasonNumber = 1,
                EpisodeNumber = 2,
                Genre = "Action",
                AirDate = new DateTime(2002, 3, 7),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/d/d0/The_Rock_-_WWE.jpg/220px-The_Rock_-_WWE.jpg",
                Clerk = User.Name,
                Show = wweShow
            },
            new Episode
            {
                Name = "The Intro to the Jungle",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                Genre = "Action",
                AirDate = new DateTime(2017, 12, 20),
                ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTkyNDQ1MDc-NjZlOC00NTc0LThkM2UtNVM0ZDBiYzIwMWU5XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_FMjpg_UX1000_.jpg",
                Clerk = User.Name,
                Show = jumanjiShow
            }
        };

                ds.Episodes.AddRange(episodes);
                ds.SaveChanges();
            }
        }




    }

    #endregion

    #region RequestUser Class

    // This "RequestUser" class includes many convenient members that make it
    // easier work with the authenticated user and render user account info.
    // Study the properties and methods, and think about how you could use this class.

    // How to use...
    // In the Manager class, declare a new property named User:
    //    public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value:
    //    User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
                IsExecutive = user.HasClaim(ClaimTypes.Role, "Executive") ? true : false;
                IsCoordinator = user.HasClaim(ClaimTypes.Role, "Coordinator") ? true : false;
                IsClerk = user.HasClaim(ClaimTypes.Role, "Clerk") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
                IsExecutive = false;
                IsCoordinator = false;
                IsClerk =  false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }

        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }

        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }

        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }
        public bool IsExecutive { get; private set; }
        public bool IsCoordinator { get; private set; }
        public bool IsClerk { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

    #endregion

}