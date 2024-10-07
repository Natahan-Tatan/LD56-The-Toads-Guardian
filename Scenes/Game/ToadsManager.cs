using Game;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class ToadsManager : Node
    {
#region Signals
        [Signal]
        public delegate void ToadsSpawnFromEgg(IEnumerable<Toad> toads);
        [Signal]
        public delegate void ToadExitGame(Toad toad);
        [Signal]
        public delegate void AllEggsFinishedSpawning();
#endregion

#region Exported Properties
#endregion

#region Public Properties
#endregion

#region Internal Properties
        private TileMap _map;
        private int _eggsRemaining = 0;
#endregion

#region Public Methods
        public override void _Ready()
        {
            // Init is made in _on_Game_StartLevel, triggered when level is
        }
#endregion

#region Internal Methods
    #endregion

#region Signals Hooks
        public void _on_Game_StartLevel(int level)
        {
            this.RemoveAllChildren();
            _eggsRemaining = 0;

            _map = this.GetNode<TileMap>("../Map");

            foreach(var egg in _map.GetChildren().OfType<Egg>())
            {
                egg.Connect(nameof(Egg.ToadSpawning), this, nameof(_on_Egg_ToadSpawning));
                egg.Connect(nameof(Egg.SpawningFinished), this, nameof(_on_Egg_SpawningFinished));

                _eggsRemaining++;
            }
        }
        public void _on_Egg_ToadSpawning(IEnumerable<Toad> toads)
        {
            foreach(var toad in toads)
            {
                toad.Map = _map;

                toad.Connect(nameof(Toad.Died), this, nameof(_on_Toad_DiedOrArrived));
                toad.Connect(nameof(Toad.Arrived), this, nameof(_on_Toad_DiedOrArrived), new Godot.Collections.Array(this));

                this.AddChild(toad);
            }

            EmitSignal(nameof(ToadsSpawnFromEgg), toads);
        } 

        public void _on_Egg_SpawningFinished(Egg egg)
        {
            _eggsRemaining--;

            if(_eggsRemaining <= 0)
            {
                EmitSignal(nameof(AllEggsFinishedSpawning));
            }
        }

        public void  _on_Toad_DiedOrArrived(Toad toad, Node killer)
        {
            EmitSignal(nameof(ToadExitGame), toad);
        }
#endregion
    }
}
