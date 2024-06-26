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
using Microsoft.AspNetCore.SignalR;
using Services.SignalRHub;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly TimeSlotService _timeSlotService;
        private readonly IHubContext<TimeSlotHub> _hubContext; 


        public TimeSlotsController(TimeSlotService timeSlotService , IHubContext<TimeSlotHub> hubContext)
        {
            _timeSlotService = timeSlotService;
            _hubContext = hubContext;
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

            await _timeSlotService.UpdateTimeSlot(id, slotModel);

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


            //              "bookingId": "12345",
            //"userId": "67890",
            //"branchId": "B001",
            //"courtId": "C002",
            //"slotDate": "2024-07-01", nên tạo các thứ này

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


        [HttpPost("lock")]
        public async Task<IActionResult> LockSlot([FromBody] SlotModel slotInfo)
        {
            
            if (slotInfo == null)
            {
                return BadRequest("Invalid slot information.");
            }
           
            await _hubContext.Clients.All.SendAsync("LockingSlot", slotInfo);
            return Ok();
        }

        //[HttpGet("locked")]
        //public async Task<IActionResult> GetLockedSlots()
        //{
        //    var lockedSlots = await _timeSlotService.GetLockedSlots();
        //    return Ok(lockedSlots);
        //}

        [HttpPost("release")]
        public async Task<IActionResult> ReleaseSlot([FromBody] SlotModel slotInfo)
        {
            if (slotInfo == null)
            {
                return BadRequest("Invalid slot information.");
            }
            await _hubContext.Clients.All.SendAsync("ReleaseSlot", slotInfo);
            return Ok();
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmBooking(SlotCheckModel slotCheckModel)
        {
            await _timeSlotService.ConfirmBooking(slotCheckModel);
            return Ok();
        }

        [HttpGet("unavailable_slot")]
        public ActionResult<List<TimeSlot>> UnavailableSlot([FromQuery] DateOnly date, [FromQuery] string branchId)
        {
            
            var result = _timeSlotService.UnavailableSlot(date, branchId);
            if (result == null)
            {
                return NotFound("Slot not found.");
            }
            return Ok(result);
        }
        [HttpPost("checkSlotAvailability")]
        public async Task<IActionResult> CheckSlotAvailability([FromBody] List<SlotCheckModel> slotCheckModels)
        {
            foreach (var slotCheckModel in slotCheckModels)
            {
                var count = _timeSlotService.CountTimeSlot(slotCheckModel);
                if (count == 0)
                {
                    // Phát sự kiện SignalR để thông báo cho tất cả các client về các slot không khả dụng
                    await _hubContext.Clients.All.SendAsync("DisableSlot", slotCheckModel);
                    return Ok(new { isAvailable = false });
                }
            }

            return Ok(new { isAvailable = true });
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
