# Graphite

Graphite is the simple 2D graphics drawing library, primarily for rendering user interface elements.

It is entirely possible that this early interface will get reworked once most of the functionality is fleshed out
and a more sensible interface can be settled on.  This will hopefully help give you an idea of what is currently
possible.

## Setup
The first basic thing is to get a 2D context; this should be created once and reused on each render loop.

The context will set up the graphics pipeline for processing 2D drawing commands.

```csharp
public class MyGameApp : IGameApp
{
    private readonly IApiLayer m_apiLayer;
    private readonly Context? m_context;

    public MyGameApp(IApiLayer apiLayer)
    {
        m_apiLayer = apiLayer;
    }

    virtual void Dispose()
    {
        m_context?.Dispose();
    }

    // ...

    public void OnLoad()
    {
        m_context = new Context(m_apiLayer);
    }

    public void OnRender()
    {
        // Draw your frame here.

        m_context!.Render(); // Flush all 2D drawing commands.
    }

    // ...
}
```

## Drawing Images
Basic image drawing can be done; though currently there there is no tested functionality for loading images.  Textures
will need to be written to manually.

```csharp
// Create a new texture that is 64 pixels square with 32 bits per pixel.
ITextureObject texture = m_apiLayer.GetTextureObject(PixelFormat.FormatR8G8B8A8, new Point(64, 64));

texture.Bitmap.Data[0] = 255;
// Set bitmap data here....

texture.Bitmap.Invalidate(); // Invalidate bitmap so texture will update.

m_context.DrawImage(texture, new Vector2(100, 100));
```

## Drawing Paths
You can create a graphical path and use it to draw some complex shape outlines including lines, rectangles, Bézier curves, and simple elliptical arcs.

(Filled shapes are not yet supported.)

The following example shows drawing a full circle at coordinates (300, 500) with a radius of 50 units.
```csharp
var path = new Tokamak.Graphite.Path();
path.ArcTo(new Vector2(300, 500), 50, 0, MathF.Tau);

var pen = new Pen
{
    Color = Color.DarkRed,
    Width = 10
};

m_context.Stroke(path, pen);
```

Currently pens only support setting their width and color.  Other line styles, end caps, joins, etc. are not yet implemented.
