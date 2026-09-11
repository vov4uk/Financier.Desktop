using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Onova;
using Onova.Exceptions;
using Onova.Services;

namespace Financier.Desktop.Services
{
    public sealed class UpdateService() : IDisposable
    {
#nullable enable
        private readonly IUpdateManager? _updateManager = new UpdateManager(
                    new GithubPackageResolver(
                        "vov4uk",
                        "Financier.Desktop",
                        $"Financier.Desktop.{RuntimeInformation.RuntimeIdentifier}.zip"
                    ),
                    new ZipPackageExtractor()
                );

        private bool _isUpdatePrepared;
        private bool _isUpdaterLaunched;
        private Version? _updateVersion;
        public async Task<Version?> CheckForUpdatesAsync()
        {
            if (_updateManager is null)
                return null;

            var check = await _updateManager.CheckForUpdatesAsync();
            return check.CanUpdate ? check.LastVersion : null;
        }
#nullable disable

        public void Dispose()
        {
            _updateManager?.Dispose();
            GC.SuppressFinalize(this);
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

        public async Task PrepareUpdateAsync(Version version)
        {
            if (_updateManager is null)
                return;

            try
            {
                _updateVersion = version;
                await _updateManager.PrepareUpdateAsync(_updateVersion);
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
    }
}
