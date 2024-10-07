using Game;
using Godot;
using System;
using System.Linq;

public class MusicManager : Node
{
    private ToadsManager _toadsManager;
    private AudioStreamPlayer _bass;
    private AudioStreamPlayer _melody;
    private bool _levelEnd = false;
    public override void _Ready()
    {
        _bass = this.GetNode<AudioStreamPlayer>("Bass");
        _melody = this.GetNode<AudioStreamPlayer>("Melody");
        _toadsManager = this.GetNode<ToadsManager>("../Scene/ToadsManager");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if(!_levelEnd)
        {
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
    
    public void _on_Game_LevelFinished(int countSpawned, int countArrived, int CountDead)
    {
        _levelEnd = true;
        var tween = this.GetTree().CreateTween();
        _bass.VolumeDb = 0;
        tween.TweenProperty(_bass, "volume_db", -80, 1f);
        tween.TweenProperty(_melody, "volume_db", -80, 1f);

        tween.Connect("finished", this, nameof(_on_MusicTween_Finished));
    }

    public void _on_Game_StartLevel(int level)
    {
        _levelEnd = false;
        _bass.Play();
        _melody.Play();
    }

    public void _on_MusicTween_Finished()
    {
        if(_levelEnd)
        {
            _bass.Stop();
            _melody.Stop();     
        }
        _bass.VolumeDb = _melody.VolumeDb = 0;
    }
}
