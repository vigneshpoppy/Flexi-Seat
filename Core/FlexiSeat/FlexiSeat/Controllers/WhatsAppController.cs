using FlexiSeat.DTO.WhatsappDTOs;
using FlexiSeat.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlexiSeat.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WhatsAppController : ControllerBase
  {
    private readonly WhatsAppMessageService _whatsAppService;
    private readonly SeatService _seatService;

    public WhatsAppController(WhatsAppMessageService whatsAppService,SeatService seatService)
    {
      _whatsAppService = whatsAppService;
      _seatService = seatService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(string to, string text)
    {
      var messageId = await _whatsAppService.SendWhatsAppMessage(to, text);
      return Ok(new { MessageId = messageId });
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve(
        [FromBody] WhatsappDto req)
    {
      var resp = await _seatService.ReserveAsync(req.PhoneNumber, req.SeatId, req.ReservedDate);
      return resp.IsSuccess ? Ok(resp) : BadRequest(resp);    
    }
    [HttpPost("sms")]
    public async Task<IActionResult> SMS(
        [FromBody] string msg)
    {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("The copy cat says: " +msg);
    
            return Ok();
    }
}
  }

}
