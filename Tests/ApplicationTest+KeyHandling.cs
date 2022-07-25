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

using Lmpessoa.Mainframe.Fields;

namespace Lmpessoa.Mainframe.Tests;

public partial class ApplicationTest {

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
      Assert.AreEqual(1, Map.CurrentFieldIndex);

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
      info.AssertIs("_____", "FFFFF", "88888");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "00000");

      Console.SendKey(ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "00000");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "88888");
   }

   [TestMethod]
   public void TestMovesToFieldWithArrowDownUsingFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "00000");

      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop(2);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "00000");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "88888");
   }

   [TestMethod]
   public void TestMovesToFieldWithArrowUpUsingFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "00000");

      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop(2);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");
      info = Console.ReadScreen(11, 5, 5);
      info.AssertIs("_____", "FFFFF", "00000");
   }

   [TestMethod]
   public void TestMovesToFieldWithTabUsingFieldColorsFromZero() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");

      Map.MoveFocusTo(0, 0);
      DoLoop();
      Assert.AreEqual(0, Console.CursorLeft);
      Assert.AreEqual(0, Console.CursorTop);
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "00000");

      Console.SendKey(ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");
   }

   [TestMethod]
   public void TestMovesToFieldWithShiftTabUsingFieldColorsFromZero() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "88888");

      Map.MoveFocusTo(0, 0);
      DoLoop();
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
      Assert.AreEqual(0, Console.CursorLeft);
      Assert.AreEqual(0, Console.CursorTop);
      info = Console.ReadScreen(11, 4, 5);
      info.AssertIs("_____", "FFFFF", "00000");

      Console.SendKey(KeyModifier.Shift, ConsoleKey.Tab);
      DoLoop(2);
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      info = Console.ReadScreen(21, 8, 5);
      info.AssertIs("_ NEV", "F7777", "80000");
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
      Map.CurrentField!.SetValue("TEST");
      DoLoop();
      Assert.IsTrue(Application.IsInsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.Text);

      Console.SendKeys("Re");
      DoLoop(3);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReTEST__", info.Text);
   }

   [TestMethod]
   public void TestInsertOnOverwritesText() {
      Map.CurrentField!.SetValue("TEST");
      DoLoop();
      Assert.IsTrue(Application.IsInsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      Console.SendKey(ConsoleKey.Insert);
      DoLoop();
      Assert.IsFalse(Application.IsInsertMode);
      Assert.AreEqual(100, Console.CursorSize);
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.Text);

      Console.SendKeys("Re");
      DoLoop(3);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReST____", info.Text);
      Console.SendKey(ConsoleKey.Insert);
      Console.SendKeys("A");
      DoLoop(3);
      Assert.IsTrue(Application.IsInsertMode);
      Assert.AreEqual(1, Console.CursorSize);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("ReAST___", info.Text);
   }

   [TestMethod]
   public void TestDeletingFromTextFieldEdges() {
      Map.CurrentField!.SetValue("-TEST-");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("-TEST-__", info.Text);
      Assert.AreEqual(11, Console.CursorLeft);

      Console.SendKey(ConsoleKey.End);
      DoLoop();
      Assert.AreEqual(17, Console.CursorLeft);

      Console.SendKey(ConsoleKey.Backspace);
      DoLoop(2);
      Assert.AreEqual(16, Console.CursorLeft);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("-TEST___", info.Text);

      Console.SendKey(ConsoleKey.Home);
      DoLoop();
      Assert.AreEqual(11, Console.CursorLeft);

      Console.SendKey(ConsoleKey.Delete);
      DoLoop(2);
      Assert.AreEqual(11, Console.CursorLeft);
      info = Console.ReadScreen(11, 4, 8);
      Assert.AreEqual("TEST____", info.Text);
   }

   [TestMethod]
   public void TestMovesToNextFieldAfterLastPositionInput() {
      Console.CursorLeft = 30;
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKeys("X");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(26, 4, 6);
      Assert.AreEqual("____X ", info.Text);
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
      Assert.AreEqual(ConsoleKey.Tab, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.DownArrow, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Shift, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.Insert, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Alt, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.Delete, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.Backspace, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Shift, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.Home, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Alt, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.End, Map.KeyWasPressed.Key);
      Assert.AreEqual(ConsoleModifiers.Control, Map.KeyWasPressed.Modifiers);
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
      Assert.AreEqual(ConsoleKey.F3, Map.KeyWasPressed.Key);
      Assert.AreEqual((ConsoleModifiers) 0, Map.KeyWasPressed.Modifiers);
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
      Assert.IsInstanceOfType(Map.Fields[3], typeof(CheckField));
      CheckField checkField = (CheckField) Map.Fields[3];
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Console.SendKeys("X");
      DoLoop();
      Assert.AreEqual("X", checkField.GetInnerValue());
      Assert.IsTrue(Map.Get<bool>("Summer"));
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
      Assert.IsInstanceOfType(Map.Fields[3], typeof(CheckField));
      CheckField checkField = (CheckField) Map.Fields[3];
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Console.SendKeys("/");
      DoLoop();
      Assert.AreEqual("/", checkField.GetInnerValue());
      Assert.IsTrue(Map.Get<bool>("Summer"));
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
      Assert.IsInstanceOfType(Map.Fields[3], typeof(CheckField));
      CheckField checkField = (CheckField) Map.Fields[3];
      Assert.IsTrue(Map.Set("Summer", "X"));
      Assert.AreEqual(6, Console.CursorTop);
      Assert.AreEqual("X", checkField.GetInnerValue());
      Assert.IsTrue(Map.Get<bool>("Summer"));
      Console.SendKeys(" ");
      DoLoop();
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
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
      Assert.IsInstanceOfType(Map.Fields[3], typeof(CheckField));
      CheckField checkField = (CheckField) Map.Fields[3];
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Assert.IsFalse(Map.Fields[3].IsDirty);
      Console.SendKeys("A");
      DoLoop();
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Assert.IsFalse(Map.Fields[3].IsDirty);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
      Console.SendKeys("-");
      DoLoop();
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Assert.IsFalse(Map.Fields[3].IsDirty);
      Assert.AreEqual(3, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestPreservingValuesGivenInFields() {
      Assert.IsInstanceOfType(Map.Fields[1], typeof(TextField));
      TextField textField = (TextField) Map.Fields[1];
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      MockConsoleInfo info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("SOME VALUE X__", info.Text);
      Assert.AreEqual("SOME VALUE X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("SOME VALUE X", Map.Get<string>("Field"));
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      textField.SetValue("");
      Console.SendKey(ConsoleKey.Home);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("SOME VALUE X__", info.Text);
      Assert.AreEqual("Some value X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("Some value X", Map.Get<string>("Field"));
   }

   [TestMethod]
   public void TestPreservingValuesAndDisplayGivenInFields() {
      Assert.IsInstanceOfType(Map.Fields[1], typeof(TextField));
      TextField textField = (TextField) Map.Fields[1];
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      MockConsoleInfo info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("SOME VALUE X__", info.Text);
      Assert.AreEqual("SOME VALUE X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("SOME VALUE X", Map.Get<string>("Field"));
      App.Stop();
      App.PreserveGivenFieldValuesAndDisplay();
      App.Start();
      textField.SetValue("");
      Console.SendKey(ConsoleKey.Home);
      Console.SendKeys("Some value X");
      Console.SendKey(ConsoleKey.Tab);
      while (Console.KeyAvailable) {
         DoLoop();
      }
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.Text);
      Assert.AreEqual("Some value X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("Some value X", Map.Get<string>("Field"));
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowLeftInField() {
      Application.SetCursorPosition(15, 4);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKey(ConsoleKey.LeftArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(14, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowRightInField() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.RightArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(12, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowLeftOnFirstLineField() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.LeftArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowRightOnLastLineField() {
      Application.SetCursorPosition(31, 4);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Console.SendKey(ConsoleKey.RightArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(31, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowUpOnTopScreenField() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop();
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(4, Console.CursorTop);
   }

   [TestMethod]
   public void TestKeepsFocusOnArrowDownOnBottomScreenField() {
      Map.MoveFocus(7);
      Assert.AreEqual(7, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop();
      Assert.AreEqual(7, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldLeftWithArrowLeft() {
      Map.MoveFocus(8);
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      Console.SendKey(ConsoleKey.LeftArrow);
      DoLoop();
      Assert.AreEqual(7, Map.CurrentFieldIndex);
      Assert.AreEqual(12, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldRightWithArrowRight() {
      Map.MoveFocus(7);
      Assert.AreEqual(7, Map.CurrentFieldIndex);
      Assert.AreEqual(11, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      Console.SendKey(ConsoleKey.RightArrow);
      Console.SendKey(ConsoleKey.RightArrow);
      DoLoop(2);
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldUpLeftWithArrowUp() {
      Map.MoveFocus(8);
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop();
      Assert.AreEqual(6, Map.CurrentFieldIndex);
      Assert.AreEqual(20, Console.CursorLeft);
      Assert.AreEqual(7, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldUpRightWithArrowUp() {
      Map.MoveFocus(6);
      Assert.AreEqual(6, Map.CurrentFieldIndex);
      Assert.AreEqual(19, Console.CursorLeft);
      Assert.AreEqual(7, Console.CursorTop);
      Console.SendKey(ConsoleKey.UpArrow);
      DoLoop();
      Assert.AreEqual(4, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(6, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldBottomLeftWithArrowDown() {
      Map.MoveFocus(4);
      Assert.AreEqual(4, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(6, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop();
      Assert.AreEqual(6, Map.CurrentFieldIndex);
      Assert.AreEqual(20, Console.CursorLeft);
      Assert.AreEqual(7, Console.CursorTop);
   }

   [TestMethod]
   public void TestMoveFocusToFieldBottomRightWithArrowDown() {
      Map.MoveFocus(6);
      Assert.AreEqual(6, Map.CurrentFieldIndex);
      Assert.AreEqual(19, Console.CursorLeft);
      Assert.AreEqual(7, Console.CursorTop);
      Console.SendKey(ConsoleKey.DownArrow);
      DoLoop();
      Assert.AreEqual(8, Map.CurrentFieldIndex);
      Assert.AreEqual(21, Console.CursorLeft);
      Assert.AreEqual(8, Console.CursorTop);
   }
}
