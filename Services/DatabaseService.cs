using SQLite;
using VolunteerConnect.Models;

namespace VolunteerConnect.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "VolunteerConnect.db3");

        _database = new SQLiteAsyncConnection(databasePath);

        await _database.CreateTableAsync<Opportunity>();
        await _database.CreateTableAsync<Registration>();

        await SeedOpportunities();
    }

    private async Task SeedOpportunities()
    {
        if (_database is null)
            return;

        int opportunityCount =
            await _database.Table<Opportunity>().CountAsync();

        if (opportunityCount > 0)
            return;

        var opportunities = new List<Opportunity>
        {
            new Opportunity
            {
                Image = "beach_cleanup.png",
                Title = "Beach Cleanup",
                Category = "Environment",
                Date = new DateTime(2026, 8, 20),
                Time = "10:00 AM",
                Location = "Mission Bay",
                ShortDescription =
                    "Help clean up the local beach.",
                FullDescription =
                    "Join other community volunteers to clean " +
                    "the beach and surrounding areas.",
                Requirements =
                    "Suitable for ages 16+. Wear suitable outdoor clothing.",
                AvailablePlaces = 15
            },

            new Opportunity
            {
                Image = "food_bank.png",
                Title = "Community Food Bank Helper",
                Category = "Community",
                Date = new DateTime(2026, 8, 22),
                Time = "9:00 AM",
                Location = "Auckland CBD",
                ShortDescription =
                    "Help sort and organise food donations.",
                FullDescription =
                    "Assist the community food bank with sorting " +
                    "and organising donated food.",
                Requirements =
                    "No previous experience required.",
                AvailablePlaces = 8
            },

            new Opportunity
            {
                Image = "community_garden.png",
                Title = "Community Garden",
                Category = "Environment",
                Date = new DateTime(2026, 8, 24),
                Time = "11:00 AM",
                Location = "Mt Eden",
                ShortDescription =
                    "Help maintain a local community garden.",
                FullDescription =
                    "Work with other volunteers to maintain plants, " +
                    "garden beds and shared spaces.",
                Requirements =
                    "Suitable for beginners.",
                AvailablePlaces = 5
            }
        };

        await _database.InsertAllAsync(opportunities);
    }

    public async Task<List<Opportunity>> GetOpportunitiesAsync()
    {
        await Init();

        return await _database!
            .Table<Opportunity>()
            .ToListAsync();
    }

    public async Task<Opportunity?> GetOpportunityAsync(int id)
    {
        await Init();

        return await _database!
            .Table<Opportunity>()
            .Where(x => x.OpportunityId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddRegistrationAsync(
        Registration registration)
    {
        await Init();

        return await _database!
            .InsertAsync(registration);
    }

    public async Task<List<Registration>> GetRegistrationsAsync()
    {
        await Init();

        return await _database!
            .Table<Registration>()
            .ToListAsync();
    }

    public async Task<Registration?> GetRegistrationAsync(int id)
    {
        await Init();

        return await _database!
            .Table<Registration>()
            .Where(x => x.RegistrationId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> UpdateRegistrationAsync(
        Registration registration)
    {
        await Init();

        return await _database!
            .UpdateAsync(registration);
    }

    public async Task<int> DeleteRegistrationAsync(
        Registration registration)
    {
        await Init();

        return await _database!
            .DeleteAsync(registration);
    }
}