# HotelStay — spec (Data models & interface contracts)

This document describes the canonical data models and service/controller contracts implemented in the repository.

Models (C# DTOs)
-------------------
- ProviderRoom (provider internal representation)
  - Provider: string
  - RoomId: string
  - RoomType: RoomType (Standard | Deluxe | Suite)
  - PerNightRate: decimal
  - TotalPrice: decimal
  - CancellationPolicy: string
  - Available: bool
  - Rating: int

- SearchResultDto (API payload returned to UI)
  - Provider: string
  - RoomId: string
  - RoomType: RoomType
  - PerNightRate: decimal
  - TotalPrice: decimal
  - CancellationPolicy: string
  - Rating: int

- ReservationRequestDto (API POST /hotels/reserve body)
  - GuestName: string
  - DocumentType: string (e.g. "Passport", "NationalID")
  - DocumentNumber: string
  - Destination: string
  - CheckIn: DateOnly (ISO yyyy-MM-dd)
  - CheckOut: DateOnly (ISO yyyy-MM-dd)
  - Provider: string
  - RoomId: string

- ReservationResponseDto (API response on success)
  - Reference: string
  - Provider: string
  - GuestName: string
  - TotalPrice: decimal
  - CancellationPolicy: string
  - CheckIn: DateOnly
  - CheckOut: DateOnly
  - Destination: string

- ProviderReservationResult (provider-level response)
  - Success: bool
  - Reference: string
  - Message: string
  - TotalPrice: decimal
  - CancellationPolicy: string

Enums
-----
- RoomType: Standard | Deluxe | Suite

Interfaces / Services
---------------------
- IHotelProvider
  - string ProviderId { get; }
  - Task<IEnumerable<ProviderRoom>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType)
  - Task<ProviderReservationResult> ReserveAsync(string roomId, string guestName, string documentType, string documentNumber, DateOnly checkIn, DateOnly checkOut)

- IHotelService
  - Task<IEnumerable<SearchResultDto>> SearchAsync(string destination, DateOnly checkIn, DateOnly checkOut, RoomType? roomType)
	- Aggregates results from registered IHotelProvider implementations
	- Filters out unavailable rooms
	- Maps ProviderRoom -> SearchResultDto
	- Orders results by TotalPrice

  - Task<(bool Success, ReservationResponseDto? Response, string? Error)> ReserveAsync(ReservationRequestDto request)
	- Locates provider by ProviderId
	- Calls provider.ReserveAsync
	- On success stores reservation in the in-memory store and returns ReservationResponseDto

  - bool TryGetReservation(string reference, out ReservationResponseDto? response)

API Endpoints (controllers)
---------------------------
- GET /hotels/search
  - Query params: destination (required), checkIn (yyyy-MM-dd, required), checkOut (yyyy-MM-dd, required), roomType (optional)
  - Validation:
	- All required params must be present
	- checkIn/checkOut must parse to DateOnly
	- checkOut must be after checkIn
  - Response: 200 OK with SearchResultDto[] or 400/ProblemDetails for validation errors

- POST /hotels/reserve
  - Body: ReservationRequestDto
  - Validation:
	- request body must be present
	- If Destination is in SeedData.InternationalCities -> DocumentType must be "Passport"
	- If Destination is domestic -> DocumentType must be "NationalID" or "Passport"
  - On provider failure -> 400 ProblemDetails
  - On success -> 201 Created with ReservationResponseDto and Location header

- GET /hotels/reservation/{reference}
  - Returns stored reservation (200) or 404

Error contract
--------------
- API uses ProblemDetails for validation and error responses. Example shape:
  - { "title": "Invalid request", "detail": "checkOut must be after checkIn" }

Notes
-----
- Implementations in this project are intentionally in-memory and deterministic (Provider stubs) for ease of testing and demonstration.
- Dates: API expects ISO yyyy-MM-dd and maps to DateOnly on the server.
