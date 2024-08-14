using DynamicWin.Utils;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ThumbnailGenerator;

namespace DynamicWin.UI.UIElements.Custom;

internal class TrayFile : UIObject
{
    private Bitmap? thumbnail;

    public string FileName { get; }

    private readonly DWImage fileIconImage;
    private readonly DWText fileTitle;

    public bool IsSelected { get; private set; }

    public static TrayFile? LastSelected;

    private readonly Tray tray;

    private float cycle;
    private readonly float speed = 5f;
    private bool wasSelected;

    public TrayFile(UIObject? parent, string file, Vec2 position, Tray tray, UIAlignment alignment = UIAlignment.TopCenter) :
        base(parent, position, new Vec2(60, 75), alignment)
    {
        FileName = file;
        SilentSetActive(false);

        this.tray = tray;

        Color = Theme.Primary.Override(a: 0.45f);
        roundRadius = 7.5f;

        fileTitle = new DWText(this, DWText.Truncate(Path.GetFileNameWithoutExtension(file), 8) + Path.GetExtension(file), new Vec2(0, -10), UIAlignment.BottomCenter)
        {
            Color = Theme.TextSecond,
            TextSize = 11
        };

        AddLocalObject(fileTitle);

        var modifyDate = File.GetLastWriteTimeUtc(file);
        var modifyString = modifyDate.ToString("yy/MM/dd HH:mm");

        var fileSize = Mathf.GetFileSizeString(file);

        AddLocalObject(new DWText(this, modifyString, new Vec2(0, 7.5f), UIAlignment.BottomCenter)
        {
            Color = Theme.TextThird,
            TextSize = 10f
        });

        AddLocalObject(new DWText(this, fileSize, new Vec2(0, 17.5f), UIAlignment.BottomCenter)
        {
            Color = Theme.TextThird,
            TextSize = 10f
        });

        fileIconImage = new DWImage(this, Resources.Res.FileIcon, new Vec2(0, 30), new Vec2(50, 50), UIAlignment.TopCenter)
        {
            allowIconThemeColor = false,
            roundRadius = 5f,
            maskOwnRect = true
        };

        AddLocalObject(fileIconImage);

        RefreshIcon();
    }

    public void RefreshIcon()
    {
        Task.Run(() =>
        {
            try
            {
                const int THUMB_SIZE = 256;
                thumbnail = WindowsThumbnailProvider.GetThumbnail(
                   FileName, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                System.Diagnostics.Debug.WriteLine("Could not load icon.");

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

                    RefreshIcon();
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
                else bMap = Resources.Res.FileIcon;

                fileIconImage.Image = bMap;

                thumbnail?.Dispose();
            }

            SetActive(true);
        });
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //Size.X = fileTitle.TextBounds.X;

        cycle += deltaTime * speed;
        Color = Theme.Primary.Override(a: Mathf.Remap((float)Math.Sin(cycle), -1, 1, 0.35f, 0.45f));
    }

    public override void OnMouseDown()
    {
        if (!IsSelected) IsSelected = true;
        else return;

        wasSelected = false;

        if (IsSelected)
        {
            if (KeyHandler.keyDown.Contains(Keys.LShiftKey) || KeyHandler.keyDown.Contains(Keys.RShiftKey))
            {
                int indexLast = 0;
                if (LastSelected != null)
                    indexLast = tray.fileObjects.IndexOf(LastSelected);

                LastSelected = this;
                var newIndex = tray.fileObjects.IndexOf(this);

                if (newIndex == indexLast) return;

                bool upwards = indexLast < newIndex;

                int start = upwards ? indexLast : newIndex;
                int end = upwards ? newIndex : indexLast;

                for (int i = start; i < end; i++)
                {
                    tray.fileObjects[i].IsSelected = true;
                }
            }
            else
            {
                LastSelected = this;
            }
        }
    }

    public override void OnMouseUp()
    {
        if (IsSelected && wasSelected)
        {
            IsSelected = false;
        }
        else
        {
            wasSelected = true;
        }
    }

    public override void OnGlobalMouseUp()
    {
        if (!IsHovering && !(KeyHandler.keyDown.Contains(Keys.LControlKey) || KeyHandler.keyDown.Contains(Keys.RControlKey)
                || KeyHandler.keyDown.Contains(Keys.LShiftKey) || KeyHandler.keyDown.Contains(Keys.RShiftKey)))
        {
            IsSelected = false;
        }
    }

    public override void Draw(SKCanvas canvas)
    {
        var paint = GetPaint();

        if (IsSelected)
        {
            SKRect textR = SKRect.Create(fileTitle.Position.X, fileTitle.Position.Y + 1.5f,
                fileTitle.Size.X, fileTitle.Size.Y);
            textR.Inflate(-2.5f, 0);
            SKRoundRect roundTextRect = new(textR, 5f);

            SKRect thumbnailR = SKRect.Create(fileIconImage.Position.X, fileIconImage.Position.Y,
                fileIconImage.Size.X, fileIconImage.Size.Y + 5);
            SKRoundRect roundThumbnailRect = new(thumbnailR, 2.5f);
            roundThumbnailRect.Inflate(5f, 5f);

            SKPath path = new();
            path.AddRoundRect(roundTextRect);
            path.AddRoundRect(roundThumbnailRect);

            canvas.DrawPath(path, paint);
        }
    }
}