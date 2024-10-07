using Godot;
using System;

namespace Game
{
    public class Menu : Node
    {

        public override void _Ready()
        {
            GetTree().Paused = true;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if(@event is InputEventJoypadButton || @event is InputEventKey)
            {
                if(@event.IsPressed())
                {
                    this.GetTree().ChangeScene("res://Scenes/Game/Game.tscn");
                }
            }
        }
    }

}