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
namespace Lmpessoa.Terminal;

/// <summary>
/// 
/// </summary>
public sealed class MockConsole : IConsole {

   private readonly Queue<ConsoleKeyInfo> _keys = new();
   private readonly List<string> _buffer = new();
   private readonly List<string> _fore = new();
   private readonly List<string> _back = new();

   /// <summary>
   /// 
   /// </summary>
   public MockConsole()
      => SetWindowSize(80, 24);

   public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

   public int BufferWidth { get; set; }

   public int BufferHeight { get; set; }

   public int CursorLeft { get; set; } = 0;

   public int CursorTop { get; set; } = 0;

   public int CursorSize { get; set; } = 1;

   public bool CursorVisible { get; set; } = true;

   public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

   public bool KeyAvailable
      => _keys.Any();

   public bool TreatControlCAsInput { get; set; } = false;

   public int WindowWidth { 
      get => _buffer[0].Length; 
      set => SetWindowSize(value, WindowHeight);
   }

   public int WindowHeight {
      get => _buffer.Count;
      set => SetWindowSize(WindowWidth, value);
   }


   public void Clear() {
      int width = WindowWidth;
      int height = WindowHeight;
      _buffer.Clear();
      _fore.Clear();
      _back.Clear();
      while (_buffer.Count < height) {
         _buffer.Add(new(' ', width));
         _fore.Add(new(ToHex(ForegroundColor), width));
         _back.Add(new(ToHex(BackgroundColor), width));
      }

   }

   public ConsoleKeyInfo ReadKey() {
      while (!_keys.Any()) { }
      return _keys.Dequeue();
   }

   public void ResetColor() {
      ForegroundColor = ConsoleColor.Gray;
      BackgroundColor = ConsoleColor.Black;
   }

   public void SetBufferSize(int width, int height)
      => (BufferWidth, BufferHeight) = (width, height);

   public void SetCursorPosition(int left, int top)
      => (CursorLeft, CursorTop) = (left, top);

   public void SetWindowSize(int width, int height) {
      while (_buffer.Count != height) {
         if (_buffer.Count < height) {
            _buffer.Add(new(' ', width));
            _fore.Add(new(ToHex(ForegroundColor), width));
            _back.Add(new(ToHex(BackgroundColor), width));
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
            : f + new string(ToHex(ForegroundColor), width - f.Length);
         string b = _back[i];
         _back[i] = b.Length > width
            ? b[..width]
            : b + new string(ToHex(BackgroundColor), width - b.Length);
      }
   }

   public void Write(string value) {
      if (value is not null) {
         _buffer[CursorTop] = _buffer[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, value);
         _fore[CursorTop] = _fore[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, new string(ToHex(ForegroundColor), value.Length));
         _back[CursorTop] = _back[CursorTop]
            .Remove(CursorLeft, value.Length)
            .Insert(CursorLeft, new string(ToHex(BackgroundColor), value.Length));
         CursorLeft += value.Length;
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="key"></param>
   public void SendKey(ConsoleKey key)
      => SendKey(KeyModifier.None, key);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="modifier"></param>
   /// <param name="key"></param>
   public void SendKey(KeyModifier modifier, ConsoleKey key) {
      if (TreatControlCAsInput || !(modifier == KeyModifier.Ctrl && key == ConsoleKey.C)) {
         _keys.Enqueue(new ConsoleKeyInfo('\0', key, modifier.HasFlag(KeyModifier.Shift), 
            modifier.HasFlag(KeyModifier.Alt), modifier.HasFlag(KeyModifier.Ctrl)));
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="keys"></param>
   public void SendKeys(string keys) {
      foreach (char ch in keys) {
         _keys.Enqueue(new ConsoleKeyInfo(ch, ConsoleKey.A, char.IsUpper(ch), false, false));
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public void WaitForEmptyKeyBuffer() {
      while (_keys.Any()) { }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="left"></param>
   /// <param name="top"></param>
   /// <param name="length"></param>
   /// <returns></returns>
   public MockConsoleInfo ReadScreen(int left, int top, int length)
      => new() {
            ScreenText = _buffer[top][left..(left + length)],
            Foreground = _fore[top][left..(left + length)],
            Background = _back[top][left..(left + length)],
         };

   private static char ToHex(ConsoleColor color)
      => "0123456789ABCDEF"[(int) color];
}
