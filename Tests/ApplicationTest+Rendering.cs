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
   public void TestMapRender() {
      Map.Fields[0].SetValue("Some very long text value");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 1, 16);
      info.AssertIs("TEST SCREEN 001 ",
                    "7777777777778887",
                    "0000000000006660");

      info = Console.ReadScreen(2, 3, 30);
      info.AssertIs("LABEL:   SOME VERY LONG TEXT  ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000000000000000000000000");

      info = Console.ReadScreen(2, 4, 30);
      info.AssertIs("FIELD:   ____________________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000000000000000000000000");

      info = Console.ReadScreen(2, 12, 8);
      info.AssertIs("PF1 Help",
                    "FFF77777",
                    "00000000");
   }

   [TestMethod]
   public void TestMapRenderMessages() {
      Map.SetMessage("Test");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 10, 14);
      info.AssertIs("MESSAGE: TEST ",
                    "777777777FFFFF",
                    "00000000000000");

      Map.SetAlert("Test");
      DoLoop();
      info = Console.ReadScreen(2, 10, 14);
      info.AssertIs("MESSAGE: TEST ",
                    "77777777766666",
                    "00000000000000");

      Map.SetError("Test");
      DoLoop();
      info = Console.ReadScreen(2, 10, 14);
      info.AssertIs("MESSAGE: TEST ",
                    "77777777744444",
                    "00000000000000");

      Map.SetSuccess("Test");
      DoLoop();
      info = Console.ReadScreen(2, 10, 14);
      info.AssertIs("MESSAGE: TEST ",
                    "77777777722222",
                    "00000000000000");
   }

   [TestMethod]
   public void TestDirtyFieldMustOverwritePrevious() {
      Map.SetError("Error");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 10, 6);
      info.AssertIs("ERROR ",
                    "444444",
                    "000000");

      Map.SetSuccess("OK");
      DoLoop();
      info = Console.ReadScreen(11, 10, 6);
      info.AssertIs("OK    ",
                    "222222",
                    "000000");
   }

   [TestMethod]
   public void TestSettingStatusMessageDirectly() {
      Map.SetError("Error");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 10, 6);
      info.AssertIs("ERROR ",
                    "444444",
                    "000000");

      Assert.IsNotNull(Map.StatusField);
      Assert.AreSame(Map.StatusField, Map.Fields[9]);
      Map.StatusField.SetValue("OK");
      DoLoop();
      info = Console.ReadScreen(11, 10, 6);
      info.AssertIs("OK    ",
                    "FFFFFF",
                    "000000");
   }

   [TestMethod]
   public void TestPasswordField() {
      MockConsoleInfo info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  ____________________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000000000000000000000000");

      Map.Fields[2].SetValue("PASSWORD");
      DoLoop();
      info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  ********____________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7");
   }

   // Options ---------------------------------------------------------------------------

   [TestMethod]
   public void TestCommandColorInForeground() {
      App.Stop();
      App.UseHighlightColorInForeground(ConsoleColor.Yellow);
      App.Start();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(2, 12, 8);
      info.AssertIs("PF1 Help",
                    "EEE77777",
                    "00000000");
   }

   [TestMethod]
   public void TestCommandColorInBackground() {
      App.Stop();
      App.UseCommandColorInBackground(ConsoleColor.Yellow);
      App.Start();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(2, 12, 8);
      info.AssertIs("PF1 Help",
                    "00077777",
                    "EEE00000");
   }

   [TestMethod]
   public void TestActiveFieldColors() {
      App.Stop();
      App.UseActiveFieldBackground();
      App.Start();
      DoLoop();

      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(2, 4, 30);
      info.AssertIs("FIELD:   ____________________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000888888888888888888880");
   }

   [TestMethod]
   public void TestShowMapInWindow() {
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 6, 34);
      info.AssertIs("|  TEST SCREEN 001               |",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      Assert.AreEqual(1, wnd.CurrentFieldIndex);
      Assert.AreEqual(35, Console.CursorLeft);
      Assert.AreEqual(9, Console.CursorTop);
   }

   [TestMethod]
   public void TestCloseMapInWindow() {
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      wnd.Close();
      DoLoop();

      info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("________                          ",
                    "FFFFFFFF77777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("                                  ",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestShowMapInWindowAtPosition() {
      App.Stop();
      App.SetDefaultWindowPosition(3, 3);
      App.Start();
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(3, 3, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(3, 5, 34);
      info.AssertIs("|  TEST SCREEN 001               |",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(3, 17, 34);
      info.AssertIs("+--------------------------------+",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestShowMapInWindowWithSquareBorder() {
      App.Stop();
      App.UseWindowBorder(WindowBorder.Square);
      App.Start();
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("┌────────────────────────────────┐",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 6, 34);
      info.AssertIs("│  TEST SCREEN 001               │",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("└────────────────────────────────┘",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestShowMapInWindowWithRoundedBorder() {
      App.Stop();
      App.UseWindowBorder(WindowBorder.Rounded);
      App.Start();
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("╭────────────────────────────────╮",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 6, 34);
      info.AssertIs("│  TEST SCREEN 001               │",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("╰────────────────────────────────╯",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestShowMapInWindowWithHeavyBorder() {
      App.Stop();
      App.UseWindowBorder(WindowBorder.Heavy);
      App.Start();
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 6, 34);
      info.AssertIs("┃  TEST SCREEN 001               ┃",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestShowMapInWindowDoubleBorder() {
      App.Stop();
      App.UseWindowBorder(WindowBorder.Double);
      App.Start();
      TestMap wnd = new();
      wnd.ShowPopup();
      DoLoop();

      MockConsoleInfo info = Console.ReadScreen(23, 4, 34);
      info.AssertIs("╔════════════════════════════════╗",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");

      info = Console.ReadScreen(23, 6, 34);
      info.AssertIs("║  TEST SCREEN 001               ║",
                    "7777777777777778887777777777777777",
                    "0000000000000006660000000000000000");

      info = Console.ReadScreen(23, 18, 34);
      info.AssertIs("╚════════════════════════════════╝",
                    "7777777777777777777777777777777777",
                    "0000000000000000000000000000000000");
   }

   [TestMethod]
   public void TestPasswordFieldWithBullets() {
      App.Stop();
      App.UseBulletsInPasswordFields();
      App.Start();
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  ____________________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000000000000000000000000");

      Map.Fields[2].SetValue("PASSWORD");
      DoLoop();
      info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  \u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF____________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7");
   }

   [TestMethod]
   public void TestPresevingValuesGivenInLabels() {
      Assert.IsNotNull(Map.StatusField);
      Assert.AreSame(Map.StatusField, Map.Fields[9]);
      Map.SetError("Some error X");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 10, 14);
      Assert.AreEqual("SOME ERROR X  ", info.Text);
      Assert.AreEqual("Some error X", Map.StatusField.GetValue());
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      Map.SetError("Some error X");
      DoLoop();
      info = Console.ReadScreen(11, 10, 14);
      Assert.AreEqual("Some error X  ", info.Text);
      Assert.AreEqual("Some error X", Map.StatusField.GetValue());
   }

   [TestMethod]
   public void TestPreservingValuesGivenInFields2() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.IsInstanceOfType(Map.Fields[1], typeof(TextField));
      TextField textField = (TextField) Map.Fields[1];
      textField.SetValue("Some value X");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.Text);
      Assert.AreEqual("Some value X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("Some value X", Map.Get<string>("Field"));
      Console.SendKey(ConsoleKey.Tab);
      DoLoop();
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      Assert.AreEqual("SOME VALUE X__", info.Text);
      Assert.AreEqual("SOME VALUE X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("SOME VALUE X", Map.Get<string>("Field"));
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      Map.Fields[1].SetValue("Some value X");
      DoLoop();
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.Text);
      Assert.AreEqual("Some value X\0\0\0\0\0\0\0\0", textField.GetInnerValue());
      Assert.AreEqual("Some value X", Map.Get<string>("Field"));
   }

   [TestMethod]
   public void TestCheckFieldChanges() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 6, 6);
      info.AssertIs("_ SUMM",
                    "F77777");
      Assert.IsInstanceOfType(Map.Fields[3], typeof(CheckField));
      CheckField checkField = (CheckField) Map.Fields[3];
      Assert.AreEqual(0, checkField.Group);
      Assert.AreEqual(11, checkField.Left);
      Assert.AreEqual(6, checkField.Top);
      Assert.AreEqual("\0", checkField.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("Summer"));
      Assert.IsTrue(Map.Fields[3].SetValue("x"));
      DoLoop();
      info = Console.ReadScreen(11, 6, 6);
      info.AssertIs("X SUMM",
                    "F77777");
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.IsTrue(Map.Get<bool>("Summer"));
   }

   [TestMethod]
   public void TestChoiceFieldChanges() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("_ YES ",
                    "F77777");
      Assert.IsInstanceOfType(Map.Fields[5], typeof(CheckField));
      Assert.IsInstanceOfType(Map.Fields[6], typeof(CheckField));
      Assert.IsInstanceOfType(Map.Fields[7], typeof(CheckField));
      Assert.IsInstanceOfType(Map.Fields[8], typeof(CheckField));
      CheckField checkField5 = (CheckField) Map.Fields[5];
      CheckField checkField6 = (CheckField) Map.Fields[6];
      CheckField checkField7 = (CheckField) Map.Fields[7];
      CheckField checkField8 = (CheckField) Map.Fields[8];
      Assert.AreEqual(1, checkField5.Group);
      Assert.AreEqual(1, checkField6.Group);
      Assert.AreEqual(1, checkField7.Group);
      Assert.AreEqual(2, checkField8.Group);
      Assert.AreEqual(11, checkField5.Left);
      Assert.AreEqual(7, checkField5.Top);
      Assert.AreEqual("\0", checkField5.GetInnerValue());
      Assert.IsFalse(Map.Get<bool>("OptYes"));
      Assert.IsTrue(checkField5.SetValue("X"));
      DoLoop();
      info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("X YES ",
                    "F77777");
      info = Console.ReadScreen(19, 7, 6);
      info.AssertIs("_ NO  ",
                    "F77777");
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.AreEqual("X", checkField5.GetInnerValue());
      Assert.IsTrue(Map.Get<bool>("OptYes"));
      Assert.IsFalse(Map.Get<bool>("OptNo"));
      Assert.IsFalse(Map.Get<bool>("OptMaybe"));
      Assert.IsFalse(Map.Get<bool>("OptNever"));
      checkField6.SetValue("/");
      DoLoop();
      info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("_ YES ",
                    "F77777");
      info = Console.ReadScreen(19, 7, 6);
      info.AssertIs("/ NO  ",
                    "F77777");
      Assert.IsFalse(Map.Get<bool>("OptYes"));
      Assert.IsTrue(Map.Get<bool>("OptNo"));
      Assert.IsFalse(Map.Get<bool>("OptMaybe"));
      Assert.IsFalse(Map.Get<bool>("OptNever"));
      checkField8.SetValue("X");
      DoLoop();
      Assert.IsFalse(Map.Get<bool>("OptYes"));
      Assert.IsTrue(Map.Get<bool>("OptNo"));
      Assert.IsFalse(Map.Get<bool>("OptMaybe"));
      Assert.IsTrue(Map.Get<bool>("OptNever"));
   }

   [TestMethod]
   public void TestHiddingLabel() {
      Assert.IsInstanceOfType(Map.Fields[0], typeof(Label));
      Assert.IsTrue(Map.Fields[0].IsVisible);
      Map.Fields[0].SetValue("Some text");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 3, 20);
      Assert.AreEqual("LABEL:   SOME TEXT  ", info.Text);
      Assert.IsTrue(Map.SetFieldVisible("Label", false));
      DoLoop();
      info = Console.ReadScreen(2, 3, 20);
      Assert.AreEqual("LABEL:              ", info.Text);
      Assert.IsTrue(Map.SetFieldVisible("Label", true));
      DoLoop();
      info = Console.ReadScreen(2, 3, 20);
      Assert.AreEqual("LABEL:   SOME TEXT  ", info.Text);
   }

   [TestMethod]
   public void TestHiddingTextField() {
      Assert.IsInstanceOfType(Map.Fields[1], typeof(TextField));
      Assert.IsTrue(Map.Fields[1].IsVisible);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Map.Fields[1].SetValue("Some text");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 4, 20);
      Assert.AreEqual("FIELD:   Some text__", info.Text);
      Assert.IsTrue(Map.SetFieldVisible("Field", false));
      DoLoop();
      info = Console.ReadScreen(2, 4, 20);
      Assert.AreEqual("FIELD:              ", info.Text);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      Assert.IsTrue(Map.SetFieldVisible("Field", true));
      DoLoop();
      info = Console.ReadScreen(2, 4, 20);
      Assert.AreEqual("FIELD:   SOME TEXT__", info.Text);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestHiddingStatusMessage() {
      Assert.IsNotNull(Map.StatusField);
      Assert.AreSame(Map.StatusField, Map.Fields[9]);
      Assert.IsTrue(Map.Fields[9].IsVisible);
      Map.Fields[9].SetValue("Some message");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 10, 20);
      Assert.AreEqual("MESSAGE: SOME MESSAG", info.Text);
      Assert.IsFalse(Map.SetFieldVisible("Status", false));
      DoLoop();
      Assert.IsTrue(Map.Fields[9].IsVisible);
      info = Console.ReadScreen(2, 10, 20);
      Assert.AreEqual("MESSAGE: SOME MESSAG", info.Text);
   }

   [TestMethod]
   public void TestVeryLargeMapIsCroppedOn80x24() {
      Assert.AreEqual(80, Console.WindowWidth);
      Assert.AreEqual(24, Console.WindowHeight);
      Assert.ThrowsException<InvalidOperationException>(() => new LargeTestMap().Show());
   }

   [TestMethod]
   public void TestVeryLargeMapIsCroppedOn120x30() {
      App.Stop();
      Console.SetWindowSize(120, 30);
      App.Start();
      Assert.ThrowsException<InvalidOperationException>(() => new LargeTestMap().Show());
   }

   [TestMethod]
   public void TestVeryLargeMapIsNotCropped() {
      App.Stop();
      App.SetWindowSize(120, 30);
      App.Start();
      new LargeTestMap().Show();
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 1, 10);
      Assert.AreEqual("SCREEN 002", info.Text);
      info = Console.ReadScreen(80, 6, 33);
      Assert.AreEqual("^ cropped at width 80 beyond this", info.Text);
      info = Console.ReadScreen(2, 29, 20);
      Assert.AreEqual("This line is visible", info.Text);
   }

   [TestMethod]
   public void TestExistingFieldGetsFocusOnStatus() {
      Map.SetAlert("Password", "Some alert");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(10, 4, 6);
      info.AssertIs(" _____", "7FFFFF", "000000");
      info = Console.ReadScreen(10, 5, 6);
      info.AssertIs(" _____", "766666", "000000");
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      info = Console.ReadScreen(2, 10, 20);
      info.AssertIs("MESSAGE: SOME ALERT ", "77777777766666666666", "00000000000000000000");

      Map.SetError("Field", "Some error");
      DoLoop();
      info = Console.ReadScreen(10, 4, 6);
      info.AssertIs(" _____", "744444", "000000");
       info = Console.ReadScreen(10, 5, 6);
      info.AssertIs(" _____", "7FFFFF", "000000");
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(2, 10, 20);
      info.AssertIs("MESSAGE: SOME ERROR ", "77777777744444444444", "00000000000000000000");
   }

   [TestMethod]
   public void TestStatusIgnoredForUndefinedField() {
      Map.SetAlert("Undefined", "Some alert");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(10, 5, 6);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      info = Console.ReadScreen(2, 10, 20);
      info.AssertIs("MESSAGE:            ", "777777777FFFFFFFFFFF", "00000000000000000000");
   }

   public class LargeTestMap : Map { }
}
