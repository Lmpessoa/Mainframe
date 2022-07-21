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

namespace Lmpessoa.Mainframe.Tests;

[TestClass]
public partial class ApplicationTest {

   [TestInitialize]
   public void Setup() {
      Console = new MockConsole();
      Console.SetWindowSize(80, 24);
      Map = new();
      App = new(Map, Console);
      App.Start();
      DoLoop();
   }

   [TestCleanup]
   public void Teardown() {
      App.Stop();
   }


   private MockConsole Console { get; set; }

   private Application App { get; set; }

   private TestMap Map { get; set; }

   protected void DoLoop(int count = 1) {
      for (int i = 0; i < count; ++i) {
         App.DoLoopStep();
      }
   }


   public sealed class TestMap : Map {

      public ConsoleKeyInfo KeyPressed { get; private set; }

      protected override void OnKeyPressed(ConsoleKeyInfo key) {
         KeyPressed = key;
      }
   }
}

internal static class ConsoleInfoExtension {

   public static void AssertIs(this MockConsoleInfo info, string text, string fgcolors, string? bgcolors = null) {
      Assert.AreEqual(text, info.Text);
      Assert.AreEqual(fgcolors, info.Foreground);
      if (bgcolors is not null) {
         Assert.AreEqual(bgcolors, info.Background);
      }
   }
}
