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

namespace Lmpessoa.Mainframe.Fields;

internal class CheckField : InputFieldBase { 

   private bool _checked = false;

   public CheckField(string name, Map parent, int left, int top, int group)
      : base(name, parent, left, top, 1, false)
      => Group = Math.Max(0, group);

   public int Group { get; }

   protected internal override object GetValue() => _checked;

   protected internal override bool SetValue(object? value) {
      if (value is bool b) {
         if (_checked == b) {
            return false;
         }
         _checked = b;
         SetInnerValue(_checked ? "X" : "");
      } else if (value is string s && s is "" or " " or "x" or "X" or "/") {
         if (GetInnerValue() == s.Trim()) {
            return false;
         }
         _checked = s is not "" and not " " || !_checked;
         SetInnerValue(s.Trim());
      } else { 
         throw new ArgumentOutOfRangeException(nameof(value));
      }
      Severity = StatusFieldSeverity.None;
      IsDirty = true;
      AdjustGroupValues();
      return true;
   }

   internal override bool InputKeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor) {
      if (key.Modifiers is 0 or ConsoleModifiers.Shift && key.KeyChar is ' ' or '/' or 'x' or 'X') {
         SetValue($"{key.KeyChar}");
         cursor.MoveNextField();
         return true;
      } else if (key.Modifiers == 0 && key.Key == ConsoleKey.Backspace && cursor.Left > Parent.Left + Left) {
         cursor.MoveLeft();
         _checked = false;
         return true;
      } else if (key.Modifiers == 0 && key.Key == ConsoleKey.Delete) {
         _checked = false;
         return true;
      }
      return false;
   }

   private void AdjustGroupValues() {
      if (Group is not 0 && _checked) {
         foreach (CheckField field in Parent.Fields
               .Where(f => f is CheckField cf && cf != this && cf.Group == Group)
               .Cast<CheckField>()) {
            field._checked = false;
            field.SetInnerValue("");
         }
      }
   }
}
