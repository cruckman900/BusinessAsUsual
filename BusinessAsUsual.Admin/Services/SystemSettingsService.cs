using System.Text.Json;
using BusinessAsUsual.Admin.Models;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Provides functionality to load and save system settings to a JSON file in the application's data directory.
    /// </summary>
    /// <remarks>The settings are persisted in a file named 'systemsettings.json' located in the 'App_Data'
    /// folder under the application's content root. This service is intended for use within ASP.NET Core applications
    /// and relies on the provided web host environment to determine the storage location. All settings are serialized
    /// and deserialized using indented JSON formatting for readability.</remarks>
    public class SystemSettingsService
    {
        private readonly string _settingsPath;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the SystemSettingsService class using the specified web host environment.
        /// </summary>
        /// <remarks>The service stores system settings in a JSON file located in the application's
        /// App_Data directory. The directory is created if it does not exist.</remarks>
        /// <param name="env">The web host environment that provides access to the application's content root path. Cannot be null.</param>
        public SystemSettingsService(IWebHostEnvironment env)
        {
            _settingsPath = Path.Combine(env.ContentRootPath, "App_Data", "systemsettings.json");

            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        /// <summary>
        /// Loads the system settings from the configured file path. If the settings file does not exist or cannot be
        /// deserialized, returns a new instance with default values.
        /// </summary>
        /// <remarks>This method attempts to read and deserialize the settings from the file specified by
        /// the internal path. If the file is missing or contains invalid data, a default settings object is returned
        /// instead of throwing an exception.</remarks>
        /// <returns>A <see cref="SystemSettings"/> object containing the loaded settings, or a new instance with default values
        /// if the file is missing or invalid.</returns>
        public SystemSettings Load()
        {
            if (!File.Exists(_settingsPath))
                return new SystemSettings();

            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<SystemSettings>(json, _jsonOptions)
                ?? new SystemSettings();
        }

        /// <summary>
        /// Saves the specified system settings to persistent storage in JSON format.
        /// </summary>
        /// <remarks>This method overwrites any existing settings file at the configured path. The
        /// settings are serialized using the current JSON options. If the target file is inaccessible or the settings
        /// object is invalid for serialization, an exception may be thrown.</remarks>
        /// <param name="settings">The system settings to be saved. Cannot be null.</param>
        public void Save(SystemSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
