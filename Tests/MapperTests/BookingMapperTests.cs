using System;

using InterviewService.Mappers;
using InterviewService.Models;

using Shouldly;
using Xunit;

namespace InterviewService.Tests.MapperTests
{
    public class BookingMapperTests
    {
        [Fact]
        public void MappingToDto_MapsAllFields()
        {
            var booking = new Booking
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
            var dto = booking.ToDto();

            dto.ShouldNotBeNull();

            dto.ShouldSatisfyAllConditions(
                () => dto.Id.ShouldBe(booking.Id),
                () => dto.ProviderId.ShouldBe(booking.ProviderId),
                () => dto.EventId.ShouldBe(booking.EventId),
                () => dto.CustomerId.ShouldBe(booking.CustomerId),
                () => dto.CustomerId.ShouldBe(booking.CustomerId),
                () => dto.Deleted.ShouldBe(booking.Deleted),
                () => dto.TimeStamp.ShouldBe(booking.TimeStamp)
            );
        }
    }
}
