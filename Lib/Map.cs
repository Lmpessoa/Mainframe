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

using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lmpessoa.Mainframe;

/// <summary>
/// 
/// </summary>
public abstract class Map {

   /// <summary>
   /// 
   /// </summary>
   public Map() {
      using Stream? stream = GetType().Assembly.GetManifestResourceStream($"{GetType().FullName}.map");
      if (stream is not null) {
         using StreamReader reader = new(stream);
         InitFromContents(reader.ReadToEnd().Replace("\r\n", "\n").Split("\n"));
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public FieldSet Fields { get; } = new();

   /// <summary>
   /// 
   /// </summary>
   public bool IsInWindow { get; private set; }

   /// <summary>
   /// 
   /// </summary>
   public void ClearMessage() {
      if (_messageField is not null) {
         _messageField.Kind = MessageKind.None;
         _messageField.Value = null;
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public void Close()
      => Application.Pop(this);

   /// <summary>
   /// 
   /// </summary>
   public void FocusOnNextField()
      => FocusOnNext(reverse: false);

   /// <summary>
   /// 
   /// </summary>
   public void FocusOnPreviousField()
      => FocusOnNext(reverse: true);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="fieldIndex"></param>
   public void MoveFocus(int fieldIndex) {
      if (fieldIndex >= 0 && fieldIndex < Fields.Count
            && Fields[fieldIndex].CanReceiveFocus) {
         if (CurrentField is not null) {
            if (Application?.UsesActiveFieldColors ?? false) {
               CurrentField.IsDirty = true;
            }
            DidLostFocus(CurrentField);
         }
         CurrentFieldIndex = fieldIndex;
         if (CurrentField is not null) {
            if (Application?.UsesActiveFieldColors ?? false) {
               CurrentField.IsDirty = true;
            }
            Application.SetCursorPosition(Left + CurrentField.Left, Top + CurrentField.Top);
         }
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetMessage(string message)
      => SetMessage(message, MessageKind.Info);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetSuccess(string message)
      => SetMessage(message, MessageKind.Success);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetAlert(string message)
      => SetMessage(message, MessageKind.Alert);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetError(string message)
      => SetMessage(message, MessageKind.Error);

   /// <summary>
   /// 
   /// </summary>
   /// <exception cref="InvalidOperationException"></exception>
   public void Show() {
      if (Application is not null) {
         throw new InvalidOperationException();
      }
      IsInWindow = false;
      CurrentFieldIndex = -1;
      Application.Push(this);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <exception cref="InvalidOperationException"></exception>
   public void ShowWindow() {
      if (Application is not null) {
         throw new InvalidOperationException();
      }
      IsInWindow = true;
      CurrentFieldIndex = -1;
      Application.Push(this);
   }



   /// <summary>
   /// 
   /// </summary>
   protected virtual void OnActivating() { }

   /// <summary>
   /// 
   /// </summary>
   protected virtual void OnClosed() { }

   /// <summary>
   /// 
   /// </summary>
   /// <returns></returns>
   protected virtual bool OnClosing() => true;

   /// <summary>
   /// 
   /// </summary>
   protected virtual void OnDeactivating() { }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="key"></param>
   protected virtual void OnKeyPressed(ConsoleKeyInfo key) { }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   protected virtual void OnLostFocus(Field field) { }



   internal Map(string[] contents) {
      InitFromContents(contents);
   }

   internal Application? Application { get; set; }

   internal IEnumerable<MapFragment> Fragments { get; private set; } = Array.Empty<MapFragment>();

   internal Field? CurrentField
      => CurrentFieldIndex > -1 && CurrentFieldIndex < Fields.Count ? Fields[CurrentFieldIndex] : null;

   internal int CurrentFieldIndex { get; private set; } = -1;

   internal (int, int) CursorLastPosition { get; set; } = (-1, -1);

   internal int Height { get; private set; }

   internal int Left { get; set; }

   internal int Top { get; set; }

   internal int Width { get; private set; }

   internal void DidClose()
      => OnClosed();

   internal void DidKeyPress(ConsoleKeyInfo key) {
      key = new('\0', key.Key,
         (key.Modifiers & ConsoleModifiers.Shift) != 0,
         (key.Modifiers & ConsoleModifiers.Alt) != 0,
         (key.Modifiers & ConsoleModifiers.Control) != 0);
      if (_actions.ContainsKey(key)) {
         _actions[key].Invoke();
      } else {
         OnKeyPressed(key);
      }
   }

   internal void DidLostFocus(Field field) { 
      if (!(Application?.PreserveValues ?? false) && field is InputField input 
            && (field is not TextField text || !text.IsPasswordField)) {
         input.SetInnerValue(input.GetInnerValue().ToUpper());
      }
      OnLostFocus(field);
   }

   internal void MoveFocusTo(int left, int top) {
      Application.SetCursorPosition(left, top);
      foreach (Field field in Fields) {
         if (field.CanReceiveFocus && top == field.Top + this.Top
               && left >= field.Left + this.Left && left <= field.Left + this.Left + field.Width) {
            int fieldIndex = Fields.IndexOf(field);
            if (CurrentField != field) {
               if (CurrentField is not null) {
                  if (Application?.UsesActiveFieldColors ?? false) {
                     CurrentField.IsDirty = true;
                  }
                  DidLostFocus(CurrentField);
               }
               CurrentFieldIndex = fieldIndex;
               if (CurrentField is not null && (Application?.UsesActiveFieldColors ?? false)) {
                  CurrentField.IsDirty = true;
               }
            }
            return;
         }
      }
      if (CurrentField is not null) {
         DidLostFocus(CurrentField);
         if (Application?.UsesActiveFieldColors ?? false) {
            CurrentField.IsDirty = true;
         }
         CurrentFieldIndex = -1;
      }
   }

   internal bool ShouldClose()
      => OnClosing();

   internal void WillActivate()
      => OnActivating();

   internal void WillDeactivate()
      => OnDeactivating();



   private static ConsoleColor? FromHex(char value) {
      int intval = "0123456789ABCDEF".IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
      return intval is >= 0 and <= 15 ? (ConsoleColor) intval : null;
   }

   private readonly Dictionary<ConsoleKeyInfo, Action> _actions = new();
   private StatusMessage? _messageField = null;

   private void FocusOnNext(bool reverse) {
      List<Field> fields = Fields.Where(f => f.CanReceiveFocus).ToList();
      if (!fields.Any()) {
         return;
      }
      if (CurrentField is not null) {
         if (reverse) {
            fields.Reverse();
         }
         int index = fields.IndexOf(CurrentField) + 1;
         index = index >= fields.Count ? 0 : index;
         MoveFocus(Fields.IndexOf(fields[index]));
      } else {
         MoveFocus(Fields.IndexOf(reverse ? fields.Last() : fields.First()));
      }
   }

   private void InitFromContents(IEnumerable<string> source) {
      source = source ?? throw new ArgumentNullException(nameof(source));
      List<Field> fields = source.Where(l => l.Length > 0 && l[0] is '¬')
                             .Select(l => Field.Create(l[1..].Trim()))
                             .ToList();
      string[] lines = source.Where(l => l.Length > 0 && l[0] is '>' or ':').ToArray();
      if (!lines.Any()) {
         throw new ArgumentException("Map has no contents", nameof(source));
      }
      Width = lines.Max(l => l.Length) - 1;
      List<MapFragment> result = new();
      Fields.Clear();
      int top = 0;
      for (int i = 0; i < lines.Length; ++i) {
         if (lines[i][0] != '>') {
            continue;
         }
         int lwidth = Width - lines[i].Length + 1;
         string line = lines[i][1..] + (lwidth > 0 ? new string(' ', lwidth) : "");
         string foreMarkup = "";
         string backMarkup = "";
         if (i + 1 < lines.Length && lines[i + 1][0] == ':') {
            foreMarkup = lines[i + 1][1..].ToUpper().TrimEnd();
            i += 1;
            if (i + 1 < lines.Length && lines[i + 1][0] == ':') {
               backMarkup = lines[i + 1][1..].ToUpper().TrimEnd();
               i += 1;
            }
         }
         foreach (Match match in Regex.Matches(line, "¬")) {
            if (fields.FirstOrDefault() is Field field) {
               fields.Remove(field);
               int fieldInField = line.IndexOf('¬', match.Index + 1, field.Width - 1);
               if (fieldInField != -1) {
                  throw new InvalidFieldException("Fields overlap", fieldInField, top);
               }
               line = line.Remove(match.Index, field.Width)
                          .Insert(match.Index, new string(' ', field.Width));
               field.Top = top;
               field.Left = match.Index;
               field.ForegroundColor = foreMarkup.Length >= match.Index + 1
                  ? FromHex(foreMarkup[match.Index])
                  : null;
               field.BackgroundColor = backMarkup.Length >= match.Index + 1
                  ? FromHex(backMarkup[match.Index])
                  : null;
               field.Owner = Fields;
               Fields.Add(field);
               if (field is StatusMessage msg) {
                  if (_messageField is not null) {
                     throw new InvalidFieldException("Duplicate status field", field.Left, field.Top);
                  }
                  _messageField = msg;
               }
            }
         }
         result.AddRange(MapFragment.Parse(line, foreMarkup, backMarkup));
         top += 1;
      }
      Height = top;
      Fragments = result.ToImmutableArray();
      var methods = this.GetType()
         .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
         .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
         .Select(m => (m, m.GetCustomAttribute<CommandKeyAttribute>()))
         .Where(e => e.Item2 is not null);
      foreach (var (method, attr) in methods) {
         if (attr is not null) {
            ConsoleKeyInfo info = new('\0', attr.Key,
               attr.Modifiers.HasFlag(KeyModifier.Shift),
               attr.Modifiers.HasFlag(KeyModifier.Alt),
               attr.Modifiers.HasFlag(KeyModifier.Ctrl));
            _actions.Add(info, () => method.Invoke(this, Array.Empty<object>()));
         }
      }
   }

   private void SetMessage(string message, MessageKind kind) {
      if (_messageField is not null) {
         _messageField.Value = message;
         _messageField.Kind = kind;
         _messageField.IsDirty = true;
      }
   }
}
