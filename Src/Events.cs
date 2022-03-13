using System;
using System.Linq;
using System.Threading.Tasks;
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

        public Events(DTE dte, RunningDocumentTable runningDocumentTable)
        {
            _runningDocumentTable = runningDocumentTable;
            _dte = dte;
        }

        public int OnBeforeSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Process(docCookie, "PreSave");
            return VSConstants.S_OK;

        }

        public int OnAfterSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Process(docCookie, "PostSave");
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

        private void Process(uint docCookie, string section)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var Documents = _dte.Documents.Cast<Document>();
                var DocumentToFormat = CookieToDoc(docCookie, Documents);
                if (DocumentToFormat != null)
                {

                    var FileSettings = new Settings(DocumentToFormat.FullName);
                    var Commands = FileSettings.GetCommand(DocumentToFormat.FullName, section);
                    var NumErrors = 0;
                    if (Commands != null && _dte.ActiveWindow.Kind == "Document")
                    {
                        var ActiveDocument = _dte.ActiveDocument;
                        DocumentToFormat.Activate();
                        foreach (string Cmd in Commands)
                        {
                            try
                            {
                                _dte.ExecuteCommand(Cmd, string.Empty);
                            }
                            catch (Exception)
                            {
                                NumErrors += 1;
                            }
                        }
                        ActiveDocument.Activate();
                    }

                    var Debug = FileSettings.ReadKey("Debug", "On");
                    if (Debug != null && Debug.Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Commands == null || Commands.Length == 0 || NumErrors > 0)
                        {
                            Log(String.Format("{0}: RunCommandOnSave/{1} was NOT processed\n", DocumentToFormat.FullName, section));
                        }
                        else
                        {
                            Log(String.Format("{0}: RunCommandOnSave/{1} WAS processed\n", DocumentToFormat.FullName, section));
                        }
                    }
                }
            }
            finally
            {

            }
        }

        private void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = _dte as DTE2;
            if (dte2 != null)
            {
                dte2.ToolWindows.OutputWindow.ActivePane.OutputString(message);
            }
        }

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