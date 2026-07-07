using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelStay.API.Models;
using HotelStay.API.Services;
using HotelStay.API.Utils;

namespace HotelStay.API.Controllers
{
    [ApiController]
    [Route("hotels")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? destination, [FromQuery] string? checkIn, [FromQuery] string? checkOut, [FromQuery] string? roomType)
        {
            if (string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(checkIn) || string.IsNullOrWhiteSpace(checkOut))
            {
                return BadRequest(Problem("destination, checkIn and checkOut are required"));
            }

            if (!DateOnly.TryParse(checkIn, out var ci) || !DateOnly.TryParse(checkOut, out var co))
            {
                return BadRequest(Problem("checkIn and checkOut must be valid dates in ISO format (yyyy-MM-dd)"));
            }

            if (co <= ci)
            {
                return BadRequest(Problem("checkOut must be after checkIn"));
            }

            RoomType? rt = null;
            if (!string.IsNullOrWhiteSpace(roomType) && Enum.TryParse<RoomType>(roomType, true, out var parsed))
            {
                rt = parsed;
            }

            var results = await _hotelService.SearchAsync(destination, ci, co, rt);
            return Ok(results);
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReservationRequestDto request)
        {
            if (request == null)
                return BadRequest(Problem("missing body"));

            // Document validation based on destination
            var dest = request.Destination ?? string.Empty;
            bool isInternational = SeedData.InternationalCities.Contains(dest);
            if (isInternational)
            {
                if (!string.Equals(request.DocumentType, "Passport", StringComparison.OrdinalIgnoreCase))
                {
                    return UnprocessableEntity(Problem("International destinations require a Passport"));
                }
            }
            else
            {
                // Domestic accepts NationalID or Passport
                if (!string.Equals(request.DocumentType, "NationalID", StringComparison.OrdinalIgnoreCase) && !string.Equals(request.DocumentType, "Passport", StringComparison.OrdinalIgnoreCase))
                {
                    return UnprocessableEntity(Problem("Domestic destinations require NationalID (or Passport)"));
                }
            }

            var (success, response, error) = await _hotelService.ReserveAsync(request);
            if (!success)
            {
                return BadRequest(Problem(error ?? "reservation failed"));
            }

            return CreatedAtAction(nameof(GetReservation), new { reference = response!.Reference }, response);
        }

        [HttpGet("reservation/{reference}")]
        public IActionResult GetReservation(string reference)
        {
            if (_hotelService.TryGetReservation(reference, out var response))
            {
                return Ok(response);
            }

            return NotFound();
        }

        private ProblemDetails Problem(string detail)
        {
            return new ProblemDetails { Title = "Invalid request", Detail = detail };
        }
    }
}
