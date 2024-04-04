using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public interface IParserPatterns : IInitializableService
    {
        Regex AreaLevel { get; }
        Regex Armor { get; }
        Regex AttacksPerSecond { get; }
        Regex Blighted { get; }
        Regex BlightRavaged { get; }
        Regex ChanceToBlock { get; }
        Regex Corrupted { get; }
        Regex CriticalStrikeChance { get; }
        Regex Crusader { get; }
        Regex Elder { get; }
        Regex ElementalDamage { get; }
        Regex EnergyShield { get; }
        Regex Evasion { get; }
        Regex Hunter { get; }
        Regex ItemLevel { get; }
        Regex ItemQuantity { get; }
        Regex ItemRarity { get; }
        Regex Level { get; }
        Regex MapTier { get; }
        Regex MonsterPackSize { get; }
        Regex PhysicalDamage { get; }
        Regex Quality { get; }
        Regex AlternateQuality { get; }
        Regex Requirements { get; }
        Dictionary<Rarity, Regex> Rarity { get; }
        Regex Redeemer { get; }
        Regex Shaper { get; }
        Regex Socket { get; }
        Regex Unidentified { get; }
        Regex Warlord { get; }
        Regex Anomalous { get; }
        Regex Divergent { get; }
        Regex Phantasmal { get; }
        Regex CorpseLevel { get; }

        Dictionary<Class, Regex> Classes { get; }
    }
}
