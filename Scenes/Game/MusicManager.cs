using Game;
using Godot;
using System;
using System.Linq;

public class MusicManager : Node
{
    private ToadsManager _toadsManager;
    private AudioStreamPlayer _bass;
    public override void _Ready()
    {
        _bass = this.GetNode<AudioStreamPlayer>("Bass");
        _toadsManager = this.GetNode<ToadsManager>("../Scene/ToadsManager");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if(_toadsManager.GetChildren().OfType<Toad>().Any(t => t.CurrentState == Toad.State.FOLLOWING || t.CurrentState == Toad.State.ARRIVED))
        {
            _bass.VolumeDb = 0;
        }
        else
        {
            _bass.VolumeDb = -80;
        }
    }

}
