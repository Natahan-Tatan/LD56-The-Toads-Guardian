using Game;
using Godot;
using System;

public class Pond : Area2D
{
    private AudioStreamPlayer2D _toadArrivedSnd;
    public override void _Ready()
    {
        base._Ready();

        _toadArrivedSnd = this.GetNode<AudioStreamPlayer2D>("ToadArrived");
    }
    public void _on_Pond_body_entered(Node body)
    {
        if(body is Toad toad)
        {
            _toadArrivedSnd.Play();
            toad.TriggerArrival(this);
        }
    }
}
