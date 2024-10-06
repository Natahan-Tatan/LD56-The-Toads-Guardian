using Godot;
using System;

namespace Game
{
    [Tool]
    public class Rank : TextureRect
    {
        public const int BADGE_SIZE = 64;
        public enum Type: int
        {
            S = 0,
            A,
            B,
            C,
            F
        }

        [Export]
        public Type RankType
        {
            get => _rankType;
            set
            {
                _rankType = value;

                if(_texture != null)
                {
                    _texture.Region = new Rect2(
                        x:BADGE_SIZE * (int)_rankType,
                        y: 0,
                        width: BADGE_SIZE,
                        height: BADGE_SIZE
                    );
                }
            }
        }
        public Type _rankType = Type.S;

        private AtlasTexture _texture;

        public override void _Ready()
        {
            base._Ready();

            _texture = this.Texture as AtlasTexture;

            // Trigger texture rect change
            RankType = RankType;
        }
    }

}