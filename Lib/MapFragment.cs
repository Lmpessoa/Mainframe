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
using System.Text.RegularExpressions;

namespace Lmpessoa.Mainframe;

internal sealed class MapFragment {

   public string Text { get; init; }

   public ConsoleColor ForegroundColor { get; init; }

   public ConsoleColor BackgroundColor { get; init; }

   public bool LineBreak { get; init; }

   public static IEnumerable<MapFragment> Parse(string text, string fore = "", string back = "") {
      if (fore.Length < text.Length) {
         fore += new string(' ', text.Length - fore.Length);
      }
      if (back.Length < text.Length) {
         back += new string(' ', text.Length - back.Length);
      }
      List<int> indices = new();
      indices.AddRange(Regex.Matches(fore, "(.)\\1*").Select(m => m.Index));
      indices.AddRange(Regex.Matches(back, "(.)\\1*").Select(m => m.Index));
      indices = indices.Distinct().OrderBy(i => i).ToList();

      List<MapFragment> result = new();
      for (int i = 0; i < indices.Count && indices[i] < text.Length; ++i) {
         result.Add(new() {
            Text = i < indices.Count - 1 ? text[indices[i]..indices[i + 1]] : text[indices[i]..],
            ForegroundColor = (ConsoleColor) "0123456789ABCDEF+".IndexOf(fore[indices[i]]),
            BackgroundColor = (ConsoleColor) (fore[indices[i]] == '+' ? 16 : "0123456789ABCDEF+".IndexOf(back[indices[i]])),
            LineBreak = i == indices.Count - 1,
         });
      }
      return result.ToImmutableList();
   }
}