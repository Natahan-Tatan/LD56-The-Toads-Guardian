using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Game : Node
    {
        public enum State
        {
            STARTING = 0,
            INGAME,
            PAUSE,
            STATS,
            ENDING,
        }
#region Signals
        [Signal]
        public delegate void LevelFinished(int countSpawned, int countArrived, int CountDead);
        [Signal]
        public delegate void StartLevel(int level);
#endregion

#region Exported Properties
        [Export]
        public int TestLevel {get; set;} = 0;
        [Export]
        public List<PackedScene> LevelsList {get;set;}
#endregion

#region Public Properties
        public State CurrentState { get => _currentState;}
        private State _currentState = State.STARTING;
#endregion

#region Internal Properties
        private Control _pausePanel;
        private Control _startLevelPanel;
        private EndPanel _endLevelPanel;
        private int _remainingToads = 0;
        private int _countArrived = 0;
        private int _countSpawned = 0;
        private int _countDead = 0;
        private bool _spawnFinished = false;

        private int _currentLevel = 0;

#endregion

#region Public Methods
        public override void _Ready()
        {
            base._Ready();

            _pausePanel = GetNode<Control>("%PausePanel");
            _startLevelPanel = GetNode<Control>("%StartLevelPanel");
            _endLevelPanel = GetNode<EndPanel>("%EndPanel");

            if(OS.HasFeature("editor"))
            {
                _currentLevel = TestLevel;
            }
            else
            {
                _currentLevel = 0;
            }

            this.LoadLevel(_currentLevel);
        }
        public void NextLevel()
        {
            this.LoadLevel(_currentLevel + 1);
        }

        public void RestartLevel()
        {
            this.LoadLevel(_currentLevel);
        }

        public void LoadLevel(int level)
        {
            if(level >= LevelsList.Count())
            {
                return;
            }

            _currentState = State.STARTING;
            _spawnFinished = false;
            _remainingToads = _countArrived = _countSpawned = _countDead;

            // Remove current map
            var scene = this.GetNode<Node>("Scene");
            scene.RemoveChild(scene.GetNode("Map"));

            // Load new map
            var map = LevelsList[level].Instance<Map>();
            scene.AddChild(map);
            scene.MoveChild(map, 0);

            _currentLevel = level;
            
            EmitSignal(nameof(StartLevel), level);
            
            this.GetTree().Paused = true;
            _startLevelPanel.Visible = true;
        }
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            switch(_currentState)
            {
                case State.STARTING:
                    if(@event is InputEventJoypadButton || @event is InputEventKey)
                    {
                        if(@event.IsPressed())
                        {
                            _startLevelPanel.Visible = false;
                            this.GetTree().Paused = false;
                            _currentState = State.INGAME;
                        }
                    }
                break;

                case State.INGAME:
                case State.PAUSE:
                    if(@event.IsActionPressed("Pause"))
                    {
                        this.GetTree().Paused = !this.GetTree().Paused;
                        _pausePanel.Visible = this.GetTree().Paused;

                        _currentState = this.GetTree().Paused ? State.PAUSE : State.INGAME;  
                    }
                    else if(@event.IsActionPressed("Restart"))
                    {
                        this.RestartLevel();
                    }
                break;

                case State.ENDING:
                    if(@event is InputEventJoypadButton || @event is InputEventKey)
                    {
                        if(@event.IsPressed())
                        {
                            if(_endLevelPanel.Pass)
                            {
                                this.NextLevel();
                            }
                            else
                            {
                                this.RestartLevel();
                            }
                        }
                    }
                break;
            }
        }
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_ToadsManager_ToadsSpawnFromEgg(IEnumerable<Toad> toads)
        {
            _remainingToads += toads.Count();

            _countSpawned += toads.Count();
        }

        public void _on_ToadsManager_ToadExitGame(Toad toad)
        {
            _remainingToads--;

            if(toad.IsAlive)
            {
                _countArrived++;
            }
            else
            {
                _countDead++;
            }

            if(_remainingToads <= 0 && _spawnFinished)
            {
                _currentState = State.STATS;
                EmitSignal(nameof(LevelFinished), _countSpawned, _countArrived, _countDead);
            }
        }

        public void _on_ToadsManager_AllEggsFinishedSpawning()
        {
            _spawnFinished = true;
        }

        public void _on_EndPanelAnimationPlayer_animation_finished(string animName)
        {
            if(animName == "Unveilling")
            {
                _currentState = State.ENDING;
            }
        }
#endregion
    }
}
