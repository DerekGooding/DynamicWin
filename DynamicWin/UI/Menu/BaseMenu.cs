using DynamicWin.Main;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Menu;

public class BaseMenu : IDisposable
{
    public List<UIObject> UiObjects { get; } = [];

    public BaseMenu()
    {
        UiObjects = InitializeMenu(RendererMain.Instance.MainIsland);
    }

    public virtual Vec2 IslandSize() => new(200, 45);

    public virtual Vec2 IslandSizeBig() => IslandSize();

    public virtual List<UIObject> InitializeMenu(IslandObject island) => [];

    public virtual void Update()
    { }

    public virtual void OnDeload()
    { }

    public void Dispose()
    {
        UiObjects.Clear();
        GC.SuppressFinalize(this);
    }
}