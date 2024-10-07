using Game;
using Godot;
using System;
using System.Linq;

public class MusicManager : Node
{
    private ToadsManager _toadsManager;
    private AudioStreamPlayer _bass;
    private AudioStreamPlayer _melody;
    public override void _Ready()
    {
        _bass = this.GetNode<AudioStreamPlayer>("Bass");
        _melody = this.GetNode<AudioStreamPlayer>("Melody");
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
    
    public void _on_Game_LevelFinished()
    {
        _bass.Stop();
        _melody.Stop();
    }

    public void _on_Game_StartLevel(int level)
    {
        _bass.Play();
        _melody.Play();
    }
}
