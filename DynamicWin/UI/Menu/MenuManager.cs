﻿using DynamicWin.Main;
using DynamicWin.Utils;

namespace DynamicWin.UI.Menu;

public class MenuManager
{
    public BaseMenu ActiveMenu { get; private set; }

    public static MenuManager Instance { get; private set; }

    public Action<BaseMenu, BaseMenu> onMenuChange;
    public Action<BaseMenu> onMenuChangeEnd;

    public MenuManager() => Instance = this;

    public void Init()
    {
        Resources.Res.CreateStaticMenus();
        ActiveMenu = Resources.Res.HomeMenu;
    }

    public static void OpenMenu(BaseMenu newActiveMenu) => Instance.Open(newActiveMenu);

    private void Open(BaseMenu newActiveMenu) => SetActiveMenu(newActiveMenu);

    public static void OpenOverlayMenu(BaseMenu newActiveMenu, float time = 5f) => Instance.OpenOverlay(newActiveMenu, time);

    private static Thread overlayThread;

    public static void CloseOverlay() => overlayThread.Interrupt();

    private void OpenOverlay(BaseMenu newActiveMenu, float time)
    {
        overlayThread = new Thread(() =>
        {
            BaseMenu lastMenu = ActiveMenu;

            QueueOpenMenu(newActiveMenu);
            int timeMillis = (int)(time * 1000);

            try
            {
                Thread.Sleep(timeMillis);
            }
            catch
            {
                QueueOpenMenu(lastMenu);
                return;
            }

            if (lastMenu == null) throw new NullReferenceException();
            QueueOpenMenu(lastMenu);
        });
        overlayThread.Start();
    }

    private readonly List<BaseMenu> menuLoadQueue = [];

    private Animator? menuAnimatorIn;
    private Animator? menuAnimatorOut;

    public void Update(float deltaTime)
    {
        menuAnimatorIn?.Update(deltaTime);
        menuAnimatorOut?.Update(deltaTime);
    }

    private void SetActiveMenu(BaseMenu newActiveMenu)
    {
        if (menuAnimatorIn?.IsRunning == true) return;
        if (menuAnimatorOut?.IsRunning == true) return;

        onMenuChange?.Invoke(ActiveMenu, newActiveMenu);

        float yOffset = RendererMain.Instance.MainIsland.Size.Y * 0.75f;

        const int length = 250;

        List<UIObject> currentObjects = new(ActiveMenu.UiObjects);

        {
            menuAnimatorOut = new Animator(length, 1);

            currentObjects = new List<UIObject>(ActiveMenu.UiObjects);

            menuAnimatorOut.onAnimationUpdate += (t) =>
            {
                float tEased = Easing.EaseOutCubic(t);

                currentObjects.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        obj.blurAmount = Mathf.Lerp(35, 0, tEased);
                    }
                });

                RendererMain.Instance.renderOffset.Y = Mathf.Lerp(-yOffset, 0, tEased);
            };

            menuAnimatorOut.onAnimationEnd += () =>
            {
                ActiveMenu = newActiveMenu;

                RendererMain.Instance.renderOffset.Y = 0;
                LoadMenuEnd();

                return;
            };
        }

        if (ActiveMenu != null)
        {
            menuAnimatorIn = new Animator(length, 1);

            menuAnimatorIn.onAnimationUpdate += (t) =>
            {
                float tEased = Easing.EaseInCubic(t);

                currentObjects.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        obj.blurAmount = Mathf.Lerp(0, 35, tEased);
                    }
                });

                if (RendererMain.Instance != null)
                    RendererMain.Instance.renderOffset.Y = Mathf.Lerp(0, yOffset, tEased);
            };

            menuAnimatorIn.onAnimationInterrupt += () =>
            {
                LoadMenuEnd();
                return;
            };

            menuAnimatorIn.onAnimationEnd += () =>
            {
                ActiveMenu?.OnUnload();
                ActiveMenu = newActiveMenu;

                RendererMain.Instance.renderOffset.Y = -yOffset;

                currentObjects = new List<UIObject>(ActiveMenu.UiObjects);
                currentObjects.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        obj.blurAmount = 35;
                    }
                });

                if (menuAnimatorOut == null)
                {
                    LoadMenuEnd();
                    return;
                }

                ActiveMenu.UiObjects.Remove(menuAnimatorIn);

                menuAnimatorOut.Start();
            };
        }

        if (menuAnimatorIn == null) menuAnimatorOut.Start();
        else
            menuAnimatorIn.Start();
    }

    private void LoadMenuEnd()
    {
        onMenuChangeEnd?.Invoke(ActiveMenu);

        if (menuLoadQueue.Count != 0)
        {
            var queueObj = menuLoadQueue[0];

            if (queueObj == ActiveMenu)
            {
                menuLoadQueue.Remove(queueObj);
                return;
            }
            else
            {
                OpenMenu(queueObj);
            }

            menuLoadQueue.Remove(queueObj);
        }

        menuAnimatorIn = null;
        menuAnimatorOut = null;
    }

    public void QueueOpenMenu(BaseMenu menu)
    {
        if (menuAnimatorIn == null && menuAnimatorOut == null)
        {
            OpenMenu(menu);
        }
        else
        {
            menuLoadQueue.Add(menu);
        }
    }
}