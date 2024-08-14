﻿using DynamicWin.Resources;
using DynamicWin.UI.Menu;
using DynamicWin.UI.Menu.Menus;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicWin.Main;

public partial class MainForm : Window
{
    private static MainForm instance;
    public static MainForm Instance => instance;

    public static Action<System.Windows.Input.MouseWheelEventArgs> onScrollEvent;

    private DateTime _lastRenderTime;
    private readonly TimeSpan _targetElapsedTime = TimeSpan.FromMilliseconds(16); // ~60 FPS

    public Action onMainFormRender;

    public MainForm()
    {
        InitializeComponent();

        CompositionTarget.Rendering += OnRendering;

        instance = this;

        WindowStyle = WindowStyle.None;
        WindowState = WindowState.Maximized;

        Topmost = true;
        AllowsTransparency = true;
        ShowInTaskbar = false;
        Title = "DynamicWin Overlay";

        Width = SystemParameters.WorkArea.Width;
        Height = SystemParameters.WorkArea.Height;

        AddRenderer();

        Res.extensions.ForEach((x) => x.LoadExtension());

        Instance.AllowDrop = true;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var currentTime = DateTime.Now;
        if (currentTime - _lastRenderTime >= _targetElapsedTime)
        {
            _lastRenderTime = currentTime;

            onMainFormRender.Invoke();
        }
    }

    public bool isDragging = false;

    public void OnScroll(object? sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        onScrollEvent?.Invoke(e);
    }

    public void AddRenderer()
    {
        RendererMain.Instance?.Dispose();

        var customControl = new RendererMain();

        var parent = new Grid();
        parent.Children.Add(customControl);

        Content = parent;
    }

    public void MainForm_DragEnter(object? sender, DragEventArgs e)
    {
        //System.Diagnostics.Debug.WriteLine("DragEnter");

        isDragging = true;
        e.Effects = DragDropEffects.Copy;

        if (MenuManager.Instance.ActiveMenu is not DropFileMenu
            && MenuManager.Instance.ActiveMenu is not ConfigureShortcutMenu)
        {
            MenuManager.OpenMenu(new DropFileMenu());
        }
    }

    public void MainForm_DragLeave(object? sender, EventArgs e)
    {
        //System.Diagnostics.Debug.WriteLine("DragLeave");

        isDragging = false;

        if (MenuManager.Instance.ActiveMenu is ConfigureShortcutMenu) return;
        MenuManager.OpenMenu(Res.HomeMenu);
    }

    private bool isLocalDrag;

    internal void StartDrag(string[] files, Action callback)
    {
        if (isLocalDrag) return;

        Array.ForEach(files, file => System.Diagnostics.Debug.WriteLine(file));

        if (files == null || files.Length == 0) return;

        try
        {
            isLocalDrag = true;

            DataObject dataObject = new(DataFormats.FileDrop, files);
            var effects = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move | DragDropEffects.Copy);

            RendererMain.Instance?.Dispose();
            Content = new Grid();
            AddRenderer();

            callback?.Invoke();

            isLocalDrag = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }

    protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
        if (e.Action == DragAction.Cancel)
        {
            isLocalDrag = false;
        }
        else if (e.Action == DragAction.Continue)
        {
            isLocalDrag = true;
        }
        else if (e.Action == DragAction.Drop)
        {
            isLocalDrag = false;
        }
    }

    protected override void OnDragOver(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Move;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        base.OnDragOver(e);
    }

    public void OnDrop(object sender, DragEventArgs e)
    {
        isDragging = false;

        if (MenuManager.Instance.ActiveMenu is ConfigureShortcutMenu)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ConfigureShortcutMenu.DropData(e);
            }
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            DropFileMenu.Drop(e);
            MenuManager.Instance.QueueOpenMenu(Res.HomeMenu);
            Res.HomeMenu.isWidgetMode = false;
        }
    }
}