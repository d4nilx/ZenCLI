# 🧘 ZenCLI

> A hardcore macOS/Linux CLI tool that helps fight procrastination by taking control away from you.

Instead of relying on willpower, ZenCLI blocks distracting sites at the OS level during focus sessions — no browser extension, no incognito workaround.

---

## Features

- 🔒 **Hard blocking** — redirects traffic for blocked sites via `/etc/hosts`. Incognito mode won't help.
- ⏱ **Terminal timer** — visual countdown with color-coded progress (green → yellow → red).
- ⚙️ **Flexible config** — add or remove sites from the block list anytime via simple commands.
- 🛑 **In-session stop** — type `stop` while the timer is running to end the session early.
- 🛡 **Graceful shutdown** — if the process is killed (Ctrl+C), `/etc/hosts` is automatically restored.

---

## Requirements

- macOS or Linux
- [.NET 10 SDK](https://dotnet.microsoft.com/download) *(only needed if building from source)*
- `sudo` access (required to modify `/etc/hosts`)

> ⚠️ **Known limitation:** Brave and Opera use their own DNS resolver and may bypass the block. Works best with Safari, Chrome, and Firefox.

---

## Installation

### Option 1 — Download binary (recommended)

1. Download the latest `zen` binary from [Releases](https://github.com/d4nilx/ZenCLI/releases)
2. Install it:

```bash
sudo cp zen /usr/local/bin/zen
sudo chmod +x /usr/local/bin/zen
```

3. Run it:

```bash
zen start
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
# macOS Apple Silicon
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

Starts a timer using your configured duration and blocks all sites on your list.
Type `stop` at any time to end the session early — sites will be unblocked immediately.

### Manage blocked sites

```bash
sudo zen add youtube.com       # Add a site to the block list
sudo zen remove youtube.com    # Remove a site from the block list
zen lst                        # View your current block list
```

### Configure timer settings

```bash
zen tms
```

Set focus duration, short break, and long break durations. Settings are saved to `~/.zencli/config.json`.

---

## Project Structure

```
ZenCLI/
├── Program.cs
├── Commands/
├── Models/
│   ├── ZenConfig.cs
│   └── BreakSettings.cs
└── Services/
    ├── BlockingService.cs
    ├── ConfigManager.cs
    └── PomodoroManager.cs
```

---

## Config

Settings are stored at `~/.zencli/config.json`:

```json
{
  "BlockedSites": ["youtube.com", "reddit.com"],
  "Breaks": {
    "PomodoroDurationMinutes": 25,
    "ShortBreakDuration": 5,
    "LongBreakDuration": 15
  }
}
```

---

## Tech Stack

- **C# / .NET 10** — console app with `async/await` and `CancellationToken`
- **System.Text.Json** — config serialization
- **`/etc/hosts`** — OS-level site blocking

---

## Status

🚧 Active development — core features working, `zen tms` and automatic break logic in progress.

---

*Built by [@d4nilx](https://github.com/d4nilx)*
