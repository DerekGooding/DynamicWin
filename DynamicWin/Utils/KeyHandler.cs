using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DynamicWin.Utils;

public partial class KeyHandler
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private static readonly LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    public static void Start() => _hookID = SetHook(_proc);

    public static void Stop() => UnhookWindowsHookEx(_hookID);

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
            GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

    public static List<Keys> keyDown = [];
    public static Action<Keys, KeyModifier> onKeyDown;

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        int vkCode = Marshal.ReadInt32(lParam);
        if (nCode >= 0 && wParam == WM_KEYDOWN && !keyDown.Contains((Keys)vkCode))
        {
            keyDown.Add((Keys)vkCode);

            var keyModi = new KeyModifier
            {
                isShiftDown = keyDown.Contains(Keys.LShiftKey) || keyDown.Contains(Keys.RShiftKey),
                isCtrlDown = keyDown.Contains(Keys.LControlKey) || keyDown.Contains(Keys.RControlKey)
            };

            onKeyDown?.Invoke((Keys)vkCode, keyModi);
        }
        else if (nCode >= 0 && wParam == WM_KEYUP && keyDown.Contains((Keys)vkCode))
        {
            keyDown.Remove((Keys)vkCode);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    private static partial IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnhookWindowsHookEx(IntPtr hhk);

    [LibraryImport("user32.dll", EntryPoint = "CallNextHookExW", SetLastError = true)]
    private static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandle(string lpModuleName);
}

public struct KeyModifier
{
    public bool isCtrlDown;
    public bool isShiftDown;
}