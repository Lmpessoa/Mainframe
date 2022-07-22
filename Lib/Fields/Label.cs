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

internal class Label : FieldBase {

   private string _value;

   public Label(string name, Map parent, int left, int top, int width)
      : base(name, parent, left, top, width) {
      _value = new string(' ', width);
   }

   protected internal override object GetValue() => _value.Trim();

   protected internal override bool SetValue(object? value) {
      if (value is not null and not string) {
         throw new ArgumentOutOfRangeException(nameof(value));
      }
      string text = (string) (value ?? "");
      if (text.Length < Width) {
         text += new string(' ', Width - text.Length);
      } else if (text.Length > Width) {
         text = text[0..Width];
      }
      if (_value != text) {
         Severity = StatusFieldSeverity.None;
         _value = text;
         IsDirty = true;
         return true;
      }
      return false;
   }

   internal override void Redraw(IConsole console, bool active) {
      console.SetCursorPosition(Parent.Left + Left, Parent.Top + Top);
      StatusFieldSeverity fstatus = Severity;
      FieldState fstate = FieldState.None;
      string fvalue = IsVisible ? _value : new string(' ', Width);
      if (!Application.PreserveValues) {
         fvalue = fvalue.ToUpper();
      }
      console.Write(fvalue[0..Math.Min(fvalue.Length, Width)], fstate, fstatus);
      IsDirty = false;
   }
}
