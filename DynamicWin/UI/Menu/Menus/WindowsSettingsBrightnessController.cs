using System.Management;

namespace DynamicWin.UI.Menu.Menus;

internal static class WindowsSettingsBrightnessController
{
    private static bool notSupported;

    public static int Get()
    {
        if (notSupported)
            return 100;

        try
        {
            using var myClass = new ManagementClass("WmiMonitorBrightness")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = myClass.GetInstances();
            foreach (ManagementObject instance in instances.Cast<ManagementObject>())
            {
                return (byte)instance.GetPropertyValue("CurrentBrightness");
            }
            return 0;
        }
        catch
        {
            notSupported = true;
            return 100;
        }
    }

    public static void Set(int brightness)
    {
        try
        {
            using var myClass = new ManagementClass("WmiMonitorBrightnessMethods")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = myClass.GetInstances();
            var args = new object[] { 1, brightness };
            foreach (ManagementObject instance in instances.Cast<ManagementObject>())
            {
                instance.InvokeMethod("WmiSetBrightness", args);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}