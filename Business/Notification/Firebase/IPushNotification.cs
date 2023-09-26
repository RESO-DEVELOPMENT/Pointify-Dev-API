using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ApplicationCore.Notification
{
    public interface IPushNotification
    {
        public void SendNotification(string title, string body, string link, string to);
    }
    public class PushNotification : IPushNotification
    {
        private const string SERVER_API_KEY = "AAAAGozfdHg:APA91bH7n-0EMx5-x5C6zry4K4tHvPrJqm3sl4pbFD6h6fPQd1RV_zZ5Va1FhLf50jDQgUu-27lHEdlZPc0Jb3LL4oq8rD57XwolYIWQbpuA0hHSdo56vn6mo8kZgcVefmAL2ZrndcbX";
        private const string SENDER_ID = "114032604280";
        public void SendNotification(string title, string body, string link, string to)
        {
            try
            {
                dynamic data = new
                {
                    to, 
                    notification = new
                    {
                        title,     // Notification title
                        body,    // Notification body data
                        link,       // When click on notification user redirect to this link
                    }
                };
                PushNotificationProcess(data);


            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PushNotificationProcess(dynamic data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n>>>>>>>>>>>>> ERROR WHEN PUSH NOTIFICATION <<<<<<<<<<<<<<<\n");
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine("\n>>>>>>>>>>>>> ERROR WHEN PUSH NOTIFICATION <<<<<<<<<<<<<<<\n");
                throw;
            }
        }
    }

    public class NotificationDto
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
    }
}
