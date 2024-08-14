using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.Menu.Menus;

public class MultiSelectionButton : UIObject
{
    private readonly string[] options;
    private readonly DWTextButton[] buttons;

    public Action<int> onClick;

    private int selectedIndex;
    public int SelectedIndex { get => selectedIndex; set => SetSelected(value); }

    public MultiSelectionButton(
        UIObject? parent,
        string[] options,
        Vec2 position,
        Vec2 size,
        UIAlignment alignment = UIAlignment.TopCenter,
        int maxInOneRow = 4) : base(parent, position, size, alignment)
    {
        this.options = options;
        buttons = new DWTextButton[options.Length];

        float xPos = 0;
        float yPos = 0;
        int counter = 0;

        for (int i = 0; i < options.Length; i++)
        {
            var lambdaIndex = i; // Either I'm going insane or I don't understand lambdas, but it seems like only the pointer given in to the OnClick() method. This is why this line is needed!
            void action() => OnClick(lambdaIndex); // For some it just outputs the length of options if there is no seperate variable for it.

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

        Size.Y += yPos;

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