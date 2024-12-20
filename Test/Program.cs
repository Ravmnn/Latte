﻿using SFML.Window;

using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Test;


class Program
{
    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElement(new ButtonElement(null, new(), new(30, 30), "Btn")
        {
            Alignment = { Value = Alignments.Center }
        });


    static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 16
        });

        WindowElement rect = new("this is a text", new(), new(600, 400))
        {
            Title =
            {
                Color = { Value = new(0, 0, 0, 0)}
            },
            
            Color = { Value = new(255, 255, 255) }
        };


        _ = new CheckBox(rect, new())
        {
            Alignment = { Value = Alignments.Center }
        };
        
        App.AddElement(rect);
        
        
        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}