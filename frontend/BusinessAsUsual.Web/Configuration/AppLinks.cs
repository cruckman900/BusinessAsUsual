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
    /// TODO: Replace with the real AppsOnAir Android install URL after uploading app-debug.apk.
    /// </summary>
    //public const string AndroidInstallUrl = "https://www.appsonair.com/apps/REPLACE_WITH_ANDROID_APP_ID";
    public const string AndroidInstallUrl = "https://sin1.contabostorage.com/127726bae0334a7b8a8425a4789fb816:appsonair-prod/1fb49565-d637-4e0d-ada6-7c80eb8e03b4/TNd9uHg0Cy-Day2iMpKAL.apk";

    /// <summary>
    /// AppsOnAir public install/share URL for the iOS build.
    /// TODO: Replace once an iOS build is uploaded to AppsOnAir. Empty = "coming soon".
    /// </summary>
    public const string IosInstallUrl = "";

    /// <summary>App display metadata shared by the download pages.</summary>
    public const string AppName = "Business As Usual";
    public const string AppVersion = "1.0.0";
    public const string AndroidMinOs = "Android 8.0+";
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
