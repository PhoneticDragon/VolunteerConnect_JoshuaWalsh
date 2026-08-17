using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect.Views;

public partial class OpportunityDetailsPage : ContentPage
{
    private readonly DatabaseService _database;

    private readonly int _opportunityId;

    private Opportunity? _opportunity;

    public OpportunityDetailsPage(int opportunityId)
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

            BindingContext = _opportunity;

            DisplayOpportunity();
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "Unable to load the opportunity.",
                "OK");

            await Navigation.PopAsync();
        }
    }

    private void DisplayOpportunity()
    {
        if (_opportunity == null)
            return;

        TitleLabel.Text =
            _opportunity.Title;

        CategoryLabel.Text =
            $"Category: {_opportunity.Category}";

        DateLabel.Text =
            $"Date: {_opportunity.Date:dd MMMM yyyy}";

        TimeLabel.Text =
            $"Time: {_opportunity.Time}";

        LocationLabel.Text =
            $"Location: {_opportunity.Location}";

        PlacesLabel.Text =
            $"{_opportunity.AvailablePlaces} places available";

        DescriptionLabel.Text =
            _opportunity.FullDescription;

        RequirementsLabel.Text =
            _opportunity.Requirements;

        // Disable registration if there are no places.
        if (_opportunity.AvailablePlaces <= 0)
        {
            PlacesLabel.Text = "No places available";
            RegisterButton.IsEnabled = false;
            RegisterButton.Text = "Fully Booked";
        }
    }

    private async void RegisterButton_Clicked(
    object sender,
    EventArgs e)
    {
        if (_opportunity == null)
            return;

        if (_opportunity.AvailablePlaces <= 0)
        {
            await DisplayAlert(
                "Opportunity Full",
                "There are currently no available places for this opportunity.",
                "OK");

            return;
        }

        await Navigation.PushAsync(
            new RegistrationPage(
                _opportunity.OpportunityId));
    }
}