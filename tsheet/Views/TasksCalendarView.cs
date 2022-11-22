using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;
using tsheet.domain;
using tsheet.ViewModels;
using Attribute = Terminal.Gui.Attribute;

namespace tsheet.Views;

public class TasksCalendarView : View
{
    private readonly CalendarView _calendarView;
    public TimesheetViewModel ViewModel { get; }

    private readonly Window _parent;
    private readonly TaskDuration _dayTotalTaskDuration;

    private readonly CompositeDisposable _disposable = new CompositeDisposable ();


    public TasksCalendarView(Window parent, TaskDuration dayTotalTaskDuration, TimesheetViewModel timesheetViewModel)
    {
        _parent = parent;
        _dayTotalTaskDuration = dayTotalTaskDuration;
        ViewModel = timesheetViewModel;
        _calendarView = new CalendarView(DateTime.Now)
        {
            X = 0, Y = 0, Width = Dim.Percent(50), Height = Dim.Fill()
        };
        
        _calendarView
            .Events ()
            .SelectedDateChanged
            .DistinctUntilChanged ()
            .BindTo (ViewModel, x => x.SelectedDay)
            .DisposeWith (_disposable);

        var fullDays = ViewModel
            .Timesheet
            .Where(d => d.Value.Sum(s => s.TaskDuration) == _dayTotalTaskDuration)
            .Select(x=> x.Key)
            .ToList();
        _calendarView.HighlightedDays = fullDays.ToDictionary(x => x, _ => 1);
        
        _parent.Add(_calendarView);
        _calendarView.SetFocus();
    }
}