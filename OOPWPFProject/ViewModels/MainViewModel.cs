using OOPWPFProject.ViewModels.Services;
using OOPWPFProject.ViewModels.VMBase;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{

    public PlaceAddViewModel PlaceAddViewModel { get; }
    public PlaceListViewModel PlaceListViewModel { get; }

    public MainViewModel ()
    {
        PlaceStore sharedStore = new();

        PlaceAddViewModel = new PlaceAddViewModel( sharedStore );
        PlaceListViewModel = new PlaceListViewModel( sharedStore );

    }
}