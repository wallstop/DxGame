using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    public enum CollidableDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    public class CollidableComponent : SpatialComponent
    {
        public List<CollidableDirection> CollidableDirections { get; } = new List<CollidableDirection>();

        public CollidableComponent(DxGame game)
            : base(game)
        {
        }

        public CollidableComponent WithCollidableDirections(IEnumerable<CollidableDirection> directions)
        {
            var collidableDirections = directions as IList<CollidableDirection> ?? directions.ToList();
            Validate.IsNotEmpty(collidableDirections,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(directions)));
            CollidableDirections.Clear();
            CollidableDirections.AddRange(collidableDirections);
            return this;
        }
    }
}