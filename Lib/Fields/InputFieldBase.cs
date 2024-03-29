﻿/*
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

using System.Globalization;
using System.Text;

namespace Lmpessoa.Mainframe.Fields;

internal abstract class InputFieldBase : FieldBase {

   protected readonly bool _protected;
   private string _value;

   protected InputFieldBase(string name, Map parent, int left, int top, int width, bool @protected)
      : base(name, parent, left, top, width) {
      _value = new('\0', width);
      _protected = @protected;
   }


   internal virtual bool AcceptsValue(string value) => true;

   internal string GetInnerValue() => _value;

   internal abstract bool InputKeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor);

   internal override bool KeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor) {
      string keyName = Application.SimplifyKeyInfo(key);
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
         case "Home":
            cursor.MoveLeft(cursor.Left - Parent.Left - Left);
            return true;
         case "End":
            cursor.MoveRight(Parent.Left + Left + GetInnerValue().TrimEnd('\0').Length - cursor.Left);
            return true;
         default:
            return InputKeyPressed(key, cursor);
      }
   }

   internal override void Redraw(ConsoleWrapper console, bool active) {
      console.CursorPosition = (Parent.Left + Left, Parent.Top + Top);
      StatusFieldSeverity fstatus = Severity;
      FieldState fstate = FieldState.None;
      string fvalue;
      if (!IsVisible) {
         fvalue = new string(' ', Width);
      } else {
         fstate = active ? FieldState.Editing : FieldState.Editable;
         fvalue = _value;
         if (_protected) {
            char passwdChar = Application.PasswordChar;
            StringBuilder masked = new();
            foreach (char ch in fvalue) {
               masked.Append(ch == '\0' ? '\0' : passwdChar);
            }
            fvalue = masked.ToString();
         }
      }
      if (Application.PreserveValues != PreserveValuesLevel.Display && !active) {
         fvalue = fvalue.ToUpper(CultureInfo.CurrentUICulture);
      }
      console.WriteField(fvalue[0..Math.Min(fvalue.Length, Width)], fstate, fstatus);
      IsDirty = false;
   }

   internal bool SetInnerValue(string value) {
      value ??= "";
      if (value.Length < Width) {
         value += new string('\0', Width - value.Length);
      } else if (value.Length > Width) {
         value = value[0..Width];
      }
      if (_value != value) {
         Severity = StatusFieldSeverity.None;
         _value = value;
         IsDirty = true;
      }
      return IsDirty;
   }
}
