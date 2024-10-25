using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace RunCommandOnSave
{
    internal class Events : IVsRunningDocTableEvents3
    {
        private readonly DTE _dte;
        private readonly RunningDocumentTable _runningDocumentTable;
        private OutputWindowPane _pane;
        private SettingsCache _settingsCache;

        public Events(DTE dte, RunningDocumentTable runningDocumentTable)
        {
            _runningDocumentTable = runningDocumentTable;
            _dte = dte;
            _settingsCache = new SettingsCache();
        }

        public int OnBeforeSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Process(CookieToDoc(docCookie, _dte.Documents.Cast<Document>()), SaveEventType.PreSave);
            return VSConstants.S_OK;

        }

        public int OnAfterSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Process(CookieToDoc(docCookie, _dte.Documents.Cast<Document>()), SaveEventType.PostSave);
            return VSConstants.S_OK;
        }

        // Required "un-overridden implementations

        public int OnAfterFirstDocumentLock(uint DocCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) { return VSConstants.S_OK; }
        public int OnBeforeLastDocumentUnlock(uint DocCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) { return VSConstants.S_OK; }
        public int OnAfterAttributeChange(uint DocCookie, uint grfAttribs) { return VSConstants.S_OK; }
        public int OnBeforeDocumentWindowShow(uint DocCookie, int fFirstShow, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
        public int OnAfterDocumentWindowHide(uint DocCookie, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
        int IVsRunningDocTableEvents3.OnAfterAttributeChangeEx(uint DocCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) { return VSConstants.S_OK; }
        int IVsRunningDocTableEvents2.OnAfterAttributeChangeEx(uint DocCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) { return VSConstants.S_OK; }

        /*
            =======
            Private
            =======
        */

        private void Process(Document documentToFormat, SaveEventType eventType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (documentToFormat != null)
            {
                // locate the settings for this document

                var fileSettings = _settingsCache.GetSettingsForDocument(documentToFormat);

                // Check if we can proceed

                if (!fileSettings.ShouldProceed(eventType, documentToFormat.FullName))
                {
                    return;
                }

                // Grab the corresponding commands

                var ext = Path.GetExtension(documentToFormat.FullName).Substring(1);
                string[] commands = null;
                string err = "";

                if (fileSettings.EventsConfig.ContainsKey(eventType))
                {
                    if (!fileSettings.EventsConfig[eventType].ContainsKey(ext))
                    {
                        ext = "*";
                    }

                    if (fileSettings.EventsConfig[eventType].ContainsKey(ext))
                    {
                        commands = fileSettings.EventsConfig[eventType][ext].Commands;
                    }
                }

                var numErrors = 0;
                if (commands != null && _dte.ActiveWindow.Kind == "Document")
                {
                    var activeDocument = _dte.ActiveDocument;
                    documentToFormat.Activate();
                    foreach (string cmd in commands)
                    {
                        try
                        {
                            _dte.ExecuteCommand(cmd, string.Empty);
                        }
                        catch (Exception e)
                        {
                            numErrors += 1;
                            err = e.Message;
                        }
                    }
                    activeDocument.Activate();
                }

                if (fileSettings.Debug)
                {
                    if (commands == null || commands.Length == 0 || numErrors > 0)
                    {
                        Log(String.Format("{0}: RunCommandOnSave/{1} was NOT processed ({2})\n", documentToFormat.FullName, eventType.ToString(), err));
                    }
                    else
                    {
                        Log(String.Format("{0}: RunCommandOnSave/{1} WAS processed\n", documentToFormat.FullName, eventType.ToString()));
                    }
                }
            }
        }

        // debug logging

        private void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = _dte as DTE2;
            var paneName = "Run Command On Save - " + VersionInfo.Version;
            if (dte2 != null)
            {
                var panes = dte2.ToolWindows.OutputWindow.OutputWindowPanes;
                try
                {
                    _pane = panes.Item(paneName);
                }
                catch (ArgumentException)
                {
                }

                if (_pane == null)
                {
                    _pane = panes.Add(paneName);
                }

                if (_pane != null)
                {
                    _pane.OutputString(message);
                }
            }
        }

        // doc cookie to document conversion

        private Document CookieToDoc(uint docCookie, IEnumerable<Document> documents)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var Doc in documents)
            {
                if (Doc.FullName == _runningDocumentTable.GetDocumentInfo(docCookie).Moniker)
                {
                    return Doc;
                }
            }
            return null;
        }

    }
}
