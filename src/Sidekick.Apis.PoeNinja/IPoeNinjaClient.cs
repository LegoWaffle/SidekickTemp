using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeNinja
{
    public interface IPoeNinjaClient
    {
        Task<NinjaPrice?> GetPriceInfo(
            string? englishName,
            string? englishType,
            Category category,
            Properties properties,
            int? numberOfLinks = null,
            string? firstModifierLine = null);

        Task<NinjaPrice?> GetClusterPrice(
            string englishGrantText,
            int passiveCount,
            int itemLevel);

        Uri GetDetailsUri(NinjaPrice price);
    }
}
