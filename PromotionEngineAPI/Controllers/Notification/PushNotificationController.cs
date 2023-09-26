using ApplicationCore.Notification;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Notification
{
    [Route("api/push-notification")]
    [ApiController]
    public class PushNotificationController : ControllerBase
    {
        private readonly IPushNotification _service;
        public PushNotificationController(IPushNotification service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("test")]
        public void PushNotification([FromBody] NotificationDto dto)
        {
            _service.SendNotification(title: dto.Title, body: dto.Body, link: dto.Link, to: dto.To);
        }


    }


}
