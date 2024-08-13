using DynamicWin.Main;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Menu;

public class BaseMenu : IDisposable
{

    public List<UIObject> UiObjects { get; } = new List<UIObject>();

    public BaseMenu()
    {
        UiObjects = InitializeMenu(RendererMain.Instance.MainIsland);
    }

    public virtual Vec2 IslandSize()
    { return new Vec2(200, 45); }

    public virtual Vec2 IslandSizeBig()
    { return IslandSize(); }

    public virtual List<UIObject> InitializeMenu(IslandObject island)
    { return new List<UIObject>(); }

    public virtual void Update()
    { }

    public virtual void OnDeload()
    { }

    public void Dispose()
    {
        UiObjects.Clear();
    }
}