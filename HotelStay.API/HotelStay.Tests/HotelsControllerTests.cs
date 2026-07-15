using Microsoft.AspNetCore.Mvc;
using HotelStay.API.Controllers;
using HotelStay.API.Services;
using HotelStay.API.Providers;
using HotelStay.API.Stores;
using HotelStay.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HotelStay.Tests
{
    public class HotelsControllerTests
    {
        private readonly IHotelService _hotelService;
        private readonly InMemoryReservationStore _store;
        private readonly IHotelProvider[] _providers;
        public HotelsControllerTests()
        {
            _providers = new IHotelProvider[] { new PremierStaysProvider(), new BudgetNestsProvider() };
            _store = new InMemoryReservationStore();
            _hotelService = new HotelService(_providers, _store);
            
        }

        [Fact]
        public async Task Search_ReturnsBadRequest_WhenMissingRequiredParams()
        {
            var result = await HotelsController.Search(null, null, null, null, _hotelService);
            var bad = Assert.IsType<BadRequest<ProblemDetails>>(result);
            Assert.Equal(bad.Value.Title, "Invalid request");
            Assert.Equal(bad.Value.Detail, "destination, checkIn and checkOut are required");
        }

        [Fact]
        public async Task Search_ReturnsOk_ForValidParams()
        {
            var res = await HotelsController.Search("kolkata", "2026-07-10", "2026-07-11", null, _hotelService);
            var ok = Assert.IsType<Ok<IEnumerable<SearchResultDto>>>(res);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Search_ReturnsBadRequest_WhenCheckOutNotAfterCheckIn()
        {
            // checkOut equal to checkIn should be invalid
            var res = await HotelsController.Search("kolkata", "2026-07-10", "2026-07-10", null, _hotelService);
            var bad = Assert.IsType<BadRequest<ProblemDetails>>(res);
            Assert.Equal(bad.Value.Title, "Invalid request");
            Assert.Equal(bad.Value.Detail, "checkOut must be after checkIn");
        }

        [Fact]
        public async Task Reserve_ReturnsUnprocessableEntity_ForInternationalMissingPassport()
        {            
            var req = new ReservationRequestDto("Bob", "NationalID", "N123", "Paris", DateOnly.Parse("2026-07-10"), DateOnly.Parse("2026-07-11"), "PremierStays", "PS-STD-1");
            var res = await HotelsController.Reserve(req,_hotelService);
            var bad = Assert.IsType<BadRequest<ProblemDetails>>(res);
            Assert.Equal(bad.Value.Title, "Invalid request");
            Assert.Equal(bad.Value.Detail, "International destinations require a Passport");
        }

        [Fact]
        public async Task Reserve_ReturnsCreated_ForValidDomesticReservation()
        {
            var req = new ReservationRequestDto("Alice", "NationalID", "N123", "Seattle", DateOnly.Parse("2026-07-10"), DateOnly.Parse("2026-07-11"), "PremierStays", "PS-STD-1");
            var res = await HotelsController.Reserve(req, _hotelService);
            var created = Assert.IsType<Created<ReservationResponseDto>>(res);
        }
    }
}
