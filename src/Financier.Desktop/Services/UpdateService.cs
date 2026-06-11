using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Onova;
using Onova.Exceptions;
using Onova.Services;

namespace Financier.Desktop.Services
{
    public class UpdateService() : IDisposable
    {
        private readonly IUpdateManager? _updateManager = new UpdateManager(
                    new GithubPackageResolver(
                        "vov4uk",
                        "Financier.Desktop",
                        $"Financier.Desktop.{RuntimeInformation.RuntimeIdentifier}.zip"
                    ),
                    new ZipPackageExtractor()
                );

        private Version? _updateVersion;
        private bool _isUpdatePrepared;
        private bool _isUpdaterLaunched;

        public async Task<Version?> CheckForUpdatesAsync()
        {
            if (_updateManager is null)
                return null;

            var check = await _updateManager.CheckForUpdatesAsync();
            return check.CanUpdate ? check.LastVersion : null;
        }

        public async Task PrepareUpdateAsync(Version version)
        {
            if (_updateManager is null)
                return;

            try
            {
                await _updateManager.PrepareUpdateAsync(_updateVersion = version);
                _isUpdatePrepared = true;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void FinalizeUpdate(bool needRestart)
        {
            if (_updateManager is null)
                return;

            if (_updateVersion is null || !_isUpdatePrepared || _isUpdaterLaunched)
                return;

            try
            {
                _updateManager.LaunchUpdater(_updateVersion, needRestart);
                _isUpdaterLaunched = true;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void Dispose() => _updateManager?.Dispose();
    }
}
