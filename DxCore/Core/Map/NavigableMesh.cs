using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using WallNetCore.Extension;
using WallNetCore.Validate;

namespace DxCore.Core.Map
{
    public sealed class NavigableMeshNode
    {
        // TODO: Access control
        public List<NavigableMeshNode> Neighbors { get; private set; }
        public DxRectangle Space { get; private set; }

        public NavigableMeshNode(DxRectangle space)
        {
            Space = space;
            Neighbors = new List<NavigableMeshNode>();
        }

        public override string ToString() => Space.ToString();
    }

    public sealed class NavigableMesh
    {
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
                return new List<NavigableMeshNode>(0);
            }

            NavigableMeshNode goal;
            if(!QueryableMesh.Closest(endPosition, out goal))
            {
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

            NavigableMeshNode current = null;
            while(openSet.Any())
            {
                current = openSet.OrderBy(node => fScore.GetOrElse(node, float.MaxValue)).First();
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

            if(ReferenceEquals(current, null))
            {
                return new List<NavigableMeshNode>(0);
            }
            /* Pick a card... any card */
            current = ThreadLocalRandom.Current.FromCollection(cameFrom.Keys);
            return ReconstructPath(cameFrom, current);
        }

        private static float AsTheCrowFliesDistanceSquared(NavigableMeshNode start, NavigableMeshNode end)
        {
            return (end.Space.Center - start.Space.Center).MagnitudeSquared;
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

                const int numAdjacentTileDirections = 2;
                List<Direction> adjacentNeighborDirections = new List<Direction>(numAdjacentTileDirections)
                {
                    Direction.East,
                    Direction.West
                };
                foreach(Direction direction in adjacentNeighborDirections.ToArray())
                {
                    NavigableMeshNode neighbor;
                    /* We can always move freely east */
                    TilePosition? adjacent = position.Neighbor(direction);
                    if(!adjacent.HasValue)
                    {
                        continue;
                    }
                    if(meshNodes.TryGetValue(adjacent.Value, out neighbor))
                    {
                        adjacentNeighborDirections.Remove(direction);
                        node.Neighbors.Add(neighbor);
                    }
                }

                /* ...and within commonly jumpable terrain */
                SearchJumpableTerrain(meshNodes, position, node);
                SearchDroppableTerrain(meshNodes, position, node, adjacentNeighborDirections);

                // TODO: Add in support for "dropping" down to lower tiles
            }
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
                        navigationPoints[belowTilePosition] = navigationPoint;
                    }
                    /* Otherwise, no, we're just floating in space. Ignore :( */
                }
            }
            return navigationPoints;
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
            path.Reverse();
            return path;
        }

        private static void SearchDroppableTerrain(Dictionary<TilePosition, NavigableMeshNode> meshNodes,
            TilePosition position, NavigableMeshNode root, List<Direction> searchDirections)
        {
            if(!searchDirections.Any())
            {
                return;
            }

            /* 
                Simple strategy: 
                    We assume that every entity can drop to any tile that is within
                    the raycast line of a slope of two

                    |OXXXX
                    |OOXXX
                    |OOOXX
                    |OOOOX
                    |OOOOO

                    Where any tile below an O is reachable and any tile at or beneath an X
                    is not. Additionally, if we do encounter a tile, we need to reign-in
                    our search, as we'll have stopped dropping at that point.
            */
            const int widthPerLevel = 2;
            int maxY = meshNodes.Keys.Select(_ => _.Y).Max();
            Dictionary<Direction, int> searchLimits = searchDirections.ToDictionary(_ => _, direction => 0);
            for(int yDepth = 1; position.Y + yDepth <= maxY; ++yDepth)
            {
                int y = position.Y + yDepth;
                foreach(Direction direction in searchDirections)
                {
                    int multiplier = direction == Direction.East ? 1 : -1;
                    int limiter = searchLimits[direction];
                    for(int i = 1; i < widthPerLevel * yDepth - limiter; ++i)
                    {
                        int x = position.X + i * multiplier;
                        if(!TilePosition.ValidTileCoordinates(x, y))
                        {
                            continue;
                        }
                        TilePosition dropTilePosition = new TilePosition(x, y);
                        NavigableMeshNode neighbor;
                        if(meshNodes.TryGetValue(dropTilePosition, out neighbor))
                        {
                            root.Neighbors.Add(neighbor);
                            searchLimits[direction] = i;
                            break;
                        }
                    }
                }
            }
        }

        private static void SearchJumpableTerrain(Dictionary<TilePosition, NavigableMeshNode> meshNodes,
            TilePosition position, NavigableMeshNode root)
        {
            const int maxXSearch = 2;
            const int maxYSearch = 2;

            List<NavigableMeshNode> potentialNeighbors = new List<NavigableMeshNode>();

            for(int i = -maxXSearch; i <= maxXSearch; ++i)
            {
                int x = position.X + i;
                for(int j = -1; -maxYSearch <= j; --j)
                {
                    int y = position.Y + j;
                    if(!TilePosition.ValidTileCoordinates(x, y))
                    {
                        continue;
                    }

                    TilePosition tilePosition = new TilePosition(x, y);
                    NavigableMeshNode neighbor;
                    if(meshNodes.TryGetValue(tilePosition, out neighbor))
                    {
                        /* Tile directly above us? We can't jump :( */
                        if(i == 0)
                        {
                            return;
                        }
                        potentialNeighbors.Add(neighbor);
                    }
                }
            }

            root.Neighbors.AddRange(potentialNeighbors);
        }
    }
}