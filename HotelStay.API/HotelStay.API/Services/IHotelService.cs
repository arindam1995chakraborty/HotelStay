using System.Collections.Generic;
using System.Threading.Tasks;
using HotelStay.API.Models;

namespace HotelStay.API.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<SearchResultDto>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType);
        Task<(bool Success, ReservationResponseDto? Response, string? Error)> ReserveAsync(ReservationRequestDto request);
        bool TryGetReservation(string reference, out ReservationResponseDto? response);
    }
}
