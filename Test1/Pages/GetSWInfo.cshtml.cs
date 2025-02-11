using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Test1.Pages
{
    public class GetSWInfoModel : PageModel
    {
        public string PlanetName { get; set; } = "Тут будет название планеты";
        public string PlanetGravity { get; set; } = "Тут будет указана информация о гравитации";
        public string PlanetResidents { get; set; } = "Тут будут имена резидентов";
        private string Planet1Url { get; set; } = "https://swapi.dev/api/planets/1/";
        private string Planet2Url { get; set; } = "https://swapi.dev/api/planets/2/";

        public class Resident
        {
            public string name { get; set; }
        }

        public class PlanetResponse
        {
            public string name { get; set; }
            public string gravity { get; set; }
            public List<string> residents { get; set; }
        }

        public async Task OnPost(IFormCollection form)
        {
            string action = form["action"];
            using HttpClient client = new();
            string tempUrl = string.Empty;

            if (action == "first")
                tempUrl = Planet1Url;
            else if (action == "second")
                tempUrl = Planet2Url;

            var response = await client.GetAsync(tempUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var planetData = JsonSerializer.Deserialize<PlanetResponse>(json);

                PlanetName = $"Название планеты : {planetData.name}";
                PlanetGravity = $"Гравитация планеты : {planetData.gravity}";
 
                var residentsNamesTasks = planetData.residents.Select(residentUrl => GetResidentName(residentUrl, client));
                string[] residentNames = await Task.WhenAll(residentsNamesTasks);

                PlanetResidents = $"Резиденты планеты : {string.Join(", ", residentNames)}";
            }
        }

        static async Task<string> GetResidentName(string url, HttpClient client)
        {
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var residentData = JsonSerializer.Deserialize<Resident>(json);
                return residentData.name;
            }
            return "Unknown";
        }

    }
}
