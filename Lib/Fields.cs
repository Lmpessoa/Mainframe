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

namespace Lmpessoa.Terminal;

/// <summary>
/// 
/// </summary>
public abstract class Field {

   /// <summary>
   /// 
   /// </summary>
   /// <param name="prefix"></param>
   /// <param name="creator"></param>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentException"></exception>
   public static void Register(string prefix, Func<string, Field> creator) {
      prefix = prefix?.ToUpper() ?? throw new ArgumentNullException(nameof(prefix));
      creator = creator ?? throw new ArgumentNullException(nameof(creator));
      if (!Regex.IsMatch(prefix, "^[A-Z]{3}$")) {
         throw new ArgumentException("Invalid field prefix", nameof(prefix));
      }
      if (_creators.ContainsKey(prefix)) {
         throw new ArgumentException("Prefix is already registered", nameof(prefix));
      }
      _creators[prefix] = creator;
   }

   static Field() {
      Register("ROT", Label.Create);
      Register("MSG", StatusMessage.Create);
      Register("CHK", CheckField.Create);
      Register("SEL", ChoiceField.Create);
   }

   /// <summary>
   /// 
   /// </summary>
   public int Left { get; internal set; }

   /// <summary>
   /// 
   /// </summary>
   public int Top { get; internal set; }

   /// <summary>
   /// 
   /// </summary>
   public int Width { get; init; }

   /// <summary>
   /// 
   /// </summary>
   public bool IsVisible {
      get => _visible;
      set {
         if (_visible != value && CanBeHidden) {
            _visible = value;
            IsDirty = true;
         }
      }
   }

   /// <summary>
   /// 
   /// </summary>
   protected internal virtual bool CanReceiveFocus => IsVisible;

   /// <summary>
   /// 
   /// </summary>
   protected internal virtual bool CanBeHidden => true;


   internal static Field Create(string args) {
      string type = "";
      if (args.Length >= 4 && args[3] == ':') {
         type = args[0..3].ToUpper();
         args = args[4..];
      }
      return _creators.ContainsKey(type)
         ? _creators[type].Invoke(args)
         : TextField.Create(args);
   }

   internal FieldSet? Owner { get; set; }

   internal Field() { }

   internal ConsoleColor? BackgroundColor { get; set; }

   internal ConsoleColor? ForegroundColor { get; set; }

   internal bool IsDirty { get; set; } = false;


   private static readonly Dictionary<string, Func<string, Field>> _creators = new();

   private bool _visible = true;
}

/// <summary>
/// 
/// </summary>
public abstract class InputField : Field {

   /// <summary>
   /// 
   /// </summary>
   public string Value {
      get => GetInnerValue()?.Replace('\t', ' ').Trim() ?? "";
      set {
         if (!SetInnerValue(value?.Trim())) {
            throw new ArgumentException("Unacceptable value");
         }
      }
   }


   /// <summary>
   /// 
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   protected abstract bool IsAcceptable(string value);

   /// <summary>
   /// 
   /// </summary>
   /// <returns></returns>
   protected virtual string InitValue()
      => new('\t', Width);

   /// <summary>
   /// 
   /// </summary>
   protected virtual void ValueChanged() { }


   internal string GetInnerValue() {
      _value ??= InitValue();
      return _value;
   }

   internal bool SetInnerValue(string? value) {
      if (value is not null) {
         if (value.Length < Width) {
            value += new string('\t', Width - value.Length);
         } else if (value.Length > Width) {
            value = value[0..Width];
         }
      }
      if (value is null || IsAcceptable(value)) {
         if (_value != value) {
            _value = value;
            IsDirty = true;
            ValueChanged();
         }
         return true;
      }
      return false;
   }


   private string? _value = null;
}


/// <summary>
/// 
/// </summary>
public class TextField : InputField {

   /// <summary>
   /// 
   /// </summary>
   public string InputMask { get; init; }

   /// <summary>
   /// 
   /// </summary>
   public bool IsPasswordField { get; init; }


   protected override bool IsAcceptable(string value) {
      for (int i = 0; i < InputMask.Length; ++i) {
         bool valid = InputMask[i] switch {
            'X' => true,
            '*' => true,
            ' ' => true,
            'A' => value[i] is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'),
            '9' => value[i] is (>= '0' and <= '9'),
            _ => throw new InvalidDataException(nameof(InputMask)),
         } || value[i] == '\t';
         if (!valid) {
            return false;
         }
      }
      return true;
   }

   protected override string InitValue()
      => InputMask.Replace('A', '\t').Replace('X', '\t').Replace('9', '\t').Replace('*', '\t').Replace(' ', '\t');


   internal static new TextField Create(string args) {
      bool passwd = args.StartsWith("*");
      args = passwd ? new string('*', args.Length) : args;
      return new() {
         Width = args.Length,
         InputMask = args,
         IsPasswordField = passwd,
      };
   }
}


/// <summary>
/// 
/// </summary>
public class CheckField : InputField {

   /// <summary>
   /// 
   /// </summary>
   public bool IsChecked
      => GetInnerValue() is not "\t" and not " ";

   protected override bool IsAcceptable(string value)
      => value is "" or "x" or "X" or "/" or " ";

   internal static new CheckField Create(string args)
      => new() {
         Width = 1,
      };
}


/// <summary>
/// 
/// </summary>
public sealed class ChoiceField : CheckField {

   /// <summary>
   /// 
   /// </summary>
   public int Group { get; init; }

   protected override bool IsAcceptable(string value)
      => value is "x" or "X" or "/";

   protected override void ValueChanged() {
      if (Owner is not null && Value != "") {
         foreach (ChoiceField sel in Owner.Where(f => f is ChoiceField sel && sel != this && sel.Group == Group)) {
            sel.SetInnerValue(null);
         }
      }
   }

   internal static new ChoiceField Create(string args)
      => new() {
         Group = int.Parse(args),
         Width = 1,
      };
}


/// <summary>
/// 
/// </summary>
public class Label : Field {

   /// <summary>
   /// 
   /// </summary>
   public string? Value {
      get => _value;
      set {
         if (value is not null) {
            if (value.Length > Width) {
               value = value[0..Width];
            }
         }
         if (_value != value) {
            _value = value;
            IsDirty = true;
            ValueChanged();
         }
      }
   }

   protected internal override bool CanReceiveFocus => false;

   /// <summary>
   /// 
   /// </summary>
   protected virtual void ValueChanged() { }

   internal static new Label Create(string args)
      => new() {
         Width = int.TryParse(args, out int len) ? len : args.Length,
      };

   private string? _value;
}


/// <summary>
/// 
/// </summary>
public sealed class StatusMessage : Label {

   internal static new StatusMessage Create(string args)
      => new() {
         Width = int.TryParse(args, out int len) ? len : args.Length,
      };

   protected internal override bool CanBeHidden => false;

   protected override void ValueChanged()
      => Kind = MessageKind.None;

   internal MessageKind Kind { get; set; }
}
