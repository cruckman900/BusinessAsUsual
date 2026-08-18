# 🎨 Course Builder UI - Feature Map

## Current Course Builder Layout

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ 🎨 Course Builder: Introduction to Project Management                       │
│ [Draft] [Beginner]                                                          │
└─────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────┬──────────────────────────────────────────────┐
│  📋 Course Structure         │  ✏️ Content Editor                           │
│  [+ Add Module]              │                                              │
│                              │  👈 Select a lesson from the left            │
│  3 modules                   │     to view its content                      │
│                              │                                              │
│  ┌─────────────────────────┐ │                                              │
│  │ 📁 Module 1: Intro      │ │                                              │
│  │ Getting started...      │ │                                              │
│  │ Order: 0                │ │                                              │
│  │ [✏️] [🗑️]              │ │                                              │
│  │ ─────────────────────── │ │                                              │
│  │ 📖 2 lessons            │ │                                              │
│  │   📄 Lesson 1.1 [✏️][🗑️]│ │                                              │
│  │   📄 Lesson 1.2 [✏️][🗑️]│ │                                              │
│  │ [+ Add Lesson]          │ │                                              │
│  └─────────────────────────┘ │                                              │
│                              │                                              │
│  ┌─────────────────────────┐ │                                              │
│  │ 📁 Module 2: Planning   │ │                                              │
│  │ Core techniques...      │ │                                              │
│  │ Order: 1                │ │                                              │
│  │ [✏️] [🗑️]              │ │                                              │
│  │ ─────────────────────── │ │                                              │
│  │ 📖 1 lesson             │ │                                              │
│  │   📄 Lesson 2.1 [✏️][🗑️]│ │                                              │
│  │ [+ Add Lesson]          │ │                                              │
│  └─────────────────────────┘ │                                              │
│                              │                                              │
└──────────────────────────────┴──────────────────────────────────────────────┘

💡 Use the rich-text editor...         [Unsaved Changes] [💾 Save] [Back]
```

## When a Lesson is Selected

```
┌──────────────────────────────┬──────────────────────────────────────────────┐
│  📋 Course Structure         │  ✏️ Lesson 1.1: What is PM?                  │
│  [+ Add Module]              │  Introduction to project management          │
│                              │  ───────────────────────────────────────     │
│  3 modules                   │                                              │
│                              │  📝 Tabs: [Content Blocks] [Quizzes]         │
│  ┌─────────────────────────┐ │                                              │
│  │ 📁 Module 1: Intro      │ │  ┌─────────────────────────────────────┐    │
│  │ Getting started...      │ │  │ 📝 Text Block                      │    │
│  │ Order: 0                │ │  │ "Welcome to this course..."        │    │
│  │ [✏️] [🗑️]              │ │  │ [🗑️]                              │    │
│  │ ─────────────────────── │ │  └─────────────────────────────────────┘    │
│  │ 📖 2 lessons            │ │                                              │
│  │ → 📄 Lesson 1.1 ✓       │ │  ┌─────────────────────────────────────┐    │
│  │   📄 Lesson 1.2         │ │  │ 🎬 Video Block                     │    │
│  │ [+ Add Lesson]          │ │  │ URL: /media/intro-video.mp4        │    │
│  └─────────────────────────┘ │  │ [🗑️]                              │    │
│                              │  └─────────────────────────────────────┘    │
│  ┌─────────────────────────┐ │                                              │
│  │ 📁 Module 2: Planning   │ │  2 content blocks                            │
│  │ ...                     │ │                                              │
│  └─────────────────────────┘ │  [Add Text] [Add Heading] [Add Callout]      │
│                              │  [Add Image] [Add Video] [Add File]          │
└──────────────────────────────┴──────────────────────────────────────────────┘
```

## Dialog: Add/Edit Module

```
┌───────────────────────────────────────────┐
│ Add Module                            [×] │
├───────────────────────────────────────────┤
│                                           │
│  Module Title *                           │
│  ┌─────────────────────────────────────┐  │
│  │ Introduction to the Basics          │  │
│  └─────────────────────────────────────┘  │
│  Enter a descriptive title                │
│                                           │
│  Description                              │
│  ┌─────────────────────────────────────┐  │
│  │ This module covers the fundamental  │  │
│  │ concepts and terminology...         │  │
│  │                                     │  │
│  └─────────────────────────────────────┘  │
│  Brief overview of what module covers     │
│                                           │
│  Order Index                              │
│  ┌─────────────────────────────────────┐  │
│  │ 0                                   │  │
│  └─────────────────────────────────────┘  │
│  Position in course (0-based)             │
│                                           │
├───────────────────────────────────────────┤
│              [Cancel]  [Add Module]       │
└───────────────────────────────────────────┘
```

## Dialog: Add/Edit Lesson

```
┌───────────────────────────────────────────┐
│ Add Lesson                            [×] │
├───────────────────────────────────────────┤
│                                           │
│  Lesson Title *                           │
│  ┌─────────────────────────────────────┐  │
│  │ Understanding Key Concepts          │  │
│  └─────────────────────────────────────┘  │
│  Enter a descriptive title                │
│                                           │
│  Description                              │
│  ┌─────────────────────────────────────┐  │
│  │ Learn the essential terminology     │  │
│  │ and definitions...                  │  │
│  │                                     │  │
│  └─────────────────────────────────────┘  │
│  Brief overview of lesson content         │
│                                           │
│  Order Index                              │
│  ┌─────────────────────────────────────┐  │
│  │ 0                                   │  │
│  └─────────────────────────────────────┘  │
│  Position in module (0-based)             │
│                                           │
│  Estimated Duration (minutes)             │
│  ┌─────────────────────────────────────┐  │
│  │ 15                                  │  │
│  └─────────────────────────────────────┘  │
│  How long should this lesson take?        │
│                                           │
├───────────────────────────────────────────┤
│              [Cancel]  [Add Lesson]       │
└───────────────────────────────────────────┘
```

## Content Block Types Available

| Button         | Icon | Content Type | JSON Structure                                      |
|----------------|------|--------------|-----------------------------------------------------|
| Add Text       | ➕   | Text         | `{ content: "<p>...</p>" }`                        |
| Add Heading    | 📝   | Heading      | `{ content: "<h2>...</h2>" }`                      |
| Add Callout    | 💡   | Callout      | `{ content: "<p>💡 ...</p>" }`                     |
| **Add Image**  | 🖼️   | **Image**    | `{ url: "", alt: "", caption: "" }`                |
| **Add Video**  | 🎬   | **Video**    | `{ url: "", thumbnail: "", duration: 0 }`          |
| **Add File**   | 📎   | **File**     | `{ url: "", filename: "", size: 0, mimeType: "" }` |
| Add Quiz       | ❓   | Quiz         | Creates quiz with questions                         |

**Bold items** = Newly added in this enhancement!

## User Workflow

### Creating a Complete Course from Scratch

```
1. Navigate to Course Builder
   ↓
2. Click "Add Module"
   → Enter: Title, Description, Order
   → Click "Add Module" (dialog)
   ↓
3. Click "Add Lesson" (within module)
   → Enter: Title, Description, Order, Duration
   → Click "Add Lesson" (dialog)
   ↓
4. Select the new lesson
   ↓
5. Add Content Blocks:
   → Click "Add Text" → Edit in rich editor
   → Click "Add Heading" → Edit heading text
   → Click "Add Image" → (Future: upload image)
   → Click "Add Video" → (Future: upload video)
   → Click "Add File" → (Future: upload document)
   → Click "Add Quiz" → Create questions
   ↓
6. Click "💾 Save Changes"
   ↓
7. Repeat steps 3-6 for more lessons
8. Repeat steps 2-7 for more modules
   ↓
9. (Future) Click "Publish" to make live
```

## Key Features

### ✅ Implemented
- [x] Hierarchical course structure display
- [x] Add/Edit/Delete modules
- [x] Add/Edit/Delete lessons  
- [x] Visual feedback for selected lesson
- [x] Unsaved changes indicator
- [x] One-click save for entire course
- [x] Rich text content blocks
- [x] Quiz integration
- [x] Content block reordering (via OrderIndex)
- [x] Image/Video/File block types (buttons added)
- [x] Responsive layout with sidebar
- [x] MudBlazor dialogs for forms
- [x] Form validation

### 🔲 Next Steps
- [ ] Publish/Unpublish buttons
- [ ] Media upload API
- [ ] MediaUploader component
- [ ] Image preview in content editor
- [ ] Video player in content editor
- [ ] File download in content editor
- [ ] Drag-and-drop content block reordering
- [ ] Course preview mode
- [ ] Version history

## Technical Implementation

### State Management
- `course` - The full course with modules and lessons
- `selectedLesson` - Currently active lesson for editing
- `_hasUnsavedChanges` - Tracks if save is needed
- `_showModuleDialog` - Controls module dialog visibility
- `_showLessonDialog` - Controls lesson dialog visibility
- `_editingModule` / `_editingLesson` - Tracks edit vs. create mode

### Key Methods
- `ShowAddModuleDialog()` - Opens create dialog
- `ShowEditModuleDialog(module)` - Opens edit dialog
- `SaveModule()` - Validates and persists module
- `DeleteModule(module)` - Removes module
- `ShowAddLessonDialog(module)` - Opens create dialog
- `ShowEditLessonDialog(lesson)` - Opens edit dialog
- `SaveLesson()` - Validates and persists lesson
- `DeleteLesson(lesson)` - Removes lesson
- `AddBlock(type)` - Creates new content block
- `DeleteBlock(block)` - Removes content block
- `UpdateBlockContent(block, json)` - Updates block data
- `SaveChanges()` - Persists entire course to database

### Data Flow
```
User Action
	↓
Event Handler (e.g., ShowAddModuleDialog)
	↓
State Update (set dialog fields)
	↓
StateHasChanged() → UI Re-renders
	↓
User Fills Form → Clicks Save
	↓
Validation & Save (e.g., SaveModule)
	↓
Update Course Object
	↓
Set _hasUnsavedChanges = true
	↓
StateHasChanged() → UI shows "Unsaved Changes"
	↓
User Clicks "💾 Save Changes"
	↓
SaveChanges() → CourseRepository.UpdateAsync()
	↓
Database Persisted ✅
```

---

**The Course Builder is now a complete authoring tool! 🎉**
