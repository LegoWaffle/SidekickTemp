using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Settings;
using Sidekick.Mock;
using Sidekick.Modules.Settings;
using Xunit;

namespace Sidekick.Apis.Poe.Tests
{
    public class ParserFixture : IAsyncLifetime
    {
        public IInvariantModifierProvider InvariantModifierProvider { get; private set; } = null!;

        public IItemParser Parser { get; private set; } = null!;

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            using var ctx = new TestContext();
            ctx.Services.AddLocalization();

            ctx.Services
                // Building blocks
                .AddSidekickCommon()
                .AddSidekickCommonBlazor()
                .AddSidekickCommonDatabase()

                // Apis
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoeWikiApi()

                // Modules
                .AddSidekickSettings()

                // Mocks
                .AddSidekickMocks();

            var settingsService = ctx.Services.GetRequiredService<ISettingsService>();
            await settingsService.Set(SettingKeys.LanguageParser, "en");
            await settingsService.Set(SettingKeys.LanguageUi, "en");
            await settingsService.Set(SettingKeys.LeagueId, "Standard");

            var initComponent = ctx.RenderComponent<Common.Blazor.Initialization.Initialization>();
            if (initComponent.Instance.InitializationTask != null)
            {
                await initComponent.Instance.InitializationTask;
            }

            Parser = ctx.Services.GetRequiredService<IItemParser>();
            InvariantModifierProvider = ctx.Services.GetRequiredService<IInvariantModifierProvider>();
        }
    }
}
