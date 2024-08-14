using DynamicWin.Resources;
using DynamicWin.UI.Menu;
using DynamicWin.UI.Menu.Menus;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using ThumbnailGenerator;

namespace DynamicWin.UI.Widgets.Big;

internal class ShortcutButton : DWButton
{
    public ShortcutSave savedShortcut;
    private readonly string saveId;

    private readonly DWText shortcutTitle;
    private readonly DWImage shortcutIcon;

    public ShortcutButton(UIObject? parent, string shortcutSaveId, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size - 12.5f, null, alignment)
    {
        roundRadius = 15;
        hoverScaleMulti = Vec2.one * 1;
        clickScaleMulti = Vec2.one * 1;
        scaleSecondOrder.SetValues(4.5f, 1, 0.1f);

        if (SaveManager.Contains(shortcutSaveId))
        {
            savedShortcut = (ShortcutSave)SaveManager.Get(shortcutSaveId);
        }
        else
        {
            savedShortcut = new ShortcutSave();
        }

        shortcutTitle = new DWText(this, " ", new Vec2(15f, 0), UIAlignment.MiddleLeft)
        {
            TextSize = 9.5f
        };
        shortcutTitle.Anchor.X = 0f;
        shortcutTitle.Font = Res.InterBold;
        shortcutTitle.Color = Theme.TextSecond;
        shortcutTitle.SilentSetActive(false);
        AddLocalObject(shortcutTitle);

        shortcutIcon = new DWImage(this, Res.FileIcon, new Vec2(0, 0), new Vec2(20, 20), UIAlignment.MiddleLeft)
        {
            allowIconThemeColor = false
        };
        shortcutIcon.SilentSetActive(false);
        AddLocalObject(shortcutIcon);

        clickCallback = RunShortcut;

        saveId = shortcutSaveId;
        LoadShortcut();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        shortcutTitle.SetActive(!string.IsNullOrEmpty(savedShortcut.path));
        shortcutIcon.SetActive(!string.IsNullOrEmpty(savedShortcut.path));
    }

    public override void Draw(SKCanvas canvas)
    {
        var rect = GetRect();
        //rect.Deflate(7.5f, 5);

        var paint = GetPaint();

        if (string.IsNullOrEmpty(savedShortcut.path))
        {
            float[] intervals = [5, 5];
            paint.PathEffect = SKPathEffect.CreateDash(intervals, 0f);

            paint.IsStroke = true;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeJoin = SKStrokeJoin.Round;
            paint.StrokeWidth = 2f;

            canvas.DrawRoundRect(rect, paint);
        }
        else
        {
            canvas.DrawRoundRect(rect, paint);

            paint.Color = Theme.Primary.Override(a: 0.35f).Value();
            paint.IsStroke = true;
            paint.StrokeWidth = 2f;

            canvas.DrawRoundRect(rect, paint);
        }
    }

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new ContextMenu();

        if (string.IsNullOrEmpty(savedShortcut.path))
        {
            var config = new MenuItem() { Header = "Configure Shortcut" };
            config.Click += (s, e) => ConfigureShortcut();
            ctx.Items.Add(config);
        }
        else
        {
            var run = new MenuItem() { Header = "Run Shortcut" };
            run.Click += (s, e) => RunShortcut();
            ctx.Items.Add(run);

            var remove = new MenuItem() { Header = "Remove Shortcut" };
            remove.Click += (s, e) => RemoveShortcut();
            ctx.Items.Add(remove);
        }

        return ctx;
    }

    internal void SetShortcut(ShortcutSave save)
    {
        if (!string.IsNullOrEmpty(save.path))
        {
            savedShortcut = save;

            SaveManager.Add("shortcuts." + saveId, JsonConvert.SerializeObject(save));
            SaveManager.SaveAll();

            UpdateDisplay();
        }
    }

    private Bitmap? thumbnail;

    private void UpdateDisplay()
    {
        shortcutTitle.SilentSetText(DWText.Truncate(string.IsNullOrEmpty(savedShortcut.name) ? " " : savedShortcut.name, 9));

        Task.Run(() =>
        {
            try
            {
                const int THUMB_SIZE = 256;
                thumbnail = WindowsThumbnailProvider.GetThumbnail(
                   savedShortcut.path, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Debug.WriteLine("Could not load icon.");

                new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(1500);
                    }
                    catch
                    {
                        return;
                    }

                    UpdateDisplay();
                }).Start();
            }
            catch
            {
                return;
            }
            finally
            {
                SKBitmap? bMap = null;
                if (thumbnail != null) bMap = thumbnail.ToSKBitmap();
                else bMap = Res.FileIcon;

                shortcutIcon.Image = bMap;

                thumbnail?.Dispose();
            }

            SetActive(true);
        });
    }

    private void LoadShortcut()
    {
        if (!SaveManager.Contains("shortcuts." + saveId)) return;
        var shortcut = JsonConvert.DeserializeObject<ShortcutSave>((string)SaveManager.Get("shortcuts." + saveId));

        if (!string.IsNullOrEmpty(shortcut.path))
        {
            savedShortcut = shortcut;
            UpdateDisplay();
        }
    }

    private void RunShortcut()
    {
        if (string.IsNullOrEmpty(savedShortcut.path))
        {
            ConfigureShortcut();
            return;
        }

        OpenWithDefaultProgram(savedShortcut.path);
    }

    private void ConfigureShortcut() => MenuManager.OpenMenu(new ConfigureShortcutMenu(this));

    private void OpenWithDefaultProgram(string path)
    {
        if (!File.Exists(path))
        {
            RemoveShortcut();
            return;
        }

        using Process fileOpener = new();

        fileOpener.StartInfo.FileName = "explorer";
        fileOpener.StartInfo.Arguments = $"\"{path}\"";
        fileOpener.Start();
    }

    private void RemoveShortcut()
    {
        if (!SaveManager.Contains($"shortcuts.{saveId}")) return;
        SaveManager.Remove($"shortcuts.{saveId}");
        SaveManager.SaveAll();

        savedShortcut = new ShortcutSave();
    }

    public struct ShortcutSave
    {
        public string path; // The path to the app / url to open
        public string name; // The displayName of the shortcut
    }
}