# Fitogram Interview Service

This service is based on our booking system and has been stripped down for the HR interview. Please dig into the service and try to understand what it does. In the interview we will have a couple of questions around areas like Coding, Distributed Systems, Testing, Performance and more. As part of the test you will have two issues to solve (see below). Before you start developing, please create a new public repository on your GitHub account and upload the code. For each of the issues below create one Pull Request so its easier for all of us to discuss your changes together.

If you run in trouble getting the project to run, just create the Pull Requests without testing, and we discuss the issues you faced together (as it would be in normal work relationships).

## Tasks to solve upfront before the interview

We expect this to take around an hour to complete. Please don't spend 2 hours or more.

- [x] ISSUE-1: It seems the tests are failing. Please find the failing tests and fix them. 
- [x] If necessary improve the test to have full coverage.

- [x] ISSUE-2: The product team wishes to have a field for notes on the booking (so that the customer can add some notes when making a booking). Please add the field `Notes` (string) to the model and make sure it can be added and read via the existing bookings api.

## Development

To start all dependencies run

```bash
docker-compose up -d redis rabbitmq postgres
```

Run the tests:

```bash
docker-compose up test
```

## Ben's notes

I've not implemented running EF migrations on startup. Please run `dotnet ef database update` to apply the migration to add `Notes` to the `Booking`

I've not gone further with adding tests. I 

These are all minor but they might be fun to talk about during the review:

- The Roslyn rules are a bit restrictive and I wonder if that reflects the production code base and, if so, has the team been happy with them? For example, if you add an EF migration via the dotnet tool then you cannot immediately remove the migration via the same tool if you're not happy with the name because it attempts to build the project which will fail because of a rule violation on the newly added files.

- The `BookingMapper` responsibility feels inverted. It's an extension method on the domain that maps domain to DTO. It feels like it might feel nicer living as a static method on the DTO itself so that control always flows from core to edge. Granted, this flow is largely in place now but the extension method means that the mapping doesn't live next to the DTO and it doesn't live next to the domain.

- It looks like there's another DTO for messaging but that's named remarkably similar to the domain (`FitogramMQ.Booking`). This is declared in an external assembly so we don't have control over naming within the scope of this project. So, in this case, the mapper needs to stay.

- I'm curious if tying EF persistence to an external service (writing to the message queue) has been problematic. I would like to separate those two and introduce a domain service that handles first saving internally and then writing to the interested 3rd parties. The reason for this is that if, say, writing to the queue fails, it might be a place we want to wrap a Polly retry policy around. Which we can still do where it is now, but that starts to increase the responsibility of the `AfterSaveChanges` method.

- It seems that the bulk of the current tests are end-to-end tests. While these are extremely valuable, I'd like to also introduced focused unit and integration tests just for the server. This would involve stubbing out the external dependencies in the test server wireup in the (new) integration tests. My goals with this would be to increase test speed and also allow us to add in specific stubs to try out things like forcing connectivity issues.

- With the focus on making the current test project the home of the end-to-end tests, I'd also like to bring in something like `FluentDocker` to handle the environment setup so that the developer doesn't need to remember to run `docker-compose up` before running the unit tests.

- I think we can revisit the `Dockerfile` and move around the `dotnet restore` command. This will improve layer caching which should positively impact testing time locally and, if layers are being cached, build time on the CI server as well.
