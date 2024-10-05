using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{

    public class Toad : KinematicBody2D
    {
#region Exported Properties
        [Export]
        public float FollowingSpeed {get;set;} = 5f;
        [Export]
        public float FleeSpeed {get;set;} = 10f;
#endregion
        public enum State: int
        {
            IDLE = 0,
            WANDERING,
            FLEEING,
            FOLLOWING,
            
            DIED
        }

#region Internal Properties
        protected Random _rand = new Random();
        protected State _currentState = State.IDLE;
        protected Node2D _currentFollower = null;
        protected Vector2 _currentDirection;
        protected Timer _wanderingTimer;
        //protected IList<Node2D> _currentEnnemies {get; set;} = new List<Node2D>();

        #endregion

        public override void _Ready()
        {
            base._Ready();

            _wanderingTimer = this.GetNode<Timer>("WanderingTimer");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            switch(_currentState)
            {
                case State.FOLLOWING:
                    _currentDirection = (_currentFollower.GlobalPosition - this.GlobalPosition).Normalized();

                    this.LookAt(this.GlobalPosition + _currentDirection);
                    this.RotationDegrees += 90;
                break;

                case State.IDLE:
                    if(_rand.Luck(1,20))
                    {                      
                        _currentDirection = _rand.NextVector2(1).Normalized();

                        if(OS.IsDebugBuild())
                        {
                            GD.Print($"{this.Name} starts WANDERING toward {_currentDirection}!");
                        }

                        this.LookAt(this.GlobalPosition + _currentDirection);
                        this.RotationDegrees += 90;

                        _currentState = State.WANDERING;

                        _wanderingTimer.WaitTime = (float)_rand.NextDouble() * 3f;
                    }
                break;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if(_currentState < State.DIED)
            {
                var move = _currentDirection * 10;

                switch(_currentState)
                {
                    case State.FOLLOWING:
                    case State.WANDERING:
                        move *= FollowingSpeed;
                    break;

                    case State.FLEEING:
                        move *= FleeSpeed;
                    break;
                }

                this.MoveAndSlide(move);

                var lastCollision = this.GetLastSlideCollision();
                if(lastCollision != null)
                {
                    if(_currentState == State.WANDERING)
                    {
                        _wanderingTimer.Stop();
                        _on_WanderingTimer_timeout();
                    }
                }
            }
        }
        
        public void _on_WanderingTimer_timeout()
        {
            _currentDirection = Vector2.Zero;
            _currentState = State.IDLE;
            
            if(OS.IsDebugBuild())
            {
                GD.Print($"{this.Name} STOPS wandering !");
            }
        }

#region Internal methods
        protected virtual void _EntityEntered(Node entity)
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

        protected virtual void _EntityExited(Node entity)
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
#endregion
#region Signals Hooks
        public void _on_Sensor_body_entered(Node body)
        {
            this._EntityEntered(body);
        }

        public void _on_Sensor_area_entered(Area2D area)
        {
            this._EntityEntered(area);
        }

        public void _on_Sensor_body_exited(Node body)
        {
            this._EntityExited(body);
        }

        public void _on_Sensor_area_exited(Area2D area)
        {
            this._EntityExited(area);
        }
#endregion
    }
}
