using DynamicWin.Main;
using DynamicWin.Resources;
using DynamicWin.Utils;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Windows;

namespace DynamicWin;

public partial class DynamicWinMain : Application
{
    public static MMDevice DefaultDevice;
    public static MMDevice DefaultMicrophone;

    [STAThread]
    public static void Main()
    {
        DynamicWinMain m = new();
        m.Run();
    }

    public static string Version => "1.0.2r";

    //private void AddToStartup()
    //{
    //    try
    //    {
    //        // Set the registry key
    //        const string appName = "DynamicWin";
    //        string appPath = Process.GetCurrentProcess().MainModule.FileName;

    //        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
    //        if (key.GetValue(appName) == null)
    //        {
    //            key.SetValue(appName, appPath);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions here
    //        MessageBox.Show($"Failed to add application to startup: {ex.Message}");
    //    }
    //}

    //private void SetHighPriority()
    //{
    //    try
    //    {
    //        Process currentProcess = Process.GetCurrentProcess();
    //        currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions here
    //        MessageBox.Show($"Failed to set process priority: {ex.Message}");
    //    }
    //}

    private Mutex mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Handle unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Dispatcher.UnhandledException += Dispatcher_UnhandledException;

        mutex = new Mutex(true, "FlorianButz.DynamicWin", out bool result);

        if (!result)
        {
            ErrorForm errorForm = new();
            errorForm.Show();
            return;
        }

        //AddToStartup();
        //SetHighPriority();

        var devEnum = new MMDeviceEnumerator();
        DefaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        DefaultMicrophone = devEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

        SaveManager.LoadData();
        Settings.InitializeSettings();

        Res.Load();
        KeyHandler.Start();
        Theme.UpdateTheme();

        HardwareMonitor.Initialize();

        new MainForm().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        SaveManager.SaveAll();
        HardwareMonitor.Stop();

        KeyHandler.Stop();
        GC.KeepAlive(mutex); // Important
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        => MessageBox.Show($"Unhandled exception: {e.ExceptionObject}");

    private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Unhandled exception: {e.Exception}");
        e.Handled = true; // Prevent the application from terminating
    }

    public static long CurrentTimeMillis() => (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

    public static long NanoTime() => (long)(Stopwatch.GetTimestamp() * 10000000.0 / Stopwatch.Frequency);

}