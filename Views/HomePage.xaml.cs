using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _database;

    private Opportunity? _featuredOpportunity;

    public HomePage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadHomePage();
    }

    private async Task LoadHomePage()
    {
        try
        {
            var opportunities =
                await _database.GetOpportunitiesAsync();

            if (opportunities.Count == 0)
            {
                OpportunityCountLabel.Text =
                    "No volunteer opportunities are currently available.";

                FeaturedTitleLabel.Text =
                    "No Featured Opportunity";

                FeaturedDescriptionLabel.Text =
                    "There are currently no volunteer opportunities available.";

                return;
            }

            // Display the number of opportunities
            OpportunityCountLabel.Text =
                $"{opportunities.Count} Available Opportunities";

            // Select the first opportunity as the featured opportunity
            _featuredOpportunity = opportunities[0];

            FeaturedTitleLabel.Text =
                _featuredOpportunity.Title;

            FeaturedCategoryLabel.Text =
                $"Category: {_featuredOpportunity.Category}";

            FeaturedDateLabel.Text =
                $"Date: {_featuredOpportunity.Date:dd MMMM yyyy}";

            FeaturedLocationLabel.Text =
                $"Location: {_featuredOpportunity.Location}";

            FeaturedDescriptionLabel.Text =
                _featuredOpportunity.ShortDescription;
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "Unable to load volunteer opportunities.",
                "OK");
        }
    }

    private async void ViewFeaturedDetails_Clicked(
    object sender,
    EventArgs e)
    {
        if (_featuredOpportunity == null)
            return;

        await Navigation.PushAsync(
            new OpportunityDetailsPage(
                _featuredOpportunity.OpportunityId));
    }
    private async void BrowseOpportunities_Clicked(
    object sender,
    EventArgs e)
    {
        await Navigation.PushAsync(
            new OpportunitiesPage());
    }
    private async void MyRegistrations_Clicked(
    object sender,
    EventArgs e)
    {
        await Navigation.PushAsync(
            new MyRegistrationsPage());
    }
    private async void PrivacyInformation_Clicked(
    object sender,
    EventArgs e)
    {
        await Navigation.PushAsync(
            new PrivacyPage());
    }
}