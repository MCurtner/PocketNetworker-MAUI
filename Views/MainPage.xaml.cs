using PocketNetworker.ViewModels;

namespace PocketNetworker
{
    public partial class MainPage : ContentPage
    {

        public MainPage(PocketNetworkerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}