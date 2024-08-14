using LibreHardwareMonitor.Hardware;

namespace DynamicWin.Utils;

internal static class HardwareMonitor
{
    private static System.Timers.Timer timer;

    public static string usageString = " ";

    public static void Initialize()
    {
        computer = new Computer()
        {
            IsMemoryEnabled = true,
            IsCpuEnabled = true // Enable CPU monitoring
        };
        computer.Open();

        timer = new System.Timers.Timer
        {
            Interval = 1000
        };
        timer.Elapsed += Timer_Elapsed;

        timer.Start();
    }

    private static Computer? computer;
    private static float lastCpu;
    private static string lastRam = string.Empty;

    private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        foreach (var hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Cpu)
            {
                if (hardware == null) continue;
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                    {
                        lastCpu = Mathf.LimitDecimalPoints((float)sensor.Value.GetValueOrDefault(), 1);
                    }
                }
            }

            if (hardware.HardwareType == HardwareType.Memory)
            {
                if (hardware == null) continue;
                hardware.Update();

                float memUsed = 0;
                float memFree = 0;

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.Name == "Memory Used")
                    {
                        memUsed = Mathf.LimitDecimalPoints((float)sensor.Value.GetValueOrDefault(), 1);
                    }
                    else if (sensor.Name == "Memory Available")
                    {
                        memFree = Mathf.LimitDecimalPoints((float)sensor.Value.GetValueOrDefault(), 1);
                    }
                    lastRam = memUsed + "GB / " + Mathf.LimitDecimalPoints(memFree + memUsed, 0) + "GB";
                }
            }
        }

        usageString = $"CPU: {lastCpu}%    RAM: {lastRam}";

        computer.Close();
    }

    public static void Stop() => computer?.Close();
}