using DXGame.Core.Properties;

/*
    Simple test health buff.
*/
namespace DXGame.TowerGame.PropertyMutators
{
    // Hatred makes you more... healthy?
    public class InheritedHatred : PropertyMutator<int>
    {
        private static readonly int HEALTH_BONUS = 30;
        private static readonly string INHERITED_HATRED = "InheritedHatred";


        public InheritedHatred()
            : base(InheritedHatredMutator, InheritedHatredDeMutator, INHERITED_HATRED)
        {
                       
        }


        protected static int InheritedHatredMutator(int input)
        {
            return input + HEALTH_BONUS;
        }

        protected static int InheritedHatredDeMutator(int input)
        {
            return input - HEALTH_BONUS;
        }
    }
}
