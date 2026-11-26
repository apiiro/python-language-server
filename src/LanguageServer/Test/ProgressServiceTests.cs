using System;
using System.Threading.Tasks;
using Microsoft.Python.Core.Services;
using Microsoft.Python.LanguageServer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using StreamJsonRpc;

namespace Microsoft.Python.LanguageServer.Tests {
    [TestClass]
    public class ProgressServiceTests {
        [DataTestMethod, Priority(0)]
        [DataRow(typeof(ObjectDisposedException))]
        [DataRow(typeof(ConnectionLostException))]
        public async Task ProgressIgnoresClosedConnection(Type closedConnectionExceptionType) {
            var closedConnection = (Exception)Activator.CreateInstance(closedConnectionExceptionType, "connection");
            var clientApp = Substitute.For<IClientApplication>();
            clientApp.NotifyAsync(Arg.Any<string>(), Arg.Any<object[]>()).Returns(Task.FromException(closedConnection));

            var progress = new ProgressService(clientApp).BeginProgress();
            await progress.Report("message");
            progress.Dispose();
        }
    }
}
