using HotelStay.API.Models;
using HotelStay.API.Providers;
using HotelStay.API.Stores;
using HotelStay.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelStay.API.Services
{
    public class HotelService : IHotelService
    {
        private readonly IEnumerable<IHotelProvider> _providers;
        private readonly InMemoryReservationStore _store;

        public HotelService(IEnumerable<IHotelProvider> providers, InMemoryReservationStore store)
        {
            _providers = providers;
            _store = store;
        }

        public async Task<IEnumerable<SearchResultDto>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType)
        {
            var tasks = _providers.Select(p => p.SearchAsync(destination, checkIn, checkOut, roomType));
            var results = await Task.WhenAll(tasks);
            var flatten = results.SelectMany(x => x)
                .Where(r => r.Available)
                .Select(r => new SearchResultDto(r.Provider, r.RoomId, r.RoomType, r.PerNightRate, r.TotalPrice, r.CancellationPolicy, r.Rating))
                .OrderBy(r => r.TotalPrice)
                .ToList();

            return flatten;
        }

        public async Task<(bool Success, ReservationResponseDto? Response, string? Error)> ReserveAsync(ReservationRequestDto request)
        {
            var provider = _providers.FirstOrDefault(p => p.ProviderId.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));
            if (provider == null)
            {
                return (false, null, "Unknown provider");
            }

            // Document validation based on destination
            var dest = request.Destination ?? string.Empty;
            bool isInternational = SeedData.InternationalCities.Contains(dest);
            if (isInternational)
            {
                if (!string.Equals(request.DocumentType, "Passport", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null, "International destinations require a Passport");
                }
            }
            else
            {
                // Domestic accepts NationalID or Passport
                if (!string.Equals(request.DocumentType, "NationalID", StringComparison.OrdinalIgnoreCase) && !string.Equals(request.DocumentType, "Passport", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null, "Domestic destinations require NationalID (or Passport)");
                }
            }

            var res = await provider.ReserveAsync(request.RoomId, request.GuestName, request.DocumentType, request.DocumentNumber, request.CheckIn, request.CheckOut);
            if (!res.Success)
            {
                return (false, null, res.Message);
            }

            var response = new ReservationResponseDto(res.Reference, provider.ProviderId, request.GuestName, res.TotalPrice, res.CancellationPolicy, request.CheckIn, request.CheckOut, request.Destination);
            var record = new ReservationRecord { Response = response };
            _store.TryAdd(res.Reference, record);

            return (true, response, null);
        }

        public bool TryGetReservation(string reference, out ReservationResponseDto? response)
        {
            response = null;
            if (_store.TryGet(reference, out var record))
            {
                response = record.Response;
                return true;
            }

            return false;
        }
    }
}
