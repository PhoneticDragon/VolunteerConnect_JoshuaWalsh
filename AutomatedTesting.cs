using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect;

public class AutomatedTesting
{
    private readonly DatabaseService _database;

    // 1 = Run automated tests
    // 0 = Do not run automated tests
    private const int RUN_AUTOMATED_TESTS = 1;

    public AutomatedTesting()
    {
        _database = new DatabaseService();
    }

    public async Task RunAllTests()
    {
        if (RUN_AUTOMATED_TESTS != 1)
        {
            Console.WriteLine();
            Console.WriteLine("Automated testing is disabled.");

            return;
        }

        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("VolunteerConnect Automated Testing");
        Console.WriteLine("=================================");

        Console.WriteLine("Test-1");
        await TestDatabaseConnection();
        Console.WriteLine("Test-2");
        await TestOpportunityRead();
        Console.WriteLine("Test-3");
        await TestRegistrationCreate();
        Console.WriteLine("Test-4");
        await TestRegistrationRead();
        Console.WriteLine("Test-5");
        await TestRegistrationUpdate();
        Console.WriteLine("Test-6");
        await TestRegistrationDelete();
        Console.WriteLine("Test-7");
        await TestRegistrationPersistence();

        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("Automated testing completed.");
        Console.WriteLine("=================================");
    }

    private async Task TestDatabaseConnection()
    {
        try
        {
            var opportunities =
                await _database.GetOpportunitiesAsync();

            Console.WriteLine(
                opportunities != null
                    ? "PASS: Database connection"
                    : "FAIL: Database connection");
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Database connection");
        }
    }

    private async Task TestOpportunityRead()
    {
        try
        {
            var opportunities =
                await _database.GetOpportunitiesAsync();

            if (opportunities.Count > 0)
            {
                Console.WriteLine(
                    "PASS: Opportunities can be read");
            }
            else
            {
                Console.WriteLine(
                    "FAIL: No opportunities found");
            }
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Opportunity read");
        }
    }

    private async Task TestRegistrationCreate()
    {
        try
        {
            var opportunities =
                await _database.GetOpportunitiesAsync();

            if (opportunities.Count == 0)
            {
                Console.WriteLine(
                    "FAIL: Registration create - no opportunity available");

                return;
            }

            var registration = new Registration
            {
                OpportunityId =
                    opportunities[0].OpportunityId,

                PreferredName =
                    "Automated Test User",

                Email =
                    "automated.test@example.com",

                Phone =
                    "0210000000",

                Availability =
                    "Weekends",

                Note =
                    "Automated testing record",

                PrivacyConsent =
                    true
            };

            await _database.AddRegistrationAsync(
                registration);

            Console.WriteLine(
                "PASS: Registration can be created");
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Registration create");
        }
    }

    private async Task TestRegistrationRead()
    {
        try
        {
            var registrations =
                await _database.GetRegistrationsAsync();

            var testRegistration =
                registrations.FirstOrDefault(
                    x => x.Email ==
                        "automated.test@example.com");

            if (testRegistration != null)
            {
                Console.WriteLine(
                    "PASS: Registration can be read");
            }
            else
            {
                Console.WriteLine(
                    "FAIL: Registration could not be read");
            }
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Registration read");
        }
    }

    private async Task TestRegistrationUpdate()
    {
        try
        {
            var registrations =
                await _database.GetRegistrationsAsync();

            var testRegistration =
                registrations.FirstOrDefault(
                    x => x.Email ==
                        "automated.test@example.com");

            if (testRegistration == null)
            {
                Console.WriteLine(
                    "FAIL: Registration update - test record not found");

                return;
            }

            testRegistration.Availability =
                "Flexible";

            await _database.UpdateRegistrationAsync(
                testRegistration);

            var updated =
                await _database.GetRegistrationAsync(
                    testRegistration.RegistrationId);

            if (updated != null &&
                updated.Availability == "Flexible")
            {
                Console.WriteLine(
                    "PASS: Registration can be updated");
            }
            else
            {
                Console.WriteLine(
                    "FAIL: Registration update");
            }
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Registration update");
        }
    }

    private async Task TestRegistrationDelete()
    {
        try
        {
            var registrations =
                await _database.GetRegistrationsAsync();

            var testRegistration =
                registrations.FirstOrDefault(
                    x => x.Email ==
                        "automated.test@example.com");

            if (testRegistration == null)
            {
                Console.WriteLine(
                    "FAIL: Registration delete - test record not found");

                return;
            }

            await _database.DeleteRegistrationAsync(
                testRegistration);

            var deleted =
                await _database.GetRegistrationAsync(
                    testRegistration.RegistrationId);

            if (deleted == null)
            {
                Console.WriteLine(
                    "PASS: Registration can be deleted");
            }
            else
            {
                Console.WriteLine(
                    "FAIL: Registration was not deleted");
            }
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Registration delete");
        }
    }

    private async Task TestRegistrationPersistence()
    {
        try
        {
            var database1 =
                new DatabaseService();

            var opportunities =
                await database1.GetOpportunitiesAsync();

            if (opportunities.Count == 0)
            {
                Console.WriteLine(
                    "FAIL: Persistence test - no opportunity available");

                return;
            }

            var registration = new Registration
            {
                OpportunityId =
                    opportunities[0].OpportunityId,

                PreferredName =
                    "Persistence Test User",

                Email =
                    "persistence.test@example.com",

                Phone =
                    "0210000001",

                Availability =
                    "Weekdays",

                Note =
                    "Persistence test",

                PrivacyConsent =
                    true
            };

            await database1.AddRegistrationAsync(
                registration);

            // Create a new database service.
            // This simulates the application being
            // closed and opened again.

            var database2 =
                new DatabaseService();

            var registrations =
                await database2.GetRegistrationsAsync();

            var found =
                registrations.Any(
                    x => x.Email ==
                        "persistence.test@example.com");

            if (found)
            {
                Console.WriteLine(
                    "PASS: Registration persists after reopening database");
            }
            else
            {
                Console.WriteLine(
                    "FAIL: Registration did not persist");
            }

            var saved =
                registrations.FirstOrDefault(
                    x => x.Email ==
                        "persistence.test@example.com");

            if (saved != null)
            {
                await database2.DeleteRegistrationAsync(
                    saved);
            }
        }
        catch
        {
            Console.WriteLine(
                "FAIL: Registration persistence");
        }
    }
}