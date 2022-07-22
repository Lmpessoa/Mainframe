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

using System.Collections;

using Lmpessoa.Mainframe.Fields;

namespace Lmpessoa.Mainframe;

internal sealed class FieldSet : IEnumerable<FieldBase> {

   public IEnumerator<FieldBase> GetEnumerator()
      => _fields.GetEnumerator();


   internal FieldBase this[int index]
      => _fields[index];

   internal int Count
      => _fields.Count;

   internal void Add(FieldBase field)
      => _fields.Add(field);

   internal void Clear() 
      => _fields.Clear();

   internal int IndexOf(FieldBase field)
      => _fields.IndexOf(field);

   internal void Move(int index, int newIndex) {
      if (index < 0 || index >= _fields.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }
      FieldBase field = _fields[index];
      _fields.Remove(field);
      _fields.Insert(Math.Min(Math.Max(0, newIndex), _fields.Count), field);
   }


   private readonly List<FieldBase> _fields = new();


   IEnumerator IEnumerable.GetEnumerator()
      => _fields.GetEnumerator();
}
