using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelStay.API.Controllers;
using HotelStay.API.Services;
using HotelStay.API.Providers;
using HotelStay.API.Stores;
using HotelStay.API.Models;
using HotelStay.API.Utils;
using Xunit;

namespace HotelStay.Tests
{
    public class HotelsControllerTests
    {
        private HotelsController CreateController()
        {
            var providers = new IHotelProvider[] { new PremierStaysProvider(), new BudgetNestsProvider() };
            var store = new InMemoryReservationStore();
            var service = new HotelService(providers, store);
            return new HotelsController(service);
        }

        [Fact]
        public async Task Search_ReturnsBadRequest_WhenMissingRequiredParams()
        {
            var ctrl = CreateController();
            var result = await ctrl.Search(null, null, null, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOk_ForValidParams()
        {
            var ctrl = CreateController();
            var res = await ctrl.Search("kolkata", "2026-07-10", "2026-07-11", null);
            var ok = Assert.IsType<OkObjectResult>(res);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Reserve_ReturnsUnprocessableEntity_ForInternationalMissingPassport()
        {
            var ctrl = CreateController();
            var req = new ReservationRequestDto("Bob", "NationalID", "N123", "Paris", DateOnly.Parse("2026-07-10"), DateOnly.Parse("2026-07-11"), "PremierStays", "PS-STD-1");
            var res = await ctrl.Reserve(req);
            Assert.IsType<UnprocessableEntityObjectResult>(res);
        }

        [Fact]
        public async Task Reserve_ReturnsCreated_ForValidDomesticReservation()
        {
            var ctrl = CreateController();
            var req = new ReservationRequestDto("Alice", "NationalID", "N123", "Seattle", DateOnly.Parse("2026-07-10"), DateOnly.Parse("2026-07-11"), "PremierStays", "PS-STD-1");
            var res = await ctrl.Reserve(req);
            var created = Assert.IsType<CreatedAtActionResult>(res);
            Assert.NotNull(created.Value);
            Assert.IsType<ReservationResponseDto>(created.Value);
        }
    }
}
