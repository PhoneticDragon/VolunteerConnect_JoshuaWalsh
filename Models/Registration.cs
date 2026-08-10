using SQLite;

namespace VolunteerConnect.Models;

public class Registration
{
    [PrimaryKey, AutoIncrement]
    public int RegistrationId { get; set; }

    public int OpportunityId { get; set; }

    public string PreferredName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Availability { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public bool PrivacyConsent { get; set; }
}