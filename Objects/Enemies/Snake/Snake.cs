using Godot;
using System;

namespace Game
{
    public class Snake : KinematicBody2D
    {
        public enum State: int
        {
            IDLE,
            WANDERING,
            CHASING,
        }
#region Signals
#endregion

#region Exported Properties
        [Export]
        public float Speed {get;set;} = 7f;
#endregion

#region Public Properties
        public State CurrenState { get => _currentState; }
        private State _currentState = State.IDLE;
#endregion

#region Internal Properties
        private Timer _wanderingTimer;
        private AnimatedSprite _animations;
        private Toad _prey;
        private Vector2 _currentDirection;
        private Random _rand = new Random();
#endregion

#region Public Methods
        public override void _Ready()
        {
            base._Ready();

            _wanderingTimer = GetNode<Timer>("WanderingTimer");
            _animations = GetNode<AnimatedSprite>("AnimatedSprite");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            switch(_currentState)
            {
                case State.IDLE:
                    _animations.Animation = "idle";
                    if(_rand.Luck(1,10))
                    {
                        _currentDirection = _rand.NextVector2(1).Normalized();

                        _currentState = State.WANDERING;

                        _wanderingTimer.WaitTime = (float)_rand.NextDouble() * 3f;
                    }
                break;

                case State.CHASING:
                    _animations.Animation = "chasing";
                    if(_prey == null || _prey.IsQueuedForDeletion() || _prey.CurrentState == Toad.State.ARRIVED || _prey.CurrentState == Toad.State.DIED)
                    {
                        _prey = null;
                        _currentState = State.IDLE;
                    }
                    else
                    {
                        _currentDirection = (_prey.GlobalPosition - this.GlobalPosition).Normalized();
                    }
                break;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            var move = _currentDirection * 10;

            switch(_currentState)
            {
                case State.WANDERING:
                    move *= Speed * 0.5f;
                break;

                case State.CHASING:
                    move *= Speed;
                break;
            }

            if(move != Vector2.Zero)
            {
                this.LookAt(this.GlobalPosition + _currentDirection);
                this.RotationDegrees += 90;
            }

            this.MoveAndSlide(move);

            var lastCollision = this.GetLastSlideCollision();
            if(lastCollision != null)
            {
                switch(_currentState)
                {
                    case State.WANDERING:
                        _wanderingTimer.Stop();
                        _on_WanderingTimer_timeout();
                    break;

                    case State.CHASING:
                        if(lastCollision.Collider == _prey)
                        {
                            _prey.TriggerDead(this);
                            _currentState = State.IDLE;
                        }
                    break;
                }
            }
            
        }
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_WanderingTimer_timeout()
        {
            _currentDirection = Vector2.Zero;
            _currentState = State.IDLE;
        }

        public void _on_Sensor_body_entered(Node body)
        {
            if(_currentState != State.CHASING)
            {
                if(body is Toad toad)
                {
                    _wanderingTimer.Stop();
                    _prey = toad;
                    _currentState = State.CHASING;
                }
            }
        }

        public void _on_Sensor_body_exited(Node body)
        {
            if(_currentState == State.CHASING)
            {
                if(body == _prey)
                {
                    _currentState = State.IDLE;
                    _prey = null;
                }
            }
        }
#endregion
    }
}
