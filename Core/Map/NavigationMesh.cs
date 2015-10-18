using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class NavigationMesh
    {
        private static readonly int MESH_PIXEL_SPACING = 2;

        private static ThreadLocal<Dictionary<UniqueId, NavigationMesh>> CACHE =
            new ThreadLocal<Dictionary<UniqueId, NavigationMesh>>(() => new Dictionary<UniqueId, NavigationMesh>());

        public class Node
        {
            public DxVector2 Position { get; }
            public Dictionary<Direction, Node> Links { get; } = new Dictionary<Direction, Node>();

            public Node(DxVector2 position)
            {
                Position = position;
            }

            public override int GetHashCode()
            {
                return Objects.HashCode(Position, Links);
            }

            public override bool Equals(object other)
            {
                var node = other as Node;
                if (ReferenceEquals(null, node))
                {
                    return false;
                }
                return node.Position == Position && Objects.Equals(Links, node.Links);
            }
        }

        private ReadOnlyCollection<Node> Nodes { get; }
        private ICollisionTree<Node> NodeQuery { get; }


        public NavigationMesh(MapModel mapModel)
        {
            Validate.IsNotNullOrDefault(mapModel, StringUtils.GetFormattedNullOrDefaultMessage(this, mapModel));

            var nodes = BuildMesh(mapModel);
            Nodes = new ReadOnlyCollection<Node>(Nodes);
        }

        private static NavigationMesh PopulateOrRetrieveMesh(MapModel mapModel)
        {
            var cachedNavigationMesh = CACHE.Value;
            var mapId = mapModel.Map.Id;
            if (cachedNavigationMesh.ContainsKey(mapId))
            {
                return cachedNavigationMesh[mapId];
            }

            // TODO
            return null;

        }

        private static List<Node> BuildMesh(MapModel mapModel)
        {
            var map = mapModel.Map;
            var space = map.MapDescriptor.Size * map.MapDescriptor.Scale;


            return null;
        }

    }
}
