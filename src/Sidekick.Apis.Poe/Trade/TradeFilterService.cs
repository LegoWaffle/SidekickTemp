using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade
{
    public class TradeFilterService : ITradeFilterService
    {
        private static readonly Regex LabelValues = new("(\\#)");

        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly FilterResources resources;

        public TradeFilterService(
            IGameLanguageProvider gameLanguageProvider,
            FilterResources resources)
        {
            this.gameLanguageProvider = gameLanguageProvider;
            this.resources = resources;
        }

        public ModifierFilters GetModifierFilters(Item item)
        {
            var result = new ModifierFilters();

            // No filters for prophecies, currencies and divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Currency
                || item.Metadata.Category == Category.Prophecy
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Watchstone
                || item.Metadata.Category == Category.Undefined)
            {
                return result;
            }

            List<string> enabledModifiers = new();

            InitializeModifierFilters(result.Pseudo, item.Modifiers.Pseudo, enabledModifiers);
            InitializeModifierFilters(result.Enchant, item.Modifiers.Enchant, enabledModifiers, false);
            InitializeModifierFilters(result.Implicit, item.Modifiers.Implicit, enabledModifiers);
            InitializeModifierFilters(result.Explicit, item.Modifiers.Explicit, enabledModifiers);
            InitializeModifierFilters(result.Crafted, item.Modifiers.Crafted, enabledModifiers);
            InitializeModifierFilters(result.Fractured, item.Modifiers.Fractured, enabledModifiers);
            InitializeModifierFilters(result.Scourge, item.Modifiers.Scourge, enabledModifiers);

            return result;
        }

        private static void InitializeModifierFilters(List<ModifierFilter> filters, List<Modifier> modifiers, List<string> enabledModifiers, bool normalizeValues = true)
        {
            if (modifiers.Count == 0) return;

            foreach (var modifier in modifiers)
            {
                if (modifier.OptionValue != null)
                {
                    InitializeModifierFilter(filters,
                        modifier,
                        modifier.OptionValue,
                        normalizeValues: normalizeValues,
                        enabled: enabledModifiers.Contains(modifier.Id)
                    );
                }
                else
                {
                    InitializeModifierFilter(filters,
                        modifier,
                        modifier.Values,
                        normalizeValues: normalizeValues,
                        enabled: enabledModifiers.Contains(modifier.Id)
                    );
                }
            }
        }

        private static void InitializeModifierFilter<T>(List<ModifierFilter> filters,
            Modifier modifier,
            T value,
            double delta = 5,
            bool enabled = false,
            bool normalizeValues = true)
        {
            double? min = null;
            double? max = null;

            if (value is List<double> groupValue)
            {
                var itemValue = groupValue.OrderBy(x => x).FirstOrDefault();

                if (itemValue >= 0)
                {
                    min = itemValue;
                    if (normalizeValues)
                    {
                        min = NormalizeMinValue(min, delta);
                    }
                }
                else
                {
                    max = itemValue;
                    if (normalizeValues)
                    {
                        max = NormalizeMaxValue(max, delta);
                    }
                }

                if (!groupValue.Any())
                {
                    min = null;
                    max = null;
                }
            }

            filters.Add(new ModifierFilter()
            {
                Enabled = enabled,
                Modifier = modifier,
                Min = min,
                Max = max,
            });
        }

        public PropertyFilters GetPropertyFilters(Item item)
        {
            var result = new PropertyFilters();

            // No filters for prophecies, currencies and divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Currency
                || item.Metadata.Category == Category.Prophecy
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Watchstone
                || item.Metadata.Category == Category.Undefined)
            {
                return result;
            }

            // Armour
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Armour,
                gameLanguageProvider.Language.DescriptionArmour,
                item.Properties.Armor);
            // Evasion
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Evasion,
                gameLanguageProvider.Language.DescriptionEvasion,
                item.Properties.Evasion);
            // Energy shield
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_EnergyShield,
                gameLanguageProvider.Language.DescriptionEnergyShield,
                item.Properties.EnergyShield);
            // Block
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Block,
                gameLanguageProvider.Language.DescriptionChanceToBlock,
                item.Properties.ChanceToBlock,
                delta: 1);

            // Physical Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_PhysicalDps,
                resources.Filters_PDps,
                item.Properties.PhysicalDps);
            // Elemental Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_ElementalDps,
                resources.Filters_EDps,
                item.Properties.ElementalDps);
            // Total Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_Dps,
                resources.Filters_Dps,
                item.Properties.DamagePerSecond);
            // Attacks per second
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_AttacksPerSecond,
                gameLanguageProvider.Language.DescriptionAttacksPerSecond,
                item.Properties.AttacksPerSecond,
                delta: 0.1);
            // Critical strike chance
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_CriticalStrikeChance,
                gameLanguageProvider.Language.DescriptionCriticalStrikeChance,
                item.Properties.CriticalStrikeChance,
                delta: 1);

            // Item quantity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemQuantity,
                gameLanguageProvider.Language.DescriptionItemQuantity,
                item.Properties.ItemQuantity);
            // Item rarity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemRarity,
                gameLanguageProvider.Language.DescriptionItemRarity,
                item.Properties.ItemRarity);
            // Monster pack size
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_MonsterPackSize,
                gameLanguageProvider.Language.DescriptionMonsterPackSize,
                item.Properties.MonsterPackSize);
            // Blighted
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Blighted,
                gameLanguageProvider.Language.PrefixBlighted,
                item.Properties.Blighted,
                enabled: item.Properties.Blighted);
            // Map tier
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Tier,
                gameLanguageProvider.Language.DescriptionMapTier,
                item.Properties.MapTier,
                enabled: true,
                min: item.Properties.MapTier);

            // Quality
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Quality,
                gameLanguageProvider.Language.DescriptionQuality,
                item.Properties.Quality,
                enabled: item.Metadata.Rarity == Rarity.Gem,
                min: item.Metadata.Rarity == Rarity.Gem && item.Properties.Quality >= 20 ? item.Properties.Quality : null);
            // Gem level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_GemLevel,
                gameLanguageProvider.Language.DescriptionLevel,
                item.Properties.GemLevel,
                enabled: true,
                min: item.Properties.GemLevel);
            // Item level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_ItemLevel,
                gameLanguageProvider.Language.DescriptionItemLevel,
                item.Properties.ItemLevel,
                enabled: item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Metadata.Rarity != Rarity.Unique,
                min: item.Properties.ItemLevel >= 80 ? (double?)item.Properties.ItemLevel : null);
            // Corrupted
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Corrupted,
                gameLanguageProvider.Language.DescriptionCorrupted,
                item.Properties.Corrupted,
                enabled: item.Properties.Corrupted);
            // Crusader
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Crusader,
                gameLanguageProvider.Language.InfluenceCrusader,
                item.Influences.Crusader,
                enabled: item.Influences.Crusader);
            // Elder
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Elder,
                gameLanguageProvider.Language.InfluenceElder,
                item.Influences.Elder,
                enabled: item.Influences.Elder);
            // Hunter
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Hunter,
                gameLanguageProvider.Language.InfluenceHunter,
                item.Influences.Hunter,
                enabled: item.Influences.Hunter);
            // Redeemer
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Redeemer,
                gameLanguageProvider.Language.InfluenceRedeemer,
                item.Influences.Redeemer,
                enabled: item.Influences.Redeemer);
            // Shaper
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Shaper,
                gameLanguageProvider.Language.InfluenceShaper,
                item.Influences.Shaper,
                enabled: item.Influences.Shaper);
            // Warlord
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Warlord,
                gameLanguageProvider.Language.InfluenceWarlord, item.Influences.Warlord,
                enabled: item.Influences.Warlord);
            // Scourged
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Scourged,
                gameLanguageProvider.Language.DescriptionScourged,
                item.Properties.Scourged ? 1 : 0,
                enabled: item.Properties.Scourged,
                min: 1);

            return result;
        }

        private static void InitializePropertyFilter<T>(List<PropertyFilter> filters,
            PropertyFilterType type,
            string label,
            T value,
            double delta = 5,
            bool enabled = false,
            double? min = null,
            double? max = null)
        {
            FilterValueType valueType;

            switch (value)
            {
                case bool boolValue:
                    valueType = FilterValueType.Boolean;
                    if (!boolValue) return;
                    break;
                case int intValue:
                    valueType = FilterValueType.Int;
                    if (intValue == 0) return;
                    min ??= NormalizeMinValue(intValue, delta);
                    if (LabelValues.IsMatch(label))
                    {
                        label = LabelValues.Replace(label, intValue.ToString());
                    }
                    else
                    {
                        label += $": {value}";
                    }
                    break;
                case double doubleValue:
                    valueType = FilterValueType.Double;
                    if (doubleValue == 0) return;
                    min ??= NormalizeMinValue(doubleValue, delta);
                    if (LabelValues.IsMatch(label))
                    {
                        label = LabelValues.Replace(label, doubleValue.ToString("0.00"));
                    }
                    else
                    {
                        label += $": {doubleValue:0.00}";
                    }
                    break;
                default: return;
            }

            filters.Add(new PropertyFilter()
            {
                Enabled = enabled,
                Type = type,
                Value = value,
                ValueType = valueType,
                Text = label,
                Min = min,
                Max = max,
            });
        }

        /// <summary>
        /// Smallest positive value between a -5 delta or 90%.
        /// </summary>
        private static int? NormalizeMinValue(double? value, double delta)
        {
            if (value.HasValue)
            {
                if (value.Value > 0)
                {
                    return (int)Math.Max(Math.Min(value.Value - delta, value.Value * 0.9), 0);
                }
                else
                {
                    return (int)Math.Min(Math.Min(value.Value - delta, value.Value * 1.1), 0);
                }
            }

            return null;
        }

        /// <summary>
        /// Smallest positive value between a +5 delta or 110%.
        /// </summary>
        private static int? NormalizeMaxValue(double? value, double delta)
        {
            if (value.HasValue)
            {
                if (value.Value > 0)
                {
                    return (int)Math.Max(Math.Max(value.Value + delta, value.Value * 1.1), 0);
                }
                else
                {
                    return (int)Math.Min(Math.Max(value.Value + delta, value.Value * 0.9), 0);
                }
            }

            return null;
        }
    }
}
