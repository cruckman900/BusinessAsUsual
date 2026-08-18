# 🧪 Testing Guide - Course Management & Builder

## What's Been Fixed

### ✅ Course Management Page (`/lms/admin/courses`)

**Fixed Issues:**
1. ✅ "Create New Course" button now opens a dialog
2. ✅ Added "Publish" button (appears on Draft courses)
3. ✅ Added "Unpublish" button (appears on Published courses)
4. ✅ Added status badge showing Draft/Published/Archived
5. ✅ Shows published date when available

**New Features:**
- Create course dialog with fields:
  - Title (required)
  - Description (required)
  - Category
  - Difficulty Level (Beginner/Intermediate/Advanced)
  - Estimated Duration
  - Issues Certificate toggle
- After creating a course, automatically navigates to Course Builder
- Publish/Unpublish buttons update course status
- Status badges color-coded: Draft (Warning/Orange), Published (Success/Green), Archived (Gray)

---

### ✅ Course Builder Page (`/lms/admin/courses/{id}/builder`)

**Module Management:**
1. ✅ "Add Module" button at top of structure panel
2. ✅ Edit button (✏️) for each module
3. ✅ Delete button (🗑️) for each module
4. ✅ Module dialog with fields:
   - Title (required)
   - Description
   - Order Index

**Lesson Management:**
1. ✅ "Add Lesson" button within each module
2. ✅ Edit button (✏️) for each lesson
3. ✅ Delete button (🗑️) for each lesson
4. ✅ Lesson dialog with fields:
   - Title (required)
   - Description
   - Order Index
   - Estimated Duration (minutes)

**Media Support:**
1. ✅ "Add Image" button (creates image content block)
2. ✅ "Add Video" button (creates video content block)
3. ✅ "Add File" button (creates file/document content block)

---

## 🧪 Step-by-Step Test Plan

### Test 1: Create a New Course

1. Navigate to `/lms/admin/courses`
2. Click **"Create New Course"** button
3. **Expected:** Dialog opens with form fields
4. Fill in:
   - Title: "Test Course 101"
   - Description: "This is a test course"
   - Category: "Testing"
   - Difficulty: "Beginner"
   - Duration: 30
   - Certificate: ON
5. Click **"Create Course"**
6. **Expected:**
   - Dialog closes
   - Success message appears
   - Page redirects to `/lms/admin/courses/{newId}/builder`

---

### Test 2: Add Modules to Course

1. In Course Builder (after creating course)
2. Click **"Add Module"** button (top of left panel)
3. **Expected:** Dialog opens
4. Fill in:
   - Title: "Module 1: Introduction"
   - Description: "Getting started"
   - Order: 0
5. Click **"Add Module"**
6. **Expected:**
   - Dialog closes
   - Module appears in left panel
   - "Unsaved Changes" indicator appears
7. Click **"💾 Save Changes"**
8. **Expected:**
   - Success message
   - "Unsaved Changes" disappears

---

### Test 3: Add Lessons to Module

1. Find the module you just created
2. Click **"Add Lesson"** button (bottom of module card)
3. **Expected:** Dialog opens
4. Fill in:
   - Title: "Lesson 1.1: Overview"
   - Description: "Course overview"
   - Order: 0
   - Duration: 15
5. Click **"Add Lesson"**
6. **Expected:**
   - Dialog closes
   - Lesson appears under module
   - "Unsaved Changes" indicator appears
7. Click **"💾 Save Changes"**

---

### Test 4: Add Content Blocks to Lesson

1. Click on the lesson you just created
2. **Expected:** Right panel shows lesson content editor
3. Click **"Add Text"** button
4. **Expected:** Text block appears
5. Click **"Add Image"** button
6. **Expected:** Image block appears with placeholder
7. Click **"Add Video"** button
8. **Expected:** Video block appears with placeholder
9. Click **"Add File"** button
10. **Expected:** File block appears with placeholder
11. Click **"💾 Save Changes"**

---

### Test 5: Edit Module

1. Click the **✏️ (Edit)** button next to a module
2. **Expected:** Dialog opens with existing values
3. Change title to "Module 1: Introduction (Updated)"
4. Click **"Save Changes"**
5. **Expected:**
   - Dialog closes
   - Module title updates
   - "Unsaved Changes" appears
6. Click **"💾 Save Changes"**

---

### Test 6: Edit Lesson

1. Click the **✏️ (Edit)** button next to a lesson
2. **Expected:** Dialog opens with existing values
3. Change duration to 20
4. Click **"Save Changes"**
5. **Expected:**
   - Dialog closes
   - Lesson updates
   - "Unsaved Changes" appears
6. Click **"💾 Save Changes"**

---

### Test 7: Delete Lesson

1. Click the **🗑️ (Delete)** button next to a lesson
2. **Expected:**
   - Lesson removed from list
   - "Unsaved Changes" appears
3. Click **"💾 Save Changes"**

---

### Test 8: Delete Module

1. Click the **🗑️ (Delete)** button next to a module
2. **Expected:**
   - Module removed from list
   - All lessons in module also removed
   - "Unsaved Changes" appears
3. Click **"💾 Save Changes"**

---

### Test 9: Publish Course

1. Navigate back to `/lms/admin/courses`
2. Find your test course (should show **"Draft"** badge in orange)
3. Click **"Publish"** button
4. **Expected:**
   - Status changes to **"Published"** (green badge)
   - Success message appears
   - "Publish" button changes to "Unpublish"
   - Published date appears

---

### Test 10: Unpublish Course

1. On the same course (now Published)
2. Click **"Unpublish"** button
3. **Expected:**
   - Status changes back to **"Draft"** (orange badge)
   - Success message appears
   - "Unpublish" button changes to "Publish"

---

## ❌ Troubleshooting

### Issue: Buttons Don't Respond

**Possible Causes:**
1. JavaScript not loaded
2. Blazor SignalR connection lost
3. Page needs refresh

**Solutions:**
- Hard refresh page (Ctrl+F5)
- Check browser console for errors
- Restart the application
- Clear browser cache

---

### Issue: Dialog Doesn't Open

**Possible Causes:**
1. MudBlazor JavaScript not loaded
2. Dialog state not updating

**Solutions:**
- Check network tab for failed JS loads
- Verify MudBlazor is referenced in _Host.cshtml
- Check browser console for errors

---

### Issue: Changes Not Saving

**Possible Causes:**
1. Database connection issue
2. Repository not injected
3. Validation failing

**Solutions:**
- Check application logs
- Verify database is running
- Check console for exceptions
- Ensure required fields are filled

---

### Issue: "Cannot Publish Course Without Modules"

**This is Expected!**
- Courses must have at least one module before publishing
- Add a module and lesson first
- Then try publishing again

---

## 🎯 Expected Behavior Summary

| Action | Location | Expected Result |
|--------|----------|----------------|
| Click "Create New Course" | Course Management | Dialog opens |
| Fill & submit course form | Course Management | Redirects to Builder |
| Click "Add Module" | Course Builder | Dialog opens |
| Save module | Course Builder | Module appears in list |
| Click "Add Lesson" | Course Builder | Dialog opens |
| Save lesson | Course Builder | Lesson appears under module |
| Click "Add Image/Video/File" | Course Builder | Content block added |
| Click "💾 Save Changes" | Course Builder | Success message, changes persist |
| Click "Edit" (✏️) | Course Builder | Dialog opens with values |
| Click "Delete" (🗑️) | Course Builder | Item removed from list |
| Click "Publish" | Course Management | Status → Published (green) |
| Click "Unpublish" | Course Management | Status → Draft (orange) |

---

## ✅ Confirmation Checklist

Before reporting an issue, verify:

- [ ] Application is running
- [ ] Database is connected
- [ ] Page has fully loaded
- [ ] No browser console errors
- [ ] MudBlazor styles are loaded
- [ ] You're on the correct page URL
- [ ] Required fields are filled in forms
- [ ] You clicked "Save Changes" after edits

---

## 🚀 Next Steps After Testing

Once everything works:

1. **Create a sample course structure:**
   ```
   📚 Project Management Fundamentals
	 📁 Module 1: Introduction
	   📖 Lesson 1.1: What is PM?
		 ✏️ Text: Welcome message
		 🎬 Video: (placeholder for now)
	   📖 Lesson 1.2: Key Concepts
		 ✏️ Text: Definitions
		 🖼️ Image: (placeholder for now)
	 📁 Module 2: Planning
	   📖 Lesson 2.1: Project Plans
		 ✏️ Text: How to plan
		 📎 File: (placeholder for now)
   ```

2. **Publish the course**

3. **Test from learner perspective:**
   - Navigate to `/lms/courses`
   - Find your published course
   - View it as a learner would

---

## 📝 Notes

- **TODO markers** in code indicate where auth needs to be integrated
- Currently uses "admin" as placeholder for user identity
- Media upload functionality (actual file upload) is not yet implemented
- Image/Video/File blocks create placeholders ready for upload integration

---

**All features should now be functional! 🎉**

If buttons still don't work, please share:
1. Browser console errors
2. Application logs
3. Specific error messages
4. Which button/action isn't working
