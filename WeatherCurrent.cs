using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetActualWeatherConsole
{
    public class WeatherCurrent
    {
        public Temperature main;
        public string name;
        public Wind wind;
        public Coordinates coord;
        public WeatherNow[] weather;
    }
    public class Coordinates
    {
        public string lon;
        public string lat;
    }
    public class Temperature
    {
        public double temp;
        public double feels_like;
        public double temp_min;
        public double temp_max;
        public double pressure;
        public double humidity;
    }
    public class Wind
    {
        public double speed;
    }
    public class WeatherNow
    {
        public string description;
    }
}
