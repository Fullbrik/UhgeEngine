namespace UhgeEngine.Tools.Editor;

public class EditorInteractiveMode
{
    public bool IsRunning { get; private set; }
    
    private EditorProject _project;
    private string? _currentAssetBundle;
    private string _currentFolder = "/";

    public EditorInteractiveMode(EditorProject project)
    {
        _project = project;
    }

    public void Run()
    {
        IsRunning = true;

        while (IsRunning)
        {
            var command = GetNextCommand();

            switch (command.ToLower())
            {
                case "exit":
                    Exit();
                    break;
                case "ls":
                    Ls();
                    break;
                default:
                    Console.WriteLine("Unknown command: {0}", command);
                    break;
            }
        }
    }

    private string GetNextCommand()
    {
        Console.Write("[{0}:{1}]$: ", _currentAssetBundle?? "", _currentFolder);
        var input = Console.ReadLine()?? "";
        return input;
    }

    private void Exit()
    {
        IsRunning = false;
    }

    private void Ls()
    {
        
    }
}