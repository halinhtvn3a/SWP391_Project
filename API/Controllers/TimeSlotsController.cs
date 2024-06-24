using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using DAOs.Models;
using Services;
using Page = DAOs.Helper;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly TimeSlotService _timeSlotService;

        public TimeSlotsController()
        {
            _timeSlotService = new TimeSlotService();
        }

        // GET: api/TimeSlots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlots()
        {
            return _timeSlotService.GetTimeSlots();
        }
        
        [HttpGet("page/")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<TimeSlot> timeSlots = await _timeSlotService.GetTimeSlots(pageResult,searchQuery);
            return Ok(timeSlots);
        }

        // GET: api/TimeSlots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSlot>> GetTimeSlot(string id)
        {
            var timeSlot = _timeSlotService.GetTimeSlot(id);

            if (timeSlot == null)
            {
                return NotFound();
            }

            return await timeSlot;
        }
        
        [HttpGet("bookingId/{bookingId}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotByBookingId(string bookingId)
        {
            var timeSlot = _timeSlotService.GetTimeSlotsByBookingId(bookingId);

            if (timeSlot == null)
            {
                return NotFound();
            }

            return timeSlot;
        }

        // PUT: api/TimeSlots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeSlot(string id, SlotModel slotModel)
        {
            var timeSlot = await _timeSlotService.GetTimeSlot(id);
            if (id != timeSlot.SlotId)
            {
                return BadRequest();
            }

            _timeSlotService.UpdateTimeSlot(id, slotModel);

            return NoContent();
        }

        // POST: api/TimeSlots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeSlot>> PostTimeSlot(TimeSlot timeSlot)
        {
            _timeSlotService.AddTimeSlot(timeSlot);

            return CreatedAtAction("GetTimeSlot", new { id = timeSlot.SlotId }, timeSlot);
        }

        // DELETE: api/TimeSlots/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTimeSlot(string id)
        //{
        //    var timeSlot = timeSlotService.GetTimeSlot(id);
        //    if (timeSlot == null)
        //    {
        //        return NotFound();
        //    }

        //    timeSlotService.TimeSlots.Remove(timeSlot);

        //    return NoContent();
        //}

        [HttpPost("isBooked")]
        public ActionResult<bool> IsSlotBookedInBranch([FromBody] SlotModel slotModel)
        {
            bool isBooked = _timeSlotService.IsSlotBookedInBranch(slotModel);
            return isBooked;
        }

        [HttpPut("changeSlot/{slotId}")]
        public async Task<ActionResult<TimeSlot>> ChangeSlot(SlotModel slotModel, string slotId)
        {
            TimeSlot timeSlot = _timeSlotService.ChangeSlot(slotModel, slotId);

            return Ok(new TimeSlot()
            {
                SlotId = timeSlot.SlotId,
                CourtId = timeSlot.CourtId,
                BookingId = timeSlot.BookingId,
                SlotDate = timeSlot.SlotDate,
                SlotStartTime = timeSlot.SlotStartTime,
                SlotEndTime = timeSlot.SlotEndTime,
                Price = timeSlot.Price,
                Status = timeSlot.Status
            });
        }

        [HttpGet("userId/{userId}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotsByUserId(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<TimeSlot> timeSlots = await _timeSlotService.GetTimeSlotsByUserId(userId, pageResult);
            return Ok(timeSlots);
        }

        [HttpGet("sortSlot/{sortBy}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> SortTimeSlot(string sortBy, bool isAsc, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await _timeSlotService.SortTimeSlot(sortBy, isAsc, pageResult);
        }
        private bool TimeSlotExists(string id)
        {
            return _timeSlotService.GetTimeSlots().Any(e => e.SlotId == id);
        }

        [Route("api/timeslots/qrcode/{timeslotid}")]
        [HttpGet]
        public async Task<IActionResult> GetQRCode(string timeslotid)
        {
            var timeslot = await _timeSlotService.GetTimeSlot(timeslotid);
            if (timeslot == null)
            {
                return NotFound("Timeslot not found.");
            }

            var qrData = new
            {
                BookingId = timeslot.BookingId,
                Status = timeslot.Status,
                TimeslotId = timeslot.SlotId,
                SlotDate = timeslot.SlotDate,
                
            };

            string qrString = JsonConvert.SerializeObject(qrData);
            string qrCodeBase64 = _timeSlotService.GenerateQRCode(qrString);

            return Ok(new { qrCodeBase64 });
        }

        [HttpPost("checkin/qr")]
        public async Task<IActionResult> CheckInWithQR([FromBody] QRCheckInModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.QRCodeData))
            {
                return BadRequest("Invalid QR code data.");
            }

            var qrData = DecryptQRCode(request.QRCodeData);

            var timeSlot = await _timeSlotService.GetTimeSlot(qrData.TimeslotId);
            if (timeSlot != null && timeSlot.Status == "Reserved" && timeSlot.SlotId == qrData.TimeslotId)
            {
                //cần phải checked-in tất cả time slot ngày hôm đó luôn chứ không phải chỉ 1 time slot
                _timeSlotService.GetTimeSlotsByDate(timeSlot.SlotDate).ForEach(async t =>
                {
                    t.Status = "checked-in";
                    await _timeSlotService.UpdateTimeSlotWithObject(t);
                });


                return Ok("Check-in successful.");
            }
            return BadRequest("Invalid QR code or timeslot.");
        }

        private QRData DecryptQRCode(string qrCodeData)
        {
            // Implement logic to decrypt and parse the QR code data
            return JsonConvert.DeserializeObject<QRData>(qrCodeData);
        }

        public class QRData
        {
            public string TimeslotId { get; set; }
            public string UserId { get; set; }
        }

    }
}
