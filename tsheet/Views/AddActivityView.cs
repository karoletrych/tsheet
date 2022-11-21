using Terminal.Gui;
using tsheet.ViewModels;

namespace tsheet.Views;

public class AddActivityView
{
    private readonly Window _window;
    private readonly Toplevel _top;

    public AddActivityView(TimesheetViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public AddActivityView(Window window, Toplevel top, TimesheetViewModel timesheetViewModel)
    {
        _window = window;
        _top = top;
        ViewModel = timesheetViewModel;
    }

    public TimesheetViewModel ViewModel { get; }

}