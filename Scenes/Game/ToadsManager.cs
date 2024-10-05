using Game;
using Godot;
using System;
using System.Linq;

public class ToadsManager : Node2D
{
    private TileMap _map;
    public override void _Ready()
    {
        _map = this.GetNode<TileMap>("%Map");

        foreach(var toad in this.GetChildren().OfType<Toad>())
        {
            toad.Map = _map;
        }
    }
}
