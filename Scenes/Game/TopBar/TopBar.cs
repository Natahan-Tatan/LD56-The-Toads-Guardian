using Godot;
using System;

namespace Game
{
    public class TopBar : PanelContainer
    {
#region Signals
#endregion

#region Exported Properties
#endregion

#region Public Properties
#endregion

#region Internal Properties
#endregion

#region Public Methods
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_Game_StartLevel(int level)
        {
            this.GetNode<Label>("HBoxContainer/LevelLabel").Text = $"Level {level}";
        }
#endregion
    }

}