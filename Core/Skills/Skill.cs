using DXGame.Core.Utils;

namespace DXGame.Core.Skills
{
    public class Skill : IIdentifiable
    {
        public string Name { get; private set; }

        public Skill(string name)
        {
            GenericUtils.CheckNullOrDefault(name, "Cannot create a Skill without a name");
            Name = name;
        }

        public UniqueId Id { get; } = new UniqueId();
    }
}