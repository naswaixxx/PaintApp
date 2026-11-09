using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintApp.Tools;

public class RectangleTool : ShapeTool
{
    public RectangleTool(Canvas canvas, DrawingSettings settings) : base(canvas, settings) { }

    public override string Name => "Прямоугольник";
    public override string IconData => "M4,6 H20 V18 H4 Z";

    protected override Shape CreateShape() => new Rectangle { StrokeLineJoin = PenLineJoin.Miter };

    protected override void UpdateShape(Shape shape, Point start, Point end)
        => SetBounds(shape, start, end);
}