using Godot;
using System;

namespace Game
{
    public class PurpleToad : Toad
    {
        protected override void _EntityEntered(Node entity, bool near)
        {
            if(entity is Player player && (_currentState == State.IDLE || _currentState == State.WANDERING))
            {
                _currentFollower = player;
                _currentState = State.FOLLOWING;

                if(OS.IsDebugBuild())
                {
                    GD.Print($"{this.Name} starts FOLLOWING {player.Name} !");
                }
            }
        }

        protected override void _EntityExited(Node entity, bool near)
        {
            if(_currentState == State.FOLLOWING && _currentFollower == entity)
            {
                if(OS.IsDebugBuild())
                {
                    GD.Print($"{this.Name} STOPS following  {_currentFollower.Name} !");
                }

                _currentFollower = null;
                _currentState = State.IDLE;
            }
        }
    }

}