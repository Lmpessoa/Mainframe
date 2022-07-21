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

using System.Text.RegularExpressions;

namespace Lmpessoa.Mainframe;

[Flags]
internal enum FieldType : byte {
   None = 0,
   Editable = 1,
   Focusable = 2,
   Protected = 4,
   Status = 0x80,
}

internal sealed class Field {

   public Field(Map parent, int left, int top, string arg) {
      Match match = Regex.Match(arg, "^([A-Za-z][A-Za-z0-9\\-_]*):([A-Z]{3})(\\[\\d+\\]|\\([^\\]]+\\))?$");
      if (!match.Success) {
         throw new InvalidFieldException($"Invalid field definition", left, top);
      }
      string name = match.Groups[1].Value;
      string type = match.Groups[2].Value;
      string args = match.Groups[3].Value;
      if (type == "CHK" && args != "" && !Regex.IsMatch(args, "\\(\\d+\\)")) {
         throw new InvalidFieldException($"Invalid field definition: {name}", 0, 0);
      }
      Top = top;
      Left = left;
      Name = name;
      Parent = parent;
      Type = type switch {
         "PWD" => FieldType.Editable | FieldType.Focusable | FieldType.Protected,
         "INP" => FieldType.Editable | FieldType.Focusable,
         "CHK" => FieldType.Editable | FieldType.Focusable,
         "ROT" => FieldType.None,
         "STA" => FieldType.Status,
         _ => throw new InvalidFieldException($"'{type}' is not a valid field type", left, top),
      };
      Width = args.Length == 0 || type == "CHK" ? 1
         : args[0] == '[' ? int.Parse(args[1..^1])
         : args.Length - 2;
      Mask = type switch {
         "INP" => args[0] == '(' ? args[1..^1] : null,
         "CHK" => "/",
         _ => null,
      };
      if (type is "CHK") {
         Group = args.Length > 0 ? byte.Parse(args[1..^1]) : 0;
      }
      _value = new('\0', Width);
   }

   public int? Group { get; init; }

   public bool IsChecked
      => Group is not null && Value.Trim('\0').Trim() != "";

   public bool IsDirty { get; set; } = false;

   public bool IsEditable
      => Type.HasFlag(FieldType.Editable);

   public bool IsFocusable
      => Type.HasFlag(FieldType.Focusable) && IsVisible;

   public bool IsProtected
      => Type.HasFlag(FieldType.Protected);

   public bool IsReadOnly
      => !Type.HasFlag(FieldType.Editable);

   public bool IsStatus
      => Type.HasFlag(FieldType.Status);

   public bool IsVisible {
      get => _visible;
      set {
         if (_visible != value) {
            _visible = value;
            IsDirty = true;
         }
      }
   }

   public int Left { get; init; }

   public string? Mask { get; init; } = null;

   public string Name { get; init; }

   public Map Parent { get; init; }

   public StatusFieldSeverity Severity { get; set; }

   public int Top { get; init; }

   public string Value
      => _value;

   public int Width { get; init; }

   public bool DidKeyPress(ConsoleKeyInfo key, ConsoleCursor cursor) {
      string keyName = Application.SimplifyKeyInfo(key);
      int pos;
      switch (keyName) {
         case "LeftArrow":
            if (cursor.Left == Parent.Left + Left) {
               cursor.MoveFieldLeft();
            } else if (cursor.Left > 0) {
               cursor.MoveLeft();
            }
            return true;
         case "RightArrow":
            if (cursor.Left == Parent.Left + Left + Width) {
               cursor.MoveFieldRight();
            } else {
               cursor.MoveRight();
            }
            return true;
         case "UpArrow":
            cursor.MoveFieldAbove();
            return true;
         case "DownArrow":
            cursor.MoveFieldBelow();
            return true;
         case "Delete":
            pos = cursor.Left - Left - Parent.Left;
            if (pos < Width) {
               SetValue(Value.Remove(pos, 1));
            }
            return true;
         case "Backspace":
            pos = cursor.Left - Left - Parent.Left;
            if (pos > 0) {
               if (SetValue(Value.Remove(pos - 1, 1))) {
                  cursor.MoveLeft();
               }
            }
            return true;
         case "Home":
            cursor.MoveLeft(cursor.Left - Parent.Left - Left);
            return true;
         case "End":
            cursor.MoveRight(cursor.Left - Parent.Left - Left + Value.TrimEnd('\0').Length);
            return true;
         default:
            if (key.Modifiers is 0 or ConsoleModifiers.Shift && (key.KeyChar == ' '
                  || char.IsLetterOrDigit(key.KeyChar)
                  || char.IsPunctuation(key.KeyChar)
                  || char.IsSymbol(key.KeyChar))) {
               string value = Value;
               pos = cursor.Left - Parent.Left - Left;
               if (Application.IsInsertMode && Mask != "/") {
                  if (value[^1] != '\0') {
                     break;
                  }
                  value = value[..^1];
               } else {
                  value = value.Remove(pos, 1);
               }
               value = value.Insert(pos, key.KeyChar.ToString());
               if (SetValue(value)) {
                  if (cursor.Left + 1 >= Parent.Left + Left + Width) {
                     cursor.MoveNextField();
                  } else {
                     cursor.MoveRight();
                  }
               }
               return true;
            }
            break;
      }
      return false;
   }

   public bool HasCursor(int left, int top)
      => left >= Parent.Left + Left && left <= Parent.Left + Left + Width + 1 && top == Parent.Top + Top;

   public void Redraw(IConsole console, bool active) {
      console.SetCursorPosition(Parent.Left + Left, Parent.Top + Top);
      StatusFieldSeverity fstatus = StatusFieldSeverity.None;
      FieldState fstate = FieldState.None;
      string fvalue;
      if (!IsVisible) {
         fvalue = new string(' ', Width);
      } else if (IsEditable) {
         fstate = active ? FieldState.Editing : FieldState.Editable;
         fvalue = Value;
         if (IsProtected) {
            char passwdChar = Application.PasswordChar;
            string masked = "";
            foreach (char ch in fvalue) {
               masked += ch == '\0' ? '\0' : passwdChar;
            }
            fvalue = masked;
         }
      } else {
         fvalue = Value.Trim('\0');
         if (fvalue.Length < Width) {
            fvalue += new string(' ', Width - fvalue.Length);
         }
         if (IsStatus) {
            fstatus = Severity;
         }
      }
      if (!Application.PreserveValues && !active) {
         fvalue = fvalue.ToUpper();
      }
      console.Write(fvalue[0..Math.Min(fvalue.Length, Width)], fstate, fstatus);
      IsDirty = false;
   }

   public bool SetValue(string value, bool propagate = true) {
      value ??= "";
      if (IsEditable && value.Length < Width) {
         value += new string('\0', Width - value.Length);
      } else if (value.Length > Width) {
         value = value[0..Width];
      }
      if (_value != value && AcceptsValue(value)) {
         Severity = StatusFieldSeverity.None;
         _value = value;
         IsDirty = true;
         if (propagate) {
            ValueChanged();
         }
         return true;
      }
      return false;
   }


   private string _value = "";
   private bool _visible = true;

   private FieldType Type { get; init; }

   private bool AcceptsValue(string value) {
      value = value ?? throw new ArgumentNullException(nameof(value));
      if (Mask is not null) {
         string mask = Mask[0..value.Length];
         for (int i = 0; i < mask.Length; ++i) {
            bool valid = mask[i] switch {
               'X' => true,
               '*' => true,
               ' ' => true,
               'A' => value[i] is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'),
               '9' => value[i] is (>= '0' and <= '9'),
               '/' => value[i] is 'X' or 'x' or '/' or ' ',
               _ => throw new InvalidDataException(nameof(Mask)),
            } || value[i] == '\0';
            if (!valid) {
               return false;
            }
         }
      }
      return true;
   }

   private void ValueChanged() {
      if (Group is not null and not 0 && Value.Trim() != "") {
         foreach (Field field in Parent.Fields.Where(f => f != this && f.Group == Group)) {
            field.SetValue("", false);
         }
      }
   }
}
