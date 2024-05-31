//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;
//using Repositories.Helper;
//using Services.Interface;

//namespace API.Controllers
//{
//    [Route("api/mail")]
//    [ApiController]
//    public class MailController : ControllerBase
//    {
//        private readonly IMailService mailService;
//        public MailController(IMailService mailService)
//        {
//            this.mailService = mailService;
//        }

//        [HttpPost("send")]
//        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
//        {
//            try
//            {
//                await mailService.SendEmailAsync(request);
//                return Ok();
//            }
//            catch
//            {
//                return BadRequest();
//            }
//        }
//    }
//}
