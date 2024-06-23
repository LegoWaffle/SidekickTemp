namespace Sidekick.Common.Settings;

public interface ISettingsService
{
    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<bool> GetBool(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<string?> GetString(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<DateTimeOffset?> GetDateTime(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<TValue?> GetObject<TValue>(string key);

    /// <summary>
    ///     Command to save a single setting.
    /// </summary>
    /// <param name="key">The key to update in the settings.</param>
    /// <param name="value">The value of the setting.</param>
    Task Set(
        string key,
        object? value);
}
