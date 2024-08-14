using System.Runtime.InteropServices;

namespace DynamicWin.Utils;

public partial class MediaController
{
    [LibraryImport("user32.dll")]
    private static partial void keyboard_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;

    public static void PlayPause() => keyboard_event(VK_MEDIA_PLAY_PAUSE, 0, 0, 0);

    public static void Next() => keyboard_event(VK_MEDIA_NEXT_TRACK, 0, 0, 0);

    public static void Previous() => keyboard_event(VK_MEDIA_PREV_TRACK, 0, 0, 0);
}