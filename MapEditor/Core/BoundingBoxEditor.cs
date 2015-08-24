using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;

namespace MapEditor.Core
{
    public class BoundingBoxEditor
    {
        private readonly MapDescriptor descriptor_;
        private RTree<Platform> tree_;
        public IEnumerable<DxRectangle> Collidables => descriptor_.Platforms.Select(platform => platform.BoundingBox);

        public BoundingBoxEditor()
        {
            tree_ = new RTree<Platform>(platform => platform.BoundingBox, new List<Platform>());
            descriptor_ = new MapDescriptor();
        }

        public void Add(IEnumerable<DxRectangle> regions)
        {
            descriptor_.Platforms.AddRange(regions.Select(region => new Platform(region)));
            ResetTree();
        }

        public void RemoveInRange(DxRectangle range)
        {
            var platformsInRange = tree_.InRange(range);
            descriptor_.Platforms.RemoveAll(platform => platformsInRange.Contains(platform));
            ResetTree();
        }

        private void ResetTree()
        {
            tree_ = new RTree<Platform>(platform => platform.BoundingBox, descriptor_.Platforms);
        }
    }
}