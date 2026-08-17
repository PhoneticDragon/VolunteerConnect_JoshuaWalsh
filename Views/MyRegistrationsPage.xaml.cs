using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect.Views;

public partial class MyRegistrationsPage : ContentPage
{
    private readonly DatabaseService _database;

    private List<RegistrationDisplay> _registrations = new();

    public MyRegistrationsPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadRegistrations();
    }

    private async Task LoadRegistrations()
    {
        try
        {
            var registrations =
                await _database.GetRegistrationsAsync();

            _registrations = new List<RegistrationDisplay>();

            foreach (var registration in registrations)
            {
                var opportunity =
                    await _database.GetOpportunityAsync(
                        registration.OpportunityId);

                _registrations.Add(
                    new RegistrationDisplay
                    {
                        Registration = registration,

                        OpportunityTitle =
                            opportunity?.Title
                            ?? "Opportunity unavailable"
                    });
            }

            RegistrationsCollection.ItemsSource =
                _registrations;

            RegistrationCountLabel.Text =
                _registrations.Count == 1
                    ? "1 saved registration"
                    : $"{_registrations.Count} saved registrations";
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "Unable to load your registrations.",
                "OK");
        }
    }

    public class RegistrationDisplay
    {
        public Registration Registration { get; set; } = null!;

        public string OpportunityTitle { get; set; } = "";

        public string PreferredName =>
            Registration.PreferredName;

        public string Email =>
            Registration.Email;

        public string Availability =>
            Registration.Availability;
    }

    private async void ViewRegistration_Clicked(
    object sender,
    EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not RegistrationDisplay display)
        {
            return;
        }

        var registration = display.Registration;

        var opportunity =
            await _database.GetOpportunityAsync(
                registration.OpportunityId);

        if (opportunity == null)
        {
            await DisplayAlert(
                "Opportunity Unavailable",
                "The opportunity associated with this registration could not be found.",
                "OK");

            return;
        }

        string details =
            $"Opportunity: {opportunity.Title}\n\n" +
            $"Preferred Name: {registration.PreferredName}\n" +
            $"Email: {registration.Email}\n" +
            $"Phone: {registration.Phone}\n" +
            $"Availability: {registration.Availability}\n\n" +
            $"Note: {registration.Note}";

        await DisplayAlert(
            "Registration Details",
            details,
            "Close");
    }

    private async void EditRegistration_Clicked(
    object sender,
    EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not RegistrationDisplay display)
        {
            return;
        }

        await ShowEditRegistrationDialog(
            display.Registration);
    }

    private async Task ShowEditRegistrationDialog(
    Registration registration)
    {
        string? preferredName =
            await DisplayPromptAsync(
                "Preferred Name",
                "Enter your preferred name:",
                initialValue: registration.PreferredName);

        if (preferredName == null)
            return;

        preferredName = preferredName.Trim();

        if (preferredName.Length < 2 ||
            preferredName.Length > 50)
        {
            await DisplayAlert(
                "Invalid Name",
                "Your preferred name must be between 2 and 50 characters.",
                "OK");

            return;
        }

        string? email =
            await DisplayPromptAsync(
                "Email",
                "Enter your email address:",
                initialValue: registration.Email);

        if (email == null)
            return;

        email = email.Trim();

        string? phone =
            await DisplayPromptAsync(
                "Phone",
                "Enter your phone number:",
                initialValue: registration.Phone);

        if (phone == null)
            return;

        phone = phone.Trim();

        string? availability =
            await DisplayActionSheet(
                "Select Availability",
                "Cancel",
                null,
                "Weekdays",
                "Weekends",
                "Evenings",
                "Flexible");

        if (availability == null ||
            availability == "Cancel")
        {
            return;
        }

        string? note =
            await DisplayPromptAsync(
                "Optional Note",
                "Enter an optional note:",
                initialValue: registration.Note);

        if (note == null)
            return;

        note = note.Trim();

        // Contact validation

        bool hasEmail =
            !string.IsNullOrWhiteSpace(email);

        bool hasPhone =
            !string.IsNullOrWhiteSpace(phone);

        if (!hasEmail && !hasPhone)
        {
            await DisplayAlert(
                "Contact Information Required",
                "Please provide either an email address or phone number.",
                "OK");

            return;
        }

        if (hasEmail && !IsValidEmail(email))
        {
            await DisplayAlert(
                "Invalid Email",
                "Please enter a valid email address.",
                "OK");

            return;
        }

        if (hasPhone && !IsValidPhone(phone))
        {
            await DisplayAlert(
                "Invalid Phone",
                "Please enter a valid phone number.",
                "OK");

            return;
        }

        if (note.Length > 500)
        {
            await DisplayAlert(
                "Note Too Long",
                "Your note must be 500 characters or fewer.",
                "OK");

            return;
        }

        registration.PreferredName = preferredName;
        registration.Email = email;
        registration.Phone = phone;
        registration.Availability = availability;
        registration.Note = note;

        await _database.UpdateRegistrationAsync(
            registration);

        await DisplayAlert(
            "Registration Updated",
            "Your registration has been updated successfully.",
            "OK");

        await LoadRegistrations();
    }

    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPhone(string phone)
    {
        string digitsOnly =
            new string(
                phone.Where(char.IsDigit).ToArray());

        return digitsOnly.Length >= 7 &&
               digitsOnly.Length <= 15;
    }

    private async void DeleteRegistration_Clicked(
    object sender,
    EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not RegistrationDisplay display)
        {
            return;
        }

        var registration =
            display.Registration;

        bool confirmed =
            await DisplayAlert(
                "Cancel Registration",
                $"Are you sure you want to cancel your registration for {display.OpportunityTitle}?",
                "Yes, Delete",
                "No");

        if (!confirmed)
            return;

        try
        {
            await _database.DeleteRegistrationAsync(
                registration);

            await DisplayAlert(
                "Registration Deleted",
                "Your registration and associated personal information have been deleted from this device.",
                "OK");

            await LoadRegistrations();
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "The registration could not be deleted.",
                "OK");
        }
    }
}