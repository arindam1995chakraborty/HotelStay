using System;

namespace HotelStay.API.Models
{
    public record ProviderRoom(
        string Provider,
        string RoomId,
        RoomType RoomType,
        decimal PerNightRate,
        decimal TotalPrice,
        string CancellationPolicy,
        bool Available,
        int Rating
    );
}
