using RTProClientToolsMac.Controllers;

namespace ClientToolsMac.Services
{
    public class PeriodicTaskRunner(
        Configurations configurations
        )
    {
        private static PeriodicTimer _periodicTimer;
        private static readonly object _lock = new();
        private static bool _started = false;
        private LinkedList<XXXX> ll;
        private Timer _timer;

        static PeriodicTaskRunner()
        {
            Start();
        }

        private void Start(Task task)
        {
            if (ll.First is not null)
            {
                _timer = new Timer(FFFF, ll.First.Value.filePath, ll.First.Value.WhenUtc - DateTime.UtcNow, Timeout.InfiniteTimeSpan);
            }
        }

        private void FFFF(object? state)
        {
            var filePath = (string)state;

        }

        private async Task LoadTempFileSchedule()
        {
            await Task.Run(() =>
            {
                var directories = Directory.GetFiles(configurations.TempDirectory, "*", SearchOption.AllDirectories)
                .Select(file =>
                {
                    var creationTime = File.GetCreationTimeUtc(file);
                    return new XXXX(creationTime.AddDays(1), file);
                })
                .OrderBy(a => a.WhenUtc);
                ll = new LinkedList<XXXX>(directories);
            });
        }

        public async Task StartFileDeleter()
        {
            Monitor.Enter(_lock);

            if (_started)
            {
                Monitor.Exit(_lock);
                return;
            }
            _started = true;
            Monitor.Exit(_lock);
#if DEBUG
            _periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
#else
            _periodicTimer = new PeriodicTimer(TimeSpan.FromHours(1));
#endif
            while (await _periodicTimer.WaitForNextTickAsync())
            {
                var nowUtc = DateTime.UtcNow;
                var directories = Directory.GetFiles(configurations.TempDirectory, "*", SearchOption.AllDirectories)
                    .GroupBy(Path.GetDirectoryName)
                    .Select(a => new XXX([.. a], a.Key));
                foreach (var directory in directories)
                {
                    foreach (var file in directory.FilePaths)
                    {
                        var creationTimeUtc = File.GetCreationTimeUtc(file);
#if DEBUG
                        if ((nowUtc - creationTimeUtc).TotalMinutes > 5)
#else
                        if ((nowUtc - creationTime) > configurations.KeepTempFilesFor)
#endif
                        {
                            try
                            {
                                if (directory.FilePaths.Length == 1 && !directory.Path.Equals(configurations.TempDirectory))
                                {
                                    Directory.Delete(directory.Path, true);
                                }
                                else
                                {
                                    File.Delete(file);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        record XXX(string[] FilePaths, string Path);
        record XXXX(DateTime WhenUtc, string filePath);
    }
}
