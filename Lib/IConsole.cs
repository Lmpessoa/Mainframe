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
public interface IConsole {

   /// <summary>
   /// 
   /// </summary>
   ConsoleColor BackgroundColor { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int BufferWidth { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int BufferHeight { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int CursorLeft { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int CursorTop { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int CursorSize { get; set; }

   /// <summary>
   /// 
   /// </summary>
   bool CursorVisible { get; set; }

   /// <summary>
   /// 
   /// </summary>
   ConsoleColor ForegroundColor { get; set; }

   /// <summary>
   /// 
   /// </summary>
   bool KeyAvailable { get; }

   /// <summary>
   /// 
   /// </summary>
   bool TreatControlCAsInput { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int WindowWidth { get; set; }

   /// <summary>
   /// 
   /// </summary>
   int WindowHeight { get; set; }

   /// <summary>
   /// 
   /// </summary>
   void Clear();

   /// <summary>
   /// 
   /// </summary>
   /// <returns></returns>
   ConsoleKeyInfo ReadKey();

   /// <summary>
   /// 
   /// </summary>
   void ResetColor();

   /// <summary>
   /// 
   /// </summary>
   /// <param name="width"></param>
   /// <param name="height"></param>
   void SetBufferSize(int width, int height);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="left"></param>
   /// <param name="top"></param>
   void SetCursorPosition(int left, int top);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="width"></param>
   /// <param name="height"></param>
   void SetWindowSize(int width, int height);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="value"></param>
   void Write(string value);
}
