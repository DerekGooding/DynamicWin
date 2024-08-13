﻿using DynamicWin.Main;
using DynamicWin.Resources;
using DynamicWin.UI.UIElements;
using DynamicWin.UI.Widgets;
using DynamicWin.UI.Widgets.Small;
using DynamicWin.Utils;
using SkiaSharp;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicWin.UI.Menu.Menus;

public class SettingsMenu : BaseMenu
{
    public SettingsMenu()
    {
        MainForm.onScrollEvent += (MouseWheelEventArgs x) => yScrollOffset += x.Delta * 0.25f;
    }

    private bool changedTheme = false;

    private void SaveAndBack()
    {
        Settings.AllowBlur = allowBlur.IsChecked;
        Settings.AllowAnimation = allowAnimation.IsChecked;
        Settings.AntiAliasing = antiAliasing.IsChecked;

        if (changedTheme)
            Theme.Instance.UpdateTheme(true);
        else
        {
            Res.HomeMenu = new HomeMenu();
            MenuManager.OpenMenu(Res.HomeMenu);
        }

        foreach (var item in customOptions)
        {
            item.SaveSettings();
        }

        Settings.Save();
    }

    private Checkbox allowBlur;
    private Checkbox allowAnimation;
    private Checkbox antiAliasing;

    public override List<UIObject> InitializeMenu(IslandObject island)
    {
        var objects = base.InitializeMenu(island);
        LoadCustomOptions();

        foreach (var item in customOptions)
        {
            item.LoadSettings();
        }

        var generalTitle = new DWText(island, "General", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Font = Res.InterBold
        };
        generalTitle.Anchor.X = 0;
        objects.Add(generalTitle);

        {
            var islandModesTitle = new DWText(island, "Island Mode", new Vec2(25, 0), UIAlignment.TopLeft)
            {
                Font = Res.InterRegular,
                Color = Theme.TextSecond,
                TextSize = 15
            };
            islandModesTitle.Anchor.X = 0;
            objects.Add(islandModesTitle);

            var islandModes = new string[] { "Island", "Notch" };
            var islandMode = new MultiSelectionButton(island, islandModes, new Vec2(25, 0), new Vec2(IslandSize().X - 50, 25), UIAlignment.TopLeft)
            {
                SelectedIndex = (Settings.IslandMode == IslandObject.IslandMode.Island) ? 0 : 1
            };
            islandMode.Anchor.X = 0;
            islandMode.onClick += (index) => Settings.IslandMode = (index == 0) ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;
            objects.Add(islandMode);
        }

        allowBlur = new Checkbox(island, "Allow Blur", new Vec2(25, 0), new Vec2(25, 25), () => { }, UIAlignment.TopLeft)
        {
            IsChecked = Settings.AllowBlur
        };
        allowBlur.Anchor.X = 0;
        objects.Add(allowBlur);

        allowAnimation = new Checkbox(island, "Allow SO Animation", new Vec2(25, 0), new Vec2(25, 25), () => { }, UIAlignment.TopLeft)
        {
            IsChecked = Settings.AllowAnimation
        };
        allowAnimation.Anchor.X = 0;
        objects.Add(allowAnimation);

        antiAliasing = new Checkbox(island, "Anti Aliasing", new Vec2(25, 0), new Vec2(25, 25), () => { }, UIAlignment.TopLeft)
        {
            IsChecked = Settings.AntiAliasing
        };
        antiAliasing.Anchor.X = 0;
        objects.Add(antiAliasing);

        {
            var themeTitle = new DWText(island, "Themes", new Vec2(25, 0), UIAlignment.TopLeft)
            {
                Font = Res.InterRegular,
                TextSize = 15
            };
            themeTitle.Anchor.X = 0;
            objects.Add(themeTitle);

            var themeOptions = new string[] { "Custom", "Dark", "Light", "Candy", "Forest Dawn", "Sunset Glow" };
            var theme = new MultiSelectionButton(island, themeOptions, new Vec2(25, 0), new Vec2(IslandSize().X - 50, 25), UIAlignment.TopLeft)
            {
                SelectedIndex = Settings.Theme + 1
            };
            theme.Anchor.X = 0;
            theme.onClick += (index) =>
            {
                Settings.Theme = index - 1;
                changedTheme = true;
            };
            objects.Add(theme);
        }

        var widgetsTitle = new DWText(island, "Widgets", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Font = Res.InterBold,
            Color = Theme.TextSecond
        };
        widgetsTitle.Anchor.X = 0;
        objects.Add(widgetsTitle);

        {
            var wTitle = new DWText(island, "Small Widgets (Right click to add / edit)", new Vec2(25, 0), UIAlignment.TopLeft)
            {
                Font = Res.InterRegular,
                Color = Theme.TextSecond,
                TextSize = 15
            };
            wTitle.Anchor.X = 0;
            objects.Add(wTitle);

            smallWidgetAdder = new SmallWidgetAdder(island, Vec2.zero, new Vec2(IslandSize().X - 50, 35), UIAlignment.TopCenter);
            objects.Add(smallWidgetAdder);
        }

        objects.Add(new DWText(island, " ", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            Anchor = new Vec2(0, 0.5f),
            TextSize = 5
        });

        {
            var wTitle = new DWText(island, "Big Widgets (Right click to add / edit)", new Vec2(25, 15), UIAlignment.TopLeft)
            {
                Font = Res.InterRegular,
                Color = Theme.TextSecond,
                TextSize = 15
            };
            wTitle.Anchor.X = 0;
            objects.Add(wTitle);

            bigWidgetAdder = new BigWidgetAdder(island, Vec2.zero, new Vec2(IslandSize().X - 50, 35), UIAlignment.TopCenter);
            objects.Add(bigWidgetAdder);
        }

        objects.Add(new DWText(island, " ", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            Anchor = new Vec2(0, 0.5f),
            TextSize = 20
        });

        var widgetOptionsTitle = new DWText(island, "Widget Settings", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Font = Res.InterBold,
            Color = Theme.TextSecond
        };
        widgetOptionsTitle.Anchor.X = 0;
        objects.Add(widgetOptionsTitle);

        {
            foreach (var option in customOptions)
            {
                var wTitle = new DWText(island, option.SettingTitle, new Vec2(25, 0), UIAlignment.TopLeft)
                {
                    Font = Res.InterRegular,
                    Color = Theme.TextSecond,
                    TextSize = 15
                };
                wTitle.Anchor.X = 0;
                objects.Add(wTitle);

                foreach (var optionItem in option.SettingsObjects())
                {
                    optionItem.Parent = island;

                    if (optionItem.alignment == UIAlignment.TopLeft)
                    {
                        optionItem.Position = new Vec2(25, 0);
                        optionItem.Anchor.X = 0;
                    }

                    if (optionItem is DWText)
                    {
                        ((DWText)optionItem).Color = Theme.TextThird;
                        ((DWText)optionItem).Font = Res.InterRegular;
                        ((DWText)optionItem).TextSize = 13;
                    }
                    else if (optionItem is Checkbox)
                    {
                        optionItem.Size = new Vec2(25, 25);
                    }

                    objects.Add(optionItem);
                }
            }
        }

        objects.Add(new DWText(island, "Software Version: " + DynamicWinMain.Version, new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            Anchor = new Vec2(0, 0.5f),
            TextSize = 15
        });

        objects.Add(new DWText(island, "Made by Florian Butz with ♡", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            Anchor = new Vec2(0, 0.5f),
            TextSize = 15
        });

        objects.Add(new DWText(island, "Licensed under the MIT license.", new Vec2(25, 0), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            Anchor = new Vec2(0, 0.5f),
            TextSize = 15
        });

        var backBtn = new DWTextButton(island, "Apply and Back", new Vec2(0, -45), new Vec2(350, 40), SaveAndBack, UIAlignment.BottomCenter)
        {
            roundRadius = 25
        };
        backBtn.Text.Font = Resources.Res.InterBold;

        bottomMask = new UIObject(island, Vec2.zero, new Vec2(IslandSizeBig().X + 100, 200), UIAlignment.BottomCenter)
        {
            Color = Theme.IslandBackground
        };

        objects.Add(bottomMask);
        objects.Add(backBtn);

        return objects;
    }

    private UIObject bottomMask;
    private SmallWidgetAdder smallWidgetAdder;
    private BigWidgetAdder bigWidgetAdder;

    private float yScrollOffset = 0f;
    private float ySmoothScroll = 0f;

    public override void Update()
    {
        base.Update();

        ySmoothScroll = Mathf.Lerp(ySmoothScroll,
            yScrollOffset, 10f * RendererMain.Instance.DeltaTime);

        bottomMask.blurAmount = 15;

        var yScrollLim = 0f;
        var yPos = 35f;
        const float spacing = 15f;

        for (int i = 0; i < UiObjects.Count - 2; i++)
        {
            var uiObject = UiObjects[i];
            if (!uiObject.IsEnabled) continue;

            uiObject.LocalPosition.Y = yPos + ySmoothScroll;
            yPos += uiObject.Size.Y + spacing;

            if (yPos > IslandSize().Y - 45) yScrollLim += uiObject.Size.Y + spacing;
        }

        yScrollOffset = Mathf.Lerp(yScrollOffset,
            Mathf.Clamp(yScrollOffset, -yScrollLim, 0f), 15f * RendererMain.Instance.DeltaTime);
    }

    public override Vec2 IslandSize()
    {
        var vec = new Vec2(525, 475);

        if (smallWidgetAdder != null)
        {
            vec.X = Math.Max(vec.X, smallWidgetAdder.Size.X + 50);
        }

        return vec;
    }

    public override Vec2 IslandSizeBig()
    {
        return IslandSize() + 5;
    }

    private static List<IRegisterableSetting> customOptions;

    private void LoadCustomOptions()
    {
        customOptions = new List<IRegisterableSetting>();

        var registerableSettings = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(IRegisterableSetting).IsAssignableFrom(p) && p.IsClass);

        foreach (var option in registerableSettings)
        {
            var optionInstance = (IRegisterableSetting)Activator.CreateInstance(option);
            customOptions.Add(optionInstance);
        }

        // Loading in custom DLLs

        var dirPath = Path.Combine(SaveManager.SavePath, "Extensions");

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        else
        {
            foreach (var file in Directory.GetFiles(dirPath))
            {
                if (Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine(file);
                    var DLL = new Assembly[] { Assembly.LoadFile(Path.Combine(dirPath, file)) };

                    var dllRegisterableSettings = DLL
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(IRegisterableSetting).IsAssignableFrom(p) && p.IsClass);

                    foreach (var option in dllRegisterableSettings)
                    {
                        var optionInstance = (IRegisterableSetting)Activator.CreateInstance(option);
                        customOptions.Add(optionInstance);
                    }
                }
            }
        }
    }
}

internal class BigWidgetAdder : UIObject
{
    private readonly AddNew addNew;

    public BigWidgetAdder(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, alignment)
    {
        Color = Theme.WidgetBackground.Override(a: 0.1f);
        roundRadius = 20;

        Anchor.Y = 0;

        addNew = new AddNew(this, Vec2.zero, new Vec2(size.X, 45), UIAlignment.BottomLeft);
        addNew.Anchor.Y = 0;
        AddLocalObject(addNew);

        UpdateWidgetDisplay();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        int line = (int)(Math.Floor(displays.Count / maxE));

        addNew.LocalPosition.Y = Mathf.Lerp(addNew.LocalPosition.Y, -line * 45 - 45, 15f * deltaTime);
        addNew.LocalPosition.X = Mathf.Lerp(addNew.LocalPosition.X, isDisplayEven() ? Size.X / 2f : Size.X / 1.3333333f, 15f * deltaTime);
        addNew.Size.X = Mathf.Lerp(addNew.Size.X, isDisplayEven() ? Size.X : Size.X / 2, 15f * deltaTime);

        var lines2 = (int)Math.Max(1, displays.Count / maxE + 1);
        Size.Y = Mathf.Lerp(Size.Y, lines2 * 45, 15f * RendererMain.Instance.DeltaTime);
    }

    private bool isDisplayEven()
    {
        return displays.Count % 2 == 0;
    }

    private readonly List<BigWidgetAdderDisplay> displays = new List<BigWidgetAdderDisplay>();
    private readonly float maxE = 2;

    private void UpdateWidgetDisplay()
    {
        displays.ForEach(DestroyLocalObject);
        displays.Clear();

        Dictionary<string, IRegisterableWidget> bigWidgets = new Dictionary<string, IRegisterableWidget>();

        foreach (var widget in Res.availableBigWidgets)
        {
            if (bigWidgets.ContainsKey(widget.GetType().FullName)) continue;
            bigWidgets.Add(widget.GetType().FullName, widget);
            System.Diagnostics.Debug.WriteLine(widget.GetType().FullName);
        }

        int c = 0;
        foreach (var bigWidget in Settings.bigWidgets)
        {
            if (!bigWidgets.ContainsKey(bigWidget)) continue;

            var widget = bigWidgets[bigWidget.ToString()];

            var display = new BigWidgetAdderDisplay(this, widget.WidgetName, UIAlignment.BottomLeft);

            display.onEditRemoveWidget += () =>
            {
                Settings.bigWidgets.Remove(bigWidget);
                UpdateWidgetDisplay();
            };

            display.onEditMoveWidgetRight += () =>
            {
                int index = Math.Clamp(Settings.bigWidgets.IndexOf(bigWidget) + 1, 0, Settings.bigWidgets.Count - 1);
                Settings.bigWidgets.Remove(bigWidget);

                Settings.bigWidgets.Insert(index, bigWidget);
                UpdateWidgetDisplay();
            };

            display.onEditMoveWidgetLeft += () =>
            {
                int index = Math.Clamp(Settings.bigWidgets.IndexOf(bigWidget) - 1, 0, Settings.bigWidgets.Count - 1);
                Settings.bigWidgets.Remove(bigWidget);

                Settings.bigWidgets.Insert(index, bigWidget);
                UpdateWidgetDisplay();
            };

            int line = (int)(c / maxE);

            display.LocalPosition.X = (c % 2) * Size.X / 2;
            display.LocalPosition.Y -= 45 + line * 45;

            displays.Add(display);
            AddLocalObject(display);

            c++;
        }
    }

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new System.Windows.Controls.ContextMenu();
        bool anyWidgetsLeft = false;

        foreach (var availableWidget in Res.availableBigWidgets)
        {
            if (Settings.bigWidgets.Contains(availableWidget.GetType().FullName)) continue;

            anyWidgetsLeft = true;

            var item = new MenuItem() { Header = availableWidget.GetType().Namespace.Split('.')[0] + ": " + availableWidget.WidgetName };
            item.Click += (x, y) =>
            {
                Settings.bigWidgets.Add(availableWidget.GetType().FullName);
                UpdateWidgetDisplay();
            };

            ctx.Items.Add(item);
        }

        if (!anyWidgetsLeft)
        {
            var ctx2 = new ContextMenu();
            ctx2.Items.Add(new MenuItem()
            {
                Header = "No Widgets Available.",
                IsEnabled = false
            });
            return ctx2;
        }

        return ctx;
    }
}

internal class AddNew : UIObject
{
    public AddNew(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, alignment)
    {
        AddLocalObject(new DWImage(this, Res.Add, Vec2.zero, new Vec2(15, 15), UIAlignment.Center)
        {
            Color = Theme.IconColor
        });

        Color = Theme.IconColor.Override(a: 0.4f);
    }

    public override void Draw(SKCanvas canvas)
    {
        var paint = GetPaint();

        var placeRect = new SKRoundRect(SKRect.Create(Position.X, Position.Y, Size.X, Size.Y), 25);
        placeRect.Deflate(5, 5);

        float[] intervals = [10, 10];
        paint.PathEffect = SKPathEffect.CreateDash(intervals, 0f);

        paint.IsStroke = true;
        paint.StrokeCap = SKStrokeCap.Round;
        paint.StrokeJoin = SKStrokeJoin.Round;
        paint.StrokeWidth = 2f;

        canvas.DrawRoundRect(placeRect, paint);

        placeRect.Deflate(5f, 5f);
        paint.Color = Color.Override(a: 0.05f).Value();
        paint.IsStroke = false;

        canvas.DrawRoundRect(placeRect, paint);
    }
}

internal class BigWidgetAdderDisplay : UIObject
{
    public BigWidgetAdderDisplay(UIObject? parent, string widgetName, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, Vec2.zero, Vec2.zero, alignment)
    {
        Size.X = parent.Size.X / 2;
        Size.Y = 45;

        Anchor = Vec2.zero;

        AddLocalObject(new DWText(this, DWText.Truncate(widgetName, 25), Vec2.zero, UIAlignment.Center)
        {
            TextSize = 14
        });

        roundRadius = 45;

        color = Theme.WidgetBackground.Override(a: 0.15f);
    }

    public override void Draw(SKCanvas canvas)
    {
        int canvasRestore = canvas.Save();

        var p = Position + Size / 2;
        canvas.Scale(this.s, this.s, p.X, p.Y);

        var paint = GetPaint();
        var rect = GetRect();

        paint.Color = color.Value();

        rect.Deflate(5, 5);
        canvas.DrawRoundRect(rect, paint);

        canvas.RestoreToCount(canvasRestore);
    }

    private readonly Col color;
    private float s = 1;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        color.a = Mathf.Lerp(color.a, IsHovering ? 0.2f : 0.15f, 7.5f * deltaTime);
        s = Mathf.Lerp(s, IsHovering ? 1.025f : 1, 15f * deltaTime);
    }

    public Action onEditRemoveWidget;
    public Action onEditMoveWidgetLeft;
    public Action onEditMoveWidgetRight;

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new System.Windows.Controls.ContextMenu();

        MenuItem remove = new MenuItem() { Header = "Remove" };
        remove.Click += (x, y) => onEditRemoveWidget?.Invoke();

        MenuItem pL = new MenuItem() { Header = "<- Push Left" };
        pL.Click += (x, y) => onEditMoveWidgetLeft?.Invoke();

        MenuItem pR = new MenuItem() { Header = "Push Right ->" };
        pR.Click += (x, y) => onEditMoveWidgetRight?.Invoke();

        ctx.Items.Add(remove);
        ctx.Items.Add(pL);
        ctx.Items.Add(pR);

        return ctx;
    }
}

internal class SmallWidgetAdder : UIObject
{
    public SmallWidgetAdder(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, alignment)
    {
        Color = Theme.WidgetBackground.Override(a: 0.1f);
        roundRadius = 25;

        container = new UIObject(this, Vec2.zero, new Vec2(size.X - 100, size.Y), UIAlignment.Center)
        {
            Color = Col.Transparent
        };
        AddLocalObject(container);

        UpdateWidgetDisplay();
    }

    private readonly UIObject container;

    public List<SmallWidgetBase> smallLeftWidgets = new List<SmallWidgetBase>();
    public List<SmallWidgetBase> smallRightWidgets = new List<SmallWidgetBase>();
    public List<SmallWidgetBase> smallCenterWidgets = new List<SmallWidgetBase>();

    private void UpdateWidgetDisplay()
    {
        smallRightWidgets.ForEach(DestroyLocalObject);
        smallLeftWidgets.ForEach(DestroyLocalObject);
        smallCenterWidgets.ForEach(DestroyLocalObject);

        smallRightWidgets.Clear();
        smallLeftWidgets.Clear();
        smallCenterWidgets.Clear();

        Dictionary<string, IRegisterableWidget> smallWidgets = new Dictionary<string, IRegisterableWidget>();

        foreach (var widget in Res.availableSmallWidgets)
        {
            smallWidgets.Add(widget.GetType().FullName, widget);
            System.Diagnostics.Debug.WriteLine(widget.GetType().FullName);
        }

        foreach (var smallWidget in Settings.smallWidgetsMiddle)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;

            var widget = smallWidgets[smallWidget.ToString()];

            var instance = (SmallWidgetBase)widget.CreateWidgetInstance(container, Vec2.zero, UIAlignment.Center);
            instance.isEditMode = true;

            instance.onEditRemoveWidget += () =>
            {
                Settings.smallWidgetsMiddle.Remove(smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetLeft += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsMiddle.IndexOf(smallWidget) + 1, 0, Settings.smallWidgetsMiddle.Count - 1);
                Settings.smallWidgetsMiddle.Remove(smallWidget);

                Settings.smallWidgetsMiddle.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetRight += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsMiddle.IndexOf(smallWidget) - 1, 0, Settings.smallWidgetsMiddle.Count - 1);
                Settings.smallWidgetsMiddle.Remove(smallWidget);

                Settings.smallWidgetsMiddle.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            smallCenterWidgets.Add(instance);
        }

        foreach (var smallWidget in Settings.smallWidgetsLeft)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;

            var widget = smallWidgets[smallWidget.ToString()];

            var instance = (SmallWidgetBase)widget.CreateWidgetInstance(container, Vec2.zero, UIAlignment.MiddleLeft);
            instance.isEditMode = true;

            instance.onEditRemoveWidget += () =>
            {
                Settings.smallWidgetsLeft.Remove(smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetLeft += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsLeft.IndexOf(smallWidget) + 1, 0, Settings.smallWidgetsLeft.Count - 1);
                Settings.smallWidgetsLeft.Remove(smallWidget);

                Settings.smallWidgetsLeft.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetRight += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsLeft.IndexOf(smallWidget) - 1, 0, Settings.smallWidgetsLeft.Count - 1);
                Settings.smallWidgetsLeft.Remove(smallWidget);

                Settings.smallWidgetsLeft.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            smallLeftWidgets.Add(instance);
        }

        foreach (var smallWidget in Settings.smallWidgetsRight)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;

            var widget = smallWidgets[smallWidget.ToString()];

            var instance = (SmallWidgetBase)widget.CreateWidgetInstance(container, Vec2.zero, UIAlignment.MiddleRight);
            instance.isEditMode = true;

            instance.onEditRemoveWidget += () =>
            {
                Settings.smallWidgetsRight.Remove(smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetLeft += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsRight.IndexOf(smallWidget) + 1, 0, Settings.smallWidgetsRight.Count - 1);
                Settings.smallWidgetsRight.Remove(smallWidget);

                Settings.smallWidgetsRight.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            instance.onEditMoveWidgetRight += () =>
            {
                int index = Math.Clamp(Settings.smallWidgetsRight.IndexOf(smallWidget) - 1, 0, Settings.smallWidgetsRight.Count - 1);
                Settings.smallWidgetsRight.Remove(smallWidget);

                Settings.smallWidgetsRight.Insert(index, smallWidget);
                UpdateWidgetDisplay();
            };

            smallRightWidgets.Add(instance);
        }

        smallCenterWidgets.ForEach(AddLocalObject);
        smallLeftWidgets.ForEach(AddLocalObject);
        smallRightWidgets.ForEach(AddLocalObject);
    }

    public float smallWidgetsSpacing = 30;
    public float middleWidgetsSpacing = 35;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        { // Left Small Widgets
            float leftStackedPos = 15f;
            foreach (var smallLeft in smallLeftWidgets)
            {
                smallLeft.Anchor.X = 0;
                smallLeft.LocalPosition.X = leftStackedPos;

                leftStackedPos += smallWidgetsSpacing + smallLeft.GetWidgetSize().X;
            }
        }

        { // Right Small Widgets
            float rightStackedPos = -15f;
            foreach (var smallRight in smallRightWidgets)
            {
                smallRight.Anchor.X = 1;
                smallRight.LocalPosition.X = rightStackedPos;

                rightStackedPos -= smallWidgetsSpacing + smallRight.GetWidgetSize().X;
            }
        }

        { // Center Small Widgets
            float centerStackPos = 0f;
            foreach (var smallCenter in smallCenterWidgets)
            {
                smallCenter.Anchor.X = 1;
                smallCenter.LocalPosition.X = centerStackPos;

                centerStackPos -= smallWidgetsSpacing + smallCenter.GetWidgetSize().X;
            }

            foreach (var smallCenter in smallCenterWidgets)
            {
                smallCenter.LocalPosition.X -= centerStackPos / 2 + smallWidgetsSpacing;
            }
        }

        Vec2 size = Size;

        float sizeTogether = 0f;
        smallLeftWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);
        smallRightWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);
        smallCenterWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);

        sizeTogether += smallWidgetsSpacing * (smallCenterWidgets.Count + smallLeftWidgets.Count + smallRightWidgets.Count + 0.25f) + middleWidgetsSpacing;

        size.X = (float)Math.Max(size.X, sizeTogether);
    }

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new System.Windows.Controls.ContextMenu();
        bool anyWidgetsLeft = false;

        MenuItem left = new MenuItem() { Header = "Left" };
        MenuItem middle = new MenuItem() { Header = "Middle" };
        MenuItem right = new MenuItem() { Header = "Right" };

        foreach (var availableWidget in Res.availableSmallWidgets)
        {
            if (Settings.smallWidgetsRight.Contains(availableWidget.GetType().FullName) ||
                Settings.smallWidgetsLeft.Contains(availableWidget.GetType().FullName) ||
                Settings.smallWidgetsMiddle.Contains(availableWidget.GetType().FullName)) continue;

            anyWidgetsLeft = true;

            var itemR = new MenuItem() { Header = availableWidget.GetType().Namespace.Split('.')[0] + ": " + availableWidget.WidgetName };
            itemR.Click += (x, y) =>
            {
                Settings.smallWidgetsRight.Add(availableWidget.GetType().FullName);
                UpdateWidgetDisplay();
            };

            var itemM = new MenuItem() { Header = availableWidget.GetType().Namespace.Split('.')[0] + ": " + availableWidget.WidgetName };
            itemM.Click += (x, y) =>
            {
                Settings.smallWidgetsMiddle.Add(availableWidget.GetType().FullName);
                UpdateWidgetDisplay();
            };

            var itemL = new MenuItem() { Header = availableWidget.GetType().Namespace.Split('.')[0] + ": " + availableWidget.WidgetName };
            itemL.Click += (x, y) =>
            {
                Settings.smallWidgetsLeft.Add(availableWidget.GetType().FullName);
                UpdateWidgetDisplay();
            };

            left.Items.Add(itemL);
            middle.Items.Add(itemM);
            right.Items.Add(itemR);
        }

        ctx.Items.Add(left);
        ctx.Items.Add(middle);
        ctx.Items.Add(right);

        if (!anyWidgetsLeft)
        {
            var ctx2 = new ContextMenu();
            ctx2.Items.Add(new MenuItem()
            {
                Header = "No Widgets Available.",
                IsEnabled = false
            });
            return ctx2;
        }

        return ctx;
    }
}

public class Checkbox : DWImageButton
{
    private bool isChecked = false;
    public bool IsChecked
    { get { return isChecked; } set => SetChecked(value); }

    private void SetChecked(bool isChecked)
    {
        this.isChecked = isChecked;
        Image.Image = isChecked ? Res.Check : null;
    }

    public Checkbox(UIObject? parent, string buttonText, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, Res.Check, position, size, clickCallback, alignment)
    {
        var text = new DWText(this, buttonText, new Vec2(15, 0), UIAlignment.MiddleRight)
        {
            Color = Theme.TextSecond
        };
        text.Anchor.X = 0;
        text.TextSize = size.Y / 1.5f;
        AddLocalObject(text);

        SetChecked(false);

        hoverScaleMulti = new Vec2(1.05f, 1f);
        clickScaleMulti = new Vec2(0.975f, 1f);
    }

    public override void OnMouseUp()
    {
        IsChecked = !IsChecked;
        base.OnMouseUp();
    }
}

public class MultiSelectionButton : UIObject
{
    private readonly string[] options;
    private readonly DWTextButton[] buttons;

    public Action<int> onClick;

    private int selectedIndex = 0;
    public int SelectedIndex { get => selectedIndex; set => SetSelected(value); }

    public MultiSelectionButton(UIObject? parent, string[] options, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter, int maxInOneRow = 4) : base(parent, position, size, alignment)
    {
        this.options = options;
        this.buttons = new DWTextButton[options.Length];

        float xPos = 0;
        float yPos = 0;
        int counter = 0;

        for (int i = 0; i < options.Length; i++)
        {
            var lambdaIndex = i; // Either I'm going insane or I don't understand lambdas, but it seems like only the pointer given in to the OnClick() method. This is why this line is needed!
            var action = () => OnClick(lambdaIndex); // For some it just outputs the length of options if there is no seperate variable for it.

            if (counter >= maxInOneRow)
            {
                counter = 0;
                yPos += 35;
                xPos = 0;
            }

            var btn = new DWTextButton(this, options[i], new Vec2(xPos, yPos), new Vec2(Math.Max(75, options[i].Length >= 11 ? (options[i].Length * 9) : 0), 25), action, UIAlignment.MiddleLeft);
            btn.Text.Color = Theme.TextSecond;
            btn.Anchor.X = 0;
            buttons[i] = btn;

            AddLocalObject(btn);

            xPos += btn.Size.X + 15;
            counter++;
        }

        Size.Y = Size.Y + yPos;

        SelectedIndex = 0;
    }

    public override void Draw(SKCanvas canvas)
    {
    }

    private void SetSelected(int index)
    {
        selectedIndex = index;

        foreach (var button in buttons)
        {
            button.normalColor = Theme.Secondary.Override(a: 0.9f);
        }

        buttons[index].normalColor = (Theme.Primary * 0.65f).Override(a: 0.75f);
    }

    private void OnClick(int index)
    {
        SelectedIndex = index;

        onClick.Invoke(index);
    }
}