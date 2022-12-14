using System.Text.Json;
using tsheet.ViewModels;

namespace tsheet;

public class JsonStorage 
{
    private readonly string _stateFilePath;

    public JsonStorage(string stateFilePath)
    {
        _stateFilePath = stateFilePath;
    }

    public TimesheetViewModel LoadState()
    {
        if (!File.Exists(_stateFilePath))
        {
            SaveState(new TimesheetViewModel());
        }
        
        var lines = File.ReadAllText(_stateFilePath);
        var state = JsonSerializer.Deserialize<TimesheetViewModel>(lines);
        if (state is null)
            throw new InvalidOperationException("file is empty");
        return state;
    }

    public void SaveState(TimesheetViewModel state)
    {
        var lines = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_stateFilePath, lines);
    }
}