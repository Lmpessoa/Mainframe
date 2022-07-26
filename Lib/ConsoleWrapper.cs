/*
 * Copyright (c) 2022 Leonardo Pessoa
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Lmpessoa.Mainframe;

internal sealed class ConsoleWrapper {

   private readonly ConsoleColor _defaultBackground;
   private readonly ConsoleColor _defaultForeground;
   private readonly ConsoleColor _fieldForeground;
   private ConsoleColor _highlightForeground;
   private ConsoleColor _highlightBackground;
   private ConsoleColor? _activeBackground;

   private (int Width, int Height) _screenSize = (80, 24);
   private (int Width, int Height) _initBuffSize;
   private (int Width, int Height) _initWinSize;
   private bool? _enforceSize = null;
   private int _initCurSize;
   private bool _initCtrlC;

   private IConsole Console { get; }

   public ConsoleWrapper(IConsole console) {
      Console = console ?? throw new ArgumentNullException(nameof(console));
      Console.ResetColor();
      _defaultBackground = Console.BackgroundColor;
      _highlightBackground = _defaultBackground;
      _defaultForeground = Console.ForegroundColor;
      _fieldForeground = _defaultForeground + (_defaultForeground > ConsoleColor.Gray ? -8 : 8);
      _highlightForeground = _fieldForeground;
   }

   public (int Left, int Top) CursorPosition {
      get => (Console.CursorLeft, Console.CursorTop);
      set => Console.SetCursorPosition(value.Left, value.Top);
   }

   public int CursorSize {
      get => Console.CursorSize;
      set => Console.CursorSize = value;
   }

   public bool CursorVisible {
      get => Console.CursorVisible;
      set => Console.CursorVisible = value;
   }

   public (int Width, int Height) ScreenSize {
      get => _screenSize;
      set {
         _screenSize = (Math.Max(80, value.Width), Math.Max(24, value.Height));
         if (_enforceSize ?? false) {
            Console.SetWindowSize(_screenSize.Width, _screenSize.Height);
            Console.SetBufferSize(_screenSize.Width, _screenSize.Height);
         }
      }
   }

   public ConsoleKeyInfo? ReadKey()
      => Console.KeyAvailable ? Console.ReadKey() : null;

   public void RestoreState() {
      if (_enforceSize is not null) {
         Console.ResetColor();
         Console.Clear();
         if (_enforceSize ?? false) {
            Console.SetWindowSize(_initWinSize.Width, _initWinSize.Height);
            Console.SetBufferSize(_initBuffSize.Width, _initBuffSize.Height);
         }
         Console.TreatControlCAsInput = _initCtrlC;
         Console.CursorSize = _initCurSize;
         Console.CursorVisible = true;
      }
      _enforceSize = null;
   }

   public void SaveState(bool enforceSize) {
      if (_enforceSize is null) {
         _initCtrlC = Console.TreatControlCAsInput;
         _initCurSize = Console.CursorSize;
         _initBuffSize = (Console.BufferWidth, Console.BufferHeight);
         _initWinSize = (Console.WindowWidth, Console.WindowHeight);
         if (enforceSize || _screenSize.Width > _initWinSize.Width || _screenSize.Height > _initWinSize.Height) {
            Console.SetWindowSize(_screenSize.Width, _screenSize.Height);
            Console.SetBufferSize(_screenSize.Width, _screenSize.Height);
            _enforceSize = true;
         } else {
            _enforceSize = false;
         }
      }
   }

   public void UseActiveFieldBackground() {
      Console.ResetColor();
      ConsoleColor bg = _defaultBackground;
      bg += (bg > ConsoleColor.Gray ? -8 : 8);
      _activeBackground = bg;
   }

   public void UseHighlightColorInBackground(ConsoleColor color)
      => (_highlightForeground, _highlightBackground) = (_defaultBackground, color);

   public void UseHighlightColorInForeground(ConsoleColor color)
      => (_highlightForeground, _highlightBackground) = (color, _defaultBackground);

   public void WritePart(MapPart part) {
      Console.BackgroundColor = part.BackgroundColor switch {
         MapPartColor.Default => _defaultBackground,
         MapPartColor.Highlight => _highlightBackground,
         _ => (ConsoleColor) (int) part.BackgroundColor,
      };
      Console.ForegroundColor = part.ForegroundColor switch {
         MapPartColor.Default => _defaultForeground,
         MapPartColor.Highlight => _highlightForeground,
         _ => (ConsoleColor) (int) part.ForegroundColor,
      };
      Console.Write(part.Text);
   }

   public void WriteField(string value, FieldState state, StatusFieldSeverity severity) {
      Console.BackgroundColor = _defaultBackground;
      Console.ForegroundColor = _fieldForeground;
      if (state is FieldState.Focused or FieldState.Editing) {
         Console.BackgroundColor = _activeBackground ?? _defaultBackground;
      }
      if (severity is not StatusFieldSeverity.None) {
         ConsoleColor color = severity switch {
            StatusFieldSeverity.Success => ConsoleColor.DarkGreen,
            StatusFieldSeverity.Alert => ConsoleColor.DarkYellow,
            StatusFieldSeverity.Error => ConsoleColor.DarkRed,
            _ => _fieldForeground,
         };
         ConsoleColor altColor = severity switch {
            StatusFieldSeverity.Success => ConsoleColor.Green,
            StatusFieldSeverity.Alert => ConsoleColor.Yellow,
            StatusFieldSeverity.Error => ConsoleColor.Red,
            _ => _defaultForeground,
         };
         Console.ForegroundColor = (_defaultBackground > ConsoleColor.Gray
            && _defaultBackground != altColor)
            || _defaultBackground == color
            ? altColor
            : color;
      }
      Console.Write(value.Replace('\0', state is FieldState.Editable or FieldState.Editing ? '_' : ' '));
   }
}
