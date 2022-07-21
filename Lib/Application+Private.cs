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

namespace Lmpessoa.Mainframe;

public sealed partial class Application {

   private static Application? _current = null;

   private readonly ConsoleColorHelper _colors;
   private readonly Map _initialMap;
   private readonly Stack<Map> _maps = new();

   private WindowBorder _border = WindowBorder.Ascii;
   private (int Width, int Height) _consoleSize = (80, 24);
   private bool _enforceSize = false;
   private DateTime _inactiveSince;
   private int _initCurSize;
   private bool _initCtrlC;
   private (int Width, int Height) _initBuffSize;
   private (int Width, int Height) _initWinSize;
   private bool _viewDirty = false;
   private uint _maxIdleTime = 0;
   private char _passwordChar = '*';
   private bool _needsRestore = false;
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
         Field? field = map.CurrentField?.IsEditable ?? false ? map.CurrentField : null;
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
                     field.SetValue(field.Value.Remove(pos, 1));
                  }
               }
               break;
            case "Backspace":
               if (field is not null) {
                  int pos = Console.CursorLeft - field.Left - map.Left;
                  if (pos > 0) {
                     if (field.SetValue(field.Value.Remove(pos - 1, 1))) {
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
                  Console.CursorLeft = map.Left + field.Left + field.Value.TrimEnd('\t').Length;
               }
               break;
            default:
               if (key.Modifiers is 0 or ConsoleModifiers.Shift && (key.KeyChar == ' '
                     || char.IsLetterOrDigit(key.KeyChar)
                     || char.IsPunctuation(key.KeyChar)
                     || char.IsSymbol(key.KeyChar))) {
                  if (field is not null) {
                     string value = field.Value;
                     int pos = Console.CursorLeft - field.Left - map.Left;
                     if (InsertMode && field.Mask != "/") {
                        if (value[^1] != '\t') {
                           break;
                        }
                        value = value[..^1];
                     } else {
                        value = value.Remove(pos, 1);
                     }
                     value = value.Insert(pos, key.KeyChar.ToString());
                     if (field.SetValue(value)) {
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
      if (CurrentMap is not Map map || CurrentMap.CurrentField is not Field field) {
         return;
      }
      int cleft = Console.CursorLeft;
      int ctop = Console.CursorTop;
      Field? newField;
      switch (key) {
         case ConsoleKey.LeftArrow:
            cleft -= cleft > 0 ? 1 : 0;
            if (cleft < map.Left + field.Left) {
               newField = map.Fields.Where(f => map.Top + f.Top == ctop && map.Left + f.Left <= cleft && f.IsFocusable)
                                    .OrderByDescending(f => f.Left)
                                    .FirstOrDefault();
               if (newField is not null) {
                  cleft = map.Left + newField.Left + newField.Width;
               } else {
                  cleft = Console.CursorLeft;
               }
            }
            break;
         case ConsoleKey.RightArrow:
            cleft += cleft < Console.WindowWidth - 1 ? 1 : 0;
            if (cleft > map.Left + field.Left + field.Width) {
               newField = map.Fields.Where(f => f.Top == ctop && f.Left >= cleft && f.IsFocusable)
                                    .OrderBy(f => f.Left)
                                    .FirstOrDefault();
               if (newField is not null) {
                  cleft = map.Left + newField.Left;
               } else {
                  cleft = Console.CursorLeft;
               }
            }
            break;
         case ConsoleKey.UpArrow:
         case ConsoleKey.DownArrow:
            IEnumerable<Field> fields;
            if (key is ConsoleKey.UpArrow) {
               fields = map.Fields
                  .Where(f => f.Top < ctop && f.IsFocusable);
            } else {
               fields = map.Fields
                  .Where(f => f.Top > ctop && f.IsFocusable);
            }
            if (fields.Any()) {
               int nextLine = key is ConsoleKey.UpArrow ? fields.Max(f => f.Top) : fields.Min(f => f.Top);
               newField = fields.Where(f => f.Top == nextLine)
                                .OrderBy(f => (cleft <= map.Left + f.Left
                                   ? map.Left + f.Left - cleft
                                   : cleft - map.Left - f.Left - f.Width))
                                .FirstOrDefault();
               if (newField is not null) {
                  ctop = newField.Top;
                  if (map.Left + newField.Left > cleft) {
                     cleft = map.Left + newField.Left;
                  } else if (cleft > map.Left + newField.Left + newField.Width) {
                     cleft = map.Left + newField.Left + newField.Width;
                  }
               } else {
                  ctop = Console.CursorTop;
               }
            }
            break;
      }
      CurrentMap?.MoveFocusTo(cleft, ctop);
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
         foreach (MapPart fragment in map.Parts) {
            Console.ForegroundColor = fragment.ForegroundColor switch {
               MapPartColor.Default => _fore,
               MapPartColor.Highlight => _colors.CommandForegroundColor,
               _ => (ConsoleColor) (int) fragment.ForegroundColor
            };
            Console.BackgroundColor = fragment.BackgroundColor switch {
               MapPartColor.Default => _back,
               MapPartColor.Highlight => _colors.CommandBackgroundColor,
               _ => (ConsoleColor) (int) fragment.BackgroundColor,
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
               } else if (field.IsEditable) {
                  _colors.SetFieldColors(field, map.CurrentFieldIndex == map.Fields.IndexOf(field));
                  fvalue = field.Value.Replace('\t', '_');
                  if (field.IsProtected) {
                     string masked = "";
                     foreach (char ch in fvalue) {
                        masked += ch == '_' ? '_' : _passwordChar;
                     }
                     fvalue = masked;
                  }
               } else {
                  fvalue = field.Value.Trim('\t');
                  if (fvalue.Length < field.Width) {
                     fvalue += new string(' ', field.Width - fvalue.Length);
                  }
                  if (field.IsStatus) {
                     _colors.SetMessageColors(field, map.StatusKind);
                  } else {
                     _colors.SetLabelColors(field);
                  }
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