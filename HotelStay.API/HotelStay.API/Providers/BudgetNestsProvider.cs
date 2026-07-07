using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelStay.API.Models;

namespace HotelStay.API.Providers
{
    // snake_case-style provider stub (simulated)
    public class BudgetNestsProvider : IHotelProvider
    {
        public string ProviderId => "BudgetNests";

        public Task<IEnumerable<ProviderRoom>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType)
        {
            var nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            // This provider may mark some rooms as unavailable depending on destination
            var baseRooms = new List<ProviderRoom>
            {
                new ProviderRoom(ProviderId, "BN-101", RoomType.Standard, 60m, 60m * nights, "Flexible up to 24h", true, 3),
                new ProviderRoom(ProviderId, "BN-102", RoomType.Deluxe, 90m, 90m * nights, "Flexible up to 24h", destination.ToLowerInvariant().StartsWith("x") ? false : true, 3),
                new ProviderRoom(ProviderId, "BN-201", RoomType.Suite, 150m, 150m * nights, "NonRefundable", true, 4)
            };

            if (roomType.HasValue)
            {
                baseRooms = baseRooms.Where(r => r.RoomType == roomType.Value).ToList();
            }

            return Task.FromResult<IEnumerable<ProviderRoom>>(baseRooms);
        }

        public Task<ProviderReservationResult> ReserveAsync(string roomId, string guestName, string documentType, string documentNumber, DateOnly checkIn, DateOnly checkOut)
        {
            // Simple deterministic reservation
            var reference = $"BN-{Guid.NewGuid():N}";
            var nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            decimal perNight = roomId.Contains("201") ? 150m : roomId.Contains("102") ? 90m : 60m;
            var total = perNight * nights;
            var cancellation = roomId.Contains("201") ? "NonRefundable" : "Flexible up to 24h";
            return Task.FromResult(new ProviderReservationResult(true, reference, "Confirmed", total, cancellation));
        }
    }
}
