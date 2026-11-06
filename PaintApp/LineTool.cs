using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintApp.Tools;

public class LineTool : ShapeTool
{
    public LineTool(Canvas canvas, DrawingSettings settings) : base(canvas, settings) { }

    public override string Name => "Линия";
    public override string IconData => "M4,20 L20,4";

    protected override Shape CreateShape() => new Line
    {
        StrokeStartLineCap = PenLineCap.Round,
        StrokeEndLineCap = PenLineCap.Round
    };

    protected override void UpdateShape(Shape shape, Point start, Point end)
    {
        var line = (Line)shape;
        line.X1 = start.X; line.Y1 = start.Y;
        line.X2 = end.X; line.Y2 = end.Y;
    }
}