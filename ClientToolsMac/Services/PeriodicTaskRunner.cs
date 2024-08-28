using RTProClientToolsMac.Controllers;

namespace ClientToolsMac.Services
{
    public class PeriodicTaskRunner
    {
        private static readonly object _lock = new();
        private LinkedList<DeleteFileTask> _deleteFileTasks;
        private Timer _timer;
        private Configurations _configurations;
        private PrintFileResolver _printFileResolver;
        private HashSet<string> _failedDeletes = [];

        private bool TimerIsSet => _timer is not null;
        record DeleteFileTask(DateTime WhenUtc, string filePath);

        public PeriodicTaskRunner(
            Configurations configurations,
            PrintFileResolver printFileResolver
            )
        {
            _configurations = configurations;
            _printFileResolver = printFileResolver;
        }

        public async Task LoadTempFileSchedule()
        {
            await Task.Run(() =>
            {
                var directories = Directory.GetFiles(_configurations.TempDirectory, "*", SearchOption.AllDirectories)
                .Select(file =>
                {
                    var creationTime = File.GetCreationTimeUtc(file);
                    return new DeleteFileTask(creationTime.AddMinutes(1), file);
                })
                .OrderBy(a => a.WhenUtc);
                _deleteFileTasks = new LinkedList<DeleteFileTask>(directories);
                Start();
            });
            _printFileResolver.FileCreated += PrintFileResolver_FileCreated;
        }

        private void Start()
        {
            Monitor.Enter(_lock);

            if (TimerIsSet)
            {
                Monitor.Exit(_lock);
                return;
            }

            if (_deleteFileTasks.First is not null)
            {
                var dueTime = _deleteFileTasks.First.Value.WhenUtc - DateTime.UtcNow;

                if (dueTime < TimeSpan.Zero)
                {
                    dueTime = TimeSpan.Zero;
                }

                _timer = new Timer(DeleteFile, _deleteFileTasks.First.Value.filePath, dueTime, Timeout.InfiniteTimeSpan);
                _deleteFileTasks.RemoveFirst();
            }
            Monitor.Exit(_lock);
        }

        private void DeleteFile(object? state)
        {
            Monitor.Enter(_lock);
            DeleteFile(state as string);
            foreach (var filePath in _failedDeletes.ToList())
            {
                _failedDeletes.Remove(filePath);
                DeleteFile(filePath);
            }
            _timer = null;
            Start();
            Monitor.Exit(_lock);
        }

        private void DeleteFile(string? filePath)
        {
            if (filePath is null)
            {
                return;
            }
            var directory = Path.GetDirectoryName(filePath);
            if (!directory.Equals(_configurations.TempDirectory))
            {
                if (Directory.GetFiles(directory).Length == 1)
                {
                    try
                    {
                        if (Directory.Exists(directory))
                        {
                            Directory.Delete(directory, true);
                        }
                    }
                    catch
                    {
                        _failedDeletes.Add(filePath);
                    }
                }
            }
            else
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch
                {
                    _failedDeletes.Add(filePath);
                }
            }
        }

        private void PrintFileResolver_FileCreated(string filePath)
        {
            _deleteFileTasks.AddLast(new DeleteFileTask(DateTime.UtcNow.AddMinutes(1), filePath));
            Start();
        }
    }
}
