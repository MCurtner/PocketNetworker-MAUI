using CommunityToolkit.Mvvm.ComponentModel;

namespace PocketNetworker.ViewModels;

public partial class PocketNetworkerViewModel : ObservableObject
{
    [ObservableProperty]
    string ipAddress;
}