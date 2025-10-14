# Document Comparison Tool

A WPF-based document comparison tool built with C# that visually compares differences between two documents. Supports plain text extraction from multiple file formats.

## Features

### 📊 Visual Comparison - **Three Highlight Modes**
- **Side-by-Side Display**: Clear parallel comparison view
- **Three Selectable Highlight Modes**:
  
  #### 🎯 Smart Highlighting (Recommended)
  - Same lines: Light background, comfortable for eyes
  - Modified lines: Light yellow background with precise character-level marking
  - Deleted characters: Red text + strikethrough
  - Inserted characters: Blue text + underline
  - Modified characters: Orange text + bold
  
  #### 📝 Full-Line Highlighting
  - Entire lines marked as same/different
  - Clean and clear, quick overview
  - Traditional comparison tool style
  
  #### ✏️ Character-Level Highlighting
  - Each character precisely marked
  - Same characters: No background
  - Different characters: Highlighted

### ⚙️ Smart Features
- **Punctuation Filtering**: Optional ignore Chinese/English punctuation differences
  - Auto-converts: `，` → `,`, `。` → `.`, `""` → `""`etc.
  - Ideal for comparing mixed Chinese/English documents
  - Focus on substantial content changes

### 📝 File Format Support
- **Plain Text (.txt)**: Direct comparison of raw text
- **Markdown (.md)**: Automatically extract plain text, remove formatting marks
  - Remove heading marks (#)
  - Remove bold, italic, strikethrough marks
  - Remove code blocks and inline code
  - Remove links and image references
  - Remove table separators
  - Remove list markers
- **Fandom WikiText (.wiki, .wikitext)**: Automatically extract plain text (**Fully Enhanced**)
  - ✅ Remove CSS/JavaScript code blocks `{{CSS|...}}`, `<style>`, `<script>`
  - ✅ Remove all code tags `<syntaxhighlight>`, `<code>`, `<pre>`, `<nowiki>`
  - ✅ **Nested template handling** (supports up to 20 levels) `{{Template1|{{Template2|...}}}}`
  - ✅ Remove file/image references `[[File:...]]`, `[[Image:...]]`
  - ✅ Remove categories and interlanguage links `[[Category:...]]`, `[[en:...]]`
  - ✅ **Complete table removal** (supports nested tables) `{| ... |}`, `<table>`
  - ✅ Remove all HTML tags and attributes `<div style="...">`, `<span>`, `<font>`
  - ✅ Process internal/external links `[[Page|Text]]`, `[http://...]`
  - ✅ Remove formatting marks `'''bold'''`, `''italic''`, `== Heading ==`
  - ✅ **HTML entity conversion** `&nbsp;`, `&emsp;`, `&lt;`, `&gt;` etc.
  - ✅ Remove reference tags `<ref>`, `<references>`
  - ✅ Remove magic words `__TOC__`, `__NOTOC__` etc.
  - ✅ Smart cleanup of extra blank lines and spaces
- **All Files (*.*)**: Treated as plain text

### 🎯 Other Features
- **File Loading**: Load documents from file system with automatic format recognition
- **Manual Input**: Directly input or paste content into text boxes
- **Clear Function**: One-click clear all content
- **Instant Comparison**: Re-compare current content at any time

## System Requirements

- .NET 6.0 or higher
- Windows Operating System

## How to Use

### 1. Build the Project

Run in project directory:

```bash
dotnet restore
dotnet build
```

### 2. Run the Program

```bash
dotnet run
```

Or open `ComparisonUtil.csproj` with Visual Studio and run.

### 3. Usage Instructions

#### Method 1: Using File Loading (Recommended)

1. **Load Left Document**: 
   - Click "Load Left Document" button
   - Select file format in file dialog:
     - **Plain Text (*.txt)** - No processing
     - **Markdown File (*.md)** - Auto-extract plain text content
     - **Fandom WikiText (*.wiki;*.wikitext)** - Auto-extract plain text content
     - **All Files (*.*)** - Treated as plain text
   - Select the file to compare

2. **Load Right Document**: 
   - Click "Load Right Document" button
   - Similarly select appropriate file format
   - Select the second file

3. **Start Comparison**: 
   - Click "Start Comparison" button
   - View color-highlighted comparison results

#### Method 2: Manual Input

1. Directly input or paste content into left or right text box
2. Click "Start Comparison" button
3. Immediately see the differences

#### Other Features

- **Clear Content**: Click "Clear" button to empty all text boxes
- **Re-compare**: After modifying text, click "Start Comparison" anytime to re-analyze

## Highlight Modes Explained

### 🎯 Smart Highlighting (Recommended)

**Best readability!**

- Same lines: Light green background, comfortable
- Modified lines: Light yellow background with character-level precise marking
- Deleted characters: **Red text + strikethrough + bold**
- Inserted characters: **Blue text + underline + bold**
- Modified characters: **Orange text + bold**

**Use Cases:**
- ✅ Daily document comparison
- ✅ Long reading sessions
- ✅ Quick difference spotting
- ✅ **Recommended for 95% of users**

### 📝 Full-Line Highlighting

**Traditional comparison tool style**

- Entire lines marked with single color
- Does not show character-level differences
- Quick overview of which lines changed

**Use Cases:**
- ✅ Quick browse of large content
- ✅ Only need to know which lines changed
- ✅ Similar to WinMerge experience

### ✏️ Character-Level Highlighting

**Extremely precise mode**

- Each character precisely marked
- Same characters: No background
- Different characters: Bright background colors

**Use Cases:**
- ✅ Need extreme precision
- ✅ Compare code, configuration files
- ⚠️ May cause eye strain with prolonged use

## Punctuation Filtering

### What It Does

When enabled, automatically standardizes Chinese/English punctuation marks during comparison:

| Chinese | Converts to | Description |
|---------|-------------|-------------|
| `，` | `,` | Comma |
| `。` | `.` | Period |
| `；` | `;` | Semicolon |
| `：` | `:` | Colon |
| `？` | `?` | Question mark |
| `！` | `!` | Exclamation mark |
| `'` `'` | `'` | Single quotes |
| `"` `"` | `"` | Double quotes |
| `（` `）` | `(` `)` | Parentheses |
| `【` `】` | `[` `]` | Brackets |
| `《` `》` | `<` `>` | Angle brackets |
| `、` | `,` | Enumeration comma |
| `—` | `-` | Dash |
| `…` | `.` | Ellipsis |

### When to Use

- ✅ Comparing mixed Chinese/English documents
- ✅ Different input habits between people
- ✅ Text from different sources (web, PDF, Word)
- ✅ Focus on substantial content changes
- ✅ OCR recognition result comparison

### When NOT to Use

- ❌ Need strict punctuation comparison
- ❌ Comparing code (punctuation is important)
- ❌ Comparing configuration files (punctuation is syntax)

## Use Cases

### 1. Code Review
Compare two versions of code files, quickly find modifications.

### 2. Document Version Control
Compare different versions of documents, understand content updates.

### 3. Markdown Document Comparison
Compare actual content of two Markdown documents, ignore format differences.

### 4. Wiki Content Migration
Before migrating Fandom Wiki content to other platforms, compare plain text content for consistency.

### 5. Translation Proofreading
Compare original and translated text (after extracting plain text), ensure translation completeness.

### 6. Configuration File Comparison
Compare differences between two configuration files, quickly locate configuration changes.

### 7. Novel/Article Proofreading
Compare two versions of novel chapters or articles, spot editorial changes precisely.

## Tips

### Tip 1: Choose the Correct File Format

**Important**: Selecting the correct file type in the file dialog is crucial!

- To compare Markdown document **content**, select "Markdown File (*.md)"
- To compare Markdown document **raw format**, select "Plain Text (*.txt)" or "All Files (*.*)"
- Same rule applies to WikiText files

### Tip 2: Use Sample Files

Project includes test samples to familiarize with features:

1. **Plain Text Test**:
   - Left: Load `sample1.txt`
   - Right: Load `sample2.txt`

2. **Markdown Test**:
   - Left: Load `sample_markdown1.md` (select Markdown format)
   - Right: Load `sample_markdown2.md` (select Markdown format)

3. **WikiText Test**:
   - Left: Load `sample_wikitext1.wiki` (select Fandom WikiText format)
   - Right: Load `sample_wikitext2.wiki` (select Fandom WikiText format)

### Tip 3: Flexible Mode Switching

**No need to reload documents!**
1. Load documents once
2. Click "Start Comparison"
3. If not satisfied, switch mode directly
4. Click "Start Comparison" again

### Tip 4: Combined Use of Modes

**Best Practice:**
1. Step 1: Full-line highlighting - Quick overview, find changed regions
2. Step 2: Smart highlighting - Carefully view specific changes
3. Step 3: Character-level highlighting - For critical parts, precise to each character

## Technology Stack

- **C# / .NET 6.0**
- **WPF (Windows Presentation Foundation)**: For building user interface
- **DiffPlex**: For implementing efficient text difference algorithm

## Project Structure

```
ComparisonUtil/
├── App.xaml                  # WPF application definition
├── App.xaml.cs               # Application code-behind
├── MainWindow.xaml           # Main window UI definition
├── MainWindow.xaml.cs        # Main window logic (file loading, comparison display)
├── DiffHelper.cs             # Document comparison core algorithm
├── TextProcessor.cs          # Text format processor (Markdown/WikiText)
├── ComparisonUtil.csproj     # Project configuration file
├── README.md                 # Project documentation (Chinese)
├── README_EN.md              # Project documentation (English)
├── 用户手册.md               # User manual (Chinese)
├── CHANGELOG.md              # Change log
└── ExampleAssets/            # Sample files for testing
    ├── sample1.txt           # Plain text sample
    ├── sample2.txt           # Plain text sample
    ├── sample_markdown1.md   # Markdown sample
    ├── sample_markdown2.md   # Markdown sample
    ├── sample_wikitext1.wiki # WikiText sample
    └── sample_wikitext2.wiki # WikiText sample
```

## Comparison with Other Tools

| Feature | This Tool | Beyond Compare | WinMerge | VSCode Diff |
|---------|-----------|----------------|----------|-------------|
| Character-level highlighting | ✅ Yes | ✅ Yes | ❌ No | ⚠️ Partial |
| Smart highlight mode | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Wiki text extraction | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Markdown extraction | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Punctuation filtering | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Free to use | ✅ Yes | ❌ No (Paid) | ✅ Yes | ✅ Yes |
| Chinese interface | ✅ Yes | ✅ Yes | ✅ Yes | ⚠️ Mainly English |
| Portability | ✅ Single file | ❌ Requires install | ❌ Requires install | ❌ Requires install |

## Frequently Asked Questions

### Q1: Why isn't my Markdown formatting removed?
**A**: Ensure you selected "Markdown File (*.md)" option in the file dialog, not "Plain Text" or "All Files".

### Q2: Can I compare very large files?
**A**: Yes, but processing speed decreases with file size. Recommend files under 10MB each.

### Q3: Colors don't look obvious, what should I do?
**A**: The new version has optimized color contrast. Ensure you're using the latest version. You can also try switching different highlight modes.

### Q4: Can I export comparison results?
**A**: Current version doesn't support export. You can take screenshots to save results.

### Q5: Are other formats supported?
**A**: Currently supports plain text, Markdown, and WikiText. If you need other formats, please submit a request.

### Q6: Which mode should I use?
**A**: Use **Smart Highlighting** in most cases!

- 📝 Daily document comparison → **Smart Highlighting**
- 🚀 Quick overview → **Full-line Highlighting**
- 🔍 Need extreme precision → **Character-level Highlighting**
- ⚙️ Unsure → Try all three, find what's most comfortable!

### Q7: Does punctuation filtering modify my documents?
**A**: **No!** Only standardizes temporarily during comparison, doesn't modify original files.

- ✅ Original files remain untouched
- ✅ Only converts in memory
- ✅ Can uncheck next time

## License

MIT License

## Author

lhx077

## Documentation

- [中文文档 (Chinese Documentation)](README.md)
- [用户手册 (User Manual - Chinese)](用户手册.md)

---

**Enjoy using the tool!** 🎉

