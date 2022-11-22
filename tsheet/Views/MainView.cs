using System.Collections;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Terminal.Gui;
using Terminal.Gui.TextValidateProviders;
using tsheet.domain;
using tsheet.ViewModels;
using tsheet.views;

namespace tsheet.Views;

public class MainView
{
    public MainView()
    {
        _timesheetViewModel = new TimesheetViewModel();
        
        Application.Init();
        RxApp.MainThreadScheduler = TerminalScheduler.Default;
        RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

        _jsonStorage = new JsonStorage("./AppState.json");

        _timesheetViewModel = _jsonStorage.LoadState();


        AppDomain.CurrentDomain.ProcessExit += SaveState;


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
        
        var queries = new Config();
        
        var siCursorPosition = new StatusItem (Key.Null, "", null);
        var statusBar = new StatusBar (new StatusItem [] {
            siCursorPosition,
            new StatusItem(Key.CtrlMask | Key.A, "~^A~ Add activity", ShowAddActivityWindow),
        });
        Top.Add (statusBar);
        
        _daySheetView = new DaySheetView(Top, Win, _timesheetViewModel);
        _tasksCalendarView = new TasksCalendarView(Win, queries.DayTotalTaskDuration, _timesheetViewModel);
        _addActivityView = new AddActivityView(Win, Top, _timesheetViewModel);
        _suggestionsApi = new SuggestionsApi();
        
        
        Win.Title = "tsheet";
        Win.Y = 1; // menu
        Win.Height = Dim.Fill(1);
        
        _tasksCalendarView.SetFocus();
    }

    private void SaveState(object? sender, EventArgs e)
    {
        _jsonStorage.SaveState(_timesheetViewModel);
    }


    private void ShowAddActivityWindow()
    {
        
        var lbl = new Label() {
            X = 0,
            Y = 1,
            Text = "Add activity"
        };

        var okPressed = false;

        var ok = new Button ("Ok", is_default: true);
        ok.Clicked += () => { okPressed = true; Application.RequestStop (); };
        var cancel = new Button ("Cancel");
        cancel.Clicked += () => { Application.RequestStop (); };
        
        
        var comboBox = new ComboBox () {
            X = 0,
            Y = 2,
            Width = Dim.Fill(),
            Height = Dim.Percent(70),
            HideDropdownListOnClick = true,
        };
        var suggestions = _suggestionsApi.GetWorkItemSuggestions(comboBox.Text.ToString() ?? string.Empty)
            .Select(x => x.Title)
            .ToList();
        comboBox.SetSource (suggestions);

        var timeSpentBox = new TextValidateField(new TextRegexProvider (@"\d*\.?\d*") { ValidateOnInput = true }) {
            X = Pos.Percent(0),
            Y = Pos.Percent(70),
            Width = Dim.Fill(),
            Height = Dim.Percent(30),
        };
        
        var dialog = new Dialog ("Add activity", ok, cancel) {Width = Dim.Percent(50), Height = Dim.Percent(50)};
        dialog.Add(lbl, comboBox, timeSpentBox);
        comboBox.SetFocus();
        
        comboBox.OpenSelectedItem += text =>
        {
            if (text.Item == -1) return;
            ok.SetFocus();
        };

        Application.Run (dialog);
        if (okPressed)
        {
            var workItem = new WorkItem(comboBox.Text.ToString(), new ManualTaskId(comboBox.Text.ToString()));
            _timesheetViewModel.AddActivity(workItem, decimal.Parse(timeSpentBox.Text.ToString()!));
        }
    }

    private Toplevel Top { get; }
    private Window Win { get; }
    private readonly DaySheetView _daySheetView;
    private readonly TasksCalendarView _tasksCalendarView;
    private readonly AddActivityView _addActivityView;
    private readonly TimesheetViewModel _timesheetViewModel;
    private readonly JsonStorage _jsonStorage;
    private readonly ISuggestionsApi _suggestionsApi;
}