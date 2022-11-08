using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using tsheet.domain;

namespace tsheet.ViewModels;

[DataContract]
public class TimesheetViewModel : ReactiveObject
{
    [DataMember]
    public IReadOnlyCollection<Day> Timesheet { get; }
    
    [Reactive]
    [DataMember]
    public DateOnly? SelectedDay { get; set; }

    public TimesheetViewModel(IQueries queries)
    {
        Timesheet = queries.GetTimesheet();
        SelectedDay = DateOnly.FromDateTime(DateTime.Today);
    }
}