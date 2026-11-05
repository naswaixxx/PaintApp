using System.Windows.Media;

namespace PaintApp;
public class DrawingSettings
{
    private Color _color = Colors.Black;
    private double _thickness = 3;

    public Color Color
    {
        get => _color;
        set => _color = value;
    }

    public double Thickness
    {
        get => _thickness;
        set => _thickness = Math.Clamp(value, 1, 100);
    }

    public Brush CreateBrush()
    {
        var brush = new SolidColorBrush(_color);
        brush.Freeze();
        return brush;
    }
}