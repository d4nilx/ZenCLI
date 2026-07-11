# 🧘 ZenCLI

> A hardcore macOS/Linux CLI tool that helps fight procrastination by taking control away from you.

Instead of relying on willpower, ZenCLI blocks distracting sites at the OS level during focus sessions — no browser extension, no incognito workaround.

---

## Features

- 🎨 **Beautiful Pixel Art UI** — animated gradient-colored pixel art text on startup. Eye-catching visual branding using `PixelArtAnimator` with smooth color transitions (Blue → Cyan → Magenta → Pink).
- 🎬 **Sleek Interactive UI** — beautifully crafted terminal interface using `Spectre.Console`. Navigate menus with arrow keys instead of typing commands! All text now in English for global accessibility.
- 🔒 **Hard blocking** — redirects traffic for blocked sites via `/etc/hosts`. Automatically handles `www.` prefixes and flushes macOS DNS cache for instant effect.
- 📝 **Custom Plans** — build a temporary, multi-task focus session with individual focus and break times. Clears from memory when done.
- ⏱ **Smart Pomodoro** — visual countdown with color-coded progress bars and automatic long/short break management. Stop anytime gracefully by pressing `Q`.
- ⚙️ **Flexible config** — add or remove sites from the block list anytime via the interactive UI.
- 🛡 **Graceful shutdown** — if the process is killed (Ctrl+C) or stopped via `Q`, `/etc/hosts` is automatically restored and sites are unblocked instantly.

---

## Features Showcase

<!-- Add your videos/photos here -->

### Startup Animation
The program displays beautiful animated pixel art text on launch:
```
[blue]█[/] [cyan]█[/] [magenta]█[/] [magenta3]█[/] [hotpink]█[/]
Z E N C L I
```

---

## Requirements

- macOS (Apple Silicon/Intel) or Linux
- [.NET 10 SDK](https://dotnet.microsoft.com/download) *(only needed if building from source)*
- `sudo` access (required to modify `/etc/hosts` and flush DNS)

> ⚠️ **Browser Note:** Works perfectly across all browsers! However, **Safari** has a particularly aggressive DNS cache and might occasionally bypass the block.
>
> 💡 **Important:** You do not need to refresh the blocked pages after starting the timer. Simply **close the tabs of distracting sites before** running the start command!

<!-- Photo placeholders - Add your screenshots here -->
<!-- 
<p align="center">
  <img src="img/your-screenshot-1.png" width="45%" alt="Your description" />
  &nbsp;
  <img src="img/your-screenshot-2.png" width="45%" alt="Your description" />
</p>
-->

---

## Installation

### Option 1 — Download binary (recommended)

1. Download the latest `zen` binary from [Releases](https://github.com/d4nilx/ZenCLI/releases)
2. Install it to your system path:

```bash
sudo cp zen /usr/local/bin/zen
sudo chmod +x /usr/local/bin/zen
```

3. Run it:

```bash
sudo zen start
```

### Option 2 — Build from source

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/d4nilx/ZenCLI.git
cd ZenCLI
dotnet build
```

To publish a self-contained binary:

```bash
# macOS Apple Silicon (M-series)
dotnet publish -c Release -r osx-arm64 --self-contained true

# macOS Intel
dotnet publish -c Release -r osx-x64 --self-contained true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true
```

Then install:
```bash
sudo cp bin/Release/net10.0/<runtime>/publish/ZenCLI /usr/local/bin/zen
sudo chmod +x /usr/local/bin/zen
```

---

## Usage

> Commands that modify `/etc/hosts` require `sudo`.

### Start a focus session

```bash
sudo zen start
```

Starts a classic Pomodoro timer using your configured duration and blocks all sites on your list. 
Use Ctrl+C or press `Q` during the session to execute an emergency stop and unblock sites.

### Manage blocked sites

Navigate through the interactive menu:
- **Add Site** — add a new domain to block
- **Remove Site** — remove a domain from the block list  
- **View Blocked Sites** — see all currently blocked sites

### Create a custom focus plan

```bash
sudo zen
```

Then select "Create Task Plan" from the menu.

Interactive prompt to create a specific list of tasks for the day, each with custom focus and break times.
Automatically cycles through them and blocks distractions.

### Configure timer settings

From the main menu, select "Timer Settings" to adjust:
- Focus duration (default: 25 min)
- Short break duration (default: 5 min)
- Long break duration (default: 15 min)

Settings are saved to `~/.zencli/config.json`.

---

## Project Structure

```
ZenCLI/
├── Program.cs                      # Main orchestrator & Spectre.Console UI (English)
├── Models/
│   ├── ZenConfig.cs                # Configuration models
│   ├── BreakSettings.cs            # Timer settings
│   └── PlanTask.cs                 # Custom plan models
└── Services/
    ├── BlockingService.cs          # /etc/hosts manipulation & DNS flush
    ├── ConfigManager.cs            # JSON serialization
    ├── PomodoroManager.cs          # Progress bars & async timer logic
    └── PixelArtAnimator.cs         # Pixel art text with gradient animations
```

---

## Config

Settings are stored at `~/.zencli/config.json`:

```json
{
  "BlockedSites": ["youtube.com", "reddit.com"],
  "Breaks": {
    "PomodoroDurationMinutes": 25,
    "ShortBreakDurationMinutes": 5,
    "LongBreakDurationMinutes": 15
  }
}
```

---

## Tech Stack

- **C# / .NET 10** — console app with `async/await` and `CancellationToken`
- **System.Text.Json** — config serialization
- **Spectre.Console** — advanced terminal UI components (spinners, tables, progress bars, colored text)
- **PixelArtAnimator** — custom pixel art rendering with gradient color transitions
- **System.Diagnostics.Process** — native macOS commands execution (killall -HUP mDNSResponder)
- **`/etc/hosts`** — OS-level site blocking

---

## Recent Updates

### v1.1.0 - Beautiful UI & Internationalization
- ✨ Added `PixelArtAnimator` service with gradient animations
- 🌐 Translated entire codebase to English
- 🎨 Enhanced startup animation with animated pixel art
- 📊 Improved menu navigation and user experience

---

## Status

✅ **Feature Complete** — all core features implemented:
- Interactive menu system ✓
- Pixel art animations ✓
- Site blocking at OS level ✓
- Custom task plans ✓
- Pomodoro timer with breaks ✓
- Graceful shutdown handling ✓

🔄 **Current Focus** — real-world testing, stability improvements, and documentation.

---

*Built by [Daniil Zhdanov / @d4nilx](https://github.com/d4nilx)*
