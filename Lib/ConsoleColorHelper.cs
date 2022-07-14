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

internal sealed class ConsoleColorHelper {

   private ConsoleColor? _activeForeground;
   private ConsoleColor? _activeBackground;
   private readonly IConsole _console;

   public ConsoleColorHelper(IConsole console) {
      _console = console;
      _console.ResetColor();
      DefaultBackgroundColor = _console.BackgroundColor;
      CommandBackgroundColor = DefaultBackgroundColor;
      ConsoleColor fg = _console.ForegroundColor;
      fg += (fg > ConsoleColor.Gray ? -8 : 8);
      CommandForegroundColor = fg;
   }

   public ConsoleColor CommandBackgroundColor { get; private set; }

   public ConsoleColor CommandForegroundColor { get; private set; }

   public ConsoleColor DefaultBackgroundColor { get; }

   public bool UsesActiveFieldColors
      => _activeBackground is not null && _activeForeground is not null;

   public void SetFieldColors(Field field, bool active) {
      if (active && UsesActiveFieldColors) {
         _console.ForegroundColor = _activeForeground!.Value;
         _console.BackgroundColor = _activeBackground!.Value;
      } else {
         _console.BackgroundColor = field.BackgroundColor ?? DefaultBackgroundColor;
         ConsoleColor fg = _console.ForegroundColor;
         fg += (fg > ConsoleColor.Gray ? -8 : 8);
         _console.ForegroundColor = fg;
      }
   }

   public void SetLabelColors(Field field) {
      _console.BackgroundColor = field.BackgroundColor ?? DefaultBackgroundColor;
      ConsoleColor fg = _console.ForegroundColor;
      _console.ForegroundColor = field.ForegroundColor ?? fg + (fg > ConsoleColor.Gray ? -8 : 8);
   }

   public void SetMessageColors(Field field, StatusMessageKind messageKind) {
      ConsoleColor fg = _console.ForegroundColor;
      ConsoleColor color = messageKind switch {
         StatusMessageKind.Success => ConsoleColor.DarkGreen,
         StatusMessageKind.Alert => ConsoleColor.DarkYellow,
         StatusMessageKind.Error => ConsoleColor.DarkRed,
         _ => fg + (fg > ConsoleColor.Gray ? -8 : 8),
      };
      ConsoleColor altColor = messageKind switch {
         StatusMessageKind.Success => ConsoleColor.Green,
         StatusMessageKind.Alert => ConsoleColor.Yellow,
         StatusMessageKind.Error => ConsoleColor.Red,
         _ => fg,
      };
      _console.BackgroundColor = field.BackgroundColor ?? DefaultBackgroundColor;
      _console.ForegroundColor = (_console.BackgroundColor > ConsoleColor.Gray
         && _console.BackgroundColor != altColor)
         || _console.BackgroundColor == color
         ? altColor
         : color;
   }

   public void UseActiveFieldBackground() {
      _console.ResetColor();
      ConsoleColor bg = _console.BackgroundColor;
      bg += (bg > ConsoleColor.Gray ? -8 : 8);
      _activeBackground = bg;
      ConsoleColor fg = _console.ForegroundColor;
      fg += (fg > ConsoleColor.Gray ? -8 : 8);
      _activeForeground = fg;
   }

   public void UseActiveFieldColors(ConsoleColor fgcolor, ConsoleColor bgcolor)
      => (_activeForeground, _activeBackground) = (fgcolor, bgcolor);

   public void UseCommandColors(ConsoleColor fgcolor, ConsoleColor bgcolor)
      => (CommandForegroundColor, CommandBackgroundColor) = (fgcolor, bgcolor);
}
