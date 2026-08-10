namespace VolunteerConnect.Views
{
    public partial class OpportunitiesPage : ContentPage
    {
        public OpportunitiesPage()
        {
            InitializeComponent();
        }

        private void OnCategoryChanged(object sender, EventArgs e)
        {
            // Filter will be handled by ViewModel
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            // Filter will be handled by ViewModel
        }

        private void OnSortChanged(object sender, EventArgs e)
        {
            // Sort will be handled by ViewModel
        }
    }
}