using System.Windows;
using System.Windows.Controls;
using PaintApp.Tools;

namespace PaintApp;
public class ToolManager
{
    private DrawingTool _activeTool;

    public ToolManager(Canvas canvas, DrawingSettings settings)
    {
        Tools = new List<DrawingTool>
        {
            new PencilTool(canvas, settings),
            new LineTool(canvas, settings),
            new RectangleTool(canvas, settings),
            new EllipseTool(canvas, settings)
        };
        _activeTool = Tools[0];
    }

    public IReadOnlyList<DrawingTool> Tools { get; }

    public DrawingTool ActiveTool
    {
        get => _activeTool;
        set => _activeTool = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void MouseDown(Point p) => _activeTool.OnMouseDown(p);
    public void MouseMove(Point p) => _activeTool.OnMouseMove(p);
    public void MouseUp(Point p) => _activeTool.OnMouseUp(p);
}