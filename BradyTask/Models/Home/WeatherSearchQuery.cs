using System.ComponentModel.DataAnnotations;

namespace BradyTask.Models.Home
{
    public class WeatherSearchQuery
    {
        [Required]
        public string Location { get; set; }
    }
}
