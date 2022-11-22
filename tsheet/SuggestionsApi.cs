using tsheet.domain;

namespace tsheet;


public interface ISuggestionsApi
{
    public IEnumerable<WorkItem> GetWorkItemSuggestions(string enteredText);
    public IEnumerable<Day> GetWorkDays();
}

public class SuggestionsApi : ISuggestionsApi
{
    public IEnumerable<WorkItem> GetWorkItemSuggestions(string enteredText) => new []
    {
        new WorkItem("Task 1", new AzureDevopsTaskId(1)),
        new WorkItem("Task 2", new AzureDevopsTaskId(2)),
        new WorkItem("Task 3", new AzureDevopsTaskId(3) ),
        new WorkItem("Task 4", new ManualTaskId("my task")),
    };

    public IEnumerable<Day> GetWorkDays() => throw new NotImplementedException();
}