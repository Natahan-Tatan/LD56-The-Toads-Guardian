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
    #endregion

    #region Exported Properties
    #endregion

    #region Public Properties
    #endregion

    #region Internal Properties
        private TileMap _map;
    #endregion

    #region Public Methods
        public override void _Ready()
        {
            _map = this.GetNode<TileMap>("%Map");

            // Mostly use for tests. In normal game, no Toads are present in the map before first spawning
            foreach(var toad in this.GetChildren().OfType<Toad>())
            {
                toad.Map = _map;
            }

            foreach(var egg in _map.GetChildren().OfType<Egg>())
            {
                egg.Connect(nameof(Egg.ToadSpawning), this, nameof(_on_Egg_ToadSpawning));
            }
        }
    #endregion

    #region Internal Methods
    #endregion

    #region Signals Hooks
        public void _on_Egg_ToadSpawning(IEnumerable<Toad> toads)
        {
            foreach(var toad in toads)
            {
                toad.Map = _map;
                this.AddChild(toad);
            }
        }    
    #endregion
    }
}
