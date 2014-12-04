using System.Collections.Generic;

namespace DXGame.Core
{
    /**
    <summary>
        A Generator is our version of the Factory pattern. See http://en.wikipedia.org/wiki/Factory_%28object-oriented_programming%29 for details
        on the Factory pattern.

        This is a placeholder class (2014-12-03) until a better solution is determined. While we have this very nice Component-based 
        architecture for game objects, something needs to create those Components in the first place. That is where the Generators come in.

        In order to create *kinds* of objects (enemies, players, map tiles), you can easily build your own Generator. The main Game class
        needs to be aware of it somehow, either as a member variable or some other scheme. Generators should generate their Components/GameObjects
        either during the Game's construction or Initialization step.

        Generators aren't limited to GameObjects, although GameObjects are the primary use case. 

        To create a Generator:
        <code>
            public MyGenerator : Generator<GameObject>
            {
                public List<GameObject> Generate()
                {
                    List<GameObject> generatedObjects = new List<GameObject>();
                    GameObject newObject = new GameObject();
                    // Generate components, add them to the object / objects
                    generatedObjects.Add(newObject);
                    return generatedObject;
                }
            }
        </code>            
    </summary>
    */

    public abstract class Generator<T>
    {
        // TODO: Not sure if we'll need this, but these will create Players, Maps, etc (maybe)
        public abstract List<T> Generate();
    }
}