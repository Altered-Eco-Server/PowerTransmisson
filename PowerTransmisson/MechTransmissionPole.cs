namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Minimap;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Pipes.LiquidComponents;
    using Eco.Gameplay.Pipes.Gases;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using Eco.Shared.Items;
    using Eco.Gameplay.Pipes;
    using Eco.World.Blocks;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Systems.Messaging.Notifications;
    using System.Runtime.Serialization;

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    public partial class MechTransmissionPoleObject : WorldObject, IRepresentsItem
    {
        [Serialized] public bool Gears = true;
        public override LocString DisplayName { get { return Localizer.DoStr("Mechanical Transmission Pole"); } }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public virtual Type RepresentedItemType { get { return typeof(MechTransmissionPoleItem); } }

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<PowerGridComponent>().Initialize(15, new MechanicalPower());
            this.ModsPostInitialize();
        }

        public override void Tick()
        {
            base.Tick();
            if (this.GetComponent<PowerGridComponent>().PowerGrid.EnergySupply > 0)
                Gears = true;
            else Gears = false;
            SetAnimatedState("gear1On", Gears);
            SetAnimatedState("gear2On", Gears);
        }

        static MechTransmissionPoleObject()
        {
            AddOccupancy<MechTransmissionPoleObject>(new List<BlockOccupancy>()
            {
            new BlockOccupancy(new Vector3i(0, 3, 0)),
            new BlockOccupancy(new Vector3i(1, 3, 0)),
            new BlockOccupancy(new Vector3i(-1, 3, 0)),
            new BlockOccupancy(new Vector3i(0, 2, 0)),
            new BlockOccupancy(new Vector3i(0, 1, 0)),
            new BlockOccupancy(new Vector3i(0, 0, 0)),
            });
        }

        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Mechanical Transmission Pole")]
    [Ecopedia("Crafted Objects", "Specialty", createAsSubPage: true)]
    public partial class MechTransmissionPoleItem : WorldObjectItem<MechTransmissionPoleObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Links Mechanical energy");
        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0 | DirectionAxisFlags.Down;
    }

    [RequiresSkill(typeof(MechanicsSkill), 4)]
    public partial class MechTransmissionPoleRecipe : RecipeFamily
    {
        public MechTransmissionPoleRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "MechanicalTransmissionPole",
                Localizer.DoStr("Mechanical Transmission Pole"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(NailItem), 16, typeof(MechanicsSkill), typeof(MechanicsLavishResourcesTalent)),
                    new IngredientElement(typeof(IronGearItem), 8, typeof(MechanicsSkill), typeof(MechanicsLavishResourcesTalent)),
                    new IngredientElement("Hewnlog", 8, typeof(MechanicsSkill), typeof(MechanicsLavishResourcesTalent)), //noloc
                },
                new List<CraftingElement>
                {
                    new CraftingElement<MechTransmissionPoleItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 2;
            this.LaborInCalories = CreateLaborInCaloriesValue(400, typeof(MechanicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(MechTransmissionPoleRecipe), 2, typeof(MechanicsSkill), typeof(MechanicsFocusedSpeedTalent), typeof(MechanicsParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Mechanical Transmission Pole"), typeof(MechTransmissionPoleRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(MachinistTableObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}
