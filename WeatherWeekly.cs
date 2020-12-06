using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetActualWeatherConsole
{
    public class WeatherWeekly
    {
        public Daily[] daily = new Daily[7];
    }
    public class Daily
    {
        public double pressure;
        public string humidity;
        public double wind_speed;
        public Temperature temp;
        public TemperatureFeels feels_like;
        public Weather[] weather;
        public class Temperature
        {
            public string day;
            public string night;
        }
        public class TemperatureFeels
        {
            public string day;
            public string night;
        }
        public class Weather
        {
            public string description;
        }
    }
}
