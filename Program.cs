// Max Sultan, Feb 11th 2026, Lab 4: The Big Todo

using System.Runtime.CompilerServices;

class Task(string Title)
{
    public string Title = Title; 
    public CompletionStatus Status = CompletionStatus.InProgess;

    public void ToggleStatus()
    {
        Status = Status == CompletionStatus.InProgess ? CompletionStatus.Done : CompletionStatus.InProgess;
    }
}
class TodoList
{
    private static readonly List<Task> tasks = [new Task("Laundry"), new Task("Homework")];
    public int Length = tasks.Count;

    private static int currentIndex = 0;
    public Task? CurrentTask = tasks[currentIndex];

    public Task? GetTask(int index)
    {
        if (index >= tasks.Count) return null;
        return tasks[index];
    }

    public void SelectPrevious()
    {
        if(tasks.Count < 2) return; 
        currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : tasks.Count - 1; 
        CurrentTask = tasks[currentIndex];
    }
    public void SelectNext()
    {
        if(tasks.Count < 2) return;
        currentIndex = currentIndex + 1 < Length ? currentIndex + 1 : 0;
        CurrentTask = tasks[currentIndex];
    }
                    
    public void SwapWithNext()
    {
        if(CurrentTask == null) return;
        int nextValidIndex = currentIndex + 1 < Length ? currentIndex + 1 : 0;
        tasks.Remove(CurrentTask);
        tasks.Insert(nextValidIndex, CurrentTask);
        currentIndex = nextValidIndex;
    }

    public void SwapWithPrevious()
    {
        if(CurrentTask == null) return;
        int previousValidIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : tasks.Count - 1; 
        tasks.Remove(CurrentTask);
        tasks.Insert(previousValidIndex, CurrentTask);
        currentIndex = previousValidIndex;
    }
    public void Insert(string title)
    {
        if(title == "" || title == null) return;
        tasks.Add(new Task(title));
        currentIndex = 0;
        CurrentTask = tasks[currentIndex];
        Length = tasks.Count;
    }
    public void DeleteSelected()
    {
        if(CurrentTask == null) return;
        tasks.RemoveAt(currentIndex);
        if(tasks.Count == 0)
        {
            CurrentTask = null;
            Length = tasks.Count;
            return;
        }
        currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : 0; 
        CurrentTask = tasks[currentIndex];
        Length = tasks.Count;
    }

}
class TodoListApp {
    private TodoList _tasks;
    private bool _showHelp = true;
    private bool _insertMode = false;
    private bool _quit = false;

    public TodoListApp(TodoList tasks) {
        _tasks = tasks;
    }

    public void Run() {
        while (!_quit) {
            Console.Clear();
            Display();
            ProcessUserInput();
        }
    }

    public void Display() {
        DisplayTasks();
        if (_showHelp) {
            DisplayHelp();
        }
    }

    public void DisplayBar() {
        Console.WriteLine("----------------------------");
    }

    public string MakeRow(int i) {
        Task task = _tasks.GetTask(i);
        if(task == null) return "";
        string arrow = "  ";
        if (task == _tasks.CurrentTask) arrow = "->";
        string check = " ";
        if (task.Status == CompletionStatus.Done) check = "X";
        return $"{arrow} [{check}] {task.Title}";
    }

    public void DisplayTasks() {
        DisplayBar();
        Console.WriteLine("Tasks:");
        for (int i = 0; i < _tasks.Length; i++) {
            if(MakeRow(i) != "") Console.WriteLine(MakeRow(i));
        }
        DisplayBar();
    }

    public void DisplayHelp() {
        Console.WriteLine(
@"Instructions:
   h: show/hide instructions
   ↕: select previous or next task (wrapping around at the top and bottom)
   ↔: reorder task (swap selected task with previous or next task)
   space: toggle completion of selected task
   e: edit title
   i: insert new tasks
   delete/backspace: delete task");
        DisplayBar();
    }

    private string GetTitle() {
        Console.WriteLine("Please enter task title (or [enter] for none): ");
        return Console.ReadLine()!;
    }

    public void ProcessUserInput() {
        if (_insertMode) {
            string taskTitle = GetTitle();
            if (taskTitle == null || taskTitle.Length == 0) {
                _insertMode = false;
            } else {
                _tasks.Insert(taskTitle);
            }
        } else {
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    _quit = true;
                    break;
                case ConsoleKey.UpArrow:
                    _tasks.SelectPrevious();
                    break;
                case ConsoleKey.DownArrow:
                    _tasks.SelectNext();
                    break;
                case ConsoleKey.LeftArrow:
                    _tasks.SwapWithPrevious();
                    break;
                case ConsoleKey.RightArrow:
                    _tasks.SwapWithNext();
                    break;
                case ConsoleKey.I:
                    _insertMode = true;
                    break;
                case ConsoleKey.E:
                    if(_tasks.CurrentTask != null ) _tasks.CurrentTask.Title = GetTitle();
                    break;
                case ConsoleKey.H:
                    _showHelp = !_showHelp;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    _tasks.CurrentTask?.ToggleStatus();
                    break;
                case ConsoleKey.Delete:
                case ConsoleKey.Backspace:
                    _tasks.DeleteSelected();
                    break;
                default:
                    break;
            }
        }
    }
}

class Program {
    static void Main() {
        new TodoListApp(new TodoList()).Run();
    }
}

enum CompletionStatus { Done, InProgess }