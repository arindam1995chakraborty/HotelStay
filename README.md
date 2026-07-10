# HotelStay

HotelStay is a minimal sample application demonstrating a hotel search & booking flow.

This repository contains a .NET 8 Web API backend and a small Angular frontend. The API exposes search and reservation endpoints; the UI provides a search form, sortable results, reservation form and confirmation view.

Project structure
-----------------
- HotelStay.API/
  - HotelStay.API/            -> ASP.NET Core Web API (controllers, services, providers, in-memory store)
  - HotelStay.Tests/          -> xUnit tests for business logic and controllers
- HotelStay.UI/               -> Angular standalone application (Search, Results, Reservation, Confirmation)

Key functionality
-----------------
- Search hotels by destination, check-in, check-out and optional room type
- Show results with provider badge, room type, per-night rate, total price, cancellation policy and rating
- Sort results by total price
- Reserve a room with guest name and document (Passport or NationalID)
- Confirmation page shows reference number, provider, total price and cancellation policy

API endpoints
-------------
- GET  /hotels/search?destination={}&checkIn={yyyy-MM-dd}&checkOut={yyyy-MM-dd}[&roomType={Standard|Deluxe|Suite}]
- POST /hotels/reserve  (body: ReservationRequestDto)
- GET  /hotels/reservation/{reference}

Prerequisites
-------------
- .NET 8 SDK
- Node.js (16+) and npm
- Optional: Angular CLI for local dev (ng serve)

Run locally
------------
1. Clone the repository

   git clone https://github.com/arindam1995chakraborty/HotelStay.git
   cd "HotelStay"

2. Run the API

   - Option A (Visual Studio): open the solution at `HotelStay.API/HotelStay.API.slnx` and run the `HotelStay.API` project.
   - Option B (dotnet):

	 cd HotelStay.API/HotelStay.API
	 dotnet run

   By default the API development URL in this project is expected to be https://localhost:7175. Update `src/environments/environment.ts` in the UI if your API listens on a different port.

3. Run the UI

   cd HotelStay.UI
   npm install
   npx ng serve

   Open http://localhost:4200 in your browser.

4. Run tests

   cd HotelStay.API/HotelStay.Tests
   dotnet test

Notes & assumptions
-------------------
- The backend uses simple in-memory provider stubs (PremierStays, BudgetNests) and an InMemoryReservationStore — data is not persisted across restarts.
- For local dev the UI expects the API at `https://localhost:7175`. Change `HotelStay.UI/src/environments/environment.ts` to point to your API URL if different.
- CORS policy in the API is configured to allow common local dev ports (4200/5173). Adjust in `Program.cs` if needed.
- No external UI framework is used; components use simple SCSS for a practical look-and-feel.
- Reservation validation: international destinations (SeedData.InternationalCities) require `Passport`; domestic destinations require `NationalID` or `Passport`.
