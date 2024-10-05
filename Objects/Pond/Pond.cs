using Game;
using Godot;
using System;

public class Pond : Area2D
{
    public void _on_Pond_body_entered(Node body)
    {
        if(body is Toad toad)
        {
            toad.TriggerArrival(this);
        }
    }
}
