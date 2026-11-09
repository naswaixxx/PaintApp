using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PaintApp.Tools;

public class EllipseTool : ShapeTool
{
    public EllipseTool(Canvas canvas, DrawingSettings settings) : base(canvas, settings) { }

    public override string Name => "Эллипс";
    public override string IconData => "M3,12 A9,7 0 1 1 21,12 A9,7 0 1 1 3,12 Z";

    protected override Shape CreateShape() => new Ellipse();

    protected override void UpdateShape(Shape shape, Point start, Point end)
        => SetBounds(shape, start, end);
}