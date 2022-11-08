using System.Reactive.Concurrency;
using ReactiveUI;
using Terminal.Gui;
using tsheet.domain;
using tsheet.ViewModels;
using tsheet.views;

namespace tsheet.Views;

public class MainView
{
    public MainView()
    {
        Application.Init();
        RxApp.MainThreadScheduler = TerminalScheduler.Default;
        RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

        Top = Application.Top;

        Win = new Window("tsheet")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = Colors.Base
        };
        Top.Add(Win);
        
        var queries = new Queries();
        var timesheetViewModel = new TimesheetViewModel(queries);
        DaySheetView = new DaySheetView(Top, Win, timesheetViewModel);
        TasksCalendarView = new TasksCalendarView(Win, queries.DayTotalTaskDuration, timesheetViewModel);
    }

    private Toplevel Top { get; }
    private Window Win { get; }
    private DaySheetView DaySheetView { get; }
    private TasksCalendarView TasksCalendarView { get; }

    public void Setup()
    {
        Win.Title = "tsheet";
        Win.Y = 1; // menu
        Win.Height = Dim.Fill(1);
    }
}