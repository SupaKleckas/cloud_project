using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CoursesAPI.Models;
using System.Security.Policy;
using Plain.RabbitMQ;

namespace CoursesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IPublisher publisher;
        public EnrollmentController(IPublisher publisher)
        {
            this.publisher = publisher;
        }
        [HttpPost]
        public void Post([FromBody] Enrollment en)
        {
            publisher.Publish(JsonConvert.SerializeObject(en), "enrollment_topic", null);
        }
    }
}
