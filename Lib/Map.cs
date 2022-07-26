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

using Lmpessoa.Mainframe.Fields;

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
   public bool IsPopup { get; private set; }

   /// <summary>
   /// 
   /// </summary>
   public void ClearMessage() {
      if (StatusField is not null) {
         StatusField.SetValue("");
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
      if (fieldIndex >= 0 && fieldIndex < Fields.Count && fieldIndex != CurrentFieldIndex
            && Fields[fieldIndex].IsFocusable) {
         if (CurrentField is not null) {
            CurrentField.IsDirty = true;
            LostFocus(CurrentField);
         }
         CurrentFieldIndex = fieldIndex;
         if (CurrentField is not null) {
            CurrentField.IsDirty = true;
            Application.SetCursorPosition(Left + CurrentField.Left, Top + CurrentField.Top);
         }
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetMessage(string message)
      => SetMessage(null, message, StatusFieldSeverity.Info);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   /// <param name="message"></param>
   public void SetMessage(string field, string message)
      => SetMessage(field, message, StatusFieldSeverity.Info);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetSuccess(string message)
      => SetMessage(null, message, StatusFieldSeverity.Success);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   /// <param name="message"></param>
   public void SetSuccess(string field, string message)
      => SetMessage(field, message, StatusFieldSeverity.Success);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetAlert(string message)
      => SetMessage(null, message, StatusFieldSeverity.Alert);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   /// <param name="message"></param>
   public void SetAlert(string field, string message)
      => SetMessage(field, message, StatusFieldSeverity.Alert);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="message"></param>
   public void SetError(string message)
      => SetMessage(null, message, StatusFieldSeverity.Error);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   /// <param name="message"></param>
   public void SetError(string field, string message)
      => SetMessage(field, message, StatusFieldSeverity.Error);

   /// <summary>
   /// 
   /// </summary>
   /// <exception cref="InvalidOperationException"></exception>
   public void Show() {
      if (Application is not null) {
         throw new InvalidOperationException();
      }
      IsPopup = false;
      CurrentFieldIndex = -1;
      Application.Push(this);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <exception cref="InvalidOperationException"></exception>
   public void ShowPopup() {
      if (Application is not null) {
         throw new InvalidOperationException();
      }
      IsPopup = true;
      CurrentFieldIndex = -1;
      Application.Push(this);
   }



   /// <summary>
   /// 
   /// </summary>
   protected virtual void WillActivate() { }

   /// <summary>
   /// 
   /// </summary>
   protected virtual void DidClose() { }

   /// <summary>
   /// 
   /// </summary>
   /// <returns></returns>
   protected virtual bool WillClose() => true;

   /// <summary>
   /// 
   /// </summary>
   protected virtual void WillDeactivate() { }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="key"></param>
   protected virtual void DidKeyPress(ConsoleKeyInfo key) { }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="field"></param>
   protected virtual void DidLostFocus(string fieldName) { }



   /// <summary>
   /// 
   /// </summary>
   /// <param name="fieldName"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   protected internal object Get(string fieldName) {
      if (GetField(fieldName) is not FieldBase field) {
         throw new ArgumentException($"Field '{fieldName}' is not defined");
      }
      return field.GetValue();
   }

   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="fieldName"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   protected internal T Get<T>(string fieldName) {
      if (GetField(fieldName) is not FieldBase field) {
         throw new ArgumentException($"Field '{fieldName}' is not defined");
      }
      return (T) field.GetValue();
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="fieldName"></param>
   /// <param name="value"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   protected internal bool Set<T>(string fieldName, T? value) {
      if (GetField(fieldName) is not FieldBase field) {
         throw new ArgumentException($"Field '{fieldName}' is not defined");
      }
      return field.SetValue(value);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="fieldName"></param>
   /// <param name="visible"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   protected internal bool SetFieldVisible(string fieldName, bool visible) {
      if (GetField(fieldName) is not FieldBase field) {
         throw new ArgumentException($"Field '{fieldName}' is not defined");
      }
      if (field != StatusField) {
         field.IsVisible = visible;
         if (field == CurrentField) {
            FocusOnNextField();
         }
         return true;
      }
      return false;
   }



   internal Map(params string[] contents) {
      InitFromContents(contents);
   }

   internal Application? Application { get; set; }

   internal IEnumerable<MapPart> Parts { get; private set; } = Array.Empty<MapPart>();

   internal FieldBase? CurrentField
      => CurrentFieldIndex > -1 && CurrentFieldIndex < Fields.Count ? Fields[CurrentFieldIndex] : null;

   internal int CurrentFieldIndex { get; private set; } = -1;

   internal (int, int)? _lastPos = null;

   internal FieldSet Fields { get; } = new();

   internal int Height { get; private set; }

   internal int Left { get; set; }

   internal Label? StatusField { get; private set; }

   internal int Top { get; set; }

   internal int Width { get; private set; }

   internal (int, int)? Activate() {
      WillActivate();
      return _lastPos;
   }

   internal void Closed()
      => DidClose();

   internal bool Closing()
      => WillClose();

   internal void Deactivate((int, int) position) {
      _lastPos = position;
      WillDeactivate();
   }

   internal void KeyPressed(ConsoleKeyInfo key, ConsoleCursor cursor) {
      bool handled = false;
      if (CurrentField is FieldBase field) {
         handled = field.KeyPressed(key, cursor);
      }
      if (!handled) {
         ConsoleKeyInfo key2 = new('\0', key.Key,
           (key.Modifiers & ConsoleModifiers.Shift) != 0,
           (key.Modifiers & ConsoleModifiers.Alt) != 0,
           (key.Modifiers & ConsoleModifiers.Control) != 0);
         if (_actions.ContainsKey(key2)) {
            _actions[key2].Invoke();
         } else {
            DidKeyPress(key);
         }
      }
   }

   internal void LostFocus(FieldBase field) {
      if (Application.PreserveValues == PreserveValuesLevel.None && field is InputFieldBase input) {
         input.SetInnerValue(input.GetInnerValue().ToUpper());
      }
      DidLostFocus(field.Name);
   }

   internal void FocusOnFieldAbove((int Left, int Top) pos) {
      if (CurrentField is FieldBase field) {
         IEnumerable<FieldBase> fields = Fields.Where(f => f.Top < pos.Top && f.IsFocusable);
         if (fields.Any()) {
            int nextLine = fields.Max(f => f.Top);
            FieldBase? newField = fields.Where(f => f.Top == nextLine)
                                        .OrderBy(f => (pos.Left <= Left + f.Left
                                           ? Left + f.Left - pos.Left
                                           : pos.Left - Left - f.Left - f.Width))
                                        .FirstOrDefault();
            if (newField is not null) {
               pos.Top = newField.Top;
               if (Left + newField.Left > pos.Left) {
                  pos.Left = Left + newField.Left;
               } else if (pos.Left > Left + newField.Left + newField.Width) {
                  pos.Left = Left + newField.Left + newField.Width;
               }
               MoveFocusTo(pos.Left, pos.Top);
            }
         }
      }
   }

   internal void FocusOnFieldBelow((int Left, int Top) pos) {
      if (CurrentField is FieldBase field) {
         IEnumerable<FieldBase> fields = Fields.Where(f => f.Top > pos.Top && f.IsFocusable);
         if (fields.Any()) {
            int nextLine = fields.Min(f => f.Top);
            FieldBase? newField = fields.Where(f => f.Top == nextLine)
                                        .OrderBy(f => (pos.Left <= Left + f.Left
                                           ? Left + f.Left - pos.Left
                                           : pos.Left - Left - f.Left - f.Width))
                                        .FirstOrDefault();
            if (newField is not null) {
               pos.Top = newField.Top;
               if (Left + newField.Left > pos.Left) {
                  pos.Left = Left + newField.Left;
               } else if (pos.Left > Left + newField.Left + newField.Width) {
                  pos.Left = Left + newField.Left + newField.Width;
               }
               MoveFocusTo(pos.Left, pos.Top);
            }
         }
      }
   }

   internal void FocusOnFieldToLeft() {
      if (CurrentField is FieldBase field) {
         int cleft = Left + field.Left;
         int ctop = Top + field.Top;
         FieldBase? newField = Fields.Where(f => Top + f.Top == ctop && Left + f.Left < cleft && f.IsFocusable)
                                     .OrderByDescending(f => f.Left)
                                     .FirstOrDefault();
         if (newField is not null) {
            cleft = Left + newField.Left + newField.Width;
            MoveFocusTo(cleft, ctop);
         }
      }
   }

   internal void FocusOnFieldToRight() {
      if (CurrentField is FieldBase field) {
         int cleft = Left + field.Left + field.Width;
         int ctop = Top + field.Top;
         FieldBase? newField = Fields.Where(f => f.Top == ctop && f.Left > cleft && f.IsFocusable)
                                     .OrderBy(f => f.Left)
                                     .FirstOrDefault();
         if (newField is not null) {
            cleft = Left + newField.Left;
            MoveFocusTo(cleft, ctop);
         }
      }
   }

   internal void MoveFocusTo(int left, int top) {
      Application.SetCursorPosition(left, top);
      FieldBase? next = Fields.FirstOrDefault(f => f.IsFocusable && top == f.Top + Top
               && left >= f.Left + Left && left <= f.Left + Left + f.Width);
      int nextIndex = next is not null ? Fields.IndexOf(next) : -1;
      if (nextIndex != CurrentFieldIndex) {
         if (CurrentField is not null) {
            CurrentField.IsDirty = true;
            LostFocus(CurrentField);
         }
         CurrentFieldIndex = nextIndex;
         if (CurrentField is not null) {
            CurrentField.IsDirty = true;
         }
      }
   }

   internal void Redraw(ConsoleWrapper console, bool viewDirty) {
      if (viewDirty) {
         if (IsPopup) {
            if (Application.BorderStyle != WindowBorder.None) {
               string borderChars = Application.BorderStyle switch {
                  WindowBorder.Square => "┌─┐│└─┘",
                  WindowBorder.Rounded => "╭─╮│╰─╯",
                  WindowBorder.Heavy => "┏━┓┃┗━┛",
                  WindowBorder.Double => "╔═╗║╚═╝",
                  _ => "+-+|+-+",
               };
               console.CursorPosition = (Left - 1, Top - 1);
               console.WritePart(MapPart.Parse($"{borderChars[0]}{new string(borderChars[1], Width)}{borderChars[2]}").First());
               console.CursorPosition = (Left - 1, Top + Height);
               console.WritePart(MapPart.Parse($"{borderChars[4]}{new string(borderChars[5], Width)}{borderChars[6]}").First());
               MapPart vertBorder = MapPart.Parse($"{borderChars[3]}{new string(' ', Width)}{borderChars[3]}").First();
               for (int i = 0; i < Height; ++i) {
                  console.CursorPosition = (Left - 1, Top + i);
                  console.WritePart(vertBorder);
               }
            }
            console.CursorPosition = (Left, Top);
         } else {
            console.CursorPosition = (0, 0);
         }
         (int swidth, int sheight) = console.ScreenSize;
         foreach (MapPart part in Parts) {
            console.WritePart(part);
            (int cleft, int ctop) = console.CursorPosition;
            if (part.LineBreak) {
               if (!IsPopup && swidth - cleft > 0) {
                  console.WritePart(MapPart.Parse(new string(' ', swidth - cleft)).First());
               }
               console.CursorPosition = (Left, ctop + 1);
            }
         }
         if (!IsPopup) {
            (int cleft, int ctop) = console.CursorPosition;
            for (int y = ctop; y < sheight; ++y) {
               console.CursorPosition = (0, y);
               console.WritePart(MapPart.Parse(new string(' ', swidth - cleft)).First());
            }
         }
      }
      foreach (FieldBase field in Fields.Where(f => viewDirty || f.IsDirty)) {
         field.Redraw(console, field == CurrentField);
      }
   }



   private readonly Dictionary<ConsoleKeyInfo, Action> _actions = new();

   private void FocusOnNext(bool reverse) {
      List<FieldBase> fields = Fields.Where(f => f.IsFocusable).ToList();
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

   private FieldBase? GetField(string fieldName)
      => Fields.FirstOrDefault(f => f.Name == fieldName);

   private void InitFromContents(IEnumerable<string> source) {
      source = source ?? throw new ArgumentNullException(nameof(source));
      string[] fieldLines = source.Where(l => l.Length > 0 && l[0] is '¬').Select(f => f[1..]).ToArray();
      string[] partLines = source.Where(l => l.Length > 0 && l[0] is '>' or ':').ToArray();
      if (!partLines.Any()) {
         throw new ArgumentException("Map has no contents");
      }
      Width = partLines.Max(l => l.Length) - 1;
      List<MapPart> parts = new();
      int findex = 0;
      int top = 0;
      for (int i = 0; i < partLines.Length; ++i) {
         if (partLines[i][0] != '>') {
            continue;
         }
         int lwidth = Width - partLines[i].Length + 1;
         string line = partLines[i][1..] + (lwidth > 0 ? new string(' ', lwidth) : "");
         string foreMarkup = "";
         string backMarkup = "";
         if (i + 1 < partLines.Length && partLines[i + 1][0] == ':') {
            foreMarkup = partLines[i + 1][1..].ToUpper().TrimEnd();
            i += 1;
            if (i + 1 < partLines.Length && partLines[i + 1][0] == ':') {
               backMarkup = partLines[i + 1][1..].ToUpper().TrimEnd();
               i += 1;
            }
         }
         foreach (Match match in Regex.Matches(line, "¬")) {
            if (fieldLines.Length > findex) {
               FieldBase field = FieldBase.Create(this, match.Index, top, fieldLines[findex]);
               if (findex > 0) {
                  FieldBase prev = Fields[findex - 1];
                  if (prev.Top == top && match.Index > prev.Left && match.Index < prev.Left + prev.Width) {
                     throw new ArgumentException($"Fields overlap: '{prev.Name}' and '{field.Name}'");
                  }
               }
               if (Fields.Any(f => f.Name == field.Name)) {
                  throw new ArgumentException($"Duplicate field name: '{field.Name}'");
               }
               line = line.Remove(match.Index, field.Width)
                          .Insert(match.Index, new string(' ', field.Width));
               if (Regex.IsMatch(fieldLines[findex], ":STA[\\[\\(]")) {
                  if (StatusField is not null) {
                     throw new ArgumentException($"Duplicate status field: '{field.Name}'");
                  }
                  StatusField = (Label) field;
               }
               Fields.Add(field);
               findex += 1;
            }
         }
         parts.AddRange(MapPart.Parse(line, foreMarkup, backMarkup));
         top += 1;
      }
      Height = top;
      Parts = parts.ToImmutableArray();
      var methods = this.GetType()
         .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
         .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
         .Select(m => (m, m.GetCustomAttribute<OnKeyPressedAttribute>()))
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

   private void SetMessage(string? fieldName, string message, StatusFieldSeverity kind) {
      FieldBase? field = fieldName is not null ? Fields.FirstOrDefault(f => f.Name == fieldName) : null;
      if (StatusField is not null && (fieldName is null || field is not null)) {
         foreach (FieldBase f in Fields) {
            f.IsDirty = f.Severity != StatusFieldSeverity.None;
            f.Severity = StatusFieldSeverity.None;
         }
         StatusField.SetValue(message);
         StatusField.Severity = kind;
         StatusField.IsDirty = true;
         if (field is not null) {
            MoveFocus(Fields.IndexOf(field));
            field.Severity = kind;
            field.IsDirty = true;
         }
      }
   }
}
