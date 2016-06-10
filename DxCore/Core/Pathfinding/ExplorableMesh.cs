using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Map;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Pathfinding
{
    /**
        <summary>
            Maintains the known Paths between NavigableSurfaceNodes. This is constructed on-the-fly in 
            order to do A* pathfinding (where we do not know the directional links between Nodes, but can find them)
        </summary>
    */
    [Serializable]
    [DataContract]
    public class ExplorableMesh
    {
        private readonly Dictionary<NavigableSurface.Node, HashSet<Path>> paths_ = new Dictionary<NavigableSurface.Node, HashSet<Path>>();

        public IEnumerable<NavigableSurface.Node> Nodes => paths_.Keys;

        public NavigableSurface NavigableSurface
        {
            get;
        }

        public HashSet<NavigableSurface.Node> Exhausted
        {
            get;
        }
 
        public ExplorableMesh(NavigableSurface surface)
        {
            Validate.IsNotNull(surface, this.GetFormattedNullOrDefaultMessage(surface));
            foreach(NavigableSurface.Node node in surface.NodeQuery.Elements)
            {
                paths_[node] = new HashSet<Path>();
            }
            NavigableSurface = surface;
            Exhausted = new HashSet<NavigableSurface.Node>();
        }

        public void AttachPath(NavigableSurface.Node start, Path path)
        {
            Validate.IsNotNull(start);
            Validate.IsNotNull(path);
            if(Objects.Equals(start, NavigableSurface.Node.EmptyNode))
            {
                return;
            }
            HashSet<Path> pathsFromStart = paths_[start];
            pathsFromStart.Add(path);
        }

        /**
             <summary>
                 Retrieves all known Paths for the provided node.
                 Note: The returned List will never be null, only (potentially) empty
             </summary>
         */
        public List<Path> PathsFrom(NavigableSurface.Node start)
        {
            Validate.IsNotNull(start);
            if(Objects.Equals(start, NavigableSurface.Node.EmptyNode))
            {
                return Enumerable.Empty<Path>().ToList();
            }
            return paths_[start].ToList();
        }

        /**
            <summary>
                Retrieves all of the CommandChains that can still be attempted (have yet to be explored) for the provided node
            </summary>
        */
        public List<CommandChain> AvailableCommandChains(NavigableSurface.Node start)
        {
            HashSet<CommandChain> exhausted = paths_[start].Select(path => path.Directions).Distinct().ToHashSet();
            if(!exhausted.Any())
            {
                return PathfindingConstants.AvailableCommandments.ToList();
            }
            return PathfindingConstants.AvailableCommandments.Except(exhausted).ToList();
        }

    }
}
