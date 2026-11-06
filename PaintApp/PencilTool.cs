using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintApp.Tools;

public class PencilTool : DrawingTool
{
    private Polyline? _line;

    public PencilTool(Canvas canvas, DrawingSettings settings) : base(canvas, settings) { }

    public override string Name => "Карандаш";
    public override string IconData => "M4,20 L5,15 L16,4 L20,8 L9,19 Z M14,6 L18,10";

    public override void OnMouseDown(Point position)
    {
        _line = new Polyline
        {
            Stroke = Settings.CreateBrush(),
            StrokeThickness = Settings.Thickness,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };

        _line.Points.Add(position);
        _line.Points.Add(position);
        Canvas.Children.Add(_line);
    }

    public override void OnMouseMove(Point position)
    {
        _line?.Points.Add(position);
    }

    public override void OnMouseUp(Point position)
    {
        _line?.Points.Add(position);
        _line = null;
    }
}