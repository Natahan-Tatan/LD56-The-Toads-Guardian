using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Game : Node
    {
#region Signals
        [Signal]
        public delegate void LevelFinished(int countSpawned, int countArrived, int CountDead);
        [Signal]
        public delegate void StartLevel(int level, string levelName);
#endregion

#region Exported Properties
#endregion

#region Public Properties
#endregion

#region Internal Properties
        private Control _pausePanel;
        private EndPanel _endPanel;
        private int _remainingToads = 0;
        private int _countArrived = 0;
        private int _countSpawned = 0;
        private int _countDead = 0;
        private bool _spawnFinished = false;

        private int _currentLevel = 1;

#endregion

#region Public Methods
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
            _spawnFinished = false;
            _remainingToads = _countArrived = _countSpawned = _countDead;

            //TODO Load Level
            
            //TODO get level name
            EmitSignal(nameof(StartLevel), _currentLevel, "TODO map name");
        }

        public override void _Ready()
        {
            base._Ready();

            _pausePanel = GetNode<Control>("%PausePanel");
        }
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if(@event.IsActionPressed("Pause"))
            {
                this.GetTree().Paused = !this.GetTree().Paused;
                _pausePanel.Visible = this.GetTree().Paused;
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
                EmitSignal(nameof(LevelFinished), _countSpawned, _countArrived, _countDead);
            }
        }

        public void _on_ToadsManager_AllEggsFinishedSpawning()
        {
            _spawnFinished = true;
        }
#endregion
    }
}
