using DXGame.Core.Map;
using DXGame.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXGame.Core.Pathfinding
{
    public class ExplorableMeshCache
    {
        private static readonly ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>> CACHE = 
            new ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>>(() => new Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>());

        public static ExplorableMesh MeshFor(GameObject entity, NavigableSurface surface)
        {
            Validate.IsNotNullOrDefault(entity,
                $"Cannot retrieve an {typeof(ExplorableMesh)} for a null {nameof(entity)}");
            Validate.IsNotNullOrDefault(surface,
                $"Cannot retrieve an {typeof(ExplorableMesh)} for a null {surface.GetType()}");
            return PopulateOrRetrieveMesh(entity, surface);
        }

        private static ExplorableMesh PopulateOrRetrieveMesh(GameObject entity, NavigableSurface surface)
        {
            var cachedNavigationMesh = CACHE.Value;
            var entityId = entity.Id;
            var key = new ImmutablePair<UniqueId, NavigableSurface>(entityId, surface);
            if(cachedNavigationMesh.ContainsKey(key))
            {
                return cachedNavigationMesh[key];
            }

            var newExplorableMesh = new ExplorableMesh(surface);
            cachedNavigationMesh[key] = newExplorableMesh;
            return newExplorableMesh;
        }
    }
}
