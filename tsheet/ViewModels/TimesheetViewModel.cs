using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using tsheet.domain;
using System.Text.Json;

namespace tsheet.ViewModels;

// todo dotnet 7
// [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public class TimesheetViewModel : ReactiveObject, IDisposable
{
    [DataMember]
    public Dictionary<DateOnly, List<Activity>> Timesheet { get; }
    
    [Reactive]
    [DataMember]
    public DateOnly? SelectedDay { get; set; }
    
    public TimesheetViewModel()
    {
        var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - 1).Select(
            x =>
                new Day(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, x),
                    Array.Empty<Activity>().ToList()));
        
        Timesheet = daysInMonth
            .ToDictionary(x=>x.Date, x=>x.Activities.ToList());
        SelectedDay = DateOnly.FromDateTime(DateTime.Today);
    }

    public void AddActivity(WorkItem workItem, decimal duration)
    {
        Timesheet[SelectedDay.Value]
            .Add(new Activity(workItem, new TaskDuration(duration)));
    }

    public void Dispose()
    {
    }
}