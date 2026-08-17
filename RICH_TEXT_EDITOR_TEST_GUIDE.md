# Rich-Text Editor Testing Guide

## 🎸 Proof-of-Concept Complete!

The Radzen.Blazor HtmlEditor has been successfully integrated into the LMS Course Builder.

## ✅ What's Been Implemented

1. **Radzen.Blazor Package** (v11.2.5)
   - Installed and configured in LMS.Web
   - CSS and JavaScript resources loaded
   - Services registered

2. **ContentBlockEditor Component** (`services/LearningManagement/LMS.Web/Components/Shared/ContentBlockEditor.razor`)
   - Wraps Radzen HtmlEditor
   - Handles JSON serialization/deserialization
   - Supports Text, Heading, and Callout block types
   - Features:
	 - Undo/Redo
	 - Bold, Italic, Underline, Strikethrough
	 - Text alignment (left, center, right, justify)
	 - Ordered/Unordered lists
	 - Links
	 - Text color and background
	 - Remove formatting
	 - HTML source view

3. **Enhanced Course Builder** (`services/LearningManagement/LMS.Web/Components/Pages/Builder.razor`)
   - Integrated rich-text editor for content blocks
   - Add new text blocks
   - Delete blocks
   - Edit block content with WYSIWYG editor
   - Unsaved changes tracking
   - Save course functionality

## 🧪 How to Test

### Step 1: Access the Standalone LMS App

The LMS.Web app is currently running at:
- **HTTPS:** https://localhost:59171
- **HTTP:** http://localhost:59173

### Step 2: Navigate to Course Builder

1. Go to https://localhost:59171/courses
2. Browse the course catalog
3. Find "C# Fundamentals for Beginners" (or any published course)
4. Look for the course ID in the URL or data
5. Navigate to `/builder/{courseId}` (replace `{courseId}` with the actual GUID)

### Step 3: Test Rich-Text Editing

1. **View Course Structure**
   - Left panel shows modules and lessons
   - Click on a lesson to view its content

2. **Edit Existing Content**
   - Existing Text/Heading/Callout blocks will show with the HtmlEditor
   - Try formatting existing content:
	 - Select text and make it **bold**, *italic*, or <u>underlined</u>
	 - Change colors
	 - Create lists
	 - Add links
	 - Try different alignments

3. **Add New Content Block**
   - Click "Add Text Block" button
   - A new editable block appears
   - Type content and apply formatting
   - Test all toolbar features

4. **Save Changes**
   - Make edits to content
   - Notice "Unsaved Changes" chip appears
   - Click "Save Course" button
   - Changes should persist to the database

5. **Verify Persistence**
   - Refresh the page
   - Navigate back to the same lesson
   - Verify your formatted content is preserved

### Step 4: Test Edge Cases

1. **HTML Source View**
   - Click the `</>` source button in the editor
   - View the raw HTML
   - Manually edit HTML if needed
   - Switch back to WYSIWYG view

2. **Multiple Blocks**
   - Add several text blocks
   - Edit each independently
   - Delete some blocks
   - Verify order is maintained

3. **Delete Functionality**
   - Click the delete button on a content block
   - Verify it's removed from the UI
   - Save and reload to confirm deletion persisted

## 📝 Content Block JSON Structure

The editor stores content in `ContentBlock.JsonContent` as:
```json
{
  "content": "<p>Your <strong>HTML</strong> content here...</p>"
}
```

## ⚠️ Known Limitations (This is a POC)

1. **No HTML Sanitization** - XSS vulnerability exists (accept trusted content only for now)
2. **No Image Upload** - Image upload endpoint not implemented
3. **Limited Block Types** - Only Text, Heading, and Callout blocks support rich-text editing
4. **No Validation** - No client-side validation of content length or structure
5. **No Auto-Save** - Must manually click "Save Course" button
6. **No Error Handling** - Save failures not displayed to user

## 🎯 Next Steps (Beyond POC)

1. **Integrate into BusinessAsUsual.Web** - Add to the integrated shell
2. **Add HTML Sanitization** - Use HtmlSanitizer library to prevent XSS
3. **Implement Image Upload** - Wire up the `UploadUrl` to actual storage
4. **Extend Block Types** - Add rich-text support for CodeSnippet, Image captions, etc.
5. **Add Auto-Save** - Debounced auto-save on content changes
6. **Add Notifications** - Toast/snackbar for save success/failure
7. **Add Validation** - Content length limits, required fields, etc.
8. **Optimize Performance** - Lazy load blocks, virtual scrolling for large lessons

## 🔧 Integration Pattern for BusinessAsUsual.Web

To integrate the rich-text editor into the integrated shell:

### 1. Add Radzen.Blazor Package
```bash
dotnet add frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj package Radzen.Blazor
```

### 2. Register Services (Program.cs)
```csharp
using Radzen;
// ...
builder.Services.AddRadzenComponents();
```

### 3. Add CSS/JS (App.razor or MainLayout)
```html
<link href="_content/Radzen.Blazor/css/material-base.css" rel="stylesheet" />
<script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
```

### 4. Copy Components
- Copy `ContentBlockEditor.razor` to `BusinessAsUsual.Web/Components/Shared/`
- Copy `ContentBlockViewer.razor` to `BusinessAsUsual.Web/Components/Shared/`
- Update namespaces if needed

### 5. Use in Integrated Pages
```razor
<ContentBlockEditor 
    BlockType="@block.BlockType"
    JsonContent="@block.JsonContent"
    OnDelete="@(() => DeleteBlock(block))"
    OnContentChanged="@(async (json) => await UpdateBlockContent(block, json))" />
```

## 📚 Usage Patterns

### Creating New Blocks
```csharp
private void AddBlock(ContentBlockType blockType)
{
    var defaultContent = blockType switch
    {
        ContentBlockType.Heading => """{"content": "<h2>Your Heading Here</h2>"}""",
        ContentBlockType.Callout => """{"content": "<p>💡 Important info...</p>"}""",
        _ => """{"content": "<p>Start typing...</p>"}"""
    };

    var newBlock = new ContentBlock
    {
        Id = Guid.NewGuid(),
        LessonId = currentLesson.Id,
        BlockType = blockType,
        OrderIndex = blocks.Count,
        JsonContent = defaultContent
    };

    blocks.Add(newBlock);
}
```

### Handling Content Changes
```csharp
private Task UpdateBlockContent(ContentBlock block, string json)
{
    block.JsonContent = json;
    hasUnsavedChanges = true;
    return Task.CompletedTask;
}
```

### Saving Changes
```csharp
private async Task SaveChanges()
{
    await repository.UpdateAsync(courseEntity);
    hasUnsavedChanges = false;
}
```

### Rendering Saved Content (View Mode)
```razor
@foreach (var block in lesson.ContentBlocks.OrderBy(b => b.OrderIndex))
{
    <ContentBlockViewer BlockType="@block.BlockType" JsonContent="@block.JsonContent" />
}
```

## 🔗 Related Files

- `services/LearningManagement/LMS.Web/LMS.Web.csproj` - Package reference
- `services/LearningManagement/LMS.Web/Program.cs` - Radzen service registration
- `services/LearningManagement/LMS.Web/Components/App.razor` - CSS/JS includes
- `services/LearningManagement/LMS.Web/Components/Shared/ContentBlockEditor.razor` - Editor component
- `services/LearningManagement/LMS.Web/Components/Pages/Builder.razor` - Course builder page
- `services/LearningManagement/LMS.Domain/Entities/ContentBlock.cs` - Domain model
- `services/LearningManagement/LMS.Infrastructure/Data/LMSSeedData.cs` - Sample courses

## 🎸 Rock On!

The foundation is solid! The editor is working, content saves to the database, and the architecture is clean and extensible. Ready to integrate into the main BusinessAsUsual.Web shell when you're ready!
