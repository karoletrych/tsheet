using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using tsheet.domain;

namespace tsheet.ViewModels;

[DataContract]
public class TimesheetViewModel : ReactiveObject
{
    [DataMember]
    public Dictionary<DateOnly, List<Activity>> Timesheet { get; }
    
    [Reactive]
    [DataMember]
    public DateOnly? SelectedDay { get; set; }
    
    [Reactive]
    public bool IsAddingActivity { get; set; }

    public TimesheetViewModel(IQueries queries)
    {
        Timesheet = queries.GetTimesheet().ToDictionary(x=>x.Date, x=>x.Activities.ToList());
        SelectedDay = DateOnly.FromDateTime(DateTime.Today);
    }

    public void AddActivity(string? activityName, decimal duration)
    {
        Timesheet[SelectedDay.Value]
            .Add(new Activity(new WorkItem(activityName), new TaskDuration(duration)));
    }
}