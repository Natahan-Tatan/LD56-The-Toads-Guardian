using Godot;
using System;

namespace Game
{
    public class Game : Node
    {
#region Signals
#endregion

#region Exported Properties
#endregion

#region Public Properties
#endregion

#region Internal Properties
        private Control _pausePanel;

        #endregion

        #region Public Methods
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
#endregion
    }
}
