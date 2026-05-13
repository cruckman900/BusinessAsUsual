# Mobile Architecture Guide - MVVM Clean Architecture

**Business As Usual Mobile Apps (iOS & Android)**

## 📱 Overview

Both iOS and Android apps follow **MVVM (Model-View-ViewModel) Clean Architecture** with UI contracts consumed from backend services.

---

## 🏗️ Clean Architecture Layers

### iOS & Android Unified Structure

```
┌─────────────────────────────────────────────────────┐
│                  PRESENTATION LAYER                 │
│  ┌───────────────────────────────────────────────┐  │
│  │              Views (SwiftUI / Jetpack Compose)│  │
│  │  - Screens                                    │  │
│  │  - Components                                 │  │
│  │  - Navigation                                 │  │
│  └───────────────┬───────────────────────────────┘  │
│                  │                                   │
│  ┌───────────────▼───────────────────────────────┐  │
│  │            ViewModels                         │  │
│  │  - State management                           │  │
│  │  - Business logic coordination                │  │
│  │  - Input handling                             │  │
│  └───────────────┬───────────────────────────────┘  │
└──────────────────┼───────────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────────┐
│                  DOMAIN LAYER                        │
│  ┌─────────────────────────────────────────────────┐ │
│  │              Use Cases (Interactors)            │ │
│  │  - GetEmployeesUseCase                          │ │
│  │  - CreateEmployeeUseCase                        │ │
│  │  - SyncDataUseCase                              │ │
│  └───────────────┬─────────────────────────────────┘ │
│                  │                                    │
│  ┌───────────────▼─────────────────────────────────┐ │
│  │         Domain Models (Entities)                │ │
│  │  - Employee                                     │ │
│  │  - Department                                   │ │
│  │  - TimeEntry                                    │ │
│  └─────────────────────────────────────────────────┘ │
│                  ▲                                    │
│  ┌───────────────┴─────────────────────────────────┐ │
│  │         Repository Interfaces                   │ │
│  │  - IEmployeeRepository                          │ │
│  │  - IUISpecRepository                            │ │
│  └─────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────┘
                   ▲
┌──────────────────┴───────────────────────────────────┐
│                  DATA LAYER                          │
│  ┌─────────────────────────────────────────────────┐ │
│  │         Repository Implementations              │ │
│  │  - EmployeeRepository                           │ │
│  │  - UISpecRepository                             │ │
│  └───────────────┬─────────────────────────────────┘ │
│                  │                                    │
│         ┌────────┼────────┐                          │
│         │        │        │                          │
│  ┌──────▼──┐ ┌──▼─────┐ ┌▼──────────┐               │
│  │  Remote │ │ Local  │ │ UI Spec   │               │
│  │  API    │ │ DB     │ │ Cache     │               │
│  │ (Retrofit│ │(SQLite)│ │ (UserDef) │               │
│  │  /Alamofire│ │/Room) │ │           │               │
│  └─────────┘ └────────┘ └───────────┘               │
└──────────────────────────────────────────────────────┘
```

---

## 📂 iOS Project Structure (Swift + SwiftUI)

### Recommended Folder Structure

```
BusinessAsUsualiOS/
│
├── App/
│   ├── BusinessAsUsualiOSApp.swift      # App entry point
│   └── AppDependencies.swift            # DI container
│
├── Presentation/                         # PRESENTATION LAYER
│   ├── Common/
│   │   ├── Theme/
│   │   │   ├── BAUTheme.swift           # Theme model
│   │   │   ├── ThemeRegistry.swift      # Theme definitions
│   │   │   └── ThemeModifier.swift      # SwiftUI modifiers
│   │   ├── Components/
│   │   │   ├── BAUTopBar.swift
│   │   │   ├── BAUBottomNav.swift
│   │   │   ├── BAUButton.swift
│   │   │   └── BAUCard.swift
│   │   └── Extensions/
│   │       └── View+Extensions.swift
│   │
│   ├── Modules/                         # Feature modules
│   │   ├── HR/
│   │   │   ├── Views/
│   │   │   │   ├── Employees/
│   │   │   │   │   ├── EmployeeListView.swift
│   │   │   │   │   ├── EmployeeDetailView.swift
│   │   │   │   │   └── AddEmployeeView.swift
│   │   │   │   └── Departments/
│   │   │   │       └── DepartmentListView.swift
│   │   │   ├── ViewModels/
│   │   │   │   ├── EmployeeListViewModel.swift
│   │   │   │   ├── EmployeeDetailViewModel.swift
│   │   │   │   └── AddEmployeeViewModel.swift
│   │   │   └── Components/
│   │   │       ├── EmployeeCard.swift
│   │   │       └── DepartmentPicker.swift
│   │   │
│   │   ├── Accounting/
│   │   │   ├── Views/
│   │   │   ├── ViewModels/
│   │   │   └── Components/
│   │   │
│   │   └── Timekeeping/
│   │       ├── Views/
│   │       ├── ViewModels/
│   │       └── Components/
│   │
│   └── Navigation/
│       ├── AppCoordinator.swift         # App navigation coordinator
│       ├── Route.swift                  # Route definitions
│       └── TabRouter.swift              # Bottom tab router
│
├── Domain/                              # DOMAIN LAYER
│   ├── Entities/                        # Domain models
│   │   ├── Employee.swift
│   │   ├── Department.swift
│   │   ├── TimeEntry.swift
│   │   └── Invoice.swift
│   │
│   ├── UseCases/                        # Business logic
│   │   ├── HR/
│   │   │   ├── GetEmployeesUseCase.swift
│   │   │   ├── CreateEmployeeUseCase.swift
│   │   │   ├── UpdateEmployeeUseCase.swift
│   │   │   └── DeleteEmployeeUseCase.swift
│   │   ├── Accounting/
│   │   │   ├── GetInvoicesUseCase.swift
│   │   │   └── CreateInvoiceUseCase.swift
│   │   └── Sync/
│   │       ├── SyncEmployeesUseCase.swift
│   │       └── SyncOfflineChangesUseCase.swift
│   │
│   └── Repositories/                    # Repository interfaces
│       ├── IEmployeeRepository.swift
│       ├── IUISpecRepository.swift
│       ├── IAuthRepository.swift
│       └── ISyncRepository.swift
│
├── Data/                                # DATA LAYER
│   ├── Repositories/                    # Repository implementations
│   │   ├── EmployeeRepository.swift
│   │   ├── UISpecRepository.swift
│   │   ├── AuthRepository.swift
│   │   └── SyncRepository.swift
│   │
│   ├── Remote/                          # API layer
│   │   ├── API/
│   │   │   ├── APIClient.swift          # Base HTTP client
│   │   │   ├── APIEndpoint.swift        # Endpoint definitions
│   │   │   └── APIError.swift           # Error handling
│   │   ├── Services/
│   │   │   ├── HRService.swift          # HR API calls
│   │   │   ├── AccountingService.swift
│   │   │   └── UISpecService.swift      # Fetch UI specs
│   │   └── DTOs/                        # Data transfer objects
│   │       ├── EmployeeDTO.swift
│   │       └── UISpecDTO.swift
│   │
│   ├── Local/                           # Local database
│   │   ├── Database/
│   │   │   ├── BAUDatabase.swift        # Core Data / SQLite wrapper
│   │   │   └── DatabaseSchema.swift
│   │   ├── DAO/                         # Data access objects
│   │   │   ├── EmployeeDAO.swift
│   │   │   └── TimeEntryDAO.swift
│   │   └── Entities/                    # Local DB entities
│   │       ├── EmployeeEntity.swift
│   │       └── SyncQueueEntity.swift
│   │
│   └── Cache/                           # Caching layer
│       ├── UISpecCache.swift            # Cache UI specs
│       ├── UserDefaultsCache.swift      # Simple key-value
│       └── ImageCache.swift             # Image caching
│
├── Core/                                # Shared utilities
│   ├── DI/                              # Dependency injection
│   │   ├── Container.swift
│   │   └── Modules/
│   │       ├── DataModule.swift
│   │       ├── DomainModule.swift
│   │       └── PresentationModule.swift
│   ├── Extensions/
│   │   ├── Date+Extensions.swift
│   │   ├── String+Extensions.swift
│   │   └── Collection+Extensions.swift
│   ├── Utils/
│   │   ├── Logger.swift
│   │   ├── Validator.swift
│   │   └── DateFormatter.swift
│   └── Network/
│       ├── NetworkMonitor.swift         # Check connectivity
│       └── NetworkError.swift
│
└── Resources/
    ├── Assets.xcassets/
    ├── Localization/                    # String resources
    │   ├── en.lproj/
    │   └── es.lproj/
    └── Info.plist
```

---

## 📂 Android Project Structure (Kotlin + Jetpack Compose)

### Recommended Folder Structure

```
com.businessasusual.android/
│
├── app/                                 # Main application module
│   ├── Application.kt                   # Application class
│   ├── MainActivity.kt                  # Single activity
│   └── di/                              # Dependency injection
│       ├── AppModule.kt
│       ├── NetworkModule.kt
│       └── DatabaseModule.kt
│
├── presentation/                        # PRESENTATION LAYER
│   ├── theme/
│   │   ├── Theme.kt                     # Material 3 theme
│   │   ├── Color.kt
│   │   ├── Typography.kt
│   │   └── Shape.kt
│   │
│   ├── common/
│   │   ├── components/
│   │   │   ├── BAUTopBar.kt
│   │   │   ├── BAUBottomNav.kt
│   │   │   ├── BAUButton.kt
│   │   │   └── BAUCard.kt
│   │   └── extensions/
│   │       └── ModifierExtensions.kt
│   │
│   ├── modules/                         # Feature modules
│   │   ├── hr/
│   │   │   ├── screens/
│   │   │   │   ├── employees/
│   │   │   │   │   ├── EmployeeListScreen.kt
│   │   │   │   │   ├── EmployeeDetailScreen.kt
│   │   │   │   │   └── AddEmployeeScreen.kt
│   │   │   │   └── departments/
│   │   │   │       └── DepartmentListScreen.kt
│   │   │   ├── viewmodels/
│   │   │   │   ├── EmployeeListViewModel.kt
│   │   │   │   ├── EmployeeDetailViewModel.kt
│   │   │   │   └── AddEmployeeViewModel.kt
│   │   │   └── components/
│   │   │       ├── EmployeeCard.kt
│   │   │       └── DepartmentPicker.kt
│   │   │
│   │   ├── accounting/
│   │   │   ├── screens/
│   │   │   ├── viewmodels/
│   │   │   └── components/
│   │   │
│   │   └── timekeeping/
│   │       ├── screens/
│   │       ├── viewmodels/
│   │       └── components/
│   │
│   └── navigation/
│       ├── NavGraph.kt                  # Navigation graph
│       ├── Route.kt                     # Route sealed class
│       └── NavigationExtensions.kt
│
├── domain/                              # DOMAIN LAYER
│   ├── model/                           # Domain entities
│   │   ├── Employee.kt
│   │   ├── Department.kt
│   │   ├── TimeEntry.kt
│   │   └── Invoice.kt
│   │
│   ├── usecase/                         # Use cases
│   │   ├── hr/
│   │   │   ├── GetEmployeesUseCase.kt
│   │   │   ├── CreateEmployeeUseCase.kt
│   │   │   ├── UpdateEmployeeUseCase.kt
│   │   │   └── DeleteEmployeeUseCase.kt
│   │   ├── accounting/
│   │   │   ├── GetInvoicesUseCase.kt
│   │   │   └── CreateInvoiceUseCase.kt
│   │   └── sync/
│   │       ├── SyncEmployeesUseCase.kt
│   │       └── SyncOfflineChangesUseCase.kt
│   │
│   └── repository/                      # Repository interfaces
│       ├── EmployeeRepository.kt
│       ├── UISpecRepository.kt
│       ├── AuthRepository.kt
│       └── SyncRepository.kt
│
├── data/                                # DATA LAYER
│   ├── repository/                      # Repository implementations
│   │   ├── EmployeeRepositoryImpl.kt
│   │   ├── UISpecRepositoryImpl.kt
│   │   ├── AuthRepositoryImpl.kt
│   │   └── SyncRepositoryImpl.kt
│   │
│   ├── remote/                          # Network layer
│   │   ├── api/
│   │   │   ├── ApiClient.kt             # Retrofit client
│   │   │   ├── ApiService.kt            # API endpoints
│   │   │   └── AuthInterceptor.kt       # JWT handling
│   │   ├── dto/                         # Data transfer objects
│   │   │   ├── EmployeeDto.kt
│   │   │   └── UISpecDto.kt
│   │   └── mapper/                      # DTO → Domain mapping
│   │       ├── EmployeeMapper.kt
│   │       └── UISpecMapper.kt
│   │
│   ├── local/                           # Local database
│   │   ├── database/
│   │   │   ├── BAUDatabase.kt           # Room database
│   │   │   └── DatabaseSchema.kt
│   │   ├── dao/                         # Data access objects
│   │   │   ├── EmployeeDao.kt
│   │   │   └── TimeEntryDao.kt
│   │   ├── entity/                      # Room entities
│   │   │   ├── EmployeeEntity.kt
│   │   │   └── SyncQueueEntity.kt
│   │   └── mapper/                      # Entity → Domain mapping
│   │       └── EmployeeEntityMapper.kt
│   │
│   └── cache/                           # Caching
│       ├── UISpecCache.kt
│       ├── PreferencesManager.kt        # SharedPreferences wrapper
│       └── ImageCache.kt
│
└── core/                                # Shared utilities
    ├── di/                              # Hilt modules
    │   ├── AppModule.kt
    │   ├── DataModule.kt
    │   ├── DomainModule.kt
    │   └── PresentationModule.kt
    ├── extension/
    │   ├── DateExtensions.kt
    │   ├── StringExtensions.kt
    │   └── CollectionExtensions.kt
    ├── util/
    │   ├── Logger.kt
    │   ├── Validator.kt
    │   └── DateFormatter.kt
    └── network/
        ├── NetworkMonitor.kt
        └── NetworkError.kt
```

---

## 🎨 MVVM Pattern Implementation

### iOS (SwiftUI) Example

#### **1. Domain Entity**
```swift
// Domain/Entities/Employee.swift
struct Employee: Identifiable, Equatable {
    let id: String
    let firstName: String
    let lastName: String
    let email: String
    let department: String
    let photoUrl: String?
    let hireDate: Date

    var fullName: String {
        "\(firstName) \(lastName)"
    }
}
```

#### **2. Use Case**
```swift
// Domain/UseCases/HR/GetEmployeesUseCase.swift
protocol GetEmployeesUseCase {
    func execute() async throws -> [Employee]
}

class GetEmployeesUseCaseImpl: GetEmployeesUseCase {
    private let repository: IEmployeeRepository

    init(repository: IEmployeeRepository) {
        self.repository = repository
    }

    func execute() async throws -> [Employee] {
        // Try local first, then remote
        if let cached = try? await repository.getLocalEmployees(), !cached.isEmpty {
            return cached
        }

        let employees = try await repository.getRemoteEmployees()
        try? await repository.saveLocalEmployees(employees)
        return employees
    }
}
```

#### **3. Repository Interface**
```swift
// Domain/Repositories/IEmployeeRepository.swift
protocol IEmployeeRepository {
    func getRemoteEmployees() async throws -> [Employee]
    func getLocalEmployees() async throws -> [Employee]
    func saveLocalEmployees(_ employees: [Employee]) async throws
    func createEmployee(_ employee: Employee) async throws -> Employee
}
```

#### **4. Repository Implementation**
```swift
// Data/Repositories/EmployeeRepository.swift
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
        let entities = employees.map { EmployeeEntity(from: $0) }
        try await dao.insertAll(entities)
    }

    func createEmployee(_ employee: Employee) async throws -> Employee {
        let dto = EmployeeDTO(from: employee)
        let created = try await apiService.createEmployee(dto)
        return created.toDomain()
    }
}
```

#### **5. ViewModel**
```swift
// Presentation/Modules/HR/ViewModels/EmployeeListViewModel.swift
@MainActor
class EmployeeListViewModel: ObservableObject {
    @Published var employees: [Employee] = []
    @Published var isLoading = false
    @Published var errorMessage: String?
    @Published var searchText = ""

    private let getEmployeesUseCase: GetEmployeesUseCase
    private let uiSpecRepository: IUISpecRepository

    @Published var uiSpec: EmployeeListSpec?

    init(getEmployeesUseCase: GetEmployeesUseCase, uiSpecRepository: IUISpecRepository) {
        self.getEmployeesUseCase = getEmployeesUseCase
        self.uiSpecRepository = uiSpecRepository
    }

    var filteredEmployees: [Employee] {
        if searchText.isEmpty {
            return employees
        }
        return employees.filter { employee in
            employee.fullName.localizedCaseInsensitiveContains(searchText) ||
            employee.email.localizedCaseInsensitiveContains(searchText)
        }
    }

    func loadData() async {
        isLoading = true
        errorMessage = nil

        do {
            // Load UI spec first
            if let spec = try await uiSpecRepository.getEmployeeListSpec() {
                uiSpec = spec
            }

            // Load employee data
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

#### **6. View (SwiftUI)**
```swift
// Presentation/Modules/HR/Views/Employees/EmployeeListView.swift
struct EmployeeListView: View {
    @StateObject private var viewModel: EmployeeListViewModel
    @Environment(\.bauTheme) private var theme

    init(viewModel: EmployeeListViewModel) {
        _viewModel = StateObject(wrappedValue: viewModel)
    }

    var body: some View {
        ZStack {
            theme.background.ignoresSafeArea()

            VStack(spacing: 0) {
                // Top bar from UI spec
                if let spec = viewModel.uiSpec {
                    BAUTopBar(title: spec.title, icon: spec.icon)
                }

                // Search bar if enabled by spec
                if viewModel.uiSpec?.searchable == true {
                    SearchBar(text: $viewModel.searchText, 
                             placeholder: viewModel.uiSpec?.searchPlaceholder ?? "Search")
                        .padding()
                }

                // Employee list
                if viewModel.isLoading {
                    ProgressView()
                        .frame(maxWidth: .infinity, maxHeight: .infinity)
                } else if let error = viewModel.errorMessage {
                    ErrorView(message: error, onRetry: {
                        Task { await viewModel.loadData() }
                    })
                } else {
                    ScrollView {
                        LazyVStack(spacing: 12) {
                            ForEach(viewModel.filteredEmployees) { employee in
                                EmployeeCard(employee: employee)
                                    .onTapGesture {
                                        // Navigate to detail
                                    }
                            }
                        }
                        .padding()
                    }
                    .refreshable {
                        await viewModel.refresh()
                    }
                }
            }
        }
        .task {
            await viewModel.loadData()
        }
    }
}
```

---

### Android (Jetpack Compose + Kotlin) Example

#### **1. Domain Entity**
```kotlin
// domain/model/Employee.kt
data class Employee(
    val id: String,
    val firstName: String,
    val lastName: String,
    val email: String,
    val department: String,
    val photoUrl: String? = null,
    val hireDate: Date
) {
    val fullName: String
        get() = "$firstName $lastName"
}
```

#### **2. Use Case**
```kotlin
// domain/usecase/hr/GetEmployeesUseCase.kt
class GetEmployeesUseCase(
    private val repository: EmployeeRepository
) {
    suspend operator fun invoke(): Result<List<Employee>> = runCatching {
        // Try local first
        val cached = repository.getLocalEmployees().getOrNull()
        if (!cached.isNullOrEmpty()) {
            return@runCatching cached
        }

        // Fetch from remote
        val employees = repository.getRemoteEmployees().getOrThrow()
        repository.saveLocalEmployees(employees)
        employees
    }
}
```

#### **3. Repository Interface**
```kotlin
// domain/repository/EmployeeRepository.kt
interface EmployeeRepository {
    suspend fun getRemoteEmployees(): Result<List<Employee>>
    suspend fun getLocalEmployees(): Result<List<Employee>>
    suspend fun saveLocalEmployees(employees: List<Employee>): Result<Unit>
    suspend fun createEmployee(employee: Employee): Result<Employee>
}
```

#### **4. Repository Implementation**
```kotlin
// data/repository/EmployeeRepositoryImpl.kt
class EmployeeRepositoryImpl(
    private val apiService: ApiService,
    private val employeeDao: EmployeeDao,
    private val mapper: EmployeeMapper
) : EmployeeRepository {

    override suspend fun getRemoteEmployees(): Result<List<Employee>> = runCatching {
        val dtos = apiService.getEmployees()
        dtos.map { mapper.dtoToDomain(it) }
    }

    override suspend fun getLocalEmployees(): Result<List<Employee>> = runCatching {
        val entities = employeeDao.getAll()
        entities.map { mapper.entityToDomain(it) }
    }

    override suspend fun saveLocalEmployees(employees: List<Employee>): Result<Unit> = runCatching {
        val entities = employees.map { mapper.domainToEntity(it) }
        employeeDao.insertAll(entities)
    }

    override suspend fun createEmployee(employee: Employee): Result<Employee> = runCatching {
        val dto = mapper.domainToDto(employee)
        val created = apiService.createEmployee(dto)
        mapper.dtoToDomain(created)
    }
}
```

#### **5. ViewModel**
```kotlin
// presentation/modules/hr/viewmodels/EmployeeListViewModel.kt
@HiltViewModel
class EmployeeListViewModel @Inject constructor(
    private val getEmployeesUseCase: GetEmployeesUseCase,
    private val uiSpecRepository: UISpecRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<EmployeeListUiState>(EmployeeListUiState.Loading)
    val uiState: StateFlow<EmployeeListUiState> = _uiState.asStateFlow()

    private val _searchText = MutableStateFlow("")
    val searchText: StateFlow<String> = _searchText.asStateFlow()

    init {
        loadData()
    }

    fun loadData() {
        viewModelScope.launch {
            _uiState.value = EmployeeListUiState.Loading

            // Load UI spec first
            val spec = uiSpecRepository.getEmployeeListSpec().getOrNull()

            // Load employees
            getEmployeesUseCase().fold(
                onSuccess = { employees ->
                    _uiState.value = EmployeeListUiState.Success(
                        employees = employees,
                        uiSpec = spec
                    )
                },
                onFailure = { error ->
                    _uiState.value = EmployeeListUiState.Error(error.message ?: "Unknown error")
                }
            )
        }
    }

    fun onSearchTextChanged(text: String) {
        _searchText.value = text
    }

    fun refresh() {
        loadData()
    }
}

sealed class EmployeeListUiState {
    object Loading : EmployeeListUiState()
    data class Success(val employees: List<Employee>, val uiSpec: EmployeeListSpec?) : EmployeeListUiState()
    data class Error(val message: String) : EmployeeListUiState()
}
```

#### **6. Screen (Jetpack Compose)**
```kotlin
// presentation/modules/hr/screens/employees/EmployeeListScreen.kt
@Composable
fun EmployeeListScreen(
    viewModel: EmployeeListViewModel = hiltViewModel(),
    onNavigateToDetail: (String) -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val searchText by viewModel.searchText.collectAsState()

    Scaffold(
        topBar = {
            when (val state = uiState) {
                is EmployeeListUiState.Success -> {
                    BAUTopBar(
                        title = state.uiSpec?.title ?: "Employees",
                        icon = state.uiSpec?.icon
                    )
                }
                else -> BAUTopBar(title = "Employees")
            }
        }
    ) { paddingValues ->
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            when (val state = uiState) {
                is EmployeeListUiState.Loading -> {
                    CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.Center)
                    )
                }
                is EmployeeListUiState.Error -> {
                    ErrorView(
                        message = state.message,
                        onRetry = { viewModel.loadData() }
                    )
                }
                is EmployeeListUiState.Success -> {
                    Column {
                        // Search bar if enabled by spec
                        if (state.uiSpec?.searchable == true) {
                            SearchBar(
                                query = searchText,
                                onQueryChange = viewModel::onSearchTextChanged,
                                placeholder = state.uiSpec.searchPlaceholder
                            )
                        }

                        // Employee list
                        val filteredEmployees = if (searchText.isBlank()) {
                            state.employees
                        } else {
                            state.employees.filter { 
                                it.fullName.contains(searchText, ignoreCase = true) ||
                                it.email.contains(searchText, ignoreCase = true)
                            }
                        }

                        LazyColumn(
                            contentPadding = PaddingValues(16.dp),
                            verticalArrangement = Arrangement.spacedBy(12.dp)
                        ) {
                            items(
                                items = filteredEmployees,
                                key = { it.id }
                            ) { employee ->
                                EmployeeCard(
                                    employee = employee,
                                    onClick = { onNavigateToDetail(employee.id) }
                                )
                            }
                        }
                    }
                }
            }
        }
    }
}
```

---

## 🔄 UI Specification Pattern (Both Platforms)

### Backend provides UI contracts that mobile apps consume

#### **Backend Contract (C#)**
```csharp
// services/HR/HR.Contracts/Specifications/EmployeeListSpec.cs
public class EmployeeListSpec : IUISpecification
{
    public string Title => "Employees";
    public string Icon => "person_outline"; // Material icon name
    public bool Searchable => true;
    public string SearchPlaceholder => "Search by name or email...";

    public List<ColumnDefinition> Columns => new()
    {
        new("fullName", "Name", sortable: true, width: 0.4f),
        new("department", "Department", sortable: true, width: 0.3f),
        new("email", "Email", sortable: false, width: 0.3f)
    };

    public List<ActionDefinition> Actions => new()
    {
        new("add", "Add Employee", "add_circle", "/employees/new", ActionType.Navigation),
        new("export", "Export", "download", null, ActionType.Download)
    };
}
```

#### **Mobile Consumption (iOS)**
```swift
// Domain/Entities/UISpec/EmployeeListSpec.swift
struct EmployeeListSpec: Codable {
    let title: String
    let icon: String?
    let searchable: Bool
    let searchPlaceholder: String?
    let columns: [ColumnDefinition]
    let actions: [ActionDefinition]
}

struct ColumnDefinition: Codable {
    let field: String
    let label: String
    let sortable: Bool
    let width: Float?
}

struct ActionDefinition: Codable {
    let id: String
    let label: String
    let icon: String
    let route: String?
    let actionType: ActionType
}

enum ActionType: String, Codable {
    case navigation
    case download
    case share
}
```

#### **Mobile Consumption (Android)**
```kotlin
// domain/model/EmployeeListSpec.kt
data class EmployeeListSpec(
    val title: String,
    val icon: String?,
    val searchable: Boolean,
    val searchPlaceholder: String?,
    val columns: List<ColumnDefinition>,
    val actions: List<ActionDefinition>
)

data class ColumnDefinition(
    val field: String,
    val label: String,
    val sortable: Boolean,
    val width: Float?
)

data class ActionDefinition(
    val id: String,
    val label: String,
    val icon: String,
    val route: String?,
    val actionType: ActionType
)

enum class ActionType {
    NAVIGATION, DOWNLOAD, SHARE
}
```

---

## 📦 Dependency Injection

### iOS (Using Custom DI Container)

```swift
// Core/DI/Container.swift
class DIContainer {
    static let shared = DIContainer()

    // MARK: - Data Layer
    lazy var apiClient: APIClient = {
        APIClient(baseURL: Configuration.apiBaseURL)
    }()

    lazy var database: BAUDatabase = {
        BAUDatabase()
    }()

    // MARK: - Repositories
    lazy var employeeRepository: IEmployeeRepository = {
        EmployeeRepository(
            apiService: HRService(client: apiClient),
            dao: EmployeeDAO(database: database)
        )
    }()

    lazy var uiSpecRepository: IUISpecRepository = {
        UISpecRepository(
            apiService: UISpecService(client: apiClient),
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
}
```

### Android (Using Hilt)

```kotlin
// core/di/AppModule.kt
@Module
@InstallIn(SingletonComponent::class)
object AppModule {

    @Provides
    @Singleton
    fun provideApiService(): ApiService {
        return Retrofit.Builder()
            .baseUrl(BuildConfig.API_BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiService::class.java)
    }

    @Provides
    @Singleton
    fun provideDatabase(@ApplicationContext context: Context): BAUDatabase {
        return Room.databaseBuilder(
            context,
            BAUDatabase::class.java,
            "bau_database"
        ).build()
    }
}

// core/di/DataModule.kt
@Module
@InstallIn(SingletonComponent::class)
object DataModule {

    @Provides
    @Singleton
    fun provideEmployeeRepository(
        apiService: ApiService,
        database: BAUDatabase,
        mapper: EmployeeMapper
    ): EmployeeRepository {
        return EmployeeRepositoryImpl(
            apiService = apiService,
            employeeDao = database.employeeDao(),
            mapper = mapper
        )
    }
}

// core/di/DomainModule.kt
@Module
@InstallIn(SingletonComponent::class)
object DomainModule {

    @Provides
    fun provideGetEmployeesUseCase(
        repository: EmployeeRepository
    ): GetEmployeesUseCase {
        return GetEmployeesUseCase(repository)
    }
}
```

---

## 📱 Offline-First Strategy

### Data Sync Pattern

**Both iOS and Android should:**
1. **Always read from local database first**
2. **Fetch from API in background**
3. **Update local database**
4. **Notify UI of changes**
5. **Queue offline changes for sync**

### Implementation Example

```swift
// iOS
class EmployeeRepository: IEmployeeRepository {
    func getEmployees() async throws -> [Employee] {
        // 1. Return cached data immediately
        let cached = try await dao.getAll()

        // 2. Fetch fresh data in background
        Task {
            do {
                let fresh = try await apiService.getEmployees()
                try await dao.deleteAll()
                try await dao.insertAll(fresh.map { EmployeeEntity(from: $0) })

                // 3. Notify observers
                NotificationCenter.default.post(name: .employeesUpdated, object: nil)
            } catch {
                // Silent fail - user still has cached data
                Logger.error("Failed to sync employees: \(error)")
            }
        }

        return cached.map { $0.toDomain() }
    }

    func createEmployee(_ employee: Employee) async throws -> Employee {
        // 1. Save locally first
        let entity = EmployeeEntity(from: employee)
        try await dao.insert(entity)

        // 2. Queue for sync
        let syncItem = SyncQueueEntity(
            entityType: "Employee",
            entityId: employee.id,
            operation: .create,
            payload: try JSONEncoder().encode(employee)
        )
        try await syncQueueDAO.insert(syncItem)

        // 3. Try immediate sync
        do {
            let created = try await apiService.createEmployee(EmployeeDTO(from: employee))
            try await syncQueueDAO.delete(syncItem)
            return created.toDomain()
        } catch {
            // Will sync later
            return employee
        }
    }
}
```

---

## 🎓 Best Practices Summary

### ✅ DO:
- **Separate concerns** - strict layer boundaries
- **Use dependency injection** - testability
- **Cache UI specs locally** - reduce API calls
- **Implement offline-first** - better UX
- **Handle errors gracefully** - user-friendly messages
- **Use async/await** - clean concurrency code
- **Write unit tests** - especially for ViewModels and Use Cases
- **Follow platform conventions** - SwiftUI for iOS, Compose for Android
- **Use navigation patterns** - Coordinator (iOS), NavHost (Android)

### ❌ DON'T:
- **Mix layers** - View shouldn't call Repository directly
- **Put business logic in Views** - keep Views dumb
- **Use ViewModels in other ViewModels** - use Use Cases
- **Skip error handling** - apps crash
- **Hardcode strings** - use localization files
- **Block main thread** - use background threads
- **Skip validation** - validate inputs before API calls

---

**Next Steps:**
1. Review this architecture guide
2. Set up iOS project with new structure
3. Set up Android project with Clean Architecture
4. Implement first module (HR) following patterns
5. Add tests for critical paths
6. Document module-specific patterns

*This architecture ensures both mobile apps are maintainable, testable, and aligned with backend services while providing excellent offline-first user experience.*
