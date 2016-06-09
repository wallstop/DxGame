using System.Collections.Generic;
using System.Threading;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Map;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Pathfinding
{
    public class ExplorableMeshCache
    {
        private static readonly ThreadLocal<Dictionary<ImmutablePair<EntityType, NavigableSurface>, ExplorableMesh>> CACHE = 
            new ThreadLocal<Dictionary<ImmutablePair<EntityType, NavigableSurface>, ExplorableMesh>>(() => new Dictionary<ImmutablePair<EntityType, NavigableSurface>, ExplorableMesh>());

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
            EntityTypeComponent entityTypeComponent = entity.ComponentOfType<EntityTypeComponent>();
            EntityType entityType = entityTypeComponent.EntityType;

            var key = new ImmutablePair<EntityType, NavigableSurface>(entityType, surface);
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
