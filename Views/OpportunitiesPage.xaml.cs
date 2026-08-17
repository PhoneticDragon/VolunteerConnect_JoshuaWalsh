using VolunteerConnect.Models;
using VolunteerConnect.Services;

namespace VolunteerConnect.Views;

public partial class OpportunitiesPage : ContentPage
{
    private readonly DatabaseService _database;

    private List<Opportunity> _allOpportunities = new();

    public OpportunitiesPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadOpportunities();
    }

    private async Task LoadOpportunities()
    {
        _allOpportunities =
            await _database.GetOpportunitiesAsync();

        OpportunitiesCollection.ItemsSource =
            _allOpportunities;

        LoadCategories();
    }

    private void LoadCategories()
    {
        var categories = _allOpportunities
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        categories.Insert(0, "All Categories");

        CategoryPicker.ItemsSource = categories;
        CategoryPicker.SelectedIndex = 0;
    }
    private void SearchBar_TextChanged(
    object sender,
    TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        string searchText =
            SearchBar.Text?.Trim().ToLower() ?? "";

        string selectedCategory =
            CategoryPicker.SelectedItem?.ToString()
            ?? "All Categories";

        var filtered = _allOpportunities.AsEnumerable();

        // Search
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            filtered = filtered.Where(x =>
                x.Title.ToLower().Contains(searchText) ||
                x.ShortDescription.ToLower().Contains(searchText) ||
                x.Location.ToLower().Contains(searchText));
        }

        // Category
        if (selectedCategory != "All Categories")
        {
            filtered = filtered.Where(x =>
                x.Category == selectedCategory);
        }

        OpportunitiesCollection.ItemsSource =
            filtered.ToList();
    }

    private void CategoryPicker_SelectedIndexChanged(
    object sender,
    EventArgs e)
    {
        ApplyFilters();
    }

    private async void ViewDetails_Clicked(
    object sender,
    EventArgs e)
    {
        if (sender is Button button &&
            button.CommandParameter is Opportunity opportunity)
        {
            await Navigation.PushAsync(
                new OpportunityDetailsPage(
                    opportunity.OpportunityId));
                    
        }
    }
}