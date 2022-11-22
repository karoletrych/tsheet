using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Terminal.Gui;
using tsheet.domain;
using tsheet.ViewModels;

namespace tsheet.views;

public record ActivityVm(string WorkItem, decimal TimeInHours);

internal class DaySheetView : View
{
    private readonly TableView _tableView;
    private readonly CompositeDisposable _disposable = new CompositeDisposable ();
    public TimesheetViewModel ViewModel { get; }

    public DaySheetView(Toplevel top, Window win, TimesheetViewModel viewModel)
    {
        Top = top;
        Win = win;
        ViewModel = viewModel;
        
        _tableView = new TableView
        {
            X = Pos.Percent(50), Y = 0, Width = Dim.Percent(50), Height = Dim.Fill(1)
        };
        
        Win.Add(_tableView);

        _tableView.KeyPress += HandleKeyPress;


        _tableView.ColorScheme = new ColorScheme
        {
            Disabled = Win.ColorScheme.Disabled,
            HotFocus = Win.ColorScheme.HotFocus,
            Focus = Win.ColorScheme.Focus,
            Normal = Application.Driver.MakeAttribute(Color.DarkGray, Color.Black)
        };

        ViewModel
            .WhenAnyValue(x => x.SelectedDay)
            .Select(selectedDate=> ToRows(selectedDate))
            .BindTo (_tableView, x => x.Table)
            .DisposeWith (_disposable);
    }

    private DataTable ToRows(DateOnly? selectedDate)
    {
        var dt = new DataTable();
        dt.Columns.Add("Work item");
        dt.Columns.Add("Time (h)");
        
        var contains = ViewModel.Timesheet.TryGetValue(selectedDate.Value, out var dateActivities);
        if (!contains)
            return dt;
        
        var activities = dateActivities.Select(s => new ActivityVm(s.WorkItem.Title, s.TaskDuration.ValueInHours));
        foreach (var activity in activities) dt.Rows.Add(activity.WorkItem, activity.TimeInHours);

        dt.Rows.Add(DBNull.Value, DBNull.Value);
        return dt;
    }

    private Toplevel Top { get; }
    private Window Win { get; }
    

    private void HandleKeyPress(KeyEventEventArgs e)
    {
        if (e.KeyEvent.Key is Key.DeleteChar or Key.Backspace)
        {
            SetActiveCellValue(null);
            return;
        }

        var digits = new[]
        {
            Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9
        };
        if (digits.Contains(e.KeyEvent.Key))
        {
            e.Handled = true;

            var digit = e.KeyEvent.KeyValue - (int)Key.D1;

            var oldValue = GetActiveCellValue();
            var newText = oldValue + digit;

            SetActiveCellValue(newText);
        }
    }

    private string? GetActiveCellValue()
    {
        var row = _tableView.SelectedRow;
        var col = _tableView.SelectedColumn;
        return _tableView.Table.Rows[row][col].ToString();
    }

    private void SetActiveCellValue(string? newText)
    {
        var row = _tableView.SelectedRow;
        var col = _tableView.SelectedColumn;
        _tableView.Table.Rows[row][col] = string.IsNullOrWhiteSpace(newText)
            ? DBNull.Value
            : newText;
        _tableView.Update();
    }
}