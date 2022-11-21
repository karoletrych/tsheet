using tsheet.domain;

namespace tsheet;


public interface ISuggestionsApi
{
    public IEnumerable<WorkItem> GetWorkItemSuggestions();
    public IEnumerable<Day> GetWorkDays();
}