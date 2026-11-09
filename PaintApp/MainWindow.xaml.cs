using Microsoft.Win32;
using PaintApp.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PaintApp;

public partial class MainWindow : Window
{
    private static readonly string[] PaletteColors =
    {
        "#000000", "#FFFFFF", "#868E96", "#E03131",
        "#F76707", "#FAB005", "#40C057", "#12B886",
        "#15AABF", "#228BE6", "#5F3DC4", "#E64980"
    };

    private readonly DrawingSettings _settings = new();
    private readonly ToolManager _tools;
    private readonly List<(Border Swatch, Color Color)> _swatches = new();
    private bool _ready;
    private bool _updatingSliders;

    public MainWindow()
    {
        InitializeComponent();
        _tools = new ToolManager(DrawCanvas, _settings);

        BuildToolButtons();
        BuildSwatches();

        _ready = true;
        SetColor(Colors.Black);
        UpdateThickness();
    }

    private void BuildToolButtons()
    {
        foreach (var tool in _tools.Tools)
        {
            var icon = new System.Windows.Shapes.Path
            {
                Data = Geometry.Parse(tool.IconData),
                Width = 26,
                Height = 26,
                Stretch = Stretch.Uniform,
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Margin = new Thickness(0, 0, 0, 6)
            };
            icon.SetBinding(Shape.StrokeProperty, new Binding("Foreground")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadioButton), 1)
            });

            var content = new StackPanel();
            content.Children.Add(icon);
            content.Children.Add(new TextBlock
            {
                Text = tool.Name,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var button = new RadioButton
            {
                GroupName = "Tools",
                Style = (Style)FindResource("ToolButton"),
                ToolTip = tool.Name,
                Tag = tool,
                Content = content,
                IsChecked = tool == _tools.ActiveTool
            };
            button.Checked += (s, _) => _tools.ActiveTool = (DrawingTool)((RadioButton)s).Tag;
            ToolsPanel.Children.Add(button);
        }
    }

    private void BuildSwatches()
    {
        foreach (var hex in PaletteColors)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            var swatch = new Border
            {
                Width = 28,
                Height = 28,
                CornerRadius = new CornerRadius(14),
                Margin = new Thickness(4),
                Background = new SolidColorBrush(color),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand,
                ToolTip = hex
            };
            swatch.MouseLeftButtonUp += (_, _) => SetColor(color);
            _swatches.Add((swatch, color));
            SwatchPanel.Children.Add(swatch);
        }
    }

    private void SetColor(Color color)
    {
        _settings.Color = color;

        _updatingSliders = true;
        RSlider.Value = color.R;
        GSlider.Value = color.G;
        BSlider.Value = color.B;
        _updatingSliders = false;

        RefreshColorUi();
    }

    private void RefreshColorUi()
    {
        var color = _settings.Color;
        var brush = new SolidColorBrush(color);

        ColorPreview.Background = brush;
        HexText.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        HexText.Foreground = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) > 150
            ? Brushes.Black : Brushes.White;
        ThicknessPreview.Fill = brush;

        foreach (var (swatch, swatchColor) in _swatches)
            swatch.BorderBrush = swatchColor == color ? Brushes.White : Brushes.Transparent;
    }

    private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_ready || _updatingSliders) return;
        _settings.Color = Color.FromRgb((byte)RSlider.Value, (byte)GSlider.Value, (byte)BSlider.Value);
        RefreshColorUi();
    }

    private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_ready) return;
        UpdateThickness();
    }

    private void UpdateThickness()
    {
        _settings.Thickness = ThicknessSlider.Value;
        ThicknessText.Text = $"{(int)_settings.Thickness} px";
        ThicknessPreview.Width = _settings.Thickness;
        ThicknessPreview.Height = _settings.Thickness;
    }

    private Point GetCanvasPoint(MouseEventArgs e)
    {
        var p = e.GetPosition(DrawCanvas);
        return new Point(
            Math.Clamp(p.X, 0, DrawCanvas.ActualWidth),
            Math.Clamp(p.Y, 0, DrawCanvas.ActualHeight));
    }

    private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DrawCanvas.CaptureMouse();
        _tools.MouseDown(GetCanvasPoint(e));
    }

    private void DrawCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && DrawCanvas.IsMouseCaptured)
            _tools.MouseMove(GetCanvasPoint(e));
    }

    private void DrawCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!DrawCanvas.IsMouseCaptured) return;
        DrawCanvas.ReleaseMouseCapture();
        _tools.MouseUp(GetCanvasPoint(e));
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(this, "Очистить холст? Это действие нельзя отменить.",
            "Очистка", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
            DrawCanvas.Children.Clear();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Сохранить изображение",
            Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg",
            FileName = "рисунок",
            DefaultExt = ".png",
            AddExtension = true
        };

        if (dialog.ShowDialog(this) != true) return;

        try
        {
            ImageExporter.Save(DrawCanvas, dialog.FileName);
            MessageBox.Show(this, "Изображение сохранено.", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось сохранить файл:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}