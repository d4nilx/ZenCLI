# 🧘 ZenCLI

> A hardcore macOS CLI tool that helps fight procrastination by taking control away from you.

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

- macOS
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- `sudo` access (required to modify `/etc/hosts`)

> ⚠️ **Known limitation:** Brave and Opera use their own DNS resolver and may bypass the block. Works best with Safari, Chrome, and Firefox.

---

## Installation

```bash
git clone https://github.com/d4nilx/ZenCLI.git
cd ZenCLI
dotnet build
```

To run commands without `dotnet run --`, publish a self-contained binary:

```bash
dotnet publish -c Release -r osx-arm64 --self-contained true   # macOS Apple Silicon
dotnet publish -c Release -r osx-x64 --self-contained true     # macOS Intel
dotnet publish -c Release -r linux-x64 --self-contained true   # Linux
```

Then add the output folder to your `$PATH`, or create an alias:

```bash
alias zen='/path/to/publish/output/ZenCLI'
```

---

## Usage

> Commands that modify `/etc/hosts` require `sudo`.

### Start a focus session

```bash
sudo dotnet run -- start
```

Starts a timer using your configured duration and blocks all sites on your list.
Type `stop` at any time to end the session early — sites will be unblocked immediately.

### Manage blocked sites

```bash
dotnet run -- add youtube.com       # Add a site to the block list
dotnet run -- remove youtube.com    # Remove a site from the block list
dotnet run -- lst                   # View your current block list
```

### Configure timer settings

```bash
dotnet run -- tms
```

Lets you set focus duration, short break, and long break durations. Settings are saved to `~/.zencli/config.json`.

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

🚧 Active development — core features working, automatic break logic in progress.

---

Built by: Daniil Zhdanov *[[@d4nilx](https://github.com/d4nilx)]*
