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
   /// <param name="fieldName"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentException"></exception>
   public string? this[string fieldName] {
      get => GetField(fieldName)?.Value.Replace('\t', ' ').Trim();
      set => GetField(fieldName)?.SetValue((value ?? "").Trim());
   }

   /// <summary>
   /// 
   /// </summary>
   public bool IsInWindow { get; private set; }

   /// <summary>
   /// 
   /// </summary>
   public void ClearMessage() {
      if (_messageField is not null) {
         StatusKind = StatusMessageKind.None;
         _messageField.SetValue("");
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
            && Fields[fieldIndex].IsFocusable) {
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
      => SetMessage(message, StatusMessageKind.Info);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetSuccess(string message)
      => SetMessage(message, StatusMessageKind.Success);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetAlert(string message)
      => SetMessage(message, StatusMessageKind.Alert);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetError(string message)
      => SetMessage(message, StatusMessageKind.Error);

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
   protected virtual void OnLostFocus(string fieldName) { }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="fieldName"></param>
   /// <param name="visible"></param>
   /// <returns></returns>
   protected internal bool SetFieldVisible(string fieldName, bool visible) {
      if (GetField(fieldName) is not Field field) {
         throw new ArgumentException($"{fieldName} is not defined: ", nameof(fieldName));
      }
      if (!field.IsStatus) {
         field.IsVisible = visible;
         return true;
      }
      return false;
   }



   internal Map(params string[] contents) {
      InitFromContents(contents);
   }

   internal Application? Application { get; set; }

   internal IEnumerable<MapFragment> Fragments { get; private set; } = Array.Empty<MapFragment>();

   internal Field? CurrentField
      => CurrentFieldIndex > -1 && CurrentFieldIndex < Fields.Count ? Fields[CurrentFieldIndex] : null;

   internal int CurrentFieldIndex { get; private set; } = -1;

   internal (int, int) CursorLastPosition { get; set; } = (-1, -1);

   internal FieldSet Fields { get; } = new();

   internal int Height { get; private set; }

   internal int Left { get; set; }

   internal StatusMessageKind StatusKind = StatusMessageKind.None;

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
      if (!(Application?.PreserveValues ?? false) && field.IsFocusable) {
         field.SetValue(field.Value.ToUpper());
      }
      OnLostFocus(field.Name);
   }

   internal void MoveFocusTo(int left, int top) {
      Application.SetCursorPosition(left, top);
      Field? next = Fields.FirstOrDefault(f => f.IsFocusable && top == f.Top + Top
               && left >= f.Left + Left && left <= f.Left + Left + f.Width);
      int nextIndex = next is not null ? Fields.IndexOf(next) : -1;
      if (nextIndex != CurrentFieldIndex) {
         if (CurrentField is not null) {
            if (Application?.UsesActiveFieldColors ?? false) {
               CurrentField.IsDirty = true;
            }
            DidLostFocus(CurrentField);
         }
         CurrentFieldIndex = nextIndex;
         if (CurrentField is not null) {
            if (Application?.UsesActiveFieldColors ?? false) {
               CurrentField.IsDirty = true;
            }
         }
      }
   }

   internal bool ShouldClose()
      => OnClosing();

   internal void WillActivate()
      => OnActivating();

   internal void WillDeactivate()
      => OnDeactivating();



   private static ConsoleColor? ColorForHex(char value) {
      int intval = "0123456789ABCDEF".IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
      return intval is >= 0 and <= 15 ? (ConsoleColor) intval : null;
   }

   private readonly Dictionary<ConsoleKeyInfo, Action> _actions = new();
   private Field? _messageField = null;

   private void FocusOnNext(bool reverse) {
      List<Field> fields = Fields.Where(f => f.IsFocusable).ToList();
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

   private Field? GetField(string fieldName)
      => Fields.FirstOrDefault(f => f.Name == fieldName);

   private void InitFromContents(IEnumerable<string> source) {
      source = source ?? throw new ArgumentNullException(nameof(source));
      string[] fieldLines = source.Where(l => l.Length > 0 && l[0] is '¬').Select(f => f[1..]).ToArray();
      string[] fragtLines = source.Where(l => l.Length > 0 && l[0] is '>' or ':').ToArray();
      if (!fragtLines.Any()) {
         throw new ArgumentException("Map has no contents", nameof(source));
      }
      Width = fragtLines.Max(l => l.Length) - 1;
      List<MapFragment> fragments = new();
      int findex = 0;
      int top = 0;
      for (int i = 0; i < fragtLines.Length; ++i) {
         if (fragtLines[i][0] != '>') {
            continue;
         }
         int lwidth = Width - fragtLines[i].Length + 1;
         string line = fragtLines[i][1..] + (lwidth > 0 ? new string(' ', lwidth) : "");
         string foreMarkup = "";
         string backMarkup = "";
         if (i + 1 < fragtLines.Length && fragtLines[i + 1][0] == ':') {
            foreMarkup = fragtLines[i + 1][1..].ToUpper().TrimEnd();
            i += 1;
            if (i + 1 < fragtLines.Length && fragtLines[i + 1][0] == ':') {
               backMarkup = fragtLines[i + 1][1..].ToUpper().TrimEnd();
               i += 1;
            }
         }
         foreach (Match match in Regex.Matches(line, "¬")) {
            if (fieldLines.Length > findex) {
               if (findex > 0) {
                  Field prev = Fields[findex - 1];
                  if (prev.Top == top && match.Index > prev.Left && match.Index < prev.Left + prev.Width) {
                     throw new InvalidFieldException("Fields overlap", prev.Left, top);
                  }
               }
               Field field = new(this, match.Index, top, fieldLines[findex],
                  foreMarkup.Length >= match.Index + 1 ? ColorForHex(foreMarkup[match.Index]) : null,
                  backMarkup.Length >= match.Index + 1 ? ColorForHex(backMarkup[match.Index]) : null);
               if (Fields.Any(f => f.Name == field.Name)) {
                  throw new InvalidFieldException($"Duplicate field name {field.Name}", match.Index, top);
               }
               line = line.Remove(match.Index, field.Width)
                          .Insert(match.Index, new string(' ', field.Width));
               if (field.IsStatus) {
                  if (_messageField is not null) {
                     throw new InvalidFieldException("Duplicate status field", field.Left, field.Top);
                  }
                  _messageField = field;
               }
               Fields.Add(field);
               findex += 1;
            }
         }
         fragments.AddRange(MapFragment.Parse(line, foreMarkup, backMarkup));
         top += 1;
      }
      Height = top;
      Fragments = fragments.ToImmutableArray();
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

   private void SetMessage(string message, StatusMessageKind kind) {
      if (_messageField is not null) {
         _messageField.SetValue(message);
         StatusKind = kind;
         _messageField.IsDirty = true;
      }
   }
}
