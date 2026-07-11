using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console;

namespace ZenCLI.Services;

/// <summary>
/// Creates beautiful pixel art text with gradient animations and typewriter effects.
/// Supports multi-line pixel art with smooth color transitions.
/// </summary>
public class PixelArtAnimator
{
    private readonly Dictionary<char, List<string>> _pixelFonts = new()
    {
        {
            'A', new List<string>
            {
                "███",
                "█ █",
                "███",
                "█ █",
                "█ █"
            }
        },
        {
            'B', new List<string>
            {
                "██ ",
                "█ █",
                "██ ",
                "█ █",
                "██ "
            }
        },
        {
            'C', new List<string>
            {
                " ██",
                "█  ",
                "█  ",
                "█  ",
                " ██"
            }
        },
        {
            'D', new List<string>
            {
                "██ ",
                "█ █",
                "█ █",
                "█ █",
                "██ "
            }
        },
        {
            'E', new List<string>
            {
                "███",
                "█  ",
                "██ ",
                "█  ",
                "███"
            }
        },
        {
            'F', new List<string>
            {
                "███",
                "█  ",
                "██ ",
                "█  ",
                "█  "
            }
        },
        {
            'G', new List<string>
            {
                " ██",
                "█  ",
                "█ █",
                "█ █",
                " ██"
            }
        },
        {
            'H', new List<string>
            {
                "█ █",
                "█ █",
                "███",
                "█ █",
                "█ █"
            }
        },
        {
            'I', new List<string>
            {
                "███",
                " █ ",
                " █ ",
                " █ ",
                "███"
            }
        },
        {
            'J', new List<string>
            {
                "███",
                "  █",
                "  █",
                "█ █",
                " █ "
            }
        },
        {
            'K', new List<string>
            {
                "█ █",
                "█ █",
                "██ ",
                "█ █",
                "█ █"
            }
        },
        {
            'L', new List<string>
            {
                "█  ",
                "█  ",
                "█  ",
                "█  ",
                "███"
            }
        },
        {
            'M', new List<string>
            {
                "█ █",
                "███",
                "███",
                "█ █",
                "█ █"
            }
        },
        {
            'N', new List<string>
            {
                "██ ",
                "███",
                "█ █",
                "█ █",
                "█ █"
            }
        },
        {
            'O', new List<string>
            {
                " █ ",
                "█ █",
                "█ █",
                "█ █",
                " █ "
            }
        },
        {
            'P', new List<string>
            {
                "██ ",
                "█ █",
                "██ ",
                "█  ",
                "█  "
            }
        },
        {
            'Q', new List<string>
            {
                " █ ",
                "█ █",
                "█ █",
                "█ █",
                " ██"
            }
        },
        {
            'R', new List<string>
            {
                "██ ",
                "█ █",
                "██ ",
                "█ █",
                "█ █"
            }
        },
        {
            'S', new List<string>
            {
                " ██",
                "█  ",
                " █ ",
                "  █",
                "██ "
            }
        },
        {
            'T', new List<string>
            {
                "███",
                " █ ",
                " █ ",
                " █ ",
                " █ "
            }
        },
        {
            'U', new List<string>
            {
                "█ █",
                "█ █",
                "█ █",
                "█ █",
                " █ "
            }
        },
        {
            'V', new List<string>
            {
                "█ █",
                "█ █",
                "█ █",
                " █ ",
                " █ "
            }
        },
        {
            'W', new List<string>
            {
                "█ █",
                "█ █",
                "███",
                "███",
                "█ █"
            }
        },
        {
            'X', new List<string>
            {
                "█ █",
                "█ █",
                " █ ",
                "█ █",
                "█ █"
            }
        },
        {
            'Y', new List<string>
            {
                "█ █",
                "█ █",
                " █ ",
                " █ ",
                " █ "
            }
        },
        {
            'Z', new List<string>
            {
                "███",
                "  █",
                " █ ",
                "█  ",
                "███"
            }
        },
        {
            ' ', new List<string>
            {
                "   ",
                "   ",
                "   ",
                "   ",
                "   "
            }
        }
    };

    private readonly Color[] _gradientColors = new[]
    {
        Color.Blue,
        Color.Cyan1,
        Color.Magenta1,
        Color.Magenta3,
        Color.HotPink
    };

    /// <summary>
    /// Gets gradient color based on position (0 to 1)
    /// Transitions: Blue -> Cyan -> Magenta -> Pink
    /// </summary>
    private Color GetGradientColor(float position)
    {
        position = Math.Max(0, Math.Min(1, position));
        int colorIndex = (int)(position * (_gradientColors.Length - 1));
        return _gradientColors[Math.Min(colorIndex, _gradientColors.Length - 1)];
    }

    /// <summary>
    /// Displays animated pixel art text with typewriter effect and gradient colors
    /// </summary>
    public async Task DisplayAnimatedTextAsync(string text, int delayMs = 50, bool useGradient = true)
    {
        var textUpper = text.ToUpper();
        var lines = GetPixelTextLines(textUpper);

        foreach (var line in lines)
        {
            var markup = BuildMarkupWithGradient(line, useGradient);
            AnsiConsole.MarkupLine(markup);
            await Task.Delay(delayMs);
        }
    }

    /// <summary>
    /// Displays pixel art text without animation
    /// </summary>
    public void DisplayPixelText(string text, bool useGradient = true)
    {
        var textUpper = text.ToUpper();
        var lines = GetPixelTextLines(textUpper);

        foreach (var line in lines)
        {
            var markup = BuildMarkupWithGradient(line, useGradient);
            AnsiConsole.MarkupLine(markup);
        }
    }

    /// <summary>
    /// Displays pixel art with a pulse animation (fade in/out effect)
    /// </summary>
    public async Task DisplayPulseAnimationAsync(string text, int cycles = 3, int delayMs = 100)
    {
        var textUpper = text.ToUpper();
        var lines = GetPixelTextLines(textUpper);

        for (int cycle = 0; cycle < cycles; cycle++)
        {
            foreach (var color in _gradientColors)
            {
                Console.Clear();
                foreach (var line in lines)
                {
                    var markup = BuildMarkupWithColor(line, GetColorName(color));
                    AnsiConsole.MarkupLine(markup);
                }
                await Task.Delay(delayMs);
            }
        }
    }

    /// <summary>
    /// Builds markup with gradient colors for a line of text
    /// </summary>
    private string BuildMarkupWithGradient(string line, bool useGradient)
    {
        if (!useGradient)
        {
            return $"[springgreen3]{line}[/]";
        }

        var markup = "";
        for (int i = 0; i < line.Length; i++)
        {
            var color = GetGradientColor((float)i / Math.Max(1, line.Length - 1));
            var colorName = GetColorName(color);
            markup += $"[{colorName}]{line[i]}[/]";
        }
        return markup;
    }

    /// <summary>
    /// Builds markup with single color for a line of text
    /// </summary>
    private string BuildMarkupWithColor(string line, string colorName)
    {
        return $"[{colorName}]{line}[/]";
    }

    /// <summary>
    /// Converts Color enum to Spectre.Console color name
    /// </summary>
    private string GetColorName(Color color)
    {
        if (color.Equals(Color.Blue)) return "blue";
        if (color.Equals(Color.Cyan1)) return "cyan1";
        if (color.Equals(Color.Magenta1)) return "magenta1";
        if (color.Equals(Color.Magenta3)) return "magenta3";
        if (color.Equals(Color.HotPink)) return "hotpink";
        return "springgreen3";
    }

    /// <summary>
    /// Builds multi-line pixel art from text by stacking character blocks
    /// </summary>
    private List<string> GetPixelTextLines(string text)
    {
        const int charHeight = 5;
        var lines = new List<string>();

        for (int row = 0; row < charHeight; row++)
        {
            var line = "";
            foreach (var ch in text)
            {
                var charUpper = char.ToUpper(ch);
                if (_pixelFonts.TryGetValue(charUpper, out var pixelChar))
                {
                    if (row < pixelChar.Count)
                    {
                        line += pixelChar[row] + " ";
                    }
                }
            }
            lines.Add(line);
        }

        return lines;
    }
}
