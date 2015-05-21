using DXGame.Core.Utils;

namespace DXGame.Core.Skills
{
    public class Skill : IIdentifiable
    {
        public string Name { get; private set; }

        public Skill(string name)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot initialize {GetType()} with a null/default name");
            Name = name;
        }

        public UniqueId Id { get; } = new UniqueId();
    }
}