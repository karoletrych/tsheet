using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace tsheet.Views;

public class CalendarView : View
{
    private const int DisplayWidth = 9;
    private const int DaysInWeek = 7;
    private readonly int _beginOfMonthOffset;
    private readonly int daysInMonth;

    public CalendarView(DateTime currentMonth)
    {
        CurrentMonth = currentMonth;

        daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var firstDayOfMonth =
            new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1).DayOfWeek;
        _beginOfMonthOffset = (int)firstDayOfMonth - 1;


        CanFocus = true;

        AddCommand(Command.Left, () => MoveLeft());
        AddCommand(Command.Right, () => MoveRight());
        AddCommand(Command.LineDown, () => MoveDown());
        AddCommand(Command.LineUp, () => MoveUp());
        AddCommand(Command.ToggleChecked, DayActivated);


        AddKeyBinding(Key.CursorLeft, Command.Left);
        AddKeyBinding(Key.CursorRight, Command.Right);
        AddKeyBinding(Key.CursorDown, Command.LineDown);
        AddKeyBinding(Key.CursorUp, Command.LineUp);
        AddKeyBinding(Key.Enter, Command.ToggleChecked);
    }

    private int _position;
    private int Position
    {
        get => _position;
        set
        {
            _position = value;
            SelectedDateChanged?.Invoke(this, ToDate(SelectedDayOfMonth));
        }
    }

    public event SelectedDateChanged SelectedDateChanged;
    public event SelectedDayActivated SelectedDayActivated;

    private DateTime CurrentMonth { get; }

    private bool? DayActivated()
    {
        SelectedDayActivated?.Invoke(this, SelectedDayOfMonth);
        return true;
    }


    /// <inheritdoc />
    public override void Redraw(Rect bounds)
    {
        Attribute currentAttribute;
        var current = ColorScheme.Focus;
        Driver.SetAttribute(current);
        Move(0, 0);

        var frame = Frame;
        int trackingColor = ColorScheme.HotFocus;

        var highlightedDays = HighlightedDays.Where(x =>
            x.Key.Year == CurrentMonth.Year && x.Key.Month == CurrentMonth.Month);

        for (var week = 0; week < frame.Height; week++)
        {
            var lineRect = new Rect(0, week, frame.Width, 1);
            if (!bounds.Contains(lineRect))
                continue;

            Move(0, week);
            currentAttribute = ColorScheme.HotNormal;
            SetAttribute(GetNormalColor());

            for (var dayOfWeek = 0; dayOfWeek < DaysInWeek; dayOfWeek++)
            {
                var offset = week * DaysInWeek + dayOfWeek;
                var day = PositionToDayOfMonth(offset);
                if (offset == Position)
                    SetAttribute(trackingColor);
                else if (day > 0 && day <= daysInMonth && HighlightedDays.TryGetValue(ToDate(day), out var attribute))
                    SetAttribute(ColorScheme.HotFocus);
                else
                    SetAttribute(GetNormalColor());

                Driver.AddStr(offset >= daysInMonth + _beginOfMonthOffset || offset < _beginOfMonthOffset
                    ? "  "
                    : day / 10 == 0
                        ? $" {day}"
                        : day.ToString());
                SetAttribute(GetNormalColor());
                Driver.AddRune(' ');
            }
        }

        void SetAttribute(Attribute attribute)
        {
            if (currentAttribute == attribute) return;
            currentAttribute = attribute;
            Driver.SetAttribute(attribute);
        }
    }

    private DateOnly ToDate(int dayOfMonth) => new DateOnly(CurrentMonth.Year, CurrentMonth.Month, dayOfMonth);

    private int PositionToDayOfMonth(int offset) => offset + 1 - _beginOfMonthOffset;
    public int SelectedDayOfMonth => Position + 1 - _beginOfMonthOffset;
    
    //TODO: do not allow selectedDayOfMonth to be set to invalid value
    public DateOnly SelectedDate => new DateOnly(CurrentMonth.Year, CurrentMonth.Month, SelectedDayOfMonth);
    public Dictionary<DateOnly, int> HighlightedDays { get; set; }


    private void RedisplayLine(long pos)
    {
        var delta = (int)pos;
        var line = delta / DaysInWeek;

        SetNeedsDisplay(new Rect(0, line, Frame.Width, 1));
    }


    private bool MoveLeft()
    {
        RedisplayLine(Position);

        if (Position == 0)
            return true;
        if (Position - 1 < 0)
            SetNeedsDisplay();
        else
            RedisplayLine(Position);

        Position--;

        return true;
    }

    private bool MoveRight()
    {
        RedisplayLine(Position);

        if (Position < 666) //TODO;
            Position++;
        if (Position >= DaysInWeek * Frame.Height)
            SetNeedsDisplay();
        else
            RedisplayLine(Position);

        return true;
    }

    private bool MoveUp()
    {
        RedisplayLine(Position);
        if (Position - DaysInWeek > -1)
            Position -= DaysInWeek;
        if (Position < 0)
            SetNeedsDisplay();
        else
            RedisplayLine(Position);

        return true;
    }

    private bool MoveDown()
    {
        RedisplayLine(Position);
        Position += DaysInWeek;
        if (Position >= DaysInWeek * Frame.Height)
            SetNeedsDisplay();
        else
            RedisplayLine(Position);

        return true;
    }

    public override bool ProcessKey(KeyEvent keyEvent)
    {
        var result = InvokeKeybindings(keyEvent);
        if (result != null)
            return (bool)result;

        return false;
    }

}

public delegate void SelectedDayActivated(object sender, int day);

public delegate void SelectedDateChanged(object sender, DateOnly day);
