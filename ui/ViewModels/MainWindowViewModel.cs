namespace algo_vis.ui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (value == _currentViewModel) return;
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        CurrentViewModel = new MainMenuViewModel(this);
    }

    public void NavigateTo(ViewModelBase next)
    {
        CurrentViewModel = next;
    }
}