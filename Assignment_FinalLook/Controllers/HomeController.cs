using Assignment_FinalLook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assignment_FinalLook.DataAccess;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assignment_FinalLook.Controllers
{
    public class HomeController : Controller
    {
        HttpClient httpClient;
        static string API_KEY = "4d2QXxfxKWafhyHR06q3vbCmKRJcTNtVF6GNWIRw"; //Add your API key here inside ""

        static string BASE_URL = "hhttps://developer.nps.gov/api/v1/parks?state";

        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        private readonly ILogger<HomeController> _logger;


        public IActionResult insertData()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //string NATIONAL_PARK_API_PATH = BASE_URL + "/parks?limit=20";
            httpClient.BaseAddress = new Uri(BASE_URL);

            string parksData = "";
            Park parks = null;
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(BASE_URL)
                                                        .GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    parksData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!parksData.Equals(""))
                {
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    parks = JsonConvert.DeserializeObject<Park>(parksData);
                }

                dbContext.Park.Add(parks);
                dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Parks")]
        public IActionResult Parks()
        {

            return View();
        }


        [Route("Activities")]
        public IActionResult Activities()
        {

            return View();
        }

        [Route("SearchParks")]
        public IActionResult SearchParks(string search)
        {
            Dictionary<string, int> zips =new Dictionary<string,int>();
            zips.Add("florida", 33614); zips.Add("ohio", 44264); zips.Add("california", 95389); zips.Add("texas", 79834); zips.Add("michigan", 38124);
            zips.Add("utah", 84767); zips.Add("alaska", 99755); zips.Add("illinois", 62234); zips.Add("arizona", 86001); zips.Add("alabama", 35203);
            zips.Add("kansas", 66701); zips.Add("maryland", 20601); zips.Add("newyork", 10007); zips.Add("indiana", 46304); zips.Add("colorado", 80517);
            zips.Add("oregon", 33614); zips.Add("missouri", 33614);
            if (search != null)
            {
                States stated = dbContext.States.Where(c => c.stateName == search).FirstOrDefault();
                try
                {
                    int x = 0;
                    if (zips.TryGetValue(stated.stateName,out x))
                    { ViewBag.zip = x; }
                    else
                        ViewBag.zip = 30412;
                }
                catch(Exception e)
                {

                }
                if (stated != null)
                {
                    List<Park> data = dbContext.Park.Include(c => c.State).Where(c => c.State.stateCode == stated.stateCode).ToList();
                    ViewBag.data = data;
                    ViewBag.state = stated.stateName;
                }
            }
            return View("Parks");
        }


        public IActionResult GetActivityDetails(string parkname)
        {
            if (parkname != null)
            {
                Park park = dbContext.Park.Where(c => c.parkName.Contains(parkname)).FirstOrDefault();
                if (park != null)
                {
                    List<Activities> Act = dbContext.Activities.Include(c => c.parkId).Where(c => c.parkId.parkId == park.parkId).ToList();
                    List<Events> evnt = dbContext.Events.Include(c => c.parkId).Include(c => c.parkId.State).Where(c => c.parkId.parkId == park.parkId).ToList();
                    ViewBag.Activities = Act;
                    ViewBag.Events = evnt;
                    ViewBag.parkname = park.parkName;
                    ViewBag.parkid = park.parkId;

                }
            }

            return View("Activities");
        }


        [Route("GetActivities")]
        public IActionResult GetActivities(string parkId)
        {
            if(parkId != null)
            {
                Park park= dbContext.Park.Where(c =>c.parkId==parkId).FirstOrDefault();
                List<Activities> Act = dbContext.Activities.Include(c=>c.parkId).Include(c=>c.parkId.State).Where(c => c.parkId.parkId == parkId).ToList();
                List<Events> evnt = dbContext.Events.Include(c => c.parkId).Include(c => c.parkId.State).Where(c => c.parkId.parkId == parkId).ToList();
                ViewBag.Activities = Act;
                ViewBag.Events = evnt;
                ViewBag.parkname = park.parkName;
                ViewBag.parkid = park.parkId;
            }
            return View("Activities");
        }

        [Route("ReportError")]
        public IActionResult ReportError(string parkId)
        {
            Park p = dbContext.Park.Include(c => c.State).Where(c => c.parkId == parkId).FirstOrDefault();
            ViewBag.state = p.State.stateName;
            ViewBag.parkname = p.parkName;
            return View();
        }
        
        public IActionResult GetUserDetails(string firstname, string lastname,string email,string phone,string parkname,string activityname, string city,string zip)
        {
            Users user = new Users();
            user.FirstName = firstname;
            user.LastName = lastname;
            user.email = email;
            user.ParkName = parkname;
            user.ActivityName = activityname;
            user.phone = phone;
            user.city = city;
            dbContext.Users.Add(user);

            Park p = dbContext.Park.Where(c => c.parkName == parkname).FirstOrDefault();
           
            Activities temp = dbContext.Activities.Where(c => c.parkId.parkId == p.parkId).Where(c => c.ActivityName.ToLower() == activityname.ToLower()).FirstOrDefault();
            if (temp != null)
                dbContext.Activities.Remove(temp);
            
            Events temp1 = dbContext.Events.Where(c => c.parkId.parkId == p.parkId).Where(c => c.EventName.ToLower() == activityname.ToLower()).FirstOrDefault();
            
            if (temp1 != null)
                dbContext.Events.Remove(temp1);
            dbContext.SaveChanges();

            return View("UpdateDataBaseEvent");
        }

        [Route("Stats")]
        public IActionResult Stats()
        {
            string[] ChartColors = new string[] { "#FFA07A", "#E9967A", "#FA8072", "#F08080", "#FF7F50", "#FFA500", "#BDB76B", "#90EE90", "#8FBC8F", "#AFEEEE", "#B0E0E6", "#ADD8E6", "#A9A9A9", "#C0C0C0", "#FFE4C4", "#D2B48C", "#FFF8DC", "#708090", "#D8BFD8", "#6495ED", "#5F9EA0", "#40E0D0", "#BDB76B", "#D3D3D3", "#DDA0DD", "#DA70D6"};
            List<string> states = new List<string>();
            states.Add("Florida"); states.Add("Ohio"); states.Add("California"); states.Add("Michigan");
            states.Add("Utah"); states.Add("Alaska"); states.Add("Illinois"); states.Add("Alabama");
            states.Add("Arizona"); states.Add("Kansas"); states.Add("Missouri"); states.Add("NEWYORK");
            states.Add("Oregon"); states.Add("Maryland"); states.Add("New Jersey");
            states.Add("Texas"); states.Add("Oklahoma"); states.Add("Indiana"); states.Add("Colorado");
            List<int> counts = new List<int>();
            for(int i=0;i<states.Count;i++)
            {
                var x = states[i];
                var state = dbContext.States.Where(c => c.stateName == x).FirstOrDefault();
                if (state == null)
                    counts.Add(0);
                else
                {
                    var parks = dbContext.Park.Where(c => c.State.stateCode == state.stateCode).ToList();
                    counts.Add(parks.Count);
                }
            }
            ViewBag.parkcounts = String.Join(",", counts.Select(d =>d));
            ViewBag.States = String.Join(",", states.Select(d => "'" + d + "'"));
            ViewBag.colors=String.Join(",", ChartColors.Select(d => "'" + d + "'"));

            return View();
        }

        public IActionResult DeletePark(string parkId)
        {
            var x = dbContext.Park.Include(c=>c.State).Where(c => c.parkId == parkId).FirstOrDefault();
            List<Events> evnt = dbContext.Events.Where(c => c.parkId.parkId == parkId).ToList();
            List<Activities> Act = dbContext.Activities.Where(c => c.parkId.parkId == parkId).ToList();
            dbContext.Activities.RemoveRange(Act);
            dbContext.Events.RemoveRange(evnt);
            dbContext.Park.Remove(x);
            dbContext.SaveChanges();
            /*
            Park p = new Park();
            p.parkId = x.parkId;
            p.parkName = x.parkName;
            p.Address = x.Address;
            p.EntryFee = x.EntryFee;
            p.State = x.State;
            p.PhoneNuber = x.PhoneNuber;
            p.website = x.website;

            dbContext.Park.Add(p);
            dbContext.SaveChanges();
            for (int i=0;i<evnt.Count;i++)
            {
                Events ev = new Events();
                ev.EventName = evnt[i].EventName;
                ev.EntryFee = evnt[i].EntryFee;
                ev.parkId = p;
                dbContext.Events.Add(ev);
            }

            for(int i=0;i<Act.Count;i++)
            {
                Activities av = new Activities();
                av.ActivityName = Act[i].ActivityName;
                av.EntryFee = Act[i].EntryFee;
                av.parkId = p;
                dbContext.Activities.Add(av);
            }
            dbContext.SaveChanges();
            Above Code is for Testing the Delete Operation.
            */
            return View("UpdateDataBaseEvent");
        }
        public IActionResult UpdateDataBaseEvent(string parkName,String Event)
        {

            return View();
        }


        public IActionResult GetparkDetails(string parkname,string address, string phone, string email,string url,string EntryFee,string state)
        {

            var sta = dbContext.States.Where(c => c.stateName == state).FirstOrDefault();
            Park p = new Park();
            if (parkname != null)
            {
                p.parkId = parkname.Substring(1,4);
                p.parkName = parkname;
                p.Address = address;
                p.PhoneNuber = phone;
                p.EntryFee = Int32.Parse(EntryFee);
                p.State = sta;
                p.website = url;

                Events ev = new Events();
                ev.EntryFee = 23;
                ev.EventName = "Agriculture";
                ev.parkId = p;

                Events ev1 = new Events();
                ev1.EntryFee = 2;
                ev1.EventName = "Fashion";
                ev1.parkId = p;
                
                Events ev2 = new Events();
                ev2.EntryFee = 233;
                ev2.EventName = "Childern";
                ev2.parkId = p;

                Events ev3 = new Events();
                ev3.EntryFee = 123;
                ev3.EventName = "Fishing";
                ev3.parkId = p;

                Activities act = new Activities();
                act.ActivityName = "Fishing";
                act.EntryFee = 123;
                act.parkId = p;

                Activities act1 = new Activities();
                act1.ActivityName = "Boating";
                act1.EntryFee = 223;
                act1.parkId = p;

                Activities act2 = new Activities();
                act2.ActivityName = "Sky Diving";
                act2.EntryFee = 153;
                act2.parkId = p;

                Activities act3 = new Activities();
                act3.ActivityName = "Paragliding";
                act3.EntryFee = 13;
                act3.parkId = p;

                Activities act4 = new Activities();
                act4.ActivityName = "Swimming";
                act4.EntryFee = 13;
                act4.parkId = p;

                dbContext.Park.Add(p);
                dbContext.SaveChanges();
                dbContext.Activities.Add(act1);
                dbContext.Activities.Add(act2);
                dbContext.Activities.Add(act3);
                dbContext.Activities.Add(act4);
                dbContext.Events.Add(ev1);
                dbContext.Events.Add(ev2);
                dbContext.Events.Add(ev);
                dbContext.Events.Add(ev3);
                dbContext.SaveChanges();

            }
            return View();
        }
        public IActionResult AddPark()
        {
            return View();
        }

        public IActionResult UpdatePark(string parkId)
        {
           var park= dbContext.Park.Include(c=>c.State).Where(c => c.parkId == parkId).FirstOrDefault();
            ViewBag.parkname = park.parkName;
            ViewBag.parkId = park.parkId;
            ViewBag.EntryFee = park.EntryFee;
            ViewBag.url = park.website;
            ViewBag.address = park.Address;
            ViewBag.phone = park.PhoneNuber;
            ViewBag.state = park.State.stateName;
            return View();
        }

        public IActionResult UpdateparkDetails( string parkId,string parkname, string address, string phone, string url, string EntryFee)
        {
            var park=dbContext.Park.Where(c => c.parkId == parkId).FirstOrDefault();
            park.parkName = parkname;
            park.Address = address;
            park.PhoneNuber = phone;
            park.website = url;
            park.EntryFee = Int32.Parse(EntryFee);
            dbContext.Park.Update(park);
            dbContext.SaveChanges();
            return View("DetailsUpdated");
        }

        [Route("FAQ")]
        public IActionResult FAQ()
        {

            return View();
        }
    }
}
