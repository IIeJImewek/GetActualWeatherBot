using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using MihaZupan; //библиотека для преобразования хттп в сокс5 прокси
using Newtonsoft.Json;


namespace GetActualWeatherConsole
{
    class Program
    {
        private static ITelegramBotClient client;
        static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("195.144.21.185", 1080);
            client = new TelegramBotClient("1314671182:AAEWiBXV_zXyb217Vv5MVST5ugy_C1YAZ1g", proxy) { Timeout = TimeSpan.FromSeconds(10)};

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot id: {me.Id}. Bot name {me.FirstName}");

            client.OnMessage += Bot_onMessage;
            client.OnCallbackQuery += Bot_onCallbackQuery;
            client.StartReceiving();
            Console.ReadKey();
        }
        private static async void Bot_onMessage(object sender, MessageEventArgs e)
        {
            var text = e?.Message.Text;
            if (text == null)
            {
                return;
            }
            if (text == "/start")
            {
                await client.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: "Здравствуйте!\nЭто бот, дающий информацию о погоде. Просто напишите название города и нажмите на необходимую кнопку."
                ).ConfigureAwait(false);
            }
            else
            {
                var ikm = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                    InlineKeyboardButton.WithCallbackData("Какая сейчас погода", $"callback1&{text}")
                    },
                    new[]
                    {
                     InlineKeyboardButton.WithCallbackData("Дай прогноз на неделю", "callback2"),
                    },
                    new[]
                    {
                     InlineKeyboardButton.WithCallbackData("Дай прогноз на месяц", "callback3"),
                    },
                });
                await client.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: $"{text}? Узнать текущую погоду в городе или же дать прогноз на неделю/месяц?",
                    replyMarkup: ikm
                    ).ConfigureAwait(false);
            }
        }

        private static async void Bot_onCallbackQuery(object sender, CallbackQueryEventArgs ev)
        {
            var message = ev.CallbackQuery.Message;
            string data = ev.CallbackQuery.Data;

            string city = data.Split('&')[1];
            ev.CallbackQuery.Data = ev.CallbackQuery.Data.Split('&')[0];

            try
            {
                if (ev.CallbackQuery.Data == "callback1")
                {
                    await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{CurrentWeather(city)}"
                    ).ConfigureAwait(false);
                }
                else
                if (ev.CallbackQuery.Data == "callback2")
                {

                }
                else
                if (ev.CallbackQuery.Data == "callback3")
                {

                }
            }
            catch
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Такой город не найден или что-то пошло не так."
                    ).ConfigureAwait(false);
            }
        }
        public static string CurrentWeather(string text)
        {
            //создаём запрос по ссылке (добавил &units=metric, чтобы писало в цельсиях и &lang=ru чтобы выводило на русском название)
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q=" + text + "&appid=2b087ac0e4797f107850e6d7595e1f87&units=metric&lang=ru");
                //пост метод для отправки данных сервису
                request.Method = "POST";
                //для получения ответа от сервиса
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string answer = string.Empty;
                //правильно используем объекты IDisposable
                using (Stream s = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(s))
                    {
                        answer = reader.ReadToEnd();
                    }
                }
                response.Close();
                string mes = "";
                WeatherCurrent info = JsonConvert.DeserializeObject<WeatherCurrent>(answer);
                mes += $"Хороший город - {info.name}! Вот какая в нём погода: ";
                mes += "___________________________________________________________________\n";
                mes += $"Температура: {info.main.temp}°C, но по ощущениям {info.main.feels_like}°C\n";
                mes += $"Погода: {info.weather[0].description}\n";
                mes += $"Давление: {info.main.pressure * 0.750064:0} мм рт.ст.\n"; //получаем давление в миллибарах, нужно конвертить в мм рт.ст
                mes += $"Влажность: {info.main.humidity}%\n";
                mes += $"Минимально зафиксированная температура за день: {info.main.temp_min}°C\n";
                mes += $"Максимально зафиксированная температура за день: {info.main.temp_max}°C\n";
                mes += $"Скорость ветра: {info.wind.speed} м/c\n";
                mes += "___________________________________________________________________\n";
                return mes;
        }
    }
}
