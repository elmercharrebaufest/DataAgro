using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace Molinos.DataAgro.Business
{
    public class PushNotificationManager : IPushNotificationManager
    {
        public bool QueueMessage(string to, string title, string message, string urlNotificationClick)
        {
            if (string.IsNullOrEmpty(to))
            {
                return false;
            }
            var serverApiKey = "AAAAFc4qd2s:APA91bGHkCQzYcbANChCkrwLPBnmRsXX6TTTFwfLwPzfp6CDI4w2w5dkCgLHUFO_bCx31ZzMMbQlmpgNWPhoPaVsy6ncrWuTRH14IYJZn-Uflb6Qa6tTnR-pVJZda_LQU2tuO0iVQY3E";
            var firebaseGoogleUrl = "https://fcm.googleapis.com/fcm/send";

            var httpClient = new WebClient();
            httpClient.Headers.Add("Content-Type", "application/json");
            httpClient.Headers.Add(HttpRequestHeader.Authorization, "key=" + serverApiKey);
            var timeToLiveInSecond = 24 * 60 * 60; // 1 day
            var data = new
            {
                to = to,
                data = new
                {
                    notification = new
                    {
                        body = message,
                        title = title,
                        icon = "/Content/Images/agro.png",
                        url = urlNotificationClick
                    }
                },
                time_to_live = timeToLiveInSecond
            };

            var json = JsonConvert.SerializeObject(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            var responsebytes = httpClient.UploadData(firebaseGoogleUrl, "POST", byteArray);
            string responsebody = Encoding.UTF8.GetString(responsebytes);
            dynamic responseObject = JsonConvert.DeserializeObject(responsebody);

            return responseObject.success == "1";
        }
    }
}