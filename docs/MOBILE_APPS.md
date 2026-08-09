# 📱 Business As Usual - Mobile Apps

Business As Usual is available as native mobile applications for both Android and iOS platforms.

## 🤖 Android App

### Download

[![Download on AppsOnAir](https://img.shields.io/badge/Download-AppsOnAir-00C853?style=for-the-badge&logo=android)](https://app.appsonair.com/install/XHllBXZJ)

**Current Version:** v1.1 (Build 2)  
**Release Date:** August 9, 2026  
**Minimum Android:** 8.0 Oreo (API 26+)  
**Size:** 12.8 MB  
**Distribution:** AppsOnAir (Beta Testing)

### Installation Instructions

1. **Open the download link** on your Android device:  
   👉 [https://app.appsonair.com/install/XHllBXZJ](https://app.appsonair.com/install/XHllBXZJ)

2. **Allow installation from unknown sources** if prompted:
   - Go to Settings → Security → Unknown Sources
   - Enable "Allow from this source" for your browser

3. **Download and install** the APK
   - Tap "Download" on the AppsOnAir page
   - Once downloaded, tap the notification to install
   - Or navigate to Downloads and tap the APK file

4. **Launch the app** and enjoy!

### Features

- 🎨 **Native Material Design 3** UI built with Jetpack Compose
- 🏗️ **Clean Architecture** with MVVM pattern
- 📦 **Modular Design** - Domain, Data, and UI layers
- 🔌 **Dependency Injection** using Koin
- 🎯 **Type-safe Navigation** with Compose Navigation
- 📱 **Responsive UI** that adapts to all screen sizes
- 🌙 **Dark Mode Support**
- ⚡ **Performance Optimized** with lazy loading and state hoisting

### Architecture

The Android app follows **Clean Architecture** principles:

```
┌─────────────────────────────────────┐
│            UI Layer                 │
│   (Jetpack Compose + ViewModels)   │
└─────────────────────────────────────┘
				 │
				 ▼
┌─────────────────────────────────────┐
│           Domain Layer              │
│  (Business Logic, Use Cases, Repos) │
└─────────────────────────────────────┘
				 │
				 ▼
┌─────────────────────────────────────┐
│           Data Layer                │
│  (API Client, Local DB, Data Models)│
└─────────────────────────────────────┘
```

### Tech Stack

- **Language:** Kotlin 2.1
- **UI Framework:** Jetpack Compose
- **Architecture:** Clean Architecture + MVVM
- **DI:** Koin
- **Navigation:** Compose Navigation
- **Build System:** Gradle (Kotlin DSL)
- **Min SDK:** 26 (Android 8.0)
- **Target SDK:** 35 (Android 15)

### Version History

| Version | Build | Date | Changes |
|---------|-------|------|---------|
| 1.1 | 2 | Aug 9, 2026 | Latest build with improved navigation and UI |
| 1.0 | 1 | Initial | First release with core modules |

## 🍎 iOS App

### Status

The iOS version is currently in development and will be available soon!

**Planned Features:**
- 🎨 Native SwiftUI interface
- 🏗️ Clean Architecture with MVVM
- 📦 Modular design matching Android
- 🔌 Dependency Injection
- 🎯 Type-safe navigation
- 📱 iPadOS optimization
- 🌙 Dark Mode

**Expected Release:** Q3 2026  
**Distribution:** TestFlight (Beta) → App Store

### Get Notified

Want to be notified when the iOS app launches? Join our waitlist:
- Email: [ios-beta@businessasusual.work](mailto:ios-beta@businessasusual.work)
- Or watch the [iOS repository](https://github.com/cruckman900/BusinessAsUsual-iOS) for updates

## 🔗 Resources

### Repositories
- **Android:** [github.com/cruckman900/BusinessAsUsual-Android](https://github.com/cruckman900/BusinessAsUsual-Android)
- **iOS:** [github.com/cruckman900/BusinessAsUsual-iOS](https://github.com/cruckman900/BusinessAsUsual-iOS) _(Coming Soon)_
- **Backend API:** [github.com/cruckman900/BusinessAsUsual](https://github.com/cruckman900/BusinessAsUsual)

### Documentation
- [Android Architecture Guide](https://github.com/cruckman900/BusinessAsUsual-Android#-business-as-usual--clean-architecture-android--koin)
- [API Documentation](../docs/API.md)
- [Onboarding Guide](../docs/ONBOARDING.md)

## 🐛 Feedback & Support

Found a bug or have a feature request?

**Android Issues:** [Report on GitHub](https://github.com/cruckman900/BusinessAsUsual-Android/issues)  
**iOS Issues:** [Report on GitHub](https://github.com/cruckman900/BusinessAsUsual-iOS/issues) _(Coming Soon)_  
**General Support:** [support@businessasusual.work](mailto:support@businessasusual.work)

## 📄 License

Both mobile applications are **proprietary software** and follow the same license as the main Business As Usual platform.

---

**Last Updated:** August 9, 2026  
**Maintained by:** Linear Descent Development Team
