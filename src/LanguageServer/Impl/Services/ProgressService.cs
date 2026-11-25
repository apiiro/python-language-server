// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Threading.Tasks;
using Microsoft.Python.Core;
using Microsoft.Python.Core.Services;
using StreamJsonRpc;

namespace Microsoft.Python.LanguageServer.Services {
    public sealed class ProgressService : IProgressService {
        private readonly IClientApplication _clientApp;
        public ProgressService(IClientApplication clientApp) {
            _clientApp = clientApp;
        }

        public IProgress BeginProgress() => new Progress(_clientApp);

        private class Progress : IProgress {
            private readonly IClientApplication _clientApp;
            public Progress(IClientApplication clientApp) {
                _clientApp = clientApp;
                NotifyUnlessConnectionClosedAsync("python/beginProgress").DoNotWait();
            }
            public Task Report(string message) => NotifyUnlessConnectionClosedAsync("python/reportProgress", message);
            public void Dispose() => NotifyUnlessConnectionClosedAsync("python/endProgress").DoNotWait();

            // Analysis can still be running when the server exits, so progress may outlive the client connection.
            private async Task NotifyUnlessConnectionClosedAsync(string targetName, params object[] arguments) {
                try {
                    await _clientApp.NotifyAsync(targetName, arguments);
                } catch (ObjectDisposedException) {
                } catch (ConnectionLostException) {
                }
            }
        }
    }
}
