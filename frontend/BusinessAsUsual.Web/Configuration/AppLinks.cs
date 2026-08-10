namespace BusinessAsUsual.Web.Configuration;

/// <summary>
/// Centralized configuration for the mobile app download / install links.
///
/// Update the AppsOnAir install URLs below once the builds are uploaded and
/// AppsOnAir provides the public share/install links. These are the only values
/// that need to change to make the "Get the App" pages fully functional.
/// </summary>
public static class AppLinks
{
    /// <summary>
    /// AppsOnAir public install/share URL for the Android build.
    /// Updated: August 9, 2026 - v1.1 (Build 2)
    /// </summary>
    public const string AndroidInstallUrl = "https://app.appsonair.com/install/XHllBXZJ";

    /// <summary>
    /// AppsOnAir public install/share URL for the iOS build.
    /// TODO: Replace once an iOS build is uploaded to AppsOnAir. Empty = "coming soon".
    /// </summary>
    public const string IosInstallUrl = "";

    /// <summary>App display metadata shared by the download pages.</summary>
    public const string AppName = "Business As Usual";

    /// <summary>
    /// Gets the current version of the application.
    /// </summary>
    public const string AppVersion = "1.1 (Build 2)";

    /// <summary>
    /// Gets the minimum Android OS version required.
    /// </summary>
    public const string AndroidMinOs = "Android 8.0+";

    /// <summary>
    /// Gets the minimum iOS version required.
    /// </summary>
    public const string IosMinOs = "iOS 15.0+";

    /// <summary>Whether an install URL is configured for the given platform.</summary>
    public static bool IsAvailable(string platform) => platform?.ToLowerInvariant() switch
    {
        "android" => IsRealUrl(AndroidInstallUrl),
        "ios" => IsRealUrl(IosInstallUrl),
        _ => false
    };

    /// <summary>Resolve the install URL for a platform, or null when unavailable.</summary>
    public static string? InstallUrlFor(string platform) => platform?.ToLowerInvariant() switch
    {
        "android" => IsRealUrl(AndroidInstallUrl) ? AndroidInstallUrl : null,
        "ios" => IsRealUrl(IosInstallUrl) ? IosInstallUrl : null,
        _ => null
    };

    private static bool IsRealUrl(string url)
        => !string.IsNullOrWhiteSpace(url) && !url.Contains("REPLACE_WITH", StringComparison.OrdinalIgnoreCase);
}
