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

namespace Lmpessoa.Mainframe;

/// <summary>
/// 
/// </summary>
internal sealed class MockConsole : IConsole {

   private readonly Queue<ConsoleKeyInfo> _keys = new();
   private readonly List<string> _buffer = new();
   private readonly List<string> _fore = new();
   private readonly List<string> _back = new();

   private char _highlightBack = '0';
   private char _highlightFore = 'F';
   private bool _activeBack = false;

   public MockConsole()
      => SetWindowSize(80, 24);

   public int BufferWidth
      => WindowWidth;

   public int BufferHeight
      => WindowHeight;

   public int CursorLeft { get; set; } = 0;

   public int CursorTop { get; set; } = 0;

   public int CursorSize { get; set; } = 1;

   public bool CursorVisible { get; set; } = true;

   public bool KeyAvailable
      => _keys.Any();

   public bool TreatControlCAsInput { get; set; } = false;

   public int WindowWidth
      => _buffer[0].Length;

   public int WindowHeight
      => _buffer.Count;

   public void Clear() {
      int width = WindowWidth;
      int height = WindowHeight;
      _buffer.Clear();
      _fore.Clear();
      _back.Clear();
      while (_buffer.Count < height) {
         _buffer.Add(new(' ', width));
         _fore.Add(new('7', width));
         _back.Add(new('0', width));
      }

   }

   public ConsoleKeyInfo ReadKey() {
      while (!_keys.Any()) { }
      return _keys.Dequeue();
   }

   public void SetBufferSize(int width, int height) { }

   public void SetCursorPosition(int left, int top)
      => (CursorLeft, CursorTop) = (left, top);

   public void SetWindowSize(int width, int height) {
      while (_buffer.Count != height) {
         if (_buffer.Count < height) {
            _buffer.Add(new(' ', width));
            _fore.Add(new('7', width));
            _back.Add(new('0', width));
         } else {
            _buffer.RemoveAt(_buffer.Count - 1);
            _fore.RemoveAt(_fore.Count - 1);
            _back.RemoveAt(_back.Count - 1);
         }
      }
      for (int i = 0; i < _buffer.Count; ++i) {
         string l = _buffer[i];
         _buffer[i] = l.Length > width
            ? l[..width]
            : l + new string(' ', width - l.Length);
         string f = _fore[i];
         _fore[i] = f.Length > width
            ? f[..width]
            : f + new string('7', width - f.Length);
         string b = _back[i];
         _back[i] = b.Length > width
            ? b[..width]
            : b + new string('0', width - b.Length);
      }
   }

   public void UseActiveFieldBackground()
      => _activeBack = true;

   public void UseHighlightColorInBackground(ConsoleColor color)
      => (_highlightBack, _highlightFore) = ("0123456789ABCDEF"[(int) color], '0');

   public void UseHighlightColorInForeground(ConsoleColor color)
      => (_highlightBack, _highlightFore) = ('0', "0123456789ABCDEF"[(int) color]);

   public void Write(MapPart part) {
      char _bg = part.BackgroundColor switch {
         MapPartColor.Default => '0',
         MapPartColor.Highlight => _highlightBack,
         _ => "0123456789ABCDEF"[(int) part.BackgroundColor],
      };
      char _fg = part.ForegroundColor switch {
         MapPartColor.Default => '7',
         MapPartColor.Highlight => _highlightFore,
         _ => "0123456789ABCDEF"[(int) part.ForegroundColor],
      };
      InternalWrite(part.Text, _bg, _fg);
   }

   public void Write(string value, FieldState state, StatusFieldSeverity severity) {
      char _bg = state is FieldState.Focused or FieldState.Editing && _activeBack ? '8' : '0';
      char _fg = severity switch {
         StatusFieldSeverity.Success => '2',
         StatusFieldSeverity.Alert => '6',
         StatusFieldSeverity.Error => '4',
         _ => 'F',
      };
      InternalWrite(value.Replace('\0', state is FieldState.Editable or FieldState.Editing ? '_' : ' '), _bg, _fg);
   }



   public void SendKey(ConsoleKey key)
      => SendKey(KeyModifier.None, key);

   public void SendKey(KeyModifier modifier, ConsoleKey key) {
      if (TreatControlCAsInput || !(modifier == KeyModifier.Ctrl && key == ConsoleKey.C)) {
         _keys.Enqueue(new ConsoleKeyInfo('\0', key, modifier.HasFlag(KeyModifier.Shift),
            modifier.HasFlag(KeyModifier.Alt), modifier.HasFlag(KeyModifier.Ctrl)));
      }
   }

   public void SendKeys(string keys) {
      foreach (char ch in keys) {
         _keys.Enqueue(new ConsoleKeyInfo(ch, ConsoleKey.A, char.IsUpper(ch), false, false));
      }
   }

   public void WaitForEmptyKeyBuffer() {
      while (_keys.Any()) { }
   }

   public MockConsoleInfo ReadScreen(int left, int top, int length)
      => new() {
         Text = _buffer[top][left..(left + length)],
         Foreground = _fore[top][left..(left + length)],
         Background = _back[top][left..(left + length)],
      };



   private void InternalWrite(string value, char bgcolor, char fgcolor) {
      if (value is not null) {
         _buffer[CursorTop] = _buffer[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, value);
         _fore[CursorTop] = _fore[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, new string(fgcolor, value.Length));
         _back[CursorTop] = _back[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, new string(bgcolor, value.Length));
         CursorLeft += value.Length;
      }
   }
}
