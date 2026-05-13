# iOS Refactoring Checklist

## 📋 Step-by-Step Guide to Refactor BusinessAsUsual-iOS

**Current State:** Basic SwiftUI app with just theme system  
**Target State:** MVVM Clean Architecture with offline-first capabilities

---

## Phase 1: Project Restructuring (1-2 days)

### ✅ Step 1: Create Folder Structure
```
BusinessAsUsualiOS/
├── App/                            # NEW
├── Presentation/                   # NEW
├── Domain/                         # NEW
├── Data/                           # NEW
├── Core/                           # NEW
└── Resources/                      # RENAME from Assets.xcassets
```

### ✅ Step 2: Move Existing Files
- [x] Keep `BAUTheme.swift` → Move to `Presentation/Common/Theme/BAUTheme.swift`
- [x] Keep `BAUTopBar.swift` → Move to `Presentation/Common/Components/BAUTopBar.swift`
- [x] Keep `BAUScreenShell.swift` → Move to `Presentation/Common/Components/BAUScreenShell.swift`
- [x] Keep `BAUBreadcrumbBar.swift` → Move to `Presentation/Common/Components/BAUBreadcrumbBar.swift`
- [x] Update `BusinessAsUsualiOSApp.swift` → Move to `App/BusinessAsUsualiOSApp.swift`
- [x] Delete `ContentView.swift` (will recreate properly)

---

## Phase 2: Core Infrastructure (2-3 days)

### ✅ Step 3: Add Dependencies (SPM - Swift Package Manager)

Add to Xcode project:
1. **Alamofire** - HTTP networking
   ```
   https://github.com/Alamofire/Alamofire
   ```
2. **GRDB** or **CoreData** - Local database (choose one)
   ```
   https://github.com/groue/GRDB.swift
   ```
3. **KeychainSwift** - Secure storage for tokens
   ```
   https://github.com/evgenyneu/keychain-swift
   ```

### ✅ Step 4: Create Core Utilities

**Files to create:**
- [ ] `Core/DI/Container.swift` - Dependency injection container
- [ ] `Core/Network/APIClient.swift` - Base HTTP client using Alamofire
- [ ] `Core/Network/APIEndpoint.swift` - Endpoint definitions
- [ ] `Core/Network/APIError.swift` - Error handling
- [ ] `Core/Network/NetworkMonitor.swift` - Check internet connectivity
- [ ] `Core/Extensions/Date+Extensions.swift`
- [ ] `Core/Extensions/String+Extensions.swift`
- [ ] `Core/Utils/Logger.swift` - Centralized logging

**Example: APIClient.swift**
```swift
import Alamofire
import Foundation

class APIClient {
    static let shared = APIClient()
    private let baseURL: String
    private let session: Session

    init(baseURL: String = Configuration.apiBaseURL) {
        self.baseURL = baseURL

        let configuration = URLSessionConfiguration.default
        configuration.timeoutIntervalForRequest = 30

        let interceptor = AuthInterceptor()
        self.session = Session(configuration: configuration, interceptor: interceptor)
    }

    func request<T: Decodable>(
        _ endpoint: APIEndpoint,
        responseType: T.Type
    ) async throws -> T {
        let url = baseURL + endpoint.path

        return try await withCheckedThrowingContinuation { continuation in
            session.request(
                url,
                method: HTTPMethod(rawValue: endpoint.method.rawValue),
                parameters: endpoint.parameters,
                encoding: endpoint.encoding,
                headers: endpoint.headers
            )
            .validate()
            .responseDecodable(of: T.self) { response in
                switch response.result {
                case .success(let data):
                    continuation.resume(returning: data)
                case .failure(let error):
                    continuation.resume(throwing: APIError.from(error))
                }
            }
        }
    }
}
```

---

## Phase 3: Domain Layer (1-2 days)

### ✅ Step 5: Create Domain Entities

**Files to create:**
- [ ] `Domain/Entities/Employee.swift`
- [ ] `Domain/Entities/Department.swift`
- [ ] `Domain/Entities/TimeEntry.swift`
- [ ] `Domain/Entities/UISpec/EmployeeListSpec.swift`

**Example: Employee.swift**
```swift
struct Employee: Identifiable, Equatable, Codable {
    let id: String
    let firstName: String
    let lastName: String
    let email: String
    let department: String
    let jobTitle: String?
    let photoUrl: String?
    let hireDate: Date

    var fullName: String {
        "\(firstName) \(lastName)"
    }
}
```

### ✅ Step 6: Create Repository Interfaces

**Files to create:**
- [ ] `Domain/Repositories/IEmployeeRepository.swift`
- [ ] `Domain/Repositories/IUISpecRepository.swift`
- [ ] `Domain/Repositories/IAuthRepository.swift`

**Example: IEmployeeRepository.swift**
```swift
protocol IEmployeeRepository {
    func getRemoteEmployees() async throws -> [Employee]
    func getLocalEmployees() async throws -> [Employee]
    func saveLocalEmployees(_ employees: [Employee]) async throws
    func createEmployee(_ employee: Employee) async throws -> Employee
    func updateEmployee(_ employee: Employee) async throws -> Employee
    func deleteEmployee(id: String) async throws
}
```

### ✅ Step 7: Create Use Cases

**Files to create:**
- [ ] `Domain/UseCases/HR/GetEmployeesUseCase.swift`
- [ ] `Domain/UseCases/HR/CreateEmployeeUseCase.swift`
- [ ] `Domain/UseCases/HR/UpdateEmployeeUseCase.swift`

**Example: GetEmployeesUseCase.swift**
```swift
protocol GetEmployeesUseCase {
    func execute() async throws -> [Employee]
}

class GetEmployeesUseCaseImpl: GetEmployeesUseCase {
    private let repository: IEmployeeRepository

    init(repository: IEmployeeRepository) {
        self.repository = repository
    }

    func execute() async throws -> [Employee] {
        // Offline-first: try local first
        if let cached = try? await repository.getLocalEmployees(), !cached.isEmpty {
            // Return cached, fetch fresh in background
            Task {
                let fresh = try? await repository.getRemoteEmployees()
                if let fresh = fresh {
                    try? await repository.saveLocalEmployees(fresh)
                }
            }
            return cached
        }

        // No cache, must fetch
        let employees = try await repository.getRemoteEmployees()
        try? await repository.saveLocalEmployees(employees)
        return employees
    }
}
```

---

## Phase 4: Data Layer (3-4 days)

### ✅ Step 8: Create Data Models (DTOs)

**Files to create:**
- [ ] `Data/Remote/DTOs/EmployeeDTO.swift`
- [ ] `Data/Remote/DTOs/UISpecDTO.swift`

### ✅ Step 9: Create API Services

**Files to create:**
- [ ] `Data/Remote/Services/HRService.swift`
- [ ] `Data/Remote/Services/UISpecService.swift`
- [ ] `Data/Remote/Services/AuthService.swift`

**Example: HRService.swift**
```swift
class HRService {
    private let client: APIClient

    init(client: APIClient) {
        self.client = client
    }

    func getEmployees() async throws -> [EmployeeDTO] {
        try await client.request(
            .getEmployees,
            responseType: [EmployeeDTO].self
        )
    }

    func createEmployee(_ dto: EmployeeDTO) async throws -> EmployeeDTO {
        try await client.request(
            .createEmployee(dto),
            responseType: EmployeeDTO.self
        )
    }
}

// APIEndpoint+HR.swift
extension APIEndpoint {
    static let getEmployees = APIEndpoint(
        path: "/api/hr/employees",
        method: .get
    )

    static func createEmployee(_ dto: EmployeeDTO) -> APIEndpoint {
        APIEndpoint(
            path: "/api/hr/employees",
            method: .post,
            body: dto
        )
    }
}
```

### ✅ Step 10: Set Up Local Database

**Choose one:**
- **Option A: CoreData** (Apple's built-in)
- **Option B: GRDB** (Lightweight, better for complex queries)

**Files to create:**
- [ ] `Data/Local/Database/BAUDatabase.swift`
- [ ] `Data/Local/DAO/EmployeeDAO.swift`
- [ ] `Data/Local/Entities/EmployeeEntity.swift`

**Example: Using CoreData**
```swift
// Create .xcdatamodeld file
// Add EmployeeEntity with attributes:
// - id: String
// - firstName: String
// - lastName: String
// - email: String
// - department: String
// - etc.

// Data/Local/Database/BAUDatabase.swift
class BAUDatabase {
    static let shared = BAUDatabase()

    lazy var persistentContainer: NSPersistentContainer = {
        let container = NSPersistentContainer(name: "BusinessAsUsual")
        container.loadPersistentStores { _, error in
            if let error = error {
                fatalError("Unable to load persistent stores: \(error)")
            }
        }
        return container
    }()

    var context: NSManagedObjectContext {
        persistentContainer.viewContext
    }
}

// Data/Local/DAO/EmployeeDAO.swift
class EmployeeDAO {
    private let context: NSManagedObjectContext

    init(database: BAUDatabase) {
        self.context = database.context
    }

    func getAll() async throws -> [EmployeeEntity] {
        let request: NSFetchRequest<EmployeeEntity> = EmployeeEntity.fetchRequest()
        return try context.fetch(request)
    }

    func insert(_ entity: EmployeeEntity) async throws {
        context.insert(entity)
        try context.save()
    }

    func deleteAll() async throws {
        let fetchRequest: NSFetchRequest<NSFetchRequestResult> = EmployeeEntity.fetchRequest()
        let deleteRequest = NSBatchDeleteRequest(fetchRequest: fetchRequest)
        try context.execute(deleteRequest)
    }
}
```

### ✅ Step 11: Implement Repositories

**Files to create:**
- [ ] `Data/Repositories/EmployeeRepository.swift`
- [ ] `Data/Repositories/UISpecRepository.swift`

**Example: EmployeeRepository.swift**
```swift
class EmployeeRepository: IEmployeeRepository {
    private let apiService: HRService
    private let dao: EmployeeDAO

    init(apiService: HRService, dao: EmployeeDAO) {
        self.apiService = apiService
        self.dao = dao
    }

    func getRemoteEmployees() async throws -> [Employee] {
        let dtos = try await apiService.getEmployees()
        return dtos.map { $0.toDomain() }
    }

    func getLocalEmployees() async throws -> [Employee] {
        let entities = try await dao.getAll()
        return entities.map { $0.toDomain() }
    }

    func saveLocalEmployees(_ employees: [Employee]) async throws {
        try await dao.deleteAll()
        for employee in employees {
            let entity = EmployeeEntity.from(employee, context: dao.context)
            try await dao.insert(entity)
        }
    }

    // ... other methods
}
```

---

## Phase 5: Presentation Layer (3-4 days)

### ✅ Step 12: Create ViewModels

**Files to create:**
- [ ] `Presentation/Modules/HR/ViewModels/EmployeeListViewModel.swift`
- [ ] `Presentation/Modules/HR/ViewModels/EmployeeDetailViewModel.swift`
- [ ] `Presentation/Modules/HR/ViewModels/AddEmployeeViewModel.swift`

**Example: EmployeeListViewModel.swift**
```swift
@MainActor
class EmployeeListViewModel: ObservableObject {
    @Published var employees: [Employee] = []
    @Published var isLoading = false
    @Published var errorMessage: String?
    @Published var searchText = ""
    @Published var uiSpec: EmployeeListSpec?

    private let getEmployeesUseCase: GetEmployeesUseCase
    private let uiSpecRepository: IUISpecRepository

    init(
        getEmployeesUseCase: GetEmployeesUseCase,
        uiSpecRepository: IUISpecRepository
    ) {
        self.getEmployeesUseCase = getEmployeesUseCase
        self.uiSpecRepository = uiSpecRepository
    }

    var filteredEmployees: [Employee] {
        guard !searchText.isEmpty else { return employees }
        return employees.filter { employee in
            employee.fullName.localizedCaseInsensitiveContains(searchText) ||
            employee.email.localizedCaseInsensitiveContains(searchText)
        }
    }

    func loadData() async {
        isLoading = true
        errorMessage = nil

        do {
            // Load UI spec
            uiSpec = try await uiSpecRepository.getEmployeeListSpec()

            // Load employees
            employees = try await getEmployeesUseCase.execute()
        } catch {
            errorMessage = error.localizedDescription
        }

        isLoading = false
    }

    func refresh() async {
        await loadData()
    }
}
```

### ✅ Step 13: Create Views

**Files to create:**
- [ ] `Presentation/Modules/HR/Views/Employees/EmployeeListView.swift`
- [ ] `Presentation/Modules/HR/Views/Employees/EmployeeDetailView.swift`
- [ ] `Presentation/Modules/HR/Views/Employees/AddEmployeeView.swift`
- [ ] `Presentation/Modules/HR/Components/EmployeeCard.swift`

**Example: EmployeeListView.swift**
```swift
struct EmployeeListView: View {
    @StateObject private var viewModel: EmployeeListViewModel
    @Environment(\.bauTheme) private var theme

    init(viewModel: EmployeeListViewModel) {
        _viewModel = StateObject(wrappedValue: viewModel)
    }

    var body: some View {
        NavigationView {
            ZStack {
                theme.background.ignoresSafeArea()

                VStack(spacing: 0) {
                    // Top bar from UI spec
                    if let spec = viewModel.uiSpec {
                        BAUTopBar(title: spec.title)
                    }

                    // Search bar if enabled
                    if viewModel.uiSpec?.searchable == true {
                        SearchBar(
                            text: $viewModel.searchText,
                            placeholder: viewModel.uiSpec?.searchPlaceholder ?? "Search"
                        )
                        .padding()
                    }

                    // Content
                    if viewModel.isLoading {
                        ProgressView()
                            .frame(maxWidth: .infinity, maxHeight: .infinity)
                    } else if let error = viewModel.errorMessage {
                        ErrorView(message: error) {
                            Task { await viewModel.loadData() }
                        }
                    } else {
                        employeeList
                    }
                }
            }
        }
        .task {
            await viewModel.loadData()
        }
    }

    private var employeeList: some View {
        ScrollView {
            LazyVStack(spacing: 12) {
                ForEach(viewModel.filteredEmployees) { employee in
                    NavigationLink(destination: employeeDetailView(employee)) {
                        EmployeeCard(employee: employee)
                    }
                }
            }
            .padding()
        }
        .refreshable {
            await viewModel.refresh()
        }
    }

    private func employeeDetailView(_ employee: Employee) -> some View {
        let detailViewModel = DIContainer.shared.makeEmployeeDetailViewModel(employee: employee)
        return EmployeeDetailView(viewModel: detailViewModel)
    }
}
```

### ✅ Step 14: Update App Entry Point

**Edit: `App/BusinessAsUsualiOSApp.swift`**
```swift
@main
struct BusinessAsUsualiOSApp: App {
    @StateObject private var coordinator = AppCoordinator()

    var body: some Scene {
        WindowGroup {
            coordinator.rootView
                .bauTheme(ThemeRegistry.bau)
                .onAppear {
                    // Initialize DI container
                    _ = DIContainer.shared
                }
        }
    }
}
```

---

## Phase 6: Navigation & DI (1-2 days)

### ✅ Step 15: Create Navigation Coordinator

**Files to create:**
- [ ] `Presentation/Navigation/AppCoordinator.swift`
- [ ] `Presentation/Navigation/Route.swift`

**Example: AppCoordinator.swift**
```swift
class AppCoordinator: ObservableObject {
    @Published var currentTab: Tab = .home

    enum Tab {
        case home
        case hr
        case accounting
        case more
    }

    var rootView: some View {
        TabView(selection: $currentTab) {
            HomeView()
                .tabItem {
                    Label("Home", systemImage: "house")
                }
                .tag(Tab.home)

            hrView
                .tabItem {
                    Label("HR", systemImage: "person.3")
                }
                .tag(Tab.hr)

            Text("Accounting")
                .tabItem {
                    Label("Accounting", systemImage: "dollarsign.circle")
                }
                .tag(Tab.accounting)

            Text("More")
                .tabItem {
                    Label("More", systemImage: "ellipsis")
                }
                .tag(Tab.more)
        }
    }

    private var hrView: some View {
        let viewModel = DIContainer.shared.makeEmployeeListViewModel()
        return EmployeeListView(viewModel: viewModel)
    }
}
```

### ✅ Step 16: Complete DI Container

**Edit: `Core/DI/Container.swift`**
```swift
class DIContainer {
    static let shared = DIContainer()

    private init() {}

    // MARK: - Core
    lazy var apiClient: APIClient = {
        APIClient.shared
    }()

    lazy var database: BAUDatabase = {
        BAUDatabase.shared
    }()

    // MARK: - Data Layer
    lazy var employeeDAO: EmployeeDAO = {
        EmployeeDAO(database: database)
    }()

    lazy var hrService: HRService = {
        HRService(client: apiClient)
    }()

    lazy var uiSpecService: UISpecService = {
        UISpecService(client: apiClient)
    }()

    // MARK: - Repositories
    lazy var employeeRepository: IEmployeeRepository = {
        EmployeeRepository(
            apiService: hrService,
            dao: employeeDAO
        )
    }()

    lazy var uiSpecRepository: IUISpecRepository = {
        UISpecRepository(
            apiService: uiSpecService,
            cache: UISpecCache()
        )
    }()

    // MARK: - Use Cases
    lazy var getEmployeesUseCase: GetEmployeesUseCase = {
        GetEmployeesUseCaseImpl(repository: employeeRepository)
    }()

    // MARK: - ViewModels
    func makeEmployeeListViewModel() -> EmployeeListViewModel {
        EmployeeListViewModel(
            getEmployeesUseCase: getEmployeesUseCase,
            uiSpecRepository: uiSpecRepository
        )
    }

    func makeEmployeeDetailViewModel(employee: Employee) -> EmployeeDetailViewModel {
        EmployeeDetailViewModel(
            employee: employee,
            employeeRepository: employeeRepository
        )
    }
}
```

---

## Phase 7: Testing & Polish (2-3 days)

### ✅ Step 17: Add Unit Tests

**Create test files:**
- [ ] `BusinessAsUsualiOSTests/ViewModelTests/EmployeeListViewModelTests.swift`
- [ ] `BusinessAsUsualiOSTests/UseCaseTests/GetEmployeesUseCaseTests.swift`
- [ ] `BusinessAsUsualiOSTests/RepositoryTests/EmployeeRepositoryTests.swift`

### ✅ Step 18: Configure Environment

**Files to create:**
- [ ] `Core/Config/Configuration.swift`
- [ ] `Core/Config/Environment.swift`

**Example: Configuration.swift**
```swift
enum Configuration {
    static var apiBaseURL: String {
        #if DEBUG
        return "https://localhost:5001"
        #else
        return "https://api.businessasusual.com"
        #endif
    }
}
```

### ✅ Step 19: Add Error Handling & Logging

**Files to create:**
- [ ] `Core/Utils/Logger.swift`
- [ ] `Core/Network/APIError.swift`

### ✅ Step 20: Final Testing

- [ ] Test offline mode
- [ ] Test data sync
- [ ] Test UI spec consumption
- [ ] Test navigation flow
- [ ] Test error scenarios
- [ ] Performance testing

---

## 📊 Progress Tracking

Use this checklist to track your progress:

- [ ] Phase 1: Project Restructuring (1-2 days)
- [ ] Phase 2: Core Infrastructure (2-3 days)
- [ ] Phase 3: Domain Layer (1-2 days)
- [ ] Phase 4: Data Layer (3-4 days)
- [ ] Phase 5: Presentation Layer (3-4 days)
- [ ] Phase 6: Navigation & DI (1-2 days)
- [ ] Phase 7: Testing & Polish (2-3 days)

**Total Estimated Time:** 2-3 weeks

---

## 🎯 Success Criteria

Your iOS app is properly refactored when:

✅ Clean separation of layers (Presentation, Domain, Data)  
✅ MVVM pattern implemented correctly  
✅ ViewModels manage state, Views are dumb  
✅ Use Cases coordinate business logic  
✅ Repositories abstract data sources  
✅ Offline-first: local database works  
✅ API integration: can fetch from backend  
✅ UI specs consumed from backend API  
✅ Dependency injection working  
✅ Navigation pattern implemented  
✅ Unit tests for ViewModels and Use Cases  
✅ Error handling throughout  
✅ Theme system integrated properly  

---

## 📚 Resources

- **iOS Architecture:** [docs/MOBILE_ARCHITECTURE.md](MOBILE_ARCHITECTURE.md)
- **Backend API Docs:** Check main repo's Swagger endpoint
- **Handover Document:** [docs/HANDOVER_DOCUMENT.md](HANDOVER_DOCUMENT.md)

---

**Next Step:** Start with Phase 1 - create the folder structure and move existing files!
