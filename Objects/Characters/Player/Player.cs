using Godot;
using System;

namespace Game
{
    public class Player : KinematicBody2D
    {
        
        [Export(PropertyHint.Range,"1;100;0.1")]
        public float Speed {get;set;} = 8f;

        public enum State: int
        {
            IDLE = 0,
            MOVING,
            DIED
        }

        protected State _currentState = State.IDLE;
        protected Vector2 _currentDirection;

        public override void _Process(float delta)
        {
            base._Process(delta);
            
            // If player is not dead
            if(_currentState < State.DIED) 
            {
                _currentDirection = Input.GetVector("Left", "Right", "Up", "Down", 0.01f);
                
                if(_currentDirection.Length() > 0f)
                {
                    this.LookAt(this.GlobalPosition + _currentDirection);
                    this.RotationDegrees += 90;
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            // If player is not dead
            if(_currentState < State.DIED) 
            {
                this.MoveAndSlide(_currentDirection * Speed * 10);
            }
        }
    }
}
