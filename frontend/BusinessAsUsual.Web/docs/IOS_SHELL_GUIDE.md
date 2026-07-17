# iOS Shell Guide — Consuming the Mobile UI Contract

> A minimal, budget-conscious iOS client that comes to life by discovering
> features from the same Mobile UI Contract the Android client uses.
> No backend changes are required — the three-instance gateway already serves
> the mobile contract at `https://api.businessasusual.work`.

## How this fits the existing architecture

The platform already exposes a **Mobile UI Contract** (see
[`docs/MOBILE_UI_CONTRACT_SYSTEM.md`](../../../docs/MOBILE_UI_CONTRACT_SYSTEM.md)).
The iOS app is just another consumer of the same flow Android uses:

```
iOS app
  1. GET https://api.businessasusual.work/api/modules/mobile   (discover modules)
  2. For each module with supportsMobile=true:
	   GET <mobileUISpecUrl>  e.g. /api/crm/mobile/ui-spec
  3. Render native SwiftUI screens from the returned JSON spec
```

The gateway routes each path to the correct instance:
- `/api/crm/*`     → HEAVY `10.0.1.250:5004`
- `/api/hr/*`      → HEAVY `10.0.1.250:5041`
- `/api/modules/*` → LIGHT `10.0.1.175:5100`

## Decision: SwiftUI shell (Simulator-first)

- **Free path:** Xcode + iOS Simulator require no paid Apple account. Do all
  early work in the Simulator to preserve budget.
- Only a paid **Apple Developer** account ($99/yr) is needed for physical
  devices / TestFlight — defer until the shell proves the contract works.

## Step 1 — Create the project (on the MacBook)

1. Xcode → **File ▸ New ▸ Project ▸ iOS ▸ App**.
2. Product Name: `BusinessAsUsualMobile`; Interface: **SwiftUI**; Language: **Swift**.
3. Run once in the Simulator to confirm the empty shell launches.

## Step 2 — App Transport Security

The gateway is HTTPS with a valid cert, so **no ATS exceptions are needed**.
(Only add exceptions if you point at a raw private IP over HTTP during local testing.)

## Step 3 — Contract models (mirror the JSON spec)

```swift
struct MobileModule: Codable, Identifiable {
	let moduleId: String
	let displayName: String
	let supportsMobile: Bool
	let mobileUISpecUrl: String?
	var id: String { moduleId }
}

// Minimal slice of a module's UI spec — expand as you render more.
struct MobileUISpec: Codable {
	let moduleId: String
	let displayName: String
	let screens: [MobileScreen]?
}

struct MobileScreen: Codable, Identifiable {
	let id: String
	let title: String
	let type: String?          // e.g. "list", "form", "pipeline"
}
```

> Tip: hit the live endpoints with `curl` and match field names exactly:
> `curl -s https://api.businessasusual.work/api/crm/mobile/ui-spec | jq`

## Step 4 — Contract client (the "hint of life")

```swift
import Foundation

enum Api {
	static let base = URL(string: "https://api.businessasusual.work")!
}

struct ContractClient {
	func discoverModules() async throws -> [MobileModule] {
		let url = Api.base.appending(path: "api/modules/mobile")
		let (data, _) = try await URLSession.shared.data(from: url)
		return try JSONDecoder().decode([MobileModule].self, from: data)
	}

	func fetchSpec(_ module: MobileModule) async throws -> MobileUISpec? {
		guard let raw = module.mobileUISpecUrl,
			  let url = URL(string: raw) else { return nil }
		let (data, _) = try await URLSession.shared.data(from: url)
		return try JSONDecoder().decode(MobileUISpec.self, from: data)
	}
}
```

## Step 5 — One screen that proves the pipe

```swift
import SwiftUI

struct ModulesView: View {
	@State private var modules: [MobileModule] = []
	@State private var error: String?
	private let client = ContractClient()

	var body: some View {
		NavigationStack {
			List(modules) { m in
				VStack(alignment: .leading) {
					Text(m.displayName).font(.headline)
					Text(m.moduleId).font(.caption).foregroundStyle(.secondary)
				}
			}
			.navigationTitle("Modules")
			.overlay { if let error { Text(error).foregroundStyle(.red) } }
			.task {
				do { modules = try await client.discoverModules().filter(\.supportsMobile) }
				catch { self.error = error.localizedDescription }
			}
		}
	}
}
```

Wire `ModulesView()` into your `App` scene, run in the Simulator, and you should
see the mobile-enabled modules — including **CRM** with its `pipeline` and
`email-templates` screens.

## Step 6 — Verify against the live contract

```sh
# Modules discovery (what iOS lists)
curl -s https://api.businessasusual.work/api/modules/mobile | jq '.[].moduleId'

# CRM spec (should include the new features)
curl -s https://api.businessasusual.work/api/crm/mobile/ui-spec | jq
```

## Next steps (after the shell has a heartbeat)

- [ ] Render a real screen from a spec `type` (start with CRM `pipeline`).
- [ ] Add loading/error states and a pull-to-refresh.
- [ ] Extract shared contract models so Android/iOS stay in sync with
	  [`services/CRM/CRM.Contracts/Specifications/MobileUISpecification.cs`](../../../services/CRM/CRM.Contracts/Specifications/MobileUISpecification.cs).
- [ ] Only when ready for devices: enroll in Apple Developer + TestFlight.

## Budget notes

- API/gateway cost is unchanged — iOS is purely a new client.
- Simulator work is free; defer the $99 Apple account until device testing.
- Keep the first milestone tiny (Steps 1–5) to prove value before spending.

## Rendering rich screen types (board & card-collection)

The contract now includes richer screen types so the pipeline and email
templates look native instead of plain lists (see
[docs/MOBILE_UI_CONTRACT_SYSTEM.md](../../../docs/MOBILE_UI_CONTRACT_SYSTEM.md#rich-screen-types-board--card-collection)).
Render by switching on the spec's `type`:

```swift
enum ScreenType: String { case list, board, cardCollection = "card-collection" }

@ViewBuilder
func screenView(for type: String, key: String) -> some View {
    switch ScreenType(rawValue: type) {
    case .board:          PipelineBoardView(screenKey: key)   // horizontal columns of cards
    case .cardCollection: CardCollectionView(screenKey: key)  // vertical grid of preview cards
    default:              ListView(screenKey: key)            // existing list fallback
    }
}
```

### Board (`type: "board"`) — Sales Pipeline
- Fetch grouped data from `GET /api/crm/mobile/data-board/{screenKey}` (returns
  `groups[]`, each with `label`, `count`, `total`, `rows[]`).
- Render each group as a column (`ScrollView(.horizontal)`), using `cardLayout`
  to map fields onto card slots (`titleField`, `subtitleField`, `valueField`,
  `progressField`, `badgeField`, `metaField`).
- If `enableDragToMove` is true, a drag onto another column should `POST`/`PUT`
  to `moveEndpoint` with `{id}` and `{group}` substituted.

### Card collection (`type: "card-collection"`) — Email Templates
- Fetch flat data from `GET /api/crm/mobile/data/{screenKey}`.
- Render each row as a preview card using `cardLayout` (`titleField`,
  `subtitleField`, `badgeField`, `statusField`).

### Backward compatibility (important)
If the app doesn't recognize a `type`, render `fallbackColumns` as a list — this
lets you ship the server change now and update the renderer incrementally.

> **Android parity:** the equivalent Android renderer work is tracked as
> "Mobile Kanban parity in Android app" in
> [docs/CRM_FEATURE_ROADMAP.md](../../../docs/CRM_FEATURE_ROADMAP.md). Both
> clients consume the same contract, so the board/card mapping above applies
> 1:1 to Android (Jetpack Compose: `LazyRow` of columns, `LazyColumn` of cards).
