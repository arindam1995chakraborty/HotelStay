using System;
using System.Linq;
using System.Threading.Tasks;
using HotelStay.API.Providers;
using HotelStay.API.Services;
using HotelStay.API.Stores;
using HotelStay.API.Models;
using Xunit;

namespace HotelStay.Tests
{
    public class HotelServiceTests
    {
        private HotelService CreateService()
        {
            var providers = new IHotelProvider[] { new PremierStaysProvider(), new BudgetNestsProvider() };
            var store = new InMemoryReservationStore();
            return new HotelService(providers, store);
        }

        [Fact]
        public async Task SearchAsync_ReturnsResults_ForValidRequest()
        {
            var svc = CreateService();
            var ci = DateOnly.Parse("2026-07-10");
            var co = DateOnly.Parse("2026-07-11");

            var results = (await svc.SearchAsync("Kolkata", ci, co, null)).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, r => Assert.True(r.TotalPrice >= 0));
        }

        [Fact]
        public async Task SearchAsync_FiltersByRoomType()
        {
            var svc = CreateService();
            var ci = DateOnly.Parse("2026-07-10");
            var co = DateOnly.Parse("2026-07-11");

            var results = (await svc.SearchAsync("Kolkata", ci, co, RoomType.Suite)).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, r => Assert.Equal(RoomType.Suite, r.RoomType));
        }

        [Fact]
        public async Task ReserveAsync_ReturnsFalse_ForUnknownProvider()
        {
            var svc = CreateService();
            var req = new ReservationRequestDto("Alice","Passport","P123","Nowhere",DateOnly.Parse("2026-07-10"),DateOnly.Parse("2026-07-11"),"NoSuchProvider","ROOM-1");

            var (success, response, error) = await svc.ReserveAsync(req);

            Assert.False(success);
            Assert.Null(response);
            Assert.Equal("Unknown provider", error);
        }
    }
}
