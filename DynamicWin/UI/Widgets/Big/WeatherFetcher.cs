using Newtonsoft.Json;
using System.Net.Http;
using System.Xml;

namespace DynamicWin.UI.Widgets.Big;

public class WeatherFetcher
{
    struct Location
    {
        public string city;
        public string region;
        public string country;
        public string loc;
    }

    public struct WeatherData
    {
        public string city;
        public string region;
        public string weatherText;
        public string temperatureCelsius;
        public string temperatureFahrenheit;
    }

    private WeatherData weatherData = new();
    public WeatherData Weather => weatherData;

    public Action<WeatherData> onWeatherDataReceived;

    public void Fetch()
    {
        Task.Run(async () =>
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://ipinfo.io/geo");
            var location = JsonConvert.DeserializeObject<Location>(response);

            var lat = location.loc.Split(',')[0];
            var lon = location.loc.Split(',')[1];

            System.Diagnostics.Debug.WriteLine($"Latitude: {lat}, Longitude: {lon}");

            string temp = string.Empty;
            string weather = string.Empty;

            XmlTextReader? reader = null;
            try
            {
                string sAddress = $"https://tile-service.weather.microsoft.com/livetile/front/{lat},{lon}";

                int nCpt = 0;

                reader = new XmlTextReader(sAddress)
                {
                    WhitespaceHandling = WhitespaceHandling.None
                };
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (nCpt == 1)
                            temp = reader.Value;
                        else if (nCpt == 2)
                            weather = reader.Value;
                        nCpt++;
                    }
                }
            }
            finally
            {
                reader?.Close();
            }

            string tempF = temp.Replace("°", "");
            double tempC = (double.Parse(tempF) - 32.0) * 5 / 9;
            string tempCText = tempC.ToString("#.#");

            System.Diagnostics.Debug.WriteLine(string.Format("{0}, {1}F({2}°C), {3}", location.city, temp, tempCText, weather));

            weatherData = new WeatherData() { city = location.city, region = location.region, temperatureCelsius = tempCText + "°C", temperatureFahrenheit = tempF + "F", weatherText = weather };
            onWeatherDataReceived?.Invoke(weatherData);

            Thread.Sleep(120000);

            Fetch();
        });
    }
}
