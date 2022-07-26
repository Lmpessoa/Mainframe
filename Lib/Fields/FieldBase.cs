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

internal abstract class FieldBase {

   private bool _visible = true;

   public static FieldBase Create(Map parent, int left, int top, string arg) {
      Match match = Regex.Match(arg, "^([A-Za-z][A-Za-z0-9\\-_]*):([A-Z]{3})(\\[\\d+\\]|\\([^\\]]+\\))?$");
      if (!match.Success) {
         throw new ArgumentException($"Invalid field definition: '{arg}'");
      }
      string name = match.Groups[1].Value;
      string type = match.Groups[2].Value;
      string args = match.Groups[3].Value;
      switch (type) {
         case "INP":
         case "TXT":
         case "PWD":
            return args[0] == '['
               ? new TextField(name, parent, left, top, int.Parse(args[1..^1]), type == "PWD")
               : type == "PWD" ? new TextField(name, parent, left, top, args.Length - 2, true)
               : new TextField(name, parent, left, top, args[1..^1]);
         case "CHK":
            if (args.Length > 0 && args[0] == '[') {
               throw new ArgumentException($"Invalid field definition: '{arg}'");
            }
            return new CheckField(name, parent, left, top, args.Length > 0 ? byte.Parse(args[1..^1]) : 0);
         case "ROT":
         case "STA":
            Label label = new(name, parent, left, top, args[0] == '[' ? int.Parse(args[1..^1]) : args.Length - 2);
            if (args[0] == '(') {
               label.SetValue(args[1..^1]);
            }
            return label;
         default:
            throw new ArgumentException($"Unknown field type: '{type}'");
      }
   }

   public FieldBase(string name, Map parent, int left, int top, int width) {
      Name = name ?? throw new ArgumentNullException(nameof(name));
      Parent = parent ?? throw new ArgumentNullException(nameof(parent));
      Left = left;
      Top = top;
      Width = width;
   }

   public bool IsDirty { get; set; } = false;

   public bool IsFocusable
      => this is IFocusableField && IsVisible;

   public bool IsVisible {
      get => _visible;
      set {
         if (_visible != value) {
            _visible = value;
            IsDirty = true;
         }
      }
   }

   public int Left { get; }

   public string Name { get; }

   public Map Parent { get; }

   public StatusFieldSeverity Severity { get; set; } = StatusFieldSeverity.None;

   public int Top { get; }

   public int Width { get; }


   protected internal abstract object GetValue();

   protected internal abstract bool SetValue(object? value);


   internal virtual bool KeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor) => false;

   internal abstract void Redraw(ConsoleWrapper console, bool active);
}

internal interface IFocusableField { }
