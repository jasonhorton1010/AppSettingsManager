using AppSettingsManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AppSettingsManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly TwilioSettings _twilioSettings;
        private readonly IOptions<TwilioSettings> _twilioOptions;
        private readonly IOptions<SocialLoginSettings> _socialLoginOptions;

        public HomeController(ILogger<HomeController> logger, 
            IConfiguration configuration, 
            IOptions<TwilioSettings> twilioOptions,
            TwilioSettings twilioSettings,
            IOptions<SocialLoginSettings> socialLoginOptions)
        {
            _logger = logger;
            _configuration = configuration;

            // The following two lines are a technique to BIND (here in the HomeController the TwilioSettings class to the Twilio App Section.
            //_twilioSettings = new TwilioSettings();
            //configuration.GetSection("Twilio").Bind(_twilioSettings);

            _twilioOptions = twilioOptions;

            // This following line is to injected TwilioSettings "BIND" class from the program.cs
            _twilioSettings = twilioSettings;

            _socialLoginOptions = socialLoginOptions;
        }

        public IActionResult Index()
        {
            ViewBag.SendGridKey = _configuration.GetValue<String>("SendGridKey");
            //// One way to get a sectionof config: notice the use of GetSection
            //ViewBag.TwilioAuthToken = _configuration.GetSection("Twilio").GetValue<String>("AuthToken");
            //// Another way to get a section of config - notice the colon in the Twilio:AccountSid
            //ViewBag.TwilioAccountSid = _configuration.GetValue<String>("Twilio:AccountSid");

            // Using a class for App Settings where it has been "BIND" to an object - see_twilioSettings
            // ViewBag.TwilioPhoneNumber = _twilioSettings.PhoneNumber;


            // The three lines below is showing how to get the values from IOptions usage (see _twilioOptions in Constructor)
            //ViewBag.TwilioAuthToken = _twilioOptions.Value.AuthToken;
            //ViewBag.TwilioAccountSid = _twilioOptions.Value.AccountSid;
            //ViewBag.TwilioPhoneNumber = _twilioOptions.Value.PhoneNumber;

            // The following three lines is showing how to get the values from twulioSettings when the class is bound pfr
            ViewBag.TwilioAuthToken = _twilioSettings.AuthToken;
            ViewBag.TwilioAccountSid = _twilioSettings.AccountSid;
            ViewBag.TwilioPhoneNumber = _twilioSettings.PhoneNumber;

            ViewBag.BottomLevelSetting = _configuration.GetValue<String>("FirstLevelSetting:SecondLevelSetting:BottomLevelSetting");

            ViewBag.FacebookKey = _socialLoginOptions.Value.FacebookSettings.Key;
            ViewBag.GoogleKey = _socialLoginOptions.Value.GoogleSettings.Key;

            ViewBag.ConnectionString = _configuration.GetConnectionString("AppSettingsManagerDb");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}