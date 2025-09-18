using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Objects;


public interface IShape : ISfmlObject
{
    Shape SfmlShape { get; }

    float BorderSize { get; set; }
    ColorRGBA Color { get; set; }
    ColorRGBA BorderColor { get; set; }
}
