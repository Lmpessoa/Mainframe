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

namespace Lmpessoa.Mainframe.Fields;

internal class TextField : InputFieldBase {

   public TextField(string name, Map parent, int left, int top, int width, bool @protected)
      : base(name, parent, left, top, width, @protected)
      => Mask = null;

   public TextField(string name, Map parent, int left, int top, string mask)
      : base(name, parent, left, top, mask.Length, false) {
      if (!Regex.IsMatch(mask, "^[aAxX\\*9 ]+$")) {
         throw new ArgumentException("Invalid mask value", nameof(mask));
      }
      Mask = mask.ToUpper();
   }

   public bool IsProtected
      => _protected;

   public string? Mask { get; }

   protected internal override object GetValue()
      => GetInnerValue().Replace('\0', ' ').Trim();

   protected internal override bool SetValue(object? value) {
      if (value is not null and not string) {
         throw new ArgumentOutOfRangeException(nameof(value));
      }
      string text = (string) (value ?? "");
      if (text.Length > Width) {
         text = text[0..Width];
      }
      if (AcceptsValue(text)) {
         SetInnerValue(text);
      }
      return IsDirty;
   }

   internal override bool AcceptsValue(string value) {
      value = value ?? throw new ArgumentNullException(nameof(value));
      if (Mask is not null) {
         string mask = Mask[0..value.Length];
         for (int i = 0; i < mask.Length; ++i) {
            bool valid = mask[i] switch {
               'X' => true,
               '*' => true,
               ' ' => true,
               'A' => value[i] is >= 'A' and <= 'Z' or >= 'a' and <= 'z',
               '9' => value[i] is >= '0' and <= '9',
               _ => throw new InvalidDataException(nameof(Mask)),
            } || value[i] == '\0';
            if (!valid) {
               return false;
            }
         }
      }
      return true;
   }

   internal override bool InputKeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor) {
      if (key.Modifiers is 0 or ConsoleModifiers.Shift && (key.KeyChar == ' '
      || char.IsLetterOrDigit(key.KeyChar)
      || char.IsPunctuation(key.KeyChar)
      || char.IsSymbol(key.KeyChar))) {
         string value = GetInnerValue();
         int pos = cursor.Left - Parent.Left - Left;
         if (Application.IsInsertMode && Mask != "/") {
            if (value[^1] != '\0') {
               return false;
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
      } else if (key.Modifiers == 0 && key.Key == ConsoleKey.Delete) {
         int pos = cursor.Left - Left - Parent.Left;
         if (pos < Width) {
            SetInnerValue(GetInnerValue().Remove(pos, 1));
         }
         return true;
      } else if (key.Modifiers == 0 && key.Key == ConsoleKey.Backspace) {
         int pos = cursor.Left - Left - Parent.Left;
         if (pos > 0) {
            string text = GetInnerValue().Remove(pos - 1, 1);
            if (AcceptsValue(text)) {
               cursor.MoveLeft();
               SetInnerValue(text);
            }
         }
         return true;
      }
      return false;
   }
}
