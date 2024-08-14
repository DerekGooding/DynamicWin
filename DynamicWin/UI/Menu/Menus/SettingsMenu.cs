using DynamicWin.Main;
using DynamicWin.Resources;
using DynamicWin.UI.UIElements;
using DynamicWin.UI.Widgets;
using DynamicWin.Utils;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace DynamicWin.UI.Menu.Menus;

public class SettingsMenu : BaseMenu
{
    public SettingsMenu()
    {
        MainForm.onScrollEvent += (MouseWheelEventArgs x) => yScrollOffset += x.Delta * 0.25f;
    }

    private bool changedTheme;

    private void SaveAndBack()
    {
        Settings.AllowBlur = allowBlur.IsChecked;
        Settings.AllowAnimation = allowAnimation.IsChecked;
        Settings.AntiAliasing = antiAliasing.IsChecked;

        if (changedTheme)
        {
            MainForm.Instance.AddRenderer();
        }
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

                    if (optionItem.Alignment == UIAlignment.TopLeft)
                    {
                        optionItem.Position = new Vec2(25, 0);
                        optionItem.Anchor.X = 0;
                    }

                    if (optionItem is DWText dWText)
                    {
                        dWText.Color = Theme.TextThird;
                        dWText.Font = Res.InterRegular;
                        dWText.TextSize = 13;
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

    private float yScrollOffset;
    private float ySmoothScroll;

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

    public override Vec2 IslandSizeBig() => IslandSize() + 5;

    private static List<IRegisterableSetting> customOptions = [];

    private static void LoadCustomOptions()
    {
        customOptions.Clear();

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