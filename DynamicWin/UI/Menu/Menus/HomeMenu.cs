﻿using DynamicWin.Main;
using DynamicWin.Resources;
using DynamicWin.UI.UIElements;
using DynamicWin.UI.UIElements.Custom;
using DynamicWin.UI.Widgets;
using DynamicWin.UI.Widgets.Small;
using DynamicWin.Utils;

namespace DynamicWin.UI.Menu.Menus;

public class HomeMenu : BaseMenu
{
    public List<SmallWidgetBase> smallLeftWidgets = [];
    public List<SmallWidgetBase> smallRightWidgets = [];
    public List<SmallWidgetBase> smallCenterWidgets = [];

    public List<WidgetBase> bigWidgets = [];

    private float songSizeAddition;
    private float songLocalPosXAddition;

    public void NextSong()
    {
        if (RendererMain.Instance.MainIsland.IsHovering) return;

        songSizeAddition = 45;
        songLocalPosXAddition = 45;
    }

    public void PrevSong()
    {
        if (RendererMain.Instance.MainIsland.IsHovering) return;

        songSizeAddition = 45;
        songLocalPosXAddition = -45;
    }

    public override Vec2 IslandSize()
    {
        Vec2 size = new(200, 35);

        float sizeTogether = 0f;
        smallLeftWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);
        smallRightWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);
        smallCenterWidgets.ForEach(x => sizeTogether += x.GetWidgetSize().X);

        sizeTogether += (smallWidgetsSpacing * (smallCenterWidgets.Count + smallLeftWidgets.Count + smallRightWidgets.Count + 0.25f)) + middleWidgetsSpacing;

        size.X = (float)Math.Max(size.X, sizeTogether) + songSizeAddition;

        return size;
    }

    public override Vec2 IslandSizeBig()
    {
        Vec2 size = new(275, 145);

        {
            float sizeTogetherBiggest = 0f;
            float sizeTogether = 0f;

            for (int i = 0; i < bigWidgets.Count; i++)
            {
                sizeTogether += bigWidgets[i].GetWidgetSize().X + (bigWidgetsSpacing * 2);
                if (i % maxBigWidgetInOneRow == 1)
                {
                    sizeTogetherBiggest = (float)Math.Max(sizeTogetherBiggest, sizeTogether);
                    sizeTogether = 0f;
                }
            }

            size.X = (float)Math.Max(size.X, sizeTogetherBiggest);
        }

        {
            float sizeTogetherBiggest = 0f;

            for (int i = 0; i < bigWidgets.Count; i++)
            {
                if (i % maxBigWidgetInOneRow == 0)
                {
                    sizeTogetherBiggest += bigWidgets[i].GetWidgetSize().Y;
                }
            }

            sizeTogetherBiggest += bCD + (bigWidgetsSpacing * (int)Math.Floor((float)(bigWidgets.Count / maxBigWidgetInOneRow))) + topSpacing;

            // Set the container height to the total height of all rows
            size.Y = Math.Max(size.Y, sizeTogetherBiggest + topSpacing);
        }

        if (!isWidgetMode) size.Y = 250;

        return size;
    }

    private UIObject smallWidgetsContainer;
    private UIObject bigWidgetsContainer;

    private readonly List<UIObject> bigMenuItems = [];

    private UIObject topContainer;

    private DWTextImageButton widgetButton;
    private DWTextImageButton trayButton;

    private Tray tray;

    public override List<UIObject> InitializeMenu(IslandObject island)
    {
        var objects = base.InitializeMenu(island);

        smallWidgetsContainer = new UIObject(island, Vec2.zero, IslandSize(), UIAlignment.Center);
        bigWidgetsContainer = new UIObject(island, Vec2.zero, IslandSize(), UIAlignment.Center);

        // Create elements

        topContainer = new UIObject(island, new Vec2(0, 30), new Vec2(island.currSize.X, 50))
        {
            Color = Theme.IslandBackground.Inverted().Override(a: 0.035f),
            roundRadius = 10f
        };
        bigMenuItems.Add(topContainer);

        widgetButton = new DWTextImageButton(topContainer, Resources.Res.Widgets, "Widgets", new Vec2((75 / 2) + 5, 0), new Vec2(75, 20), () => isWidgetMode = true,
        UIAlignment.MiddleLeft)
        {
            normalColor = Col.Transparent,
            hoverColor = Col.Transparent,
            clickColor = Theme.Primary.Override(a: 0.35f),
            roundRadius = 25
        };

        bigMenuItems.Add(widgetButton);

        trayButton = new DWTextImageButton(topContainer, Res.Tray, "Tray", new Vec2(110, 0), new Vec2(55, 20), () => isWidgetMode = false,
        UIAlignment.MiddleLeft)
        {
            normalColor = Col.Transparent,
            hoverColor = Col.Transparent,
            clickColor = Theme.Primary.Override(a: 0.35f),
            roundRadius = 25
        };

        bigMenuItems.Add(trayButton);

        var settingsButton = new DWImageButton(topContainer, Res.Settings, new Vec2(-20f, 0), new Vec2(20, 20), () => MenuManager.OpenMenu(new SettingsMenu()),
        UIAlignment.MiddleRight)
        {
            normalColor = Col.Transparent,
            hoverColor = Col.Transparent,
            clickColor = Theme.Primary.Override(a: 0.35f),
            roundRadius = 25
        };

        bigMenuItems.Add(settingsButton);

        tray = new Tray(island, new Vec2(0, -bCD / 2), Vec2.zero, UIAlignment.BottomCenter);
        tray.Anchor.Y = 1f;
        tray.SilentSetActive(false);

        bigMenuItems.Add(tray);

        // Get all widgets

        Dictionary<string, IRegisterableWidget> smallWidgets = [];
        Dictionary<string, IRegisterableWidget> widgets = [];

        foreach (var widget in Res.availableSmallWidgets)
        {
            if (smallWidgets.ContainsKey(widget.GetType().FullName)) continue;
            smallWidgets.Add(widget.GetType().FullName, widget);
            System.Diagnostics.Debug.WriteLine(widget.GetType().FullName);
        }

        foreach (var widget in Res.availableBigWidgets)
        {
            if (widgets.ContainsKey(widget.GetType().FullName)) continue;
            widgets.Add(widget.GetType().FullName, widget);
            System.Diagnostics.Debug.WriteLine(widget.GetType().FullName);
        }

        // Create widgets

        foreach (var smallWidget in Settings.smallWidgetsMiddle)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;
            var widget = smallWidgets[smallWidget];

            smallCenterWidgets.Add((SmallWidgetBase)widget.CreateWidgetInstance(smallWidgetsContainer, Vec2.zero, UIAlignment.Center));
        }

        foreach (var smallWidget in Settings.smallWidgetsLeft)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;
            var widget = smallWidgets[smallWidget];

            smallLeftWidgets.Add((SmallWidgetBase)widget.CreateWidgetInstance(smallWidgetsContainer, Vec2.zero, UIAlignment.MiddleLeft));
        }

        foreach (var smallWidget in Settings.smallWidgetsRight)
        {
            if (!smallWidgets.ContainsKey(smallWidget)) continue;
            var widget = smallWidgets[smallWidget];

            smallRightWidgets.Add((SmallWidgetBase)widget.CreateWidgetInstance(smallWidgetsContainer, Vec2.zero, UIAlignment.MiddleRight));
        }

        foreach (var bigWidget in Settings.bigWidgets)
        {
            if (!widgets.ContainsKey(bigWidget)) continue;
            var widget = widgets[bigWidget];

            bigWidgets.Add(widget.CreateWidgetInstance(bigWidgetsContainer, Vec2.zero, UIAlignment.BottomCenter));
        }

        smallLeftWidgets.ForEach(objects.Add);

        smallRightWidgets.ForEach(objects.Add);

        smallCenterWidgets.ForEach(objects.Add);

        bigWidgets.ForEach(x =>
        {
            objects.Add(x);
            x.SilentSetActive(false);
        });

        // Add lists

        bigMenuItems.ForEach(x =>
        {
            objects.Add(x);
            x.SilentSetActive(false);
        });

        next = new DWImage(island, Res.Next, new Vec2(-7.5f, 0), new Vec2(15, 15), UIAlignment.MiddleRight)
        {
            Anchor = new Vec2(1, 0.5f),
            blurSizeOnDisable = 3f
        };
        next.SilentSetActive(false);
        objects.Add(next);

        previous = new DWImage(island, Res.Previous, new Vec2(7.5f, 0), new Vec2(15, 15), UIAlignment.MiddleLeft)
        {
            Anchor = new Vec2(0, 0.5f),
            blurSizeOnDisable = 3f
        };
        previous.SilentSetActive(false);
        objects.Add(previous);

        return objects;
    }

    private DWImage next;
    private DWImage previous;

    public float topSpacing = 20;
    public float bigWidgetsSpacing = 15;
    private readonly int maxBigWidgetInOneRow = 2;

    public float smallWidgetsSpacing = 10;
    public float middleWidgetsSpacing = 35;

    private bool wasHovering;

    private readonly float sCD = 35;
    private readonly float bCD = 50;

    public bool isWidgetMode = true;
    private bool wasWidgetMode;

    public override void Update()
    {
        tray.Size = new Vec2(IslandSizeBig().X - bCD, RendererMain.Instance.MainIsland.Size.Y - bCD - topSpacing - (topContainer.Size.Y / 2));
        tray.SetActive(!(isWidgetMode || !RendererMain.Instance.MainIsland.IsHovering));

        widgetButton.normalColor = Col.Lerp(widgetButton.normalColor, isWidgetMode
            ? Col.White.Override(a: 0.075f)
            : Col.Transparent, 15f * RendererMain.Instance.DeltaTime);
        trayButton.normalColor = Col.Lerp(trayButton.normalColor, (!isWidgetMode)
            ? Col.White.Override(a: 0.075f)
            : Col.Transparent, 15f * RendererMain.Instance.DeltaTime);
        widgetButton.hoverColor = Col.Lerp(widgetButton.hoverColor, isWidgetMode
            ? Col.White.Override(a: 0.075f)
            : Col.Transparent, 15f * RendererMain.Instance.DeltaTime);
        trayButton.hoverColor = Col.Lerp(trayButton.hoverColor, (!isWidgetMode)
            ? Col.White.Override(a: 0.075f)
            : Col.Transparent, 15f * RendererMain.Instance.DeltaTime);

        RendererMain.Instance.MainIsland.LocalPosition.X = Mathf.Lerp(RendererMain.Instance.MainIsland.LocalPosition.X,
            songLocalPosXAddition, 2f * RendererMain.Instance.DeltaTime);
        songLocalPosXAddition = Mathf.Lerp(songLocalPosXAddition, 0f, 10 * RendererMain.Instance.DeltaTime);
        songSizeAddition = Mathf.Lerp(songSizeAddition, 0f, 10 * RendererMain.Instance.DeltaTime);

        if (Math.Abs(songLocalPosXAddition) < 5f)
        {
            if (next.IsEnabled)
                next.SetActive(false);
            if (previous.IsEnabled)
                previous.SetActive(false);
        }
        else if (songLocalPosXAddition > 15f)
        {
            next.SetActive(true);
        }
        else if (songLocalPosXAddition < -15f)
        {
            previous.SetActive(true);
        }

        if (!RendererMain.Instance.MainIsland.IsHovering)
        {
            var smallContainerSize = IslandSize() - songSizeAddition;
            smallContainerSize -= sCD;
            smallWidgetsContainer.LocalPosition.X = -RendererMain.Instance.MainIsland.LocalPosition.X;
            smallWidgetsContainer.Size = smallContainerSize;

            { // Left Small Widgets
                float leftStackedPos = 0f;
                foreach (var smallLeft in smallLeftWidgets)
                {
                    smallLeft.Anchor.X = 0;
                    smallLeft.LocalPosition.X = leftStackedPos;

                    leftStackedPos += smallWidgetsSpacing + smallLeft.GetWidgetSize().X;
                }
            }

            { // Right Small Widgets
                float rightStackedPos = 0f;
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
                    smallCenter.LocalPosition.X -= (centerStackPos / 2) + smallWidgetsSpacing;
                }
            }

            smallLeftWidgets.ForEach(x => x.SetActive(true));
            smallRightWidgets.ForEach(x => x.SetActive(true));
            smallCenterWidgets.ForEach(x => x.SetActive(true));

            if (wasHovering)
            {
                bigWidgets.ForEach(x => x.SetActive(false));
                bigMenuItems.ForEach(x => x.SetActive(false));
            }

            wasHovering = false;
        }
        else if (RendererMain.Instance.MainIsland.IsHovering)
        {
            topContainer.Size = new Vec2(RendererMain.Instance.MainIsland.currSize.X - bCD, 30);

            var bigContainerSize = IslandSizeBig();
            bigContainerSize -= bCD;
            bigWidgetsContainer.Size = bigContainerSize;

            { // Big Widgets
                List<WidgetBase> widgetsInOneLine = [];

                float lastBiggestY = 0f;

                for (int i = 0; i < bigWidgets.Count; i++)
                {
                    int line = i / maxBigWidgetInOneRow; // Correct line calculation

                    widgetsInOneLine.Add(bigWidgets[i]);
                    bigWidgets[i].Anchor.Y = 1;
                    bigWidgets[i].Anchor.X = 0.5f;
                    lastBiggestY = (float)Math.Max(lastBiggestY, bigWidgets[i].GetWidgetSize().Y);

                    bigWidgets[i].LocalPosition.Y = -line * (lastBiggestY + bigWidgetsSpacing);

                    if (i % maxBigWidgetInOneRow == 1)
                    {
                        lastBiggestY = 0f;
                        CenterWidgets(widgetsInOneLine, bigWidgetsContainer);
                        widgetsInOneLine.Clear();
                        line++;
                    }
                }
            }

            if (!wasHovering)
            {
                smallLeftWidgets.ForEach(x => x.SetActive(false));
                smallCenterWidgets.ForEach(x => x.SetActive(false));
                smallRightWidgets.ForEach(x => x.SetActive(false));

                bigWidgets.ForEach(x => x.SetActive(isWidgetMode));
                bigMenuItems.ForEach(x => x.SetActive(true));
            }

            if (isWidgetMode && !wasWidgetMode)
            {
                wasWidgetMode = true;
                bigWidgets.ForEach(x => x.SetActive(true));
            }
            else if (!isWidgetMode && wasWidgetMode)
            {
                wasWidgetMode = false;
                bigWidgets.ForEach(x => x.SetActive(false));
            }

            wasHovering = true;
        }
    }

    public void CenterWidgets(List<WidgetBase> widgets, UIObject container)
    {
        float spacing = bigWidgetsSpacing;
        float stackedXPosition = 0f;

        float fullWidth = 0f;

        for (int i = 0; i < widgets.Count; i++)
        {
            fullWidth += widgets[i].GetWidgetSize().X;

            widgets[i].LocalPosition.X = stackedXPosition + (widgets[i].GetWidgetSize().X / 2) - (container.Size.X / 2);
            stackedXPosition += widgets[i].GetWidgetSize().X + spacing;
        }

        float offset = (fullWidth / 2) - (container.Size.X / 2) + (bigWidgetsSpacing / 2);

        for (int i = 0; i < widgets.Count; i++)
        {
            widgets[i].LocalPosition.X -= offset;
        }
    }
}