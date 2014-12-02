using DXGame.Core.Components;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator : ObjectGenerator<GameObject>
    {
        private readonly PositionalComponent position_;
        private readonly SimpleSpriteComponent sprite_;
        private readonly SimplePlayerInputComponent input_;
        private readonly PhysicsComponent physics_;

        private const string PLAYER = "Player";
        private const float MAX_VELOCITY = 10.0f;

        public PlayerGenerator(int x, int y)
        {
            position_ = new PositionalComponent().WithCoordinates(x, y);
            physics_ = new PhysicsComponent().WithMaxVelocity(MAX_VELOCITY).WithPosition(position_);
            sprite_ = new SimpleSpriteComponent().WithAsset(PLAYER).WithPosition(position_);
            input_ = new SimplePlayerInputComponent().WithPhysics(physics_);
        }

        public override GameObject Generate()
        {
            GameObject player = new GameObject();
            player.AttachComponents(position_, physics_, sprite_, input_);
            return player;
        }
    }
}