using Agenda.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

//https://console.cloud.google.com/apis/
//https://www.youtube.com/watch?v=56WZ1D4lVo8
//https://www.youtube.com/watch?v=wA-Iy6ThYz4
//https://www.youtube.com/watch?v=Jt9vSY802mM
//http://www.dotnetawesome.com/2017/07/curd-operation-on-fullcalendar-in-aspnet-mvc.html
//http://www.dotnetawesome.com/2017/06/event-calendar-in-aspnet-mvc.html
//https://fullcalendar.io/
// https://fullcalendar.io/docs/Calendar-addEvent-demo

namespace Agenda.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public List<string> GoogleEvents = new List<string>();
        string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        string ApplicationName = "Google Calendar API .NET QuicStart";

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment; 
        }

        //Com Full Calendar
        public IActionResult Index()
        {
            

            return View();
        }


        //Com API do Google - Precisa conta do google
        public IActionResult IndexGoogle()
        {
            CalendarEvents();
            ViewBag.EventList = GoogleEvents;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public void CalendarEvents()
        {
            UserCredential credential;

            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "Credentials.json");

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                ).Result;
            }

            //Create Google Calendar API Service
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "pcfuncional",
            });

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events
            Events events = request.Execute();

            if(events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    GoogleEvents.Add(eventItem.Summary);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}