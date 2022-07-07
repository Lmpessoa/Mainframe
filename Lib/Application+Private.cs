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

using System.Diagnostics;

namespace Lmpessoa.Terminal;

public sealed partial class Application {

   private static Application? _current = null;

   private readonly ConsoleColorHelper _colors;
   private readonly Map _initialMap;
   private readonly Stack<Map> _maps = new();

   private WindowBorder _border = WindowBorder.Ascii;
   private (int Width, int Height) _consoleSize;
   private DateTime _inactiveSince;
   private int _initCurSize;
   private bool _initCtrlC;
   private (int Width, int Height) _initWinSize;
   private bool _viewDirty = false;
   private uint _maxIdleTime = 0;
   private char _passwordChar = '*';
   private bool _restoreConsole = false;
   private (int Left, int Top) _windowPos = (-1, -1);

   private IConsole Console { get; }

   [StackTraceHidden]
   private void AssertAppIsNotRunning() {
      if (_current == this && IsRunning) {
         throw new InvalidOperationException();
      }
   }

   private void HandleIfKeyPressed() {
      if (Console.KeyAvailable && CurrentMap is Map map) {
         InputField? field = map.CurrentField as InputField;
         ConsoleKeyInfo key = Console.ReadKey();
         switch (SimplifyKeyInfo(key)) {
            case "Tab":
               map.FocusOnNextField();
               break;
            case "Shift+Tab":
               map.FocusOnPreviousField();
               break;
            case "LeftArrow":
            case "RightArrow":
            case "UpArrow":
            case "DownArrow":
               MoveCursor(key.Key);
               break;
            case "Insert":
               InsertMode = !InsertMode;
               Console.CursorSize = InsertMode ? 1 : 100;
               break;
            case "Delete":
               if (field is not null) {
                  int pos = Console.CursorLeft - field.Left - map.Left;
                  if (pos < field.Width) {
                     field.SetInnerValue(field.GetInnerValue().Remove(pos, 1));
                  }
               }
               break;
            case "Backspace":
               if (field is not null) {
                  int pos = Console.CursorLeft - field.Left - map.Left;
                  if (pos > 0) {
                     if (field.SetInnerValue(field.GetInnerValue().Remove(pos - 1, 1))) {
                        Console.CursorLeft -= 1;
                     }
                  }
               }
               break;
            case "Home":
               if (field is not null) {
                  Console.CursorLeft = map.Left + field.Left;
               }
               break;
            case "End":
               if (field is not null) {
                  Console.CursorLeft = map.Left + field.Left + field.GetInnerValue().TrimEnd('\t').Length;
               }
               break;
            default:
               if (key.Modifiers is 0 or ConsoleModifiers.Shift && (key.KeyChar == ' '
                     || char.IsLetterOrDigit(key.KeyChar)
                     || char.IsPunctuation(key.KeyChar)
                     || char.IsSymbol(key.KeyChar))) {
                  if (field is not null) {
                     string value = field.GetInnerValue();
                     int pos = Console.CursorLeft - field.Left - map.Left;
                     if (InsertMode && field is not CheckField) {
                        if (value[^1] != '\t') {
                           break;
                        }
                     } else {
                        value = value.Remove(pos, 1);
                     }
                     value = value.Insert(pos, key.KeyChar.ToString());
                     if (field.SetInnerValue(value)) {
                        Console.CursorLeft += 1;
                        if (Console.CursorLeft >= map.Left + field.Left + field.Width) {
                           map.FocusOnNextField();
                        }
                     }
                  }
               } else if (key.Modifiers == 0 && key.Key == 0) {
                  _viewDirty = true;
                  RedrawMapIfNeeded();
               } else {
                  map.DidKeyPress(key);
               }
               break;
         }
         _inactiveSince = DateTime.Now;
      }
   }

   private static string SimplifyKeyInfo(ConsoleKeyInfo info) {
      string result = "";
      result += info.Modifiers.HasFlag(ConsoleModifiers.Control) ? "Ctrl+" : "";
      result += info.Modifiers.HasFlag(ConsoleModifiers.Alt) ? "Alt+" : "";
      result += info.Modifiers.HasFlag(ConsoleModifiers.Shift) ? "Shift+" : "";
      result += info.Key.ToString();
      return result;
   }

   private void MoveCursor(ConsoleKey key) {
      switch (key) {
         case ConsoleKey.LeftArrow:
            if (Console.CursorLeft > 0) {
               Console.CursorLeft -= 1;
            }
            break;
         case ConsoleKey.RightArrow:
            if (Console.CursorLeft < Console.WindowWidth - 1) {
               Console.CursorLeft += 1;
            }
            break;
         case ConsoleKey.UpArrow:
            if (Console.CursorTop > 0) {
               Console.CursorTop -= 1;
            }
            break;
         case ConsoleKey.DownArrow:
            if (Console.CursorTop < Console.WindowHeight - 1) {
               Console.CursorTop += 1;
            }
            break;
      }
      CurrentMap?.MoveFocusTo(Console.CursorLeft, Console.CursorTop);
   }

   private void RedrawMapIfNeeded() {
      if (_viewDirty && CurrentMap is Map map) {
         Console.ResetColor();
         if (map.IsInWindow) {
            if (_border != WindowBorder.None) {
               string borderChars = _border switch {
                  WindowBorder.Square => "┌─┐│└─┘",
                  WindowBorder.Rounded => "╭─╮│╰─╯",
                  WindowBorder.Heavy => "┏━┓┃┗━┛",
                  WindowBorder.Double => "╔═╗║╚═╝",
                  _ => "+-+|+-+",
               };
               Console.SetCursorPosition(map.Left - 1, map.Top - 1);
               Console.Write($"{borderChars[0]}{new string(borderChars[1], map.Width)}{borderChars[2]}");
               Console.SetCursorPosition(map.Left - 1, map.Top + map.Height);
               Console.Write($"{borderChars[4]}{new string(borderChars[5], map.Width)}{borderChars[6]}");
               string vertBorder = $"{borderChars[3]}{new string(' ', map.Width)}{borderChars[3]}";
               for (int i = 0; i < map.Height; ++i) {
                  Console.SetCursorPosition(map.Left - 1, map.Top + i);
                  Console.Write(vertBorder);
               }
            }
            Console.SetCursorPosition(map.Left, map.Top);
         } else {
            Console.SetCursorPosition(0, 0);
         }
         ConsoleColor _fore = Console.ForegroundColor;
         ConsoleColor _back = Console.BackgroundColor;
         foreach (MapFragment fragment in map.ContentFragments) {
            Console.ForegroundColor = (int) fragment.ForegroundColor switch {
               -1 => _fore,
               16 => _colors.CommandForegroundColor,
               _ => fragment.ForegroundColor
            };
            Console.BackgroundColor = (int) fragment.BackgroundColor switch {
               -1 => (int) fragment.ForegroundColor != 16 ? _back : _colors.CommandBackgroundColor,
               16 => _colors.CommandBackgroundColor,
               _ => fragment.BackgroundColor,
            };
            Console.Write(fragment.Text);
            if (fragment.LineBreak) {
               if (!map.IsInWindow && Console.WindowWidth - Console.CursorLeft > 0) {
                  Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
               }
               Console.SetCursorPosition(map.Left, Console.CursorTop + 1);
            }
         }
         if (!map.IsInWindow) {
            Console.ResetColor();
            for (int y = Console.CursorTop; y < Console.WindowHeight; ++y) {
               Console.SetCursorPosition(0, y);
               Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
            }
         }
      }
   }

   private void RedrawFieldsIfNeeded() {
      if (CurrentMap is Map map) {
         foreach (Field field in map.Fields) {
            if (_viewDirty || field.IsDirty) {
               Console.ResetColor();
               Console.SetCursorPosition(field.Left + map.Left, field.Top + map.Top);
               string fvalue;
               if (!field.IsVisible) {
                  fvalue = new string(' ', field.Width);
                  Console.BackgroundColor = field.BackgroundColor ?? Console.BackgroundColor;
                  if (field == map.CurrentField) {
                     map.MoveFocusTo(Console.CursorLeft, Console.CursorTop);
                  }
               } else if (field is InputField input) {
                  _colors.SetFieldColors(field, map.CurrentFieldIndex == map.Fields.IndexOf(field));
                  fvalue = input.GetInnerValue().Replace('\t', '_');
                  if (input is TextField text && text.IsPasswordField) {
                     string masked = "";
                     foreach (char ch in fvalue) {
                        masked += ch == '_' ? '_' : _passwordChar;
                     }
                     fvalue = masked;
                  }
               } else if (field is Label label) {
                  fvalue = label.Value ?? "";
                  if (fvalue.Length < field.Width) {
                     fvalue += new string(' ', field.Width - fvalue.Length);
                  }
                  if (field is StatusMessage msg) {
                     _colors.SetMessageColors(msg);
                  } else {
                     _colors.SetLabelColors(field);
                  }
               } else {
                  continue;
               }
               if (!PreserveValues && field != map.CurrentField) {
                  fvalue = fvalue.ToUpper();
               }
               Console.Write(fvalue[0..Math.Min(fvalue.Length, field.Width)]);
               field.IsDirty = false;
            }
         }
      }
   }
}