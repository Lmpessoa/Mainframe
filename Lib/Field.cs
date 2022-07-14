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

internal enum FieldType {
   Protected,
   Editable,
   ReadOnly,
}

internal sealed class Field {

   internal Field(Map parent, int left, int top, string arg, ConsoleColor? fgcolor, ConsoleColor? bgcolor) {
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
      BackgroundColor = bgcolor;
      ForegroundColor = fgcolor;
      Type = type switch {
         "PWD" => FieldType.Protected,
         "INP" => FieldType.Editable,
         "CHK" => FieldType.Editable,
         "ROT" => FieldType.ReadOnly,
         "STA" => FieldType.ReadOnly,
         _ => throw new InvalidFieldException($"'{type}' is not a valid field type", left, top),
      };
      Width = args.Length == 0 || type == "CHK" ? 1
         : args[0] == '[' ? int.Parse(args[1..^1]) 
         : args.Length - 2;
      IsStatus = type is "STA";
      Mask = type switch {
         "INP" => args[0] == '(' ? args[1..^1] : null,
         "CHK" => "/",
         _ => null,
      };
      if (type is "CHK") {
         Group = args.Length > 0 ? byte.Parse(args[1..^1]) : 0;
      }
      _value = new('\t', Width);
   }

   internal ConsoleColor? BackgroundColor { get; init; }

   internal ConsoleColor? ForegroundColor { get; init; }

   internal int? Group { get; init; }

   internal bool IsChecked
      => Group is not null && Value.Trim() != "";

   internal bool IsDirty { get; set; } = false;

   internal bool IsEditable
      => Type is FieldType.Protected or FieldType.Editable;

   internal bool IsFocusable
      => Type is not FieldType.ReadOnly && IsVisible;

   internal bool IsReadOnly
      => Type is FieldType.ReadOnly;

   internal bool IsStatus { get; init; }

   internal bool IsVisible {
      get => _visible;
      set {
         if (_visible != value) {
            _visible = value;
            IsDirty = true;
         }
      }
   }

   internal int Left { get; init; }

   internal string? Mask { get; init; } = null;

   internal string Name { get; init; }

   internal Map Parent { get; init; }

   internal int Top { get; init; }

   internal FieldType Type { get; init; }

   internal string Value
      => _value;

   internal int Width { get; init; }

   internal bool SetValue(string value) {
      value ??= "";
      if (IsEditable && value.Length < Width) {
         value += new string('\t', Width - value.Length);
      } else if (value.Length > Width) {
         value = value[0..Width];
      }
      if (_value != value && AcceptsValue(value)) {
         _value = value;
         IsDirty = true;
         ValueChanged();
         return true;
      }
      return false;
   }


   private string _value = "";
   private bool _visible = true;

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
            } || value[i] == '\t';
            if (!valid) {
               return false;
            }
         }
      }
      return true;
   }

   private void ValueChanged() {
      if (IsStatus) {
         Parent.StatusKind = StatusMessageKind.None;
      } else if (Group is not null and not 0 && Value.Trim() != "") {
         foreach (Field field in Parent.Fields.Where(f => f != this && f.Group == Group)) {
            field.SetValue("");
         }
      }
   }
}
