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
    public  static class HotelsController
    {

        public static async Task<IResult> Search(string? destination, string? checkIn, string? checkOut, string? roomType, IHotelService hotelService)
        {
            if (string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(checkIn) || string.IsNullOrWhiteSpace(checkOut))
            {
                return Results.BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "destination, checkIn and checkOut are required" });
            }

            if (!DateOnly.TryParse(checkIn, out var ci) || !DateOnly.TryParse(checkOut, out var co))
            {
                return Results.BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "checkIn and checkOut must be valid dates in ISO format (yyyy-MM-dd)" });
            }

            if (co <= ci)
            {
                return Results.BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "checkOut must be after checkIn" });
            }

            RoomType? rt = null;
            if (!string.IsNullOrWhiteSpace(roomType) && Enum.TryParse<RoomType>(roomType, true, out var parsed))
            {
                rt = parsed;
            }

            var results = await hotelService.SearchAsync(destination, ci, co, rt);
            return Results.Ok(results);
        }

        public static async Task<IResult> Reserve(ReservationRequestDto request, IHotelService hotelService)
        {
            if (request == null)
                return Results.BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "missing body" });

            var (success, response, error) = await hotelService.ReserveAsync(request);
            if (!success)
            {
                return Results.BadRequest(new ProblemDetails { Title = "Invalid request", Detail = error ?? "reservation failed" });
            }

            return Results.Created($"/hotels/reservation/{response!.Reference}", response);
        }
        public static IResult GetReservation(string reference, IHotelService hotelService)
        {
            if (hotelService.TryGetReservation(reference, out var response))
            {
                return Results.Ok(response);
            }
            return Results.NotFound();
        }
    }
}
