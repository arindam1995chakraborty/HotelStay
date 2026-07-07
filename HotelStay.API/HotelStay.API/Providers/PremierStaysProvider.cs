using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelStay.API.Models;

namespace HotelStay.API.Providers
{
    // PascalCase-style provider stub
    public class PremierStaysProvider : IHotelProvider
    {
        public string ProviderId => "PremierStays";

        public Task<IEnumerable<ProviderRoom>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType)
        {
            // Deterministic sample data
            var nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            var rooms = new List<ProviderRoom>
            {
                new ProviderRoom(ProviderId, "PS-STD-1", RoomType.Standard, 80m, 80m * nights, "FreeCancellation up to 48h", true, 4),
                new ProviderRoom(ProviderId, "PS-DLX-1", RoomType.Deluxe, 120m, 120m * nights, "FreeCancellation up to 48h", true, 5),
                new ProviderRoom(ProviderId, "PS-SUI-1", RoomType.Suite, 200m, 200m * nights, "NonRefundable", true, 5)
            };

            if (roomType.HasValue)
            {
                rooms = rooms.Where(r => r.RoomType == roomType.Value).ToList();
            }

            return Task.FromResult<IEnumerable<ProviderRoom>>(rooms);
        }

        public Task<ProviderReservationResult> ReserveAsync(string roomId, string guestName, string documentType, string documentNumber, DateOnly checkIn, DateOnly checkOut)
        {
            // Deterministic confirmation
            var reference = $"PS-{Guid.NewGuid():N}";
            var nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            decimal perNight = roomId.Contains("DLX") ? 120m : roomId.Contains("SUI") ? 200m : 80m;
            var total = perNight * nights;
            return Task.FromResult(new ProviderReservationResult(true, reference, "Confirmed", total, "FreeCancellation up to 48h"));
        }
    }
}
