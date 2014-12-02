using System.Collections.Generic;

namespace DXGame.Core
{
    public abstract class ObjectGenerator<T>
    {
        // TODO: Not sure if we'll need this, but these will create Players, Maps, etc (maybe)

        public abstract List<T> Generate();
    }
}