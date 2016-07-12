using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DxCore.Core.Map;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.Pathfinding
{
    public sealed class NavigableMeshNode
    {
        public DxRectangle Space { get; private set; }

        // TODO: Access control
        public List<NavigableMeshNode> Neighbors { get; private set; }

        public NavigableMeshNode(DxRectangle space)
        {
            Space = space;
            Neighbors = new List<NavigableMeshNode>();
        }
    }

    public sealed class NavigableMesh
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReadOnlyDictionary<TilePosition, NavigableMeshNode> Mesh { get; private set; }
        public ISpatialTree<NavigableMeshNode> QueryableMesh { get; private set; }

        public NavigableMesh(MapDescriptor mapDescriptor)
        {
            Validate.Hard.IsNotNull(mapDescriptor);
            /* Assume map is static, it ain't never gonna change */
            Dictionary<TilePosition, NavigableMeshNode> navigationPoints = GenerateNavigationPoints(mapDescriptor);
            GenerateInternodeLinks(navigationPoints);

            Mesh = new ReadOnlyDictionary<TilePosition, NavigableMeshNode>(navigationPoints);
            QueryableMesh = new QuadTree<NavigableMeshNode>(meshNode => meshNode.Space.Center, mapDescriptor.Bounds,
                Mesh.Values);
        }

        public List<NavigableMeshNode> PathFind(DxVector2 startPosition, DxVector2 endPosition)
        {
            NavigableMeshNode start;
            if(!QueryableMesh.Closest(startPosition, out start))
            {
                Logger.Info("Start: Could not find a {0} that was closest to {1}", typeof(NavigableMeshNode),
                    startPosition);
                return new List<NavigableMeshNode>(0);
            }

            NavigableMeshNode goal;
            if(!QueryableMesh.Closest(endPosition, out goal))
            {
                Logger.Info("End: Could not find a {0} that was closest to {1}", typeof(NavigableMeshNode), endPosition);
                return new List<NavigableMeshNode>(0);
            }

            Func<NavigableMeshNode, NavigableMeshNode, float> scoreFunction = AsTheCrowFliesDistanceSquared;

            /* TODO: Optimize the shit out of this */
            HashSet<NavigableMeshNode> closedSet = new HashSet<NavigableMeshNode>();
            HashSet<NavigableMeshNode> openSet = new HashSet<NavigableMeshNode> {start};
            Dictionary<NavigableMeshNode, NavigableMeshNode> cameFrom =
                new Dictionary<NavigableMeshNode, NavigableMeshNode>();
            Dictionary<NavigableMeshNode, float> gScore = new Dictionary<NavigableMeshNode, float> {[goal] = 0};
            Dictionary<NavigableMeshNode, float> fScore = new Dictionary<NavigableMeshNode, float>
            {
                [start] = scoreFunction(start, goal)
            };

            while(openSet.Any())
            {
                NavigableMeshNode current = openSet.OrderBy(node => fScore.GetOrElse(node, float.MaxValue)).First();
                if(current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }
                openSet.Remove(current);
                closedSet.Add(current);
                foreach(NavigableMeshNode neighbor in current.Neighbors)
                {
                    if(closedSet.Contains(neighbor))
                    {
                        continue;
                    }
                    float tentativeGScore = gScore.GetOrElse(neighbor, float.MaxValue) +
                                            scoreFunction(current, neighbor);
                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if(tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + scoreFunction(neighbor, goal);
                }
            }
            Logger.Info("Failed to find path from {0} to {1}", startPosition, endPosition);
            return new List<NavigableMeshNode>(0);
        }

        private static float AsTheCrowFliesDistanceSquared(NavigableMeshNode start, NavigableMeshNode end)
        {
            return (end.Space.Center - start.Space.Center).MagnitudeSquared;
        }

        private static List<NavigableMeshNode> ReconstructPath(
            Dictionary<NavigableMeshNode, NavigableMeshNode> cameFrom, NavigableMeshNode current)
        {
            List<NavigableMeshNode> path = new List<NavigableMeshNode> {current};
            while(cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            return path;
        }

        private static Dictionary<TilePosition, NavigableMeshNode> GenerateNavigationPoints(MapDescriptor mapDescriptor)
        {
            int mapWidth = mapDescriptor.Width;
            int mapHeight = mapDescriptor.Height;

            int tileWidth = mapDescriptor.TileWidth;
            int tileHeight = mapDescriptor.TileHeight;

            Dictionary<TilePosition, NavigableMeshNode> navigationPoints =
                new Dictionary<TilePosition, NavigableMeshNode>();
            for(int i = 0; i < mapWidth; ++i)
            {
                for(int j = 0; j < mapHeight; ++j)
                {
                    TilePosition originalTilePosition = new TilePosition(i, j);
                    Tile originalTile;
                    if(mapDescriptor.Tiles.TryGetValue(originalTilePosition, out originalTile))
                    {
                        /* If there is a tile, ignore it, move on. We can't navigate to tiles currently */
                        continue;
                    }
                    /* No tile? Great. Check if there's a tile below */
                    TilePosition belowTilePosition = new TilePosition(i, j + 1);
                    Tile belowTile;
                    if(mapDescriptor.Tiles.TryGetValue(belowTilePosition, out belowTile))
                    {
                        /* Tile? Awesome, we have a navigation point */
                        DxRectangle navigationSpace = new DxRectangle(i * tileWidth, j * tileHeight, tileWidth,
                            tileHeight);
                        NavigableMeshNode navigationPoint = new NavigableMeshNode(navigationSpace);
                        navigationPoints[originalTilePosition] = navigationPoint;
                    }
                    /* Otherwise, no, we're just floating in space. Ignore :( */
                }
            }
            return navigationPoints;
        }

        private static void GenerateInternodeLinks(Dictionary<TilePosition, NavigableMeshNode> meshNodes)
        {
            Queue<KeyValuePair<TilePosition, NavigableMeshNode>> unexploredPositionsAndNodes = meshNodes.ToQueue();

            while(unexploredPositionsAndNodes.Any())
            {
                KeyValuePair<TilePosition, NavigableMeshNode> chosenPositionAndNode =
                    unexploredPositionsAndNodes.Dequeue();
                TilePosition position = chosenPositionAndNode.Key;
                NavigableMeshNode node = chosenPositionAndNode.Value;

                NavigableMeshNode neighbor;
                TilePosition? immediateRight = position.Neighbor(Direction.East);
                if(immediateRight.HasValue)
                {
                    if(meshNodes.TryGetValue(immediateRight.Value, out neighbor))
                    {
                        node.Neighbors.Add(neighbor);
                    }
                }

                TilePosition? immediateLeft = position.Neighbor(Direction.West);
                if(immediateLeft.HasValue)
                {
                    if(meshNodes.TryGetValue(immediateLeft.Value, out neighbor))
                    {
                        node.Neighbors.Add(neighbor);
                    }
                }

                // TODO: Jumpable terrain. Ignore for now, MVP style
            }
        }
    }
}
