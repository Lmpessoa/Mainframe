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

   private const int DEFAULT_WIDTH = 80;
   private const int DEFAULT_HEIGHT = 24;

   private readonly Queue<ConsoleKeyInfo> _keys = new();
   private readonly List<string> _textBuffer = new();
   private readonly List<string> _foreBuffer = new();
   private readonly List<string> _backBuffer = new();

   private (int Width, int Height) _size;
   private char _foreColor = '7';
   private char _backColor = '0';

   public MockConsole()
      => SetWindowSize(DEFAULT_WIDTH, DEFAULT_HEIGHT);

   public ConsoleColor BackgroundColor {
      get => CharToColor(_backColor);
      set => _backColor = ColorToChar(value);
   }

   public int BufferWidth
      => _size.Width;

   public int BufferHeight
      => _size.Height;

   public int CursorLeft { get; set; }

   public int CursorTop { get; set; }

   public int CursorSize { get; set; } = 1;

   public bool CursorVisible { get; set; } = true;

   public ConsoleColor ForegroundColor {
      get => CharToColor(_foreColor);
      set => _foreColor = ColorToChar(value);
   }

   public bool KeyAvailable
      => _keys.Any();

   public bool TreatControlCAsInput { get; set; }

   public int WindowWidth
      => _size.Width;

   public int WindowHeight
      => _size.Height;

   public void Clear() {
      (int width, int height) = _size;
      _textBuffer.Clear();
      _foreBuffer.Clear();
      _backBuffer.Clear();
      while (_textBuffer.Count < height) {
         _textBuffer.Add(new(' ', width));
         _foreBuffer.Add(new(_foreColor, width));
         _backBuffer.Add(new(_backColor, width));
      }
   }

   public ConsoleKeyInfo ReadKey() {
      while (!_keys.Any()) { 
         // Wait for an available key
      }
      return _keys.Dequeue();
   }

   public void ResetColor()
      => (_backColor, _foreColor) = ('0', '7');

   public void SetBufferSize(int width, int height) { }

   public void SetCursorPosition(int left, int top)
      => (CursorLeft, CursorTop) = (left, top);

   public void SetWindowSize(int width, int height) {
      _size = (width, height);
      while (_textBuffer.Count != height) {
         if (_textBuffer.Count < height) {
            _textBuffer.Add(new(' ', width));
            _foreBuffer.Add(new(_foreColor, width));
            _backBuffer.Add(new(_backColor, width));
         } else {
            _textBuffer.RemoveAt(_textBuffer.Count - 1);
            _foreBuffer.RemoveAt(_foreBuffer.Count - 1);
            _backBuffer.RemoveAt(_backBuffer.Count - 1);
         }
      }
      for (int i = 0; i < _textBuffer.Count; ++i) {
         string l = _textBuffer[i];
         _textBuffer[i] = l.Length > width ? l[..width] : l + new string(' ', width - l.Length);
         string f = _foreBuffer[i];
         _foreBuffer[i] = f.Length > width ? f[..width] : f + new string(_foreColor, width - f.Length);
         string b = _backBuffer[i];
         _backBuffer[i] = b.Length > width ? b[..width] : b + new string(_backColor, width - b.Length);
      }
   }

   public void Write(string str) {
      if (str is not null) {
         _textBuffer[CursorTop] = _textBuffer[CursorTop]
            .Remove(CursorLeft, str.Length)
            .Insert(CursorLeft, str);
         _foreBuffer[CursorTop] = _foreBuffer[CursorTop]
            .Remove(CursorLeft, str.Length)
            .Insert(CursorLeft, new string(_foreColor, str.Length));
         _backBuffer[CursorTop] = _backBuffer[CursorTop]
            .Remove(CursorLeft, str.Length)
            .Insert(CursorLeft, new string(_backColor, str.Length));
         CursorLeft += str.Length;
      }
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
      while (_keys.Any()) { 
         // Just ignore while has keys
      }
   }

   public MockConsoleInfo ReadScreen(int left, int top, int length)
      => new() {
            Text = _textBuffer[top][left..(left + length)],
            Foreground = _foreBuffer[top][left..(left + length)],
            Background = _backBuffer[top][left..(left + length)],
         };


      private static ConsoleColor CharToColor(char ch)
      => (ConsoleColor) "0123456789ABCDEF".IndexOf(ch);

   private static char ColorToChar(ConsoleColor color)
      => color is >= ConsoleColor.Black and <= ConsoleColor.White ? "0123456789ABCDEF"[(int) color] : '0';
}
