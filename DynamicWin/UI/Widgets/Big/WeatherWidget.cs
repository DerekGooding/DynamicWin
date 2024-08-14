using DynamicWin.Resources;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;
using System.Windows.Controls;

namespace DynamicWin.UI.Widgets.Big;

public class WeatherWidget : WidgetBase
{
    readonly DWText temperatureText;
    readonly DWText weatherText;
    readonly DWText locationText;

    readonly UIObject locationTextReplacement;

    static WeatherFetcher? weatherFetcher;

    readonly DWImage weatherTypeIcon;

    bool hideLocation;

    public WeatherWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        AddLocalObject(new DWImage(this, Res.Location, new Vec2(20, 17.5f), new Vec2(12.5f, 12.5f), UIAlignment.TopLeft)
        {
            Color = Theme.TextSecond,
            allowIconThemeColor = true
        });

        locationText = new DWText(this, "--", new Vec2(32.5f, 17.5f), UIAlignment.TopLeft)
        {
            TextSize = 15,
            Anchor = new Vec2(0, 0.5f),
            Color = Theme.TextSecond
        };
        AddLocalObject(locationText);

        locationTextReplacement = new UIObject(this, new Vec2(32.5f, 17.5f), new Vec2(75, 15), UIAlignment.TopLeft)
        {
            roundRadius = 5f,
            Anchor = new Vec2(0, 0.5f),
            Color = Theme.TextSecond
        };
        AddLocalObject(locationTextReplacement);

        AddLocalObject(new DWImage(this, Res.Weather, new Vec2(20, 37.5f), new Vec2(12.5f, 12.5f), UIAlignment.TopLeft)
        {
            Color = Theme.TextThird,
            allowIconThemeColor = true
        });

        weatherText = new DWText(this, "--", new Vec2(32.5f, 37.5f), UIAlignment.TopLeft)
        {
            TextSize = 13,
            Font = Res.InterBold,
            Anchor = new Vec2(0, 0.5f),
            Color = Theme.TextThird
        };
        AddLocalObject(weatherText);

        temperatureText = new DWText(this, "--", new Vec2(15, -27.5f), UIAlignment.BottomLeft)
        {
            TextSize = 34,
            Anchor = new Vec2(0, 0.5f),
            Color = Theme.TextMain
        };
        AddLocalObject(temperatureText);

        weatherTypeIcon = new DWImage(this, Res.Weather, new Vec2(0, 0), new Vec2(100, 100), UIAlignment.MiddleRight)
        {
            Color = Theme.TextThird,
            allowIconThemeColor = true
        };

        weatherFetcher ??= new WeatherFetcher();

        weatherFetcher.onWeatherDataReceived += OnWeatherDataReceived;
        weatherFetcher.Fetch();

        LoadPersistentData();
        locationTextReplacement.SilentSetActive(hideLocation);
        locationText.SilentSetActive(!hideLocation);
    }

    void LoadPersistentData() => hideLocation = (bool?)SaveManager.Get("weather.hideLoc") ?? false;

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new ContextMenu();

        var hideLocationItem = new MenuItem() { Header = "Hide Location", IsCheckable = true, IsChecked = hideLocation };
        hideLocationItem.Click += (x, y) =>
        {
            hideLocation = hideLocationItem.IsChecked;

            locationTextReplacement.SetActive(hideLocation);
            locationText.SetActive(!hideLocation);

            SaveManager.Add("weather.hideLoc", hideLocation);
            SaveManager.SaveAll();
        };

        ctx.Items.Add(hideLocationItem);

        return ctx;
    }

    void OnWeatherDataReceived(WeatherFetcher.WeatherData weatherData)
    {
        temperatureText.SetText(weatherData.temperatureCelsius);
        weatherText.SetText(weatherData.weatherText);
        locationText.SetText(weatherData.city);

        UpdateIcon(weatherData.weatherText);
    }

    void UpdateIcon(string weather)
    {
        if (weather.Contains("sun", StringComparison.CurrentCultureIgnoreCase) || weather.Contains("clear", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Sunny;
        }
        else if (weather.Contains("cloud", StringComparison.CurrentCultureIgnoreCase) || weather.Contains("overcast", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Cloudy;
        }
        else if (weather.Contains("rain", StringComparison.CurrentCultureIgnoreCase) || weather.Contains("shower", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Rainy;
        }
        else if (weather.Contains("thunder", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Thunderstorm;
        }
        else if (weather.Contains("snow", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Snowy;
        }
        else if (weather.Contains("sleet", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Rainy;
        }
        else if (weather.Contains("fog", StringComparison.CurrentCultureIgnoreCase)
                    || weather.Contains("haze", StringComparison.CurrentCultureIgnoreCase)
                    || weather.Contains("mist", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Foggy;
        }
        else if (weather.Contains("windy", StringComparison.CurrentCultureIgnoreCase) || weather.Contains("breezy", StringComparison.CurrentCultureIgnoreCase))
        {
            weatherTypeIcon.Image = Res.Windy;
        }
        else
        {
            weatherTypeIcon.Image = Res.SevereWeatherWarning;
        }
    }

    public override void DrawWidget(SKCanvas canvas)
    {
        base.DrawWidget(canvas);

        var paint = GetPaint();
        paint.Color = GetColor(Theme.WidgetBackground).Value();
        canvas.DrawRoundRect(GetRect(), paint);

        canvas.ClipRoundRect(GetRect(), SKClipOperation.Intersect, true);
        weatherTypeIcon.DrawCall(canvas);
    }
}