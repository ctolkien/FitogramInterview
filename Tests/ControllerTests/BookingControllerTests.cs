using InterviewService.Models;
using InterviewService.Models.External;
using Xunit;
using Shouldly;
using Microsoft.AspNetCore.Mvc;
using InterviewService.Dto.Output;
using InterviewService.Dto.Input;
using System;

namespace Tests.ControllerTests
{
    public class BookingControllerTests
    {
        [Fact]
        public void GetBooking_ById_Exists()
        {
            // [Arrange]

            TestEnvironment testEnvironment = new TestEnvironment();

            Provider provider = testEnvironment.AddProvider();
            Customer customer = testEnvironment.AddCustomer(provider: provider);
            Event evnt = testEnvironment.AddEvent(provider: provider);

            const string notes = "Ensure guest has a current COVID test";
            Booking booking = testEnvironment.AddBooking(evnt: evnt, customer: customer, bookingNotes: notes);
            testEnvironment.AddRoleToProvider(provider.Id);

            // [Act]

            ActionResult<BookingDto> existingBooking = testEnvironment
                .GetBookingsController()
                .GetBooking(id: booking.Id);

            existingBooking.ShouldNotBeNull();

            BookingDto bookingDto = existingBooking.Value;

            // [Assert]

            bookingDto.ShouldNotBe(null);
            bookingDto.Id.ShouldBe(booking.Id);
            bookingDto.Notes.ShouldBe(booking.Notes);
        }

        [Fact]
        public void CreateBooking_Then_GetBooking_ById()
        {
            TestEnvironment testEnvironment = new TestEnvironment();

            Provider provider = testEnvironment.AddProvider();
            Customer customer = testEnvironment.AddCustomer(provider: provider);
            Event evnt = testEnvironment.AddEvent(provider: provider);
            testEnvironment.AddRoleToProvider(provider.Id);

            BookingPostBody create = new BookingPostBody
            {
                CustomerId = customer.Id,
                EventId = evnt.Id,
                Notes = "Requested an initial meeting with a trainer if possible"
            };

            ActionResult<BookingDto> response = testEnvironment
                .GetBookingsController()
                .CreateBooking(create);

            ActionResult<BookingDto> createdBooking = testEnvironment
                .GetBookingsController()
                .GetBooking(id: response.Value.Id);

            BookingDto bookingDto = createdBooking.Value;

            bookingDto.ShouldNotBe(null);
            bookingDto.Id.ShouldNotBeNull();
            bookingDto.Id.ShouldNotBe(Guid.Empty);
            bookingDto.Notes.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void GetBooking_ById_FailsIfNotAuthorized()
        {
            TestEnvironment testEnvironment = new TestEnvironment();

            Provider provider = testEnvironment.AddProvider();
            Customer customer = testEnvironment.AddCustomer(provider: provider);
            Event evnt = testEnvironment.AddEvent(provider: provider);

            Booking booking = testEnvironment.AddBooking(evnt: evnt, customer: customer);

            ActionResult<BookingDto> actionResult = testEnvironment
                .GetBookingsController()
                .GetBooking(id: booking.Id);

            actionResult.Value.ShouldBeNull();
            actionResult.Result.ShouldNotBeNull();
            actionResult.Result.ShouldBeOfType(typeof(UnauthorizedResult));
        }
    }
}
