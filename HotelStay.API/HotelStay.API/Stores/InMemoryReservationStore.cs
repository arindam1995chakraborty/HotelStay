using System.Collections.Concurrent;
using HotelStay.API.Models;

namespace HotelStay.API.Stores
{
    public class InMemoryReservationStore
    {
        private readonly ConcurrentDictionary<string, ReservationRecord> _store = new();

        public bool TryAdd(string reference, ReservationRecord record) => _store.TryAdd(reference, record);

        public bool TryGet(string reference, out ReservationRecord? record) => _store.TryGetValue(reference, out record);
    }

    public class ReservationRecord
    {
        public ReservationResponseDto Response { get; set; } = default!;
    }
}
