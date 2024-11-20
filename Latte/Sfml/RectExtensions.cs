using SFML.Graphics;

using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Sfml;


public static class RectExtensions
{
    public static IntRect ToWindowCoordinates(this FloatRect rect)
    {
        Vec2i transformedPosition = App.MainWindow.MapCoordsToPixel(rect.Position);
        Vec2i transformedSize = App.MainWindow.MapCoordsToPixel(rect.Position + rect.Size) - transformedPosition;
        
        return new(transformedPosition, transformedSize);
    }


    public static FloatRect ToWorldCoordinates(this IntRect rect)
    {
        Vec2f transformedPosition = App.MainWindow.MapPixelToCoords(rect.Position);
        Vec2f transformedSize = App.MainWindow.MapPixelToCoords(rect.Position + rect.Size) - transformedPosition;
        
        return new(transformedPosition, transformedSize);
    }
}