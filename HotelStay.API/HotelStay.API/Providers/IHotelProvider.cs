using System.Collections.Generic;
using System.Threading.Tasks;
using HotelStay.API.Models;

namespace HotelStay.API.Providers
{
    public interface IHotelProvider
    {
        string ProviderId { get; }
        Task<IEnumerable<ProviderRoom>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType);
        Task<ProviderReservationResult> ReserveAsync(string roomId, string guestName, string documentType, string documentNumber, DateOnly checkIn, DateOnly checkOut);
    }

    public record ProviderReservationResult(bool Success, string Reference, string Message, decimal TotalPrice, string CancellationPolicy);
}
