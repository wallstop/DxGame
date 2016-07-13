using System.Collections.Generic;
using Babel.Items;
using Babel.Player;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Animated;
using DxCore.Core.Components.Advanced.Team;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Player;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.State;
using Microsoft.Xna.Framework;
using NLog;

namespace Babel.Generators
{
    public class BabelPlayerGenerator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly FloatingHealthIndicator healthBar_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;

        public BabelPlayerGenerator(DxVector2 playerPosition)
        {
            physics_ =
                PhysicsComponent.Builder()
                    .WithBounds(new DxVector2(75, 75))
                    .WithPosition(playerPosition)
                    .WithCollisionGroup(CollisionGroup.Entities)
                    .WithCollidesWith(CollisionGroup.All.Not(CollisionGroup.Entities))
                    .WithPhysicsInitialization(SensorFactory.WorldCollisionSensor)
                    .Build();
            playerProperties_ = new EntityPropertiesComponent(PlayerFactory.BasicPlayerProperties,
                PlayerFactory.GenericLevelUp);
            /* Fuck with the health so we can check if the hp bar works */
            playerProperties_.EntityProperties.Health.BaseValue -= 3;
            // TODO: Need to add state machine in (how?)

            // TODO make these colors not shit and/or blend into the current biome
            healthBar_ =
                FloatingHealthIndicator.Builder()
                    .WithForegroundColor(Color.Green)
                    .WithEntityProperties(playerProperties_)
                    .WithPosition(physics_)
                    .Build();
            healthBar_.LoadContent();
        }

        public BabelPlayerGenerator From(DxVector2 playerPosition)
        {
            return new BabelPlayerGenerator(playerPosition);
        }

        public GameObject GeneratePlayer(AbstractCommandComponent playerCommander, bool isActivePlayer)
        {
            GameObject.GameObjectBuilder playerBuilder = GameObject.Builder();
            FacingComponent facingComponent = new FacingComponent();
            TeamComponent teamComponent = new TeamComponent(Team.PlayerTeam);
            EntityLevelComponent entityLevelComponent = new EntityLevelComponent();
            BasicAttackComponent basicAttackListener = new BasicAttackComponent();
            ItemManager itemManager = new ItemManager();

            string playerName = "rektorOfSouls";
            PlayerNameComponent playerNameComponent = new PlayerNameComponent(playerName);
            if(isActivePlayer)
            {
                ActivePlayerComponent activePlayerComponent = new ActivePlayerComponent();
                playerBuilder.WithComponent(activePlayerComponent);
            }
            playerBuilder.WithComponents(physics_, playerProperties_, healthBar_, playerCommander, facingComponent,
                teamComponent, entityLevelComponent, basicAttackListener, itemManager, playerNameComponent);
            GameObject playerObject = playerBuilder.Build();

            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(playerObject, "Poison");
            return playerObject;
        }

        public GameObject GeneratePlayer()
        {
            // Let's not do that shitty audio
            //new AudioMessage("Audio/Music/MusicToDelight", AudioType.Music).Emit();
            return GeneratePlayer(new PlayerInputListener(), true);
        }

        public List<GameObject> Generate()
        {
            return new List<GameObject> {GeneratePlayer()};
        }
    }
}
