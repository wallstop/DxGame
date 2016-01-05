namespace DXGame.Core.Level
{
    /**
        <summary>
            We somehow want to distinguish between somepossible implementations:
            * Simple Rotation (Simple Maps)
            * Fixed Rotation (Complex Maps)
            * Procedural (Based on some kind of ... procedure)

            How do?
        </summary>

    */

    public interface ILevelProgressionStrategy
    {
        Level InitialLevel { get; }
        Level DetermineNextLevel(Level currentLevel);
    }
}