# Android Migration — Rename to `work.businessasusual` + Merge Mobile UI Contract

This folder is a **staging area** that lives in your .NET backend repo only so I could hand
you exact, corrected files. Nothing here builds. You copy the files into your **separate**
`BusinessAsUsual-Android` repo in Android Studio, then delete this `android-migration/` folder.

Domain: **businessasusual.work** → package root **`work.businessasusual`**

---

## Part A — Rename the whole app to `work.businessasusual` (do this FIRST)

Your app currently uses **three** different package roots. Rename all three with Android
Studio's refactor tool so every reference (imports, MainActivity, dashboard/hr screens, etc.)
updates automatically:

| Module  | Old root                             | New root                        |
|---------|--------------------------------------|---------------------------------|
| app     | `com.example.businessasusualandroid` | `work.businessasusual`          |
| data    | `com.example.data`                   | `work.businessasusual.data`     |
| domain  | `com.example.domain`                 | `work.businessasusual.domain`   |

### How to rename in Android Studio (per module root)
1. Switch the Project tool window to the **Project** view (not Android view) so you see the
   real `java/` folders.
2. Expand `app/src/main/java`. You'll see the folder chain `com/example/businessasusualandroid`.
3. Right-click the **`com`** node → **Refactor ▸ Rename…**. If it offers "Rename package" vs
   "Rename directory", choose **Rename package**. Rename in place so you end up with the
   `work/businessasusual` chain. Easiest reliable path:
   - Right-click `businessasusualandroid` → Refactor ▸ Rename → `businessasusual`.
   - Right-click `example` → Refactor ▸ Rename → `work`.
   - Right-click `com` → Refactor ▸ Rename → (leave as a container) — actually you want the
	 final segments to read `work/businessasusual`, so after the two renames above, drag
	 `businessasusual` out from under `work/` is **not** needed; instead use **Move**:
	 select `businessasusual`, Refactor ▸ Move, target package `work`.
   - Let the IDE update all usages when prompted (check "Search in comments and strings").
4. Repeat for `data/src/main/java`: `com.example.data` → `work.businessasusual.data`.
5. Repeat for `domain/src/main/java`: `com.example.domain` → `work.businessasusual.domain`.

> Tip: If the multi-step folder dance is fussy, the fastest reliable method is a global
> **Edit ▸ Find ▸ Replace in Files** across the repo:
> - `com.example.businessasusualandroid` → `work.businessasusual`
> - `com.example.data` → `work.businessasusual.data`
> - `com.example.domain` → `work.businessasusual.domain`
>
> Then physically move the source folders to match: `java/com/example/businessasusualandroid`
> → `java/work/businessasusual`, `java/com/example/data` → `java/work/businessasusual/data`,
> `java/com/example/domain` → `java/work/businessasusual/domain`. Delete the now-empty
> `com/example` trees. Do a Gradle **Sync** afterward.

### Gradle changes that go with the rename
Replace your three `build.gradle.kts` files with the corrected copies in this folder
(`android-migration/app/build.gradle.kts`, `.../data/build.gradle.kts`,
`.../domain/build.gradle.kts`). The only differences from yours are:

- **app**: `namespace = "work.businessasusual"` and `applicationId = "work.businessasusual"`
- **data**: `namespace = "work.businessasusual.data"`
- **domain**: `namespace = "work.businessasusual.domain"`

### AndroidManifest — no edits needed
Your manifest uses **relative** names (`android:name=".BusinessAsUsualApp"` and
`.MainActivity"`). Because we place the app classes **directly** under `work.businessasusual`
(the module namespace), those relative names still resolve. Leave the manifest as-is.

---

## Part B — Add the Mobile UI Contract feature (the 14 pushed files)

You pushed 14 files to the **root of each module** (outside `src/`) and they used a
placeholder `com.businessasusual.*` namespace. Below is the exact fate of each. The
**corrected** versions are already written in this folder at their final paths.

### B1. MOVE + fix package (10 files) — copy the corrected versions from this folder
| Copy this corrected file (in android-migration) | Into your Android repo at |
|---|---|
| `domain/.../domain/util/Resource.kt`             | `domain/src/main/java/work/businessasusual/domain/util/Resource.kt` |
| `domain/.../domain/model/MobileUIModels.kt`      | `domain/src/main/java/work/businessasusual/domain/model/MobileUIModels.kt` |
| `domain/.../domain/repository/MobileUIRepository.kt` | `domain/src/main/java/work/businessasusual/domain/repository/MobileUIRepository.kt` |
| `domain/.../domain/usecase/MobileUIUsecases.kt`  | `domain/src/main/java/work/businessasusual/domain/usecase/MobileUIUsecases.kt` |
| `data/.../data/remote/dto/MobileUIDtos.kt`       | `data/src/main/java/work/businessasusual/data/remote/dto/MobileUIDtos.kt` |
| `data/.../data/remote/MobileUIApi.kt`            | `data/src/main/java/work/businessasusual/data/remote/MobileUIApi.kt` |
| `data/.../data/mapper/MobileUIMappers.kt`        | `data/src/main/java/work/businessasusual/data/mapper/MobileUIMappers.kt` |
| `data/.../data/repository/MobileUIRepositoryImpl.kt` | `data/src/main/java/work/businessasusual/data/repository/MobileUIRepositoryImpl.kt` |
| `app/.../ui/mobileui/MobileUiViewModel.kt`       | `app/src/main/java/work/businessasusual/ui/mobileui/MobileUiViewModel.kt` |
| `app/.../ui/mobileui/DynamicUi.kt`               | `app/src/main/java/work/businessasusual/ui/mobileui/DynamicUi.kt` |

### B2. MERGE, then DELETE the pushed copy (3 DI files)
Your app already has `AppModule.kt`, `DataModule.kt`, `DomainModule.kt`. Do **not** keep the
pushed duplicates. Replace your existing three module files with the **merged** versions in
this folder (they contain your original bindings **plus** the new Mobile UI bindings):

| Replace your existing file | With the merged copy |
|---|---|
| `app/src/main/java/work/businessasusual/di/AppModule.kt`       | `android-migration/app/.../di/AppModule.kt` |
| `data/src/main/java/work/businessasusual/data/di/DataModule.kt` | `android-migration/data/.../di/DataModule.kt` |
| `domain/src/main/java/work/businessasusual/domain/di/DomainModule.kt` | `android-migration/domain/.../di/DomainModule.kt` |

### B3. DELETE outright (1 file)
- `app/BauApp.kt` — a **duplicate** Application class. Your existing `BusinessAsUsualApp`
  stays the only Application. (A renamed `BusinessAsUsualApp.kt` is included in this folder
  in case you want to confirm its final form.)

### B4. Delete the 14 mis-placed originals
After copying the corrected versions in, delete every file you pushed to the module **roots**
(the ones sitting directly in `app/`, `data/`, `domain/` outside `src/`):

```
app/BauApp.kt
app/di/AppModule.kt
app/ui/mobileui/DynamicUi.kt
app/ui/mobileui/MobileUiViewModel.kt
data/di/DataModule.kt
data/mapper/MobileUIMappers.kt
data/remote/dto/MobileUIDtos.kt
data/remote/MobileUIApi.kt
data/repository/MobileUIRepositoryImpl.kt
domain/di/DomainModule.kt
domain/model/MobileUIModels.kt
domain/repository/MobileUIRepository.kt
domain/usecase/MobileUIUsecases.kt
domain/util/Resource.kt
```

---

## Part C — Gradle dependencies: nothing to add
I checked your three `build.gradle.kts` files. Everything the new feature needs is already
present:
- **data**: Retrofit 2.11, `converter-kotlinx-serialization`, `okhttp logging-interceptor`,
  `kotlinx-serialization-json`, `koin-android`, and the `kotlin("plugin.serialization")` plugin. ✅
- **app**: Compose, Koin (`koin-androidx-compose`), `lifecycle-runtime-compose`,
  `material-icons-extended`. ✅
- **domain**: `koin-core`. ✅

Only edits are the `namespace` / `applicationId` values in Part A.

---

## Part D — Point the app at your backend
`DataModule.kt` sets `BASE_URL = "http://10.0.2.2:5001/"` (that's the emulator's alias for
your host machine's `localhost`). Change `5001` to whatever port your API gateway / HR
service listens on. The Retrofit endpoints call `api/hr/mobile/ui-spec` and friends.

---

## Part E — Verify
1. Android Studio ▸ **Sync Project with Gradle Files**.
2. **Build ▸ Rebuild Project** (or `./gradlew assembleDebug`). Expect 0 errors.
3. Render the screen where you want it (it's Koin-wired):
   ```kotlin
   // inside a Composable
   work.businessasusual.ui.mobileui.MobileUiScreen()
   ```
4. Run the app with your backend up; the HR module UI is built entirely from the contract.

When the Android repo builds clean, delete this `android-migration/` folder from the backend
repo — it was only a delivery vehicle.
