using System.Runtime.InteropServices;
using System.Text;

namespace DynamicWin.Utils;

public partial class PowerStatusChecker
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_POWER_STATUS
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public uint BatteryLifeTime;
        public uint BatteryFullLifeTime;

        public readonly string GetACLineStatusString => ACLineStatus switch
        {
            0 => "Offline",
            1 => "Online",
            _ => "Unknown",
        };

        public readonly string GetBatteryFlagString => BatteryFlag switch
        {
            1 => "High, more than 66 percent",
            2 => "Low, less than 33 percent",
            4 => "Critical, less than five percent",
            8 => "Charging",
            128 => "No system battery",
            _ => "Unknown",
        };

        public readonly string GetBatteryLifePercent => BatteryLifePercent == 255 ? "Unknown" : BatteryLifePercent + "%";

        public readonly string GetBatteryLifeTime => BatteryLifeTime == uint.MaxValue ? "Unknown" : BatteryLifeTime + " seconds";

        public readonly string GetBatteryFullLifeTime => BatteryFullLifeTime == uint.MaxValue ? "Unknown" : BatteryFullLifeTime + " seconds";

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"ACLineStatus: {GetACLineStatusString}");
            sb.AppendLine($"Battery Flag: {GetBatteryFlagString}");
            sb.AppendLine($"Battery Life: {GetBatteryLifePercent}");
            sb.AppendLine($"Battery Left: {GetBatteryLifeTime}");
            sb.AppendLine($"Battery Full: {GetBatteryFullLifeTime}");
            return sb.ToString();
        }
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

    public static SYSTEM_POWER_STATUS GetPowerStatus()
        => GetSystemPowerStatus(out SYSTEM_POWER_STATUS status) ? status : throw new Exception("Unable to get power status.");
}