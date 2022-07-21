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

internal enum ConsoleCursorMode {
   Offset,
   FieldNext,
   FieldLeft,
   FieldRight,
   FieldAbove,
   FieldBelow,
}

internal class ConsoleCursor {

   public ConsoleCursor(int left, int top)
      => (Left, Top) = (left, top);

   public int Left { get; }

   public int Top { get; }

   public (int Left, int Top)? Offset { get; private set; }
 
   public ConsoleCursorMode Mode { get; private set; }

   public void MoveLeft(int count = 1)
      => (Mode, Offset) = (ConsoleCursorMode.Offset, (-count, 0));

   public void MoveRight(int count = 1)
      => (Mode, Offset) = (ConsoleCursorMode.Offset, (count, 0));

   public void MoveUp(int count = 1)
      => (Mode, Offset) = (ConsoleCursorMode.Offset, (0, -count));

   public void MoveDown(int count = 1)
      => (Mode, Offset) = (ConsoleCursorMode.Offset, (0, count));

   public void SetOffset(int left, int top)
      => (Mode, Offset) = (ConsoleCursorMode.Offset, (left, top));

   public void MoveNextField()
      => (Mode, Offset) = (ConsoleCursorMode.FieldNext, null);

   public void MoveFieldLeft()
      => (Mode, Offset) = (ConsoleCursorMode.FieldLeft, null);

   public void MoveFieldRight()
      => (Mode, Offset) = (ConsoleCursorMode.FieldRight, null);

   public void MoveFieldAbove()
      => (Mode, Offset) = (ConsoleCursorMode.FieldAbove, null);

   public void MoveFieldBelow()
      => (Mode, Offset) = (ConsoleCursorMode.FieldBelow, null);
}
