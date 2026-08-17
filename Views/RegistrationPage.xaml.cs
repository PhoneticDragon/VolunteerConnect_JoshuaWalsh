using System.Text.RegularExpressions;
using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect.Views;

public partial class RegistrationPage : ContentPage
{
    private readonly DatabaseService _database;

    private readonly int _opportunityId;

    private Opportunity? _opportunity;

    public RegistrationPage(int opportunityId)
    {
        InitializeComponent();

        _database = new DatabaseService();

        _opportunityId = opportunityId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadOpportunity();
    }

    private async Task LoadOpportunity()
    {
        try
        {
            _opportunity =
                await _database.GetOpportunityAsync(
                    _opportunityId);

            if (_opportunity == null)
            {
                await DisplayAlert(
                    "Opportunity Not Found",
                    "The selected volunteer opportunity could not be found.",
                    "OK");

                await Navigation.PopAsync();

                return;
            }

            OpportunityNameLabel.Text =
                _opportunity.Title;

            if (_opportunity.AvailablePlaces <= 0)
            {
                SaveButton.IsEnabled = false;

                await DisplayAlert(
                    "Opportunity Full",
                    "There are no available places for this opportunity.",
                    "OK");

                await Navigation.PopAsync();
            }
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "Unable to load the volunteer opportunity.",
                "OK");

            await Navigation.PopAsync();
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool ValidateForm()
    {
        // Preferred name
        if (string.IsNullOrWhiteSpace(
            PreferredNameEntry.Text))
        {
            DisplayAlert(
                "Missing Information",
                "Please enter your preferred name.",
                "OK");

            PreferredNameEntry.Focus();

            return false;
        }

        string preferredName =
            PreferredNameEntry.Text.Trim();

        if (preferredName.Length < 2 ||
            preferredName.Length > 50)
        {
            DisplayAlert(
                "Invalid Name",
                "Your preferred name must be between 2 and 50 characters.",
                "OK");

            PreferredNameEntry.Focus();

            return false;
        }

        // Email
        if (string.IsNullOrWhiteSpace(
            EmailEntry.Text))
        {
            DisplayAlert(
                "Missing Information",
                "Please enter your email address.",
                "OK");

            EmailEntry.Focus();

            return false;
        }

        if (!IsValidEmail(
            EmailEntry.Text.Trim()))
        {
            DisplayAlert(
                "Invalid Email",
                "Please enter a valid email address.",
                "OK");

            EmailEntry.Focus();

            return false;
        }

        // Availability
        if (AvailabilityPicker.SelectedIndex == -1)
        {
            DisplayAlert(
                "Missing Information",
                "Please select your availability.",
                "OK");

            AvailabilityPicker.Focus();

            return false;
        }

        // Note length
        if (!string.IsNullOrWhiteSpace(NoteEditor.Text) &&
            NoteEditor.Text.Length > 500)
        {
            DisplayAlert(
                "Note Too Long",
                "Your note must be 500 characters or fewer.",
                "OK");

            NoteEditor.Focus();

            return false;
        }

        // Privacy consent
        if (!PrivacyConsentCheckBox.IsChecked)
        {
            DisplayAlert(
                "Consent Required",
                "You must provide privacy consent before your registration can be saved.",
                "OK");

            return false;
        }

        return true;
    }

    private async void SaveButton_Clicked(
    object sender,
    EventArgs e)
    {
        if (_opportunity == null)
            return;

        if (!ValidateForm())
            return;

        try
        {
            SaveButton.IsEnabled = false;

            var registration = new Registration
            {
                OpportunityId =
                    _opportunity.OpportunityId,

                PreferredName =
                    PreferredNameEntry.Text.Trim(),

                Email =
                    EmailEntry.Text?.Trim() ?? string.Empty,

                Phone =
                    PhoneEntry.Text?.Trim() ?? string.Empty,

                Availability =
                    AvailabilityPicker.SelectedItem?.ToString()
                    ?? string.Empty,

                Note =
                    NoteEditor.Text?.Trim() ?? string.Empty,

                PrivacyConsent =
                    PrivacyConsentCheckBox.IsChecked
            };

            await _database.AddRegistrationAsync(
                registration);

            await DisplayAlert(
                "Registration Saved",
                "Your volunteer registration has been saved successfully.",
                "OK");

            await Navigation.PopToRootAsync();
        }
        catch
        {
            SaveButton.IsEnabled = true;

            await DisplayAlert(
                "Error",
                "Your registration could not be saved. Please try again.",
                "OK");
        }
    }
}