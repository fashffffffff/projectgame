using Godot;
using System;

public partial class Button : Godot.Button
{
    // Called when the node enters the scene tree for the first time.
    private void _on_pressed()
    {
        GetTree().ChangeSceneToFile("res://Level1.tscn");
    }
}
