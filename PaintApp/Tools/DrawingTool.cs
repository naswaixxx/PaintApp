using System.Windows;
using System.Windows.Controls;

namespace PaintApp.Tools;

public abstract class DrawingTool
{
    protected DrawingTool(Canvas canvas, DrawingSettings settings)
    {
        Canvas = canvas;
        Settings = settings;
    }

    protected Canvas Canvas { get; }
    protected DrawingSettings Settings { get; }

    public abstract string Name { get; }

    public abstract string IconData { get; }

    public abstract void OnMouseDown(Point position);
    public abstract void OnMouseMove(Point position);
    public abstract void OnMouseUp(Point position);
}