using BradyTask.Models;
using BradyTask.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BradyTask.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly string _openWeatherOneCallExclude;
        private readonly string _openWeatherOneCallUnits;
        private readonly string _openWeatherOneCallQueryStart;
        private readonly string _openWeatherGeoCodingQueryStart;
        private readonly string _openWeatherGeoCodingLimit;
        private readonly string _openWeatherApiKey;


        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _openWeatherApiKey = _configuration["OpenWeatherApiSettings:ApiKey"];
            _openWeatherOneCallExclude = _configuration["OpenWeatherApiSettings:OneCallSettings:Exclude"];
            _openWeatherOneCallUnits = _configuration["OpenWeatherApiSettings:OneCallSettings:Units"];
            _openWeatherOneCallQueryStart = _configuration["OpenWeatherApiSettings:OneCallSettings:QueryStart"];
            _openWeatherGeoCodingQueryStart = _configuration["OpenWeatherApiSettings:GeoCodingSettings:QueryStart"];
            _openWeatherGeoCodingLimit = _configuration["OpenWeatherApiSettings:GeoCodingSettings:Limit"];
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(WeatherSearchQuery searchQuery)
        {
            if (ModelState.IsValid)
            {
                var httpClient = _httpClientFactory.CreateClient("OpenWeather");

                var locationData = await GetLocationData(httpClient, searchQuery.Location);
                if (locationData != null)
                {
                    var weatherForecast = await GetWeatherData(httpClient, locationData);
                    if (weatherForecast.ErrorMessage != null)
                    {
                        ViewBag.ErrorMessage = locationData.ErrorMessage;
                    }
                    ViewBag.Location = searchQuery.Location;
                    TempData["WeatherForecast"] = weatherForecast;
                }
                else {
                    ViewBag.ErrorMessage = 
                        locationData == null 
                        ? "Location does not exist" 
                        : locationData.ErrorMessage;
                }
            }
            return View();
        }

        private async Task<LocationData> GetLocationData(HttpClient httpClient, string locationName)
        {
            var httpRequestMessage = 
                $"{_openWeatherGeoCodingQueryStart}{locationName}" +
                $"&limit={_openWeatherGeoCodingLimit}" +
                $"&appid={_openWeatherApiKey}";
            var httpResponseMessage = await httpClient.GetAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                var locationList =
                    await JsonSerializer.DeserializeAsync<List<LocationData>>(contentStream);
                return locationList.FirstOrDefault();
            }
            else
            {
                _logger.LogError($"{httpResponseMessage.RequestMessage.RequestUri} - {httpResponseMessage.ReasonPhrase}");
                return new LocationData(){
                    ErrorMessage = "There was a problem with the server. Please try again or contact support."
                };
            }
        }

        private async Task<WeatherData> GetWeatherData(HttpClient httpClient, LocationData locationData)
        {
            var httpRequestMessage =
                $"{_openWeatherOneCallQueryStart}" +
                $"lat={locationData.Lat}" +
                $"&lon={locationData.Lon}" +
                $"&exclude={_openWeatherOneCallExclude}" +
                $"&units={_openWeatherOneCallUnits}" +
                $"&appid={_openWeatherApiKey}";
            var httpResponseMessage = await httpClient.GetAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<WeatherData>(contentStream);
            }
            else
            {
                _logger.LogError($"{httpResponseMessage.RequestMessage.RequestUri} - {httpResponseMessage.ReasonPhrase}");
                return new WeatherData()
                {
                    ErrorMessage = "There was a problem with the server. Please try again or contact support."
                };
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
