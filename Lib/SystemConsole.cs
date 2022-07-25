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

internal sealed class SystemConsole : IConsole {

   private readonly ConsoleColor _defaultBackground;
   private readonly ConsoleColor _defaultForeground;
   private readonly ConsoleColor _fieldForeground;
   private ConsoleColor _highlightForeground;
   private ConsoleColor _highlightBackground;
   private ConsoleColor? _activeBackground;

   public SystemConsole() {
      Console.ResetColor();
      _defaultBackground = Console.BackgroundColor;
      _highlightBackground = _defaultBackground;
      _defaultForeground = Console.ForegroundColor;
      _fieldForeground = _defaultForeground + (_defaultForeground > ConsoleColor.Gray ? -8 : 8);
      _highlightForeground = _fieldForeground;
   }

   public int BufferWidth
      => Console.BufferWidth;

   public int BufferHeight
      => Console.BufferHeight;

   public int CursorLeft {
      get => Console.CursorLeft;
      set => Console.CursorLeft = value;
   }

   public int CursorSize {
      get => Console.CursorSize;
      set => Console.CursorSize = value;
   }

   public int CursorTop {
      get => Console.CursorTop;
      set => Console.CursorTop = value;
   }

   public bool CursorVisible {
      get => Console.CursorVisible;
      set => Console.CursorVisible = value;
   }

   public bool KeyAvailable
      => Console.KeyAvailable;

   public bool TreatControlCAsInput {
      get => Console.TreatControlCAsInput;
      set => Console.TreatControlCAsInput = value;
   }

   public int WindowWidth
      => Console.WindowWidth;

   public int WindowHeight
      => Console.WindowHeight;

   public void Clear() {
      Console.ResetColor();
      Console.Clear();
   }

   public ConsoleKeyInfo ReadKey()
      => Console.ReadKey(true);

   public void SetBufferSize(int width, int height)
      => Console.SetBufferSize(width, height);

   public void SetCursorPosition(int left, int top)
      => Console.SetCursorPosition(left, top);

   public void SetWindowSize(int width, int height)
      => Console.SetWindowSize(width, height);

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

   public void Write(MapPart part) {
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

   public void Write(string value, FieldState state, StatusFieldSeverity severity) {
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
