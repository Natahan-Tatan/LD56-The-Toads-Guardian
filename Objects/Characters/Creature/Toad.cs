using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{

    public abstract class Toad : KinematicBody2D
    {
#region Signals
        [Signal]
        public delegate void Arrived(Toad toad);
        [Signal]
        public delegate void Died(Toad toad, Node killer);

        public TileMap Map {get;set;}
#endregion

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
            ARRIVED,
            DIED
        }

#region Internal Properties
        protected Random _rand = new Random();
        protected State _currentState
        {
            get => _currentStateRaw;
            set
            {
                if(value != _currentStateRaw)
                {
                    _currentStateRaw = value;
                    this._UpdateAnimations();
                }
            }
        }

        private bool _isReady = false;
        protected State _currentStateRaw = State.IDLE;
        protected Node2D _currentFollower = null;
        protected Vector2 _currentDirection;
        protected Timer _wanderingTimer;
        protected AnimatedSprite _bodyAnimations;
        protected AnimatedSprite _eyesAnimations;
        protected CollisionShape2D _nearShape;
        protected Area2D _sensor;

#endregion

#region Public Methods
        public void TriggerDead(Node killer)
        {
            if(_currentState != State.DIED && _currentState != State.ARRIVED)
            {
                GD.Print($"☠️ {killer.Name} killed {this.Name} !");
                _currentState = State.DIED;

                // We disable all collisions
                _sensor.SetDeferred("monitoring", false);
                _sensor.SetDeferred("monitorable", false);
                this.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
                this.ZIndex = -1;

                EmitSignal(nameof(Died), this, killer);
            }
        }
        public void TriggerArrival(Pond pond)
        {
            if(_currentState != State.ARRIVED && _currentState != State.DIED)
            {
                _currentDirection = (pond.GlobalPosition - this.GlobalPosition).Normalized();
                this.LookAt(this.GlobalPosition + _currentDirection);
                this.RotationDegrees += 90;

                _currentState = State.ARRIVED;
                this.GetTree().CreateTimer(1.5f).Connect("timeout", this, nameof(_on_ArrivedTimer_timeout));
            }
        }
        
        public override void _Ready()
        {
            base._Ready();

            _wanderingTimer = this.GetNode<Timer>("WanderingTimer");
            _bodyAnimations = this.GetNode<AnimatedSprite>("Body");
            _eyesAnimations = this.GetNode<AnimatedSprite>("Eyes");
            _nearShape = this.GetNode<CollisionShape2D>("Sensor/NearCircle");
            _sensor = this.GetNode<Area2D>("Sensor");

            _isReady = true;
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
                    case State.ARRIVED:
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
                    switch(_currentState)
                    {
                        case State.WANDERING:
                            _wanderingTimer.Stop();
                            _on_WanderingTimer_timeout();
                        break;
                    }
                }
            }
        }
#endregion

#region Internal methods

        protected virtual void _UpdateAnimations()
        {
            if(_isReady)
            {
                switch(_currentState)
                {
                    case State.IDLE:
                        _bodyAnimations.Animation = "idle";
                        _eyesAnimations.Animation = "opening";
                    break;

                    case State.FOLLOWING:
                    case State.WANDERING:
                        _bodyAnimations.Animation = "move";
                        _eyesAnimations.Animation = "opening";
                    break;

                    case State.FLEEING:
                        _bodyAnimations.Animation = "move";
                        _eyesAnimations.Animation = "afraid";
                    break;

                    case State.DIED:
                        _bodyAnimations.Animation = "dead";
                        _eyesAnimations.Visible = false;
                    break;
                }
            }
        }

        protected virtual void _EntityEntered(Node entity, bool near)
        {
            if(entity is Player player && (_currentState == State.IDLE || _currentState == State.WANDERING))
            {
                _currentFollower = player;
                _currentState = State.FOLLOWING;
            }
        }

        protected virtual void _EntityExited(Node entity, bool near)
        {
            if(_currentState == State.FOLLOWING && _currentFollower == entity)
            {
                _currentFollower = null;
                _currentState = State.IDLE;
            }
        }

#endregion

#region Signals Hooks
        public void _on_Sensor_body_shape_entered(RID bodyRID, Node body, int bodyShapeIndex, int localShapeIndex)
        {
            this._EntityEntered(
                entity: body,
                near: _sensor.ShapeOwnerGetOwner(_sensor.ShapeFindOwner(localShapeIndex)) == _nearShape 
            );
        }

        public void _on_Sensor_area_shape_entered(RID areaRID, Area2D area, int areaShapeIndex, int localShapeIndex)
        {
            this._EntityEntered(
                entity: area,
                near: _sensor.ShapeOwnerGetOwner(_sensor.ShapeFindOwner(localShapeIndex)) == _nearShape 
            );
        }

        public void _on_Sensor_body_shape_exited(RID bodyRID, Node body, int bodyShapeIndex, int localShapeIndex)
        {
            this._EntityEntered(
                entity: body,
                near: _sensor.ShapeOwnerGetOwner(_sensor.ShapeFindOwner(localShapeIndex)) == _nearShape 
            );
        }

        public void _on_Sensor_area_shape_exited(RID areaRID, Area2D area, int areaShapeIndex, int localShapeIndex)
        {
            this._EntityEntered(
                entity: area,
                near: _sensor.ShapeOwnerGetOwner(_sensor.ShapeFindOwner(localShapeIndex)) == _nearShape 
            );
        }

        public void _on_Body_animation_finished()
        {
            if(_currentState == State.ARRIVED && _bodyAnimations.Animation == "arrived")
            {
                EmitSignal(nameof(Arrived), this);
                QueueFree();
            }
        }

        public void _on_WanderingTimer_timeout()
        {
            _currentDirection = Vector2.Zero;
            _currentState = State.IDLE;
        }

        public void _on_ArrivedTimer_timeout()
        {
            _currentDirection = Vector2.Zero;
            _bodyAnimations.Animation = "arrived";
        }
#endregion
    }
}
