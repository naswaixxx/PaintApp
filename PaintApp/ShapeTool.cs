using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PaintApp.Tools;

public abstract class ShapeTool : DrawingTool
{
    private Point _start;
    private Shape? _shape;

    protected ShapeTool(Canvas canvas, DrawingSettings settings) : base(canvas, settings) { }

    protected abstract Shape CreateShape();
    protected abstract void UpdateShape(Shape shape, Point start, Point end);

    public override void OnMouseDown(Point position)
    {
        _start = position;
        _shape = CreateShape();
        _shape.Stroke = Settings.CreateBrush();
        _shape.StrokeThickness = Settings.Thickness;
        _shape.Fill = null;
        Canvas.Children.Add(_shape);
        UpdateShape(_shape, _start, position);
    }

    public override void OnMouseMove(Point position)
    {
        if (_shape != null) UpdateShape(_shape, _start, position);
    }

    public override void OnMouseUp(Point position)
    {
        if (_shape != null) UpdateShape(_shape, _start, position);
        _shape = null;
    }
    protected static void SetBounds(FrameworkElement element, Point a, Point b)
    {
        Canvas.SetLeft(element, Math.Min(a.X, b.X));
        Canvas.SetTop(element, Math.Min(a.Y, b.Y));
        element.Width = Math.Abs(a.X - b.X);
        element.Height = Math.Abs(a.Y - b.Y);
    }
}