using System;

namespace HotelStay.API.Models
{
    public record SearchResultDto(
        string Provider,
        string RoomId,
        RoomType RoomType,
        decimal PerNightRate,
        decimal TotalPrice,
        string CancellationPolicy,
        int Rating
    );

    public record ReservationRequestDto(
        string GuestName,
        string DocumentType,
        string DocumentNumber,
        string Destination,
        DateOnly CheckIn,
        DateOnly CheckOut,
        string Provider,
        string RoomId
    );

    public record ReservationResponseDto(
        string Reference,
        string Provider,
        string GuestName,
        decimal TotalPrice,
        string CancellationPolicy,
        DateOnly CheckIn,
        DateOnly CheckOut,
        string Destination
    );
}
