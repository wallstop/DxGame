using DxCore.Core.Components.Advanced.Behaviors;
using DXGame.Core.Behaviors;
using DXGame.Core.Behaviors.Development;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DxCore.Core.Services
{
    public class BehaviorService : DxService
    {
        private static readonly string AUTOMATIC_BEHAVIOR_NAMESPACE = "DXGame.Core.Behaviors.Automatic";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override void OnCreate()
        {
            // Pull behavior definitions from the automatic namespace
            IEnumerable<Behavior> automaticBehaviors = InitializeFromNamespace<Behavior>(AUTOMATIC_BEHAVIOR_NAMESPACE);
            // TODO: This printout is nonsense; expose the class names of the behaviors loaded plox? 
            Logger.Info($"Automatically considering the following {automaticBehaviors.Count()} behaviors: {automaticBehaviors}");

            // TODO: Fix this
            IEnumerable<Behavior> manualBehaviors = new List<Behavior> { new NaiveChaseAnyPlayerBehavior() };
            Logger.Info($"Just kidding; considering the following {manualBehaviors.Count()} behaviors: {manualBehaviors}");
            Self.AttachComponent(new GoalAssigner(manualBehaviors));
        }

        //? Useful utility elsewhere? 
        /// <summary>
        /// Initialize all classes in the given namespace using reflection 
        /// http://stackoverflow.com/questions/79693/getting-all-types-in-a-namespace-via-reflection
        /// </summary>
        /// <returns>One instance of each type in the given namespace</returns>
        private IEnumerable<T> InitializeFromNamespace<T>(string namespaceString)
        {
            IEnumerable<Type> types = from type in Assembly.GetExecutingAssembly().GetTypes()
                                      where type.IsClass &&
                                      type.Namespace == namespaceString &&
                                      /* Filter out compiler-generated classes */
                                      System.Attribute.GetCustomAttribute(type, typeof(CompilerGeneratedAttribute)) == null
                                      select type;
            // Create one of each type
            return types.ToList().ConvertAll(type => (T)Activator.CreateInstance(type));
        }
    }
}
