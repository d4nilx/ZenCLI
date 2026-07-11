# 📸 How to Add Photos & Videos to ZenCLI README

## Quick Start

### 1️⃣ Add Screenshots/Photos

#### Method A: Direct Image Links (Easiest)

Use image hosting services like:
- **GitHub (Recommended)** - Free, reliable, no watermarks
- **Imgur** - Simple, no account needed
- **CloudFront/S3** - Professional option

**Example in README:**
```markdown
<p align="center">
  <img src="https://github.com/d4nilx/ZenCLI/assets/YOUR_GITHUB_ID/FILE_ID/your-screenshot.png" 
       width="600" alt="Description of your screenshot" />
</p>
```

#### Method B: Store in Repository (Version Control)

1. Create an `img/` folder (if not exists):
```bash
mkdir -p img
```

2. Add your images:
```bash
cp /path/to/your/screenshot.png img/main-menu.png
cp /path/to/your/timer-view.png img/timer.png
cp /path/to/your/blocked-site.png img/blocked.png
```

3. Reference in README:
```markdown
<p align="center">
  <img src="img/main-menu.png" width="600" alt="ZenCLI Main Menu" />
</p>
```

4. Commit to git:
```bash
git add img/
git commit -m "Add screenshots and demo images"
```

---

### 2️⃣ Add Videos

#### Option A: Host on GitHub (Up to 10MB)

```markdown
<p align="center">
  <video width="600" controls>
    <source src="img/demo.mp4" type="video/mp4">
    Your browser does not support the video tag.
  </video>
</p>
```

#### Option B: Link to YouTube/Vimeo (Recommended for larger files)

```markdown
<p align="center">
  <a href="https://www.youtube.com/watch?v=YOUR_VIDEO_ID">
    <img src="https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg" 
         width="600" alt="ZenCLI Demo - Click to Watch" />
  </a>
</p>
```

#### Option C: Embed YouTube Video

```markdown
<p align="center">
  <a href="https://www.youtube.com/watch?v=YOUR_VIDEO_ID">
    <img src="https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg" 
         width="600" alt="Watch ZenCLI Demo on YouTube" />
  </a>
</p>

**[Watch Demo on YouTube](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)**
```

---

## Where to Add Them in README

The README has comment placeholders ready:

```markdown
## Features Showcase

<!-- Add your videos/photos here -->

### Startup Animation
[Your images go here]

### Main Menu
[Your images go here]

### Timer in Action
[Your images go here]
```

---

## Step-by-Step Example

### Add a Screenshot to Git

```bash
# 1. Copy your screenshot
cp ~/Pictures/zencli-demo.png img/demo-main-menu.png

# 2. Add and commit
git add img/demo-main-menu.png
git commit -m "Add demo screenshot: main menu"

# 3. Update README
```

Then edit README.md:
```markdown
## Features Showcase

### Main Menu Experience
<p align="center">
  <img src="img/demo-main-menu.png" width="700" alt="ZenCLI Interactive Menu" />
</p>
```

### Add a Video Link

```markdown
## Video Demo

<p align="center">
  <a href="https://youtube.com/watch?v=...">
    <img src="https://img.youtube.com/vi/VIDEO_ID/maxresdefault.jpg" 
         width="700" alt="Watch ZenCLI Pomodoro Timer in Action" />
  </a>
</p>

**[Watch Full Demo on YouTube](https://youtube.com/watch?v=...)**
```

---

## File Format Recommendations

| Type | Format | Max Size | Location |
|------|--------|----------|----------|
| Screenshots | `.png` or `.jpg` | 2-5 MB | `img/` folder |
| Screen Recordings | `.mp4` (H.264) | < 10 MB | `img/` folder |
| Demo Videos | `.mp4` | Any | YouTube/Vimeo |
| GIFs | `.gif` | < 5 MB | `img/` folder |

---

## Example Complete README Section

```markdown
## 🎬 Features in Action

### Startup Animation with Pixel Art
<p align="center">
  <img src="img/startup-animation.png" width="600" alt="Beautiful pixel art startup" />
</p>

### Interactive Menu Navigation
<p align="center">
  <img src="img/main-menu.png" width="600" alt="Main menu with arrow key navigation" />
</p>

### Pomodoro Timer
<p align="center">
  <img src="img/timer-running.png" width="600" alt="Pomodoro timer with progress bar" />
</p>

### Site Blocked Notification
<p align="center">
  <img src="img/site-blocked.png" width="600" alt="Blocked site in browser" />
</p>

### Full Demo Video
<p align="center">
  <a href="https://youtube.com/watch?v=YOUR_VIDEO_ID">
    <img src="https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg" 
         width="700" alt="Watch complete ZenCLI demo" />
  </a>
</p>
```

---

## Pro Tips 🎨

1. **Use consistent sizing** — keep image widths uniform (600-700px)
2. **Add alt text** — helps with accessibility and SEO
3. **Center images** — wrap in `<p align="center">` tags
4. **Compress images** — reduce file size without quality loss:
   ```bash
   # Using ImageMagick
   convert input.png -resize 70% output.png
   
   # Or use online tools: TinyPNG, Squoosh
   ```

5. **For GIFs** — show brief interactions (menu selection, timer countdown)
6. **For videos** — show full workflow or key features

---

## Commands to Get Started

```bash
# Create img directory
mkdir -p img

# Add your files
cp /path/to/screenshot1.png img/main-menu.png
cp /path/to/screenshot2.png img/timer.png

# Commit
git add img/
git commit -m "Add beautiful screenshots and demos"

# Push to GitHub
git push origin main
```

---

**Need help?** Check the [GitHub Docs on Images](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#images)
