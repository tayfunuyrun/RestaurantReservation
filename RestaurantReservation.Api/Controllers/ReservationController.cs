using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Api.Services;
using RestaurantReservation.Data.Models.Dto;

namespace RestaurantReservation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }


        [HttpGet("AllReservation")]
        public async Task<IActionResult> GetAllReservation()
        {
            var vResult = await _reservationService.GetAllReservation();
            return Ok(vResult);
        }


        [HttpPost("SaveReservation")]
        public async Task<IActionResult> SaveReservation(SaveReservationDto dto)
        {
            var vResult = await _reservationService.SaveReservation(dto);
            return Ok(vResult);
        }

        [HttpPut("UpdateReservation")]
        public async Task<IActionResult> UpdateReservation(ReservationInfo info)
        {
            var vResult = await _reservationService.UpdateReservation(info);
            return Ok(vResult);
        }

        [HttpPost("DeleteReservation")]
        public async Task<IActionResult> SaveReservation(int id)
        {
            var vResult = await _reservationService.DeleteReservation(id);
            return Ok(vResult);
        }
    }
}
