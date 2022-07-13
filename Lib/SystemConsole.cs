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

internal sealed class SystemConsole : IConsole {

   public static IConsole Instance { get; } = new SystemConsole();

   public ConsoleColor BackgroundColor {
      get => Console.BackgroundColor;
      set => Console.BackgroundColor = value;
   }

   public int BufferWidth {
      get => Console.BufferWidth;
      set => Console.BufferWidth = value;
   }

   public int BufferHeight {
      get => Console.BufferHeight;
      set => Console.BufferHeight = value;
   }

   public int CursorLeft {
      get => Console.CursorLeft;
      set => Console.CursorLeft = value;
   }

   public int CursorSize {
      get => Console.CursorSize;
      set => Console.CursorSize = value;
   }

   public int CursorTop {
      get => Console.CursorTop;
      set => Console.CursorTop = value;
   }

   public bool CursorVisible {
      get => Console.CursorVisible;
      set => Console.CursorVisible = value;
   }

   public ConsoleColor ForegroundColor {
      get => Console.ForegroundColor;
      set => Console.ForegroundColor = value;
   }

   public bool KeyAvailable
      => Console.KeyAvailable;

   public bool TreatControlCAsInput {
      get => Console.TreatControlCAsInput;
      set => Console.TreatControlCAsInput = value;
   }

   public int WindowWidth {
      get => Console.WindowWidth;
      set => Console.WindowWidth = value;
   }

   public int WindowHeight {
      get => Console.WindowHeight;
      set => Console.WindowHeight = value;
   }

   public void Clear()
      => Console.Clear();

   public ConsoleKeyInfo ReadKey()
      => Console.ReadKey(true);

   public void ResetColor()
      => Console.ResetColor();

   public void SetBufferSize(int width, int height)
      => Console.SetBufferSize(width, height);

   public void SetCursorPosition(int left, int top)
      => Console.SetCursorPosition(left, top);

   public void SetWindowSize(int width, int height)
      => Console.SetWindowSize(width, height);

   public void Write(string value)
      => Console.Write(value);
}
