namespace tsheet.domain;

public interface IQueries
{
    public TaskDuration DayTotalTaskDuration { get; }
    public IReadOnlyCollection<Day> GetTimesheet();
}

public class Queries : IQueries
{
    public IReadOnlyCollection<Day> GetTimesheet() => Consts.SampleDays;

    public TaskDuration DayTotalTaskDuration => new TaskDuration(8m);
}

public record WorkItem(string Name);

public record TaskDuration(decimal ValueInHours);

public static class DomainEx
{
    public static TaskDuration Sum<T>(this IReadOnlyCollection<T> collection, Func<T, TaskDuration> selector)
    {
        return collection.Select(selector)
            .Aggregate((a, b) => new TaskDuration(a.ValueInHours + b.ValueInHours));
    }
}

public record Activity(WorkItem WorkItem, TaskDuration TaskDuration);

public record Day(DateOnly Date, IReadOnlyCollection<Activity> Activities);

public class Consts
{
    public static IReadOnlyCollection<Day> SampleDays = new List<Day>
    {
        new Day(
            new DateOnly(2022, 11, 01),
            new[]
            {
                new Activity(new WorkItem("Przycisk Przelicz A"), new TaskDuration(2m)),
                new Activity(new WorkItem("Przycisk Przelicz B"), new TaskDuration(2m)),
                new Activity(new WorkItem("Przycisk Przelicz C"), new TaskDuration(4m))
            }
        ),
        new Day(
            new DateOnly(2022, 11, 02),
            new[]
            {
                new Activity(new WorkItem("Przycisk Przelicz A"), new TaskDuration(2m)),
                new Activity(new WorkItem("Przycisk Przelicz B"), new TaskDuration(2m)),
                new Activity(new WorkItem("Przycisk Przelicz C"), new TaskDuration(4m))
            }
        ),
        new Day(
            new DateOnly(2022, 11, 03),
            new[]
            {
                new Activity(new WorkItem("Przycisk Przelicz A"), new TaskDuration(2m)),
                new Activity(new WorkItem("Przycisk Przelicz B"), new TaskDuration(2m))
            }
        )
    };
}