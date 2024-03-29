﻿/*
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

namespace Lmpessoa.Mainframe.Test;

internal enum BookFormat {
   Paperback,
   HardCover,
   EBook,
}

internal class BookInfoMap : Map {

   [OnKeyPressed(ConsoleKey.F3)]
   public void DoClose() => Close();

   internal string Title {
      get => Get<string>("Title");
      set => Set("Title", value);
   }

   internal string SortTitle {
      get => Get<string>("SortTitle");
      set => Set("SortTitle", value);
   }

   internal BookFormat Format {
      get {
         if (Get<bool>("FormatPaperback")) {
            return BookFormat.Paperback;
         } else if (Get<bool>("FormatHardCover")) {
            return BookFormat.HardCover;
         } else if (Get<bool>("FormatEBook")) {
            return BookFormat.EBook;
         }
         return (BookFormat) (-1);
      }
      set => Set($"Format{value}", true);
   }
}
