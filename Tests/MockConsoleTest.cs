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
public sealed class MockConsoleTest {

   private MockConsole console;

   [TestInitialize]
   public void Initialize() {
      console = new MockConsole();
      console.SetCursorPosition(0, 0);
   }

   [TestMethod]
   public void TestSendKey1() {
      Assert.IsFalse(console.KeyAvailable);
      console.SendKey(ConsoleKey.F1);
      Assert.IsTrue(console.KeyAvailable);
      ConsoleKeyInfo key = console.ReadKey();
      Assert.AreEqual(ConsoleKey.F1, key.Key);
      Assert.AreEqual((ConsoleModifiers) 0, key.Modifiers);
      Assert.IsFalse(console.KeyAvailable);
   }

   [TestMethod]
   public void TestSendKey2() {
      Assert.IsFalse(console.KeyAvailable);
      console.SendKey(KeyModifier.CtrlShift, ConsoleKey.F2);
      Assert.IsTrue(console.KeyAvailable);
      ConsoleKeyInfo key = console.ReadKey();
      Assert.AreEqual(ConsoleKey.F2, key.Key);
      Assert.AreEqual(ConsoleModifiers.Control | ConsoleModifiers.Shift, key.Modifiers);
      Assert.IsFalse(console.KeyAvailable);
   }

   [TestMethod]
   public void TestSendKeys() {
      Assert.IsFalse(console.KeyAvailable);
      console.SendKeys("Test");

      Assert.IsTrue(console.KeyAvailable);
      ConsoleKeyInfo key = console.ReadKey();
      Assert.AreEqual('T', key.KeyChar);
      Assert.AreEqual(ConsoleModifiers.Shift, key.Modifiers);

      Assert.IsTrue(console.KeyAvailable);
      key = console.ReadKey();
      Assert.AreEqual('e', key.KeyChar);
      Assert.AreEqual((ConsoleModifiers) 0, key.Modifiers);

      Assert.IsTrue(console.KeyAvailable);
      key = console.ReadKey();
      Assert.AreEqual('s', key.KeyChar);
      Assert.AreEqual((ConsoleModifiers) 0, key.Modifiers);

      Assert.IsTrue(console.KeyAvailable);
      key = console.ReadKey();
      Assert.AreEqual('t', key.KeyChar);
      Assert.AreEqual((ConsoleModifiers) 0, key.Modifiers);

      Assert.IsFalse(console.KeyAvailable);
   }

   [TestMethod]
   public void TestControlCAsInput() {
      console.TreatControlCAsInput = false;
      console.SendKey(KeyModifier.Ctrl, ConsoleKey.C);
      Assert.IsFalse(console.KeyAvailable);

      console.TreatControlCAsInput = true;
      console.SendKey(KeyModifier.Ctrl, ConsoleKey.C);
      Assert.IsTrue(console.KeyAvailable);
      ConsoleKeyInfo key = console.ReadKey();
      Assert.AreEqual(ConsoleKey.C, key.Key);
      Assert.AreEqual(ConsoleModifiers.Control, key.Modifiers);
      Assert.IsFalse(console.KeyAvailable);
   }

   [TestMethod]
   public void TestWritePosition1() {
      Assert.AreEqual(0, console.CursorLeft);
      Assert.AreEqual(0, console.CursorTop);
      console.Write(MapPart.Parse("Test").First());
      Assert.AreEqual(4, console.CursorLeft);
      Assert.AreEqual(0, console.CursorTop);
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      Assert.AreEqual("Test", read.Text);
   }

   [TestMethod]
   public void TestWritePosition2() {
      console.SetCursorPosition(38, 12);
      Assert.AreEqual(38, console.CursorLeft);
      Assert.AreEqual(12, console.CursorTop);
      console.Write(MapPart.Parse("Test").First());
      Assert.AreEqual(42, console.CursorLeft);
      Assert.AreEqual(12, console.CursorTop);
      MockConsoleInfo read = console.ReadScreen(38, 12, 4);
      Assert.AreEqual("Test", read.Text);
      read = console.ReadScreen(0, 0, 4);
      Assert.AreEqual("    ", read.Text);
   }

   [TestMethod]
   public void TestWriteColours1() {
      console.Write(MapPart.Parse("Test").First());
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      read.AssertIs("Test", "7777", "0000");
   }

   [TestMethod]
   public void TestWriteColours2() {
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      read.AssertIs("    ", "7777", "0000");
      console.Write(MapPart.Parse("Test", "EEEE", "1111").First());
      read = console.ReadScreen(0, 0, 4);
      read.AssertIs("Test", "EEEE", "1111");
   }

   [TestMethod]
   public void TestClear() {
      console.Write(MapPart.Parse("Test", "2222", "FFFF").First());
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      read.AssertIs("Test", "2222", "FFFF");
      console.Clear();
      read = console.ReadScreen(0, 0, 4);
      read.AssertIs("    ", "7777", "0000");
   }

   [TestMethod]
   public void TestEnlargeWindowSize() {
      Assert.AreEqual(80, console.WindowWidth);
      Assert.AreEqual(24, console.WindowHeight);
      console.Write("Test",FieldState.None, StatusFieldSeverity.None);
      console.SetWindowSize(120, 30);
      Assert.AreEqual(120, console.WindowWidth);
      Assert.AreEqual(30, console.WindowHeight);
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      read.AssertIs("Test", "FFFF", "0000");
   }

   [TestMethod]
   public void TestShrinkWindowSize() {
      Assert.AreEqual(80, console.WindowWidth);
      Assert.AreEqual(24, console.WindowHeight);
      console.Write("Test", FieldState.None, StatusFieldSeverity.None);
      console.SetWindowSize(40, 10);
      Assert.AreEqual(40, console.WindowWidth);
      Assert.AreEqual(10, console.WindowHeight);
      MockConsoleInfo read = console.ReadScreen(0, 0, 4);
      read.AssertIs("Test", "FFFF", "0000");
   }
}
