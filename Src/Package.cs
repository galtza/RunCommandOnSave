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
    [Guid("035efc93-c6ac-4591-a200-b58286bde51f")]
    public sealed class Package : ToolkitPackage
    {
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var runningDocumentTable = new RunningDocumentTable(this);
            runningDocumentTable.Advise(new Events(await this.GetServiceAsync(typeof(DTE)) as DTE, runningDocumentTable));
            await this.RegisterCommandsAsync();
        }
    }
}
