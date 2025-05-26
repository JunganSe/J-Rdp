using WinApp.Tray;

namespace WinAppTests.Tray;

[TestClass]
public class TrayWorkerTests
{
    private TrayWorker _worker;

    [TestInitialize]
    public void Setup()
    {
        _worker = new TrayWorker();
    }
}
