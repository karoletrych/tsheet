namespace tsheet;

public record WorkItem;

public record WorkItemSuggestion(string Name);

public record WorkItemShare(WorkItem WorkItem, int Percentage);

public record WorkDay(DateOnly Date, IEnumerable<WorkItemShare> WorkItemShares);

public interface SuggestionsApi
{
    public IEnumerable<WorkItemSuggestion> GetWorkItemSuggestions();
    public IEnumerable<WorkDay> GetWorkDays();
}