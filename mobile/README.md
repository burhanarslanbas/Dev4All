# Dev4All — Mobile

Android native app for **Dev4All** — built with Kotlin, Jetpack Compose, and Material 3.

| | |
|---|---|
| **Language** | Kotlin |
| **UI** | Jetpack Compose + Material 3 |
| **Build** | Gradle 8.9, Kotlin DSL (`build.gradle.kts`) |
| **Min SDK** | 24 (Android 7.0) |
| **Target SDK** | 35 (Android 15) |
| **Package** | `com.dev4all.mobile` |

---

## Prerequisites

- JDK 17+
- Android Studio Hedgehog (2023.1.1) or newer
- Android SDK with API level 35 platform tools (install via Android Studio SDK Manager)

---

## Running on an emulator or device

1. Open Android Studio → **File → Open** → select the `mobile/` folder.
2. Wait for Gradle sync to complete.
3. Select a run configuration (device or emulator) in the toolbar and press **▶ Run**.

From the command line:

```bash
cd mobile
./gradlew installDebug          # deploys to a connected device/running emulator
adb shell am start -n com.dev4all.mobile/.MainActivity
```

---

## Building a debug APK

```bash
cd mobile
./gradlew assembleDebug
```

The APK is produced at:

```
mobile/app/build/outputs/apk/debug/app-debug.apk
```

---

## Running tests

```bash
cd mobile
./gradlew test            # JVM unit tests
./gradlew connectedCheck  # instrumented tests (requires a connected device/emulator)
```

---

## Configuration — API base URL & feature flags

Configuration values are injected at compile time via `BuildConfig` fields:

| Field | Default value |
|---|---|
| `BuildConfig.API_BASE_URL` | `https://api.dev4all.com/api` |
| `BuildConfig.FEATURE_FLAGS` | `{"darkMode":false,"betaFeatures":false}` |

Default values are set in `mobile/gradle.properties`:

```properties
API_BASE_URL=https://api.dev4all.com/api
FEATURE_FLAGS={"darkMode":false,"betaFeatures":false}
```

### Local override (not committed)

Create `mobile/local.properties` (already git-ignored) and add any key you want to override:

```properties
# mobile/local.properties — git-ignored, never committed
API_BASE_URL=http://10.0.2.2:5000/api   # Android emulator → host machine
FEATURE_FLAGS={"darkMode":true,"betaFeatures":true}
```

Values in `local.properties` take precedence over `gradle.properties`.

---

## Security

- **Never commit** `local.properties`, keystores (`.keystore`, `.jks`), or signing credentials.
- These patterns are already covered by the root `.gitignore`.
- For release signing, use environment variables or a CI secret manager — never store keys in the repository.
