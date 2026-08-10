using SQLite;

namespace VolunteerConnect.Models;

public class Opportunity
{
    [PrimaryKey, AutoIncrement]
    public int OpportunityId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string Time { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string FullDescription { get; set; } = string.Empty;

    public string Requirements { get; set; } = string.Empty;

    public int AvailablePlaces { get; set; }

    public string Image { get; set; } = string.Empty;
}