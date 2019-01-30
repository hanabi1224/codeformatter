// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.DotNet.CodeFormatting
{
    public sealed class ResponseFileWorkspace : Workspace
    {
        private static Encoding s_utf8WithoutBom = new UTF8Encoding(false);

        private ResponseFileWorkspace()
            : base(DesktopMefHostServices.DefaultServices, "Custom")
        {
        }

        public static ResponseFileWorkspace Create()
        {
            return new ResponseFileWorkspace();
        }

        public Project OpenCommandLineProject(string responseFile, string language)
        {
            // This line deserves better error handling, but the tools current model is just throwing exception for most errors.
            // Issue: #90
            string rspContents = File.ReadAllText(responseFile);

            var projectInfo = CommandLineProject.CreateProjectInfo(
                projectName: Path.GetFileNameWithoutExtension(responseFile),
                language: language,
                commandLine: rspContents,
                baseDirectory: Path.GetDirectoryName(Path.GetFullPath(responseFile)),
                workspace: this);

            this.OnProjectAdded(projectInfo);

            return this.CurrentSolution.GetProject(projectInfo.Id);
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return feature == ApplyChangesKind.ChangeDocument;
        }

        protected override void ApplyDocumentTextChanged(DocumentId documentId, SourceText text)
        {
            var document = this.CurrentSolution.GetDocument(documentId);
            if (document != null)
            {
                try
                {
                    using (var memoryFile = new MemoryStream())
                    {
                        var encoding = text.Encoding ?? s_utf8WithoutBom;
                        using (var writer = new StreamWriter(memoryFile, encoding: encoding))
                        {
                            text.Write(writer);
                        }

                        var bytes = memoryFile.ToArray();
                        var contextTextNew = encoding.GetString(bytes);
                        var contentTextRaw = File.ReadAllText(document.FilePath, encoding);

                        if (!StringComparer.Ordinal.Equals(contextTextNew, contentTextRaw))
                        {
                            Console.WriteLine($"Updating {document.FilePath}");
                            File.WriteAllBytes(document.FilePath, bytes);
                        }
                    }
                }
                catch (IOException e)
                {
                    this.OnWorkspaceFailed(new DocumentDiagnostic(WorkspaceDiagnosticKind.Failure, e.Message, documentId));
                }
                catch (UnauthorizedAccessException e)
                {
                    this.OnWorkspaceFailed(new DocumentDiagnostic(WorkspaceDiagnosticKind.Failure, e.Message, documentId));
                }

                this.OnDocumentTextChanged(documentId, text, PreservationMode.PreserveValue);
            }
        }
    }
}
