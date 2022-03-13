using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

namespace RunCommandOnSave
{
    [PackageRegistration(UseManagedResourcesOnly = false, AllowsBackgroundLoading = true, RegisterUsing = RegistrationMethod.Assembly)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid("be7475e2-c591-4c89-b43e-b6b5b770aa3c")]
    public sealed class Package : ToolkitPackage
    {
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var runningDocumentTable = new RunningDocumentTable(this);
            runningDocumentTable.Advise(new Events(await this.GetServiceAsync(typeof(DTE)) as DTE, runningDocumentTable));
            await this.RegisterCommandsAsync();
        }

    }
}