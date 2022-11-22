using System.Text.Json.Serialization;

namespace tsheet.domain;

public interface IConfig
{
    public TaskDuration DayTotalTaskDuration { get; }
}

public class Config : IConfig
{
    public TaskDuration DayTotalTaskDuration => new TaskDuration(8m);
}

public record WorkItem(string Title, ITaskId TaskId);

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(ManualTaskId))]
public interface ITaskId
{
}

public record AzureDevopsTaskId : ITaskId
{
    public AzureDevopsTaskId(int taskId)
    {
        TaskId = taskId;
    }

    private int TaskId { get; }
}

public record ManualTaskId : ITaskId
{
    public ManualTaskId(string taskId)
    {
        TaskId = taskId;
    }

    private string TaskId { get; }
}

public record TaskDuration(decimal ValueInHours);

public static class DomainEx
{
    public static TaskDuration Sum<T>(this IReadOnlyCollection<T> collection, Func<T, TaskDuration> selector)
    {
        return collection.Select(selector)
            .Aggregate(new TaskDuration(0m),
                (a, b) => new TaskDuration(a.ValueInHours + b.ValueInHours));
    }
}

public record Activity(WorkItem WorkItem, TaskDuration TaskDuration);

public record Day(DateOnly Date, IReadOnlyCollection<Activity> Activities);

