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
        
        var siCursorPosition = new StatusItem (Key.Null, "", null);
        var statusBar = new StatusBar (new StatusItem [] {
            siCursorPosition,
            new StatusItem(Key.CtrlMask | Key.A, "~^A~ Add activity", () => AddActivity()),
        });
        Top.Add (statusBar);
        
        _timesheetViewModel = new TimesheetViewModel(queries);
        _daySheetView = new DaySheetView(Top, Win, _timesheetViewModel);
        _tasksCalendarView = new TasksCalendarView(Win, queries.DayTotalTaskDuration, _timesheetViewModel);
        _addActivityView = new AddActivityView(Win, Top, _timesheetViewModel);
    }

    private void AddActivity()
    {
        
        var lbl = new Label() {
            X = 0,
            Y = 1,
            Text = "Add activity"
        };

        var textField = new TextField()
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill()
        };
        bool okPressed = false;

        var ok = new Button ("Ok", is_default: true);
        ok.Clicked += () => { okPressed = true; Application.RequestStop (); };
        var cancel = new Button ("Cancel");
        cancel.Clicked += () => { Application.RequestStop (); };
        
        var dialog = new Dialog ("Add activity", 20, 10, ok, cancel) {};
        dialog.Add(lbl, textField);
        textField.SetFocus();
        
        Application.Run (dialog);
        if (okPressed)
        {
            _timesheetViewModel.AddActivity(textField.Text.ToString(), 0m);
        }
    }

    private Toplevel Top { get; }
    private Window Win { get; }
    private readonly DaySheetView _daySheetView;
    private readonly TasksCalendarView _tasksCalendarView;
    private readonly AddActivityView _addActivityView;
    private TimesheetViewModel _timesheetViewModel;

    public void Setup()
    {
        Win.Title = "tsheet";
        Win.Y = 1; // menu
        Win.Height = Dim.Fill(1);
    }
}