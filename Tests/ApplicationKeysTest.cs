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

namespace Lmpessoa.Terminal.Tests;

[TestClass]
public class ApplicationKeysTest : ApplicationTest {

   [TestMethod]
   public void TestMovesToFieldWithTab() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKey(ConsoleKey.Tab);
      DoLoop();
      Assert.AreEqual(2, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMovesToFieldWithArrow() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);

      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop();
      Assert.AreEqual(2, Map.CurrentFieldIndex);

      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);

      Console.SendKey(ConsoleKey.LeftArrow);
      DoLoop();
      Assert.AreEqual(-1, Map.CurrentFieldIndex);

      Console.SendKey(ConsoleKey.RightArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMovesToFieldWithTabUsingFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("00000", info.Background);

      Console.SendKey(ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("22222", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
   }

   [TestMethod]
   public void TestMovesToFieldWithArrowDownUsingFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("00000", info.Background);

      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("22222", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
   }

   [TestMethod]
   public void TestMovesToFieldWithArrowUpUsingFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("00000", info.Background);

      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop(2);
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("22222", info.Background);
      info = Console.ReadScreen(11, 5, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("00000", info.Background);
   }

   [TestMethod]
   public void TestMovesToFieldWithTabUsingFieldColorsFromZero() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);

      Map.MoveFocusTo(0, 0);
      DoLoop();
      Assert.AreEqual(0, Console.CursorLeft);
      Assert.AreEqual(0, Console.CursorTop);
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("22222", info.Background);

      Console.SendKey(ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);
   }

   [TestMethod]
   public void TestMovesToFieldWithShiftTabUsingFieldColorsFromZero() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("88888", info.Background);

      Map.MoveFocusTo(0, 0);
      DoLoop();
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
      Assert.AreEqual(0, Console.CursorLeft);
      Assert.AreEqual(0, Console.CursorTop);
      info = Console.ReadScreen(11, 4, 5);
      Assert.AreEqual("_____", info.ScreenText);
      Assert.AreEqual("FFFFF", info.Foreground);
      Assert.AreEqual("22222", info.Background);

      Console.SendKey(KeyModifier.Shift, ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      info = Console.ReadScreen(21, 8, 5);
      Assert.AreEqual("_ NEV", info.ScreenText);
      Assert.AreEqual("F7777", info.Foreground);
      Assert.AreEqual("80000", info.Background);
   }

   [TestMethod]
   public void TestMovesToFieldWithShiftTab() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKey(KeyModifier.Shift, ConsoleKey.Tab);
      DoLoop();
      Assert.AreEqual(8, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestInsertOffPushesText() {
      ((TextField) Map.CurrentField!).Value = "TEST";
      DoLoop();
      Assert.IsTrue(App.InsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.ScreenText);

      Console.SendKeys("Re");
      DoLoop(3);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReTEST__", info.ScreenText);
   }

   [TestMethod]
   public void TestInsertOnOverwritesText() {
      ((TextField) Map.CurrentField!).Value = "TEST";
      DoLoop();
      Assert.IsTrue(App.InsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      Console.SendKey(ConsoleKey.Insert);
      DoLoop();
      Assert.IsFalse(App.InsertMode);
      Assert.AreEqual(100, Console.CursorSize);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.ScreenText);

      Console.SendKeys("Re");
      DoLoop(3);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReST____", info.ScreenText);
      Console.SendKey(ConsoleKey.Insert);
      Console.SendKeys("A");
      DoLoop(3);
      Assert.IsTrue(App.InsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReAST___", info.ScreenText);
   }

   [TestMethod]
   public void TestDeletingFromTextFieldEdges() {
      ((TextField) Map.CurrentField!).Value = "-TEST-";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("-TEST-__", info.ScreenText);
      Assert.AreEqual(11, Console.CursorLeft);

      Console.SendKey(ConsoleKey.End);
      DoLoop();
      Assert.AreEqual(17, Console.CursorLeft);

      Console.SendKey(ConsoleKey.Backspace);
      DoLoop(2);
      Assert.AreEqual(16, Console.CursorLeft);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("-TEST___", info.ScreenText);

      Console.SendKey(ConsoleKey.Home);
      DoLoop();
      Assert.AreEqual(11, Console.CursorLeft);

      Console.SendKey(ConsoleKey.Delete);
      DoLoop(2);
      Assert.AreEqual(11, Console.CursorLeft);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.ScreenText);
   }

   [TestMethod]
   public void TestMovesToNextFieldAfterLastPositionInput() {
      Console.CursorLeft = 30;
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKeys("X");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(26, 4, 6);
      Assert.AreEqual("____X ", info.ScreenText);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      Assert.AreEqual(5, Console.CursorTop);
      Assert.AreEqual(11, Console.CursorLeft);
   }

   [TestMethod]
   public void TestMapHandlesControlTab() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Ctrl, ConsoleKey.Tab);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.Tab, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesShiftDownArrow() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Shift, ConsoleKey.DownArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.DownArrow, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Shift, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesAltInsert() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Alt, ConsoleKey.Insert);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.Insert, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Alt, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesControlDelete() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Ctrl, ConsoleKey.Delete);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.Delete, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesShiftBackspace() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Shift, ConsoleKey.Backspace);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.Backspace, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Shift, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesAltHome() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Alt, ConsoleKey.Home);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.Home, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Alt, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesControlEnd() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(KeyModifier.Ctrl, ConsoleKey.End);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.End, Map.KeyPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestMapHandlesPF3() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.F3);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(ConsoleKey.F3, Map.KeyPressed.Key);
      Assert.AreEqual((ConsoleModifiers) 0, Map.KeyPressed.Modifiers);
   }

   [TestMethod]
   public void TestCheckFieldChangesWithX() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Assert.AreEqual(6, Console.CursorTop);
      Assert.IsTrue(Map.Fields[3] is CheckField);
      CheckField field = (CheckField) Map.Fields[3];
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Console.SendKeys("X");
      DoLoop();
      Assert.AreEqual("X", field.Value);
      Assert.IsTrue(field.IsChecked);
      Assert.AreEqual(4, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestCheckFieldChangesWithSlash() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Assert.AreEqual(6, Console.CursorTop);
      Assert.IsTrue(Map.Fields[3] is CheckField);
      CheckField field = (CheckField) Map.Fields[3];
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Console.SendKeys("/");
      DoLoop();
      Assert.AreEqual("/", field.Value);
      Assert.IsTrue(field.IsChecked);
      Assert.AreEqual(4, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestCheckFieldChangesWithSpace() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Assert.IsTrue(Map.Fields[3] is CheckField);
      CheckField field = (CheckField) Map.Fields[3];
      field.Value = "X";
      Assert.AreEqual(6, Console.CursorTop);
      Assert.AreEqual("X", field.Value);
      Assert.IsTrue(field.IsChecked);
      Console.SendKeys(" ");
      DoLoop();
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Assert.AreEqual(4, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestCheckFieldIgnoresAOrDash() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Assert.AreEqual(6, Console.CursorTop);
      Assert.IsTrue(Map.Fields[3] is CheckField);
      CheckField field = (CheckField) Map.Fields[3];
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Console.SendKeys("A");
      DoLoop();
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Console.SendKeys("-");
      DoLoop();
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestPreservingValuesGivenInFields() {
      Assert.IsTrue(Map.Fields[1] is TextField);
      TextField field = (TextField) Map.Fields[1];
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      MockConsoleInfo info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("SOME VALUE X__", info.ScreenText);
      Assert.AreEqual("SOME VALUE X", field.Value);
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      field.Value = "";
      Console.SendKey(ConsoleKey.Home);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.ScreenText);
      Assert.AreEqual("Some value X", field.Value);
   }
}
