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
public class ApplicationRenderTest : ApplicationTest {

   [TestMethod]
   public void TestMapRender() {
      ((Label) Map.Fields[0]).Value = "Some very long text value";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 1, 16);
      info.AssertIs("TEST SCREEN 001 ",
                    "7777777777778887",
                    "0000000000006660");

      info = Console.ReadScreen(2, 3, 30);
      info.AssertIs("LABEL:   SOME VERY LONG TEXT  ",
                    "777777777EEEEEEEEEEEEEEEEEEEE7",
                    "000000000888888888888888888880");

      info = Console.ReadScreen(2, 4, 30);
      info.AssertIs("FIELD:   ____________________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7",
                    "000000000222222222222222222220");

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

      Assert.IsTrue(Map.Fields[9] is StatusMessage);
      StatusMessage field = (StatusMessage) Map.Fields[9];
      field.Value = "OK";
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

      ((TextField) Map.Fields[2]).Value = "PASSWORD";
      DoLoop();
      info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  ********____________ ",
                    "777777777FFFFFFFFFFFFFFFFFFFF7");
   }

   // Options ---------------------------------------------------------------------------

   [TestMethod]
   public void TestCommandColorInForeground() {
      App.Stop();
      App.UseCommandColorInForeground(ConsoleColor.Yellow);
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
   public void TestActiveFieldChosenColors() {
      App.Stop();
      App.UseActiveFieldColors(ConsoleColor.Black, ConsoleColor.Gray);
      App.Start();
      DoLoop();

      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(2, 4, 30);
      info.AssertIs("FIELD:   ____________________ ",
                    "777777777000000000000000000007",
                    "000000000777777777777777777770");
   }

   [TestMethod]
   public void TestShowMapInWindow() {
      TestMap wnd = new();
      wnd.ShowWindow();
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
      wnd.ShowWindow();
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
                    "2222222200000000000000000000000000");

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
      wnd.ShowWindow();
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
      wnd.ShowWindow();
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
      wnd.ShowWindow();
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
      wnd.ShowWindow();
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
      wnd.ShowWindow();
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

      ((TextField) Map.Fields[2]).Value = "PASSWORD";
      DoLoop();
      info = Console.ReadScreen(2, 5, 30);
      info.AssertIs("PASSWD:  \u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF____________ ", 
                    "777777777FFFFFFFFFFFFFFFFFFFF7");
   }

   [TestMethod]
   public void TestPresevingValuesGivenInLabels() {
      Assert.IsTrue(Map.Fields[9] is StatusMessage);
      StatusMessage field = (StatusMessage) Map.Fields[9];
      Map.SetError("Some error X");
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 10, 14);
      Assert.AreEqual("SOME ERROR X  ", info.ScreenText);
      Assert.AreEqual("Some error X", field.Value);
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      Map.SetError("Some error X");
      DoLoop();
      info = Console.ReadScreen(11, 10, 14);
      Assert.AreEqual("Some error X  ", info.ScreenText);
      Assert.AreEqual("Some error X", field.Value);
   }

   [TestMethod]
   public void TestPreservingValuesGivenInFields() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.IsTrue(Map.Fields[1] is TextField);
      TextField field = (TextField) Map.Fields[1];
      field.Value = "Some value X";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.ScreenText);
      Assert.AreEqual("Some value X", field.Value);
      Console.SendKey(ConsoleKey.Tab);
      DoLoop();
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual(2, Map.CurrentFieldIndex);
      Assert.AreEqual("SOME VALUE X__", info.ScreenText);
      Assert.AreEqual("SOME VALUE X", field.Value);
      App.Stop();
      App.PreserveGivenFieldValues();
      App.Start();
      field.Value = "Some value X";
      DoLoop();
      info = Console.ReadScreen(11, 4, 14);
      Assert.AreEqual("Some value X__", info.ScreenText);
      Assert.AreEqual("Some value X", field.Value);
   }

   [TestMethod]
   public void TestCheckFieldChanges() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 6, 6);
      info.AssertIs("_ SUMM", 
                    "F77777");
      Assert.IsTrue(Map.Fields[3] is CheckField);
      CheckField field = (CheckField) Map.Fields[3];
      Assert.AreEqual(11, field.Left);
      Assert.AreEqual(6, field.Top);
      Assert.AreEqual("", field.Value);
      Assert.IsFalse(field.IsChecked);
      field.Value = "x";
      DoLoop();
      info = Console.ReadScreen(11, 6, 6);
      info.AssertIs("X SUMM",
                    "F77777");
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.IsTrue(field.IsChecked);
   }

   [TestMethod]
   public void TestChoiceFieldChanges() {
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      MockConsoleInfo info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("_ YES ",
                    "F77777");
      Assert.IsTrue(Map.Fields[5] is ChoiceField);
      Assert.IsTrue(Map.Fields[6] is ChoiceField);
      Assert.IsTrue(Map.Fields[7] is ChoiceField);
      Assert.IsTrue(Map.Fields[8] is ChoiceField);
      ChoiceField field1 = (ChoiceField) Map.Fields[5];
      ChoiceField field2 = (ChoiceField) Map.Fields[6];
      ChoiceField field3 = (ChoiceField) Map.Fields[7];
      ChoiceField field4 = (ChoiceField) Map.Fields[8];
      Assert.AreEqual(11, field1.Left);
      Assert.AreEqual(7, field1.Top);
      Assert.AreEqual("", field1.Value);
      Assert.IsFalse(field1.IsChecked);
      field1.Value = "X";
      DoLoop();
      info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("X YES ",
                    "F77777");
      info = Console.ReadScreen(19, 7, 6);
      info.AssertIs("_ NO  ",
                    "F77777");
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      Assert.IsTrue(field1.IsChecked);
      Assert.IsFalse(field2.IsChecked);
      Assert.IsFalse(field3.IsChecked);
      Assert.IsFalse(field4.IsChecked);
      field2.Value = "/";
      DoLoop();
      info = Console.ReadScreen(11, 7, 6);
      info.AssertIs("_ YES ",
                    "F77777");
      info = Console.ReadScreen(19, 7, 6);
      info.AssertIs("/ NO  ",
                    "F77777");
      Assert.IsFalse(field1.IsChecked);
      Assert.IsTrue(field2.IsChecked);
      Assert.IsFalse(field3.IsChecked);
      Assert.IsFalse(field4.IsChecked);
      field4.Value = "X";
      DoLoop();
      Assert.IsFalse(field1.IsChecked);
      Assert.IsTrue(field2.IsChecked);
      Assert.IsFalse(field3.IsChecked);
      Assert.IsTrue(field4.IsChecked);
   }

   [TestMethod]
   public void TestHiddingLabel() {
      Assert.IsTrue(Map.Fields[0] is Label);
      Label field = (Label) Map.Fields[0];
      Assert.IsTrue(field.IsVisible);
      field.Value = "Some text";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 3, 20);
      Assert.AreEqual("LABEL:   SOME TEXT  ", info.ScreenText);
      field.IsVisible = false;
      DoLoop();
      info = Console.ReadScreen(2, 3, 20);
      Assert.AreEqual("LABEL:              ", info.ScreenText);
   }

   [TestMethod]
   public void TestHiddingTextField() {
      Assert.IsTrue(Map.Fields[1] is TextField);
      TextField field = (TextField) Map.Fields[1];
      Assert.IsTrue(field.IsVisible);
      Assert.AreEqual(1, Map.CurrentFieldIndex);
      field.Value = "Some text";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 4, 20);
      Assert.AreEqual("FIELD:   Some text__", info.ScreenText);
      field.IsVisible = false;
      DoLoop();
      info = Console.ReadScreen(2, 4, 20);
      Assert.AreEqual("FIELD:              ", info.ScreenText);
      Assert.AreEqual(-1, Map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestHiddingStatusMessage() {
      Assert.IsTrue(Map.Fields[9] is StatusMessage);
      StatusMessage field = (StatusMessage) Map.Fields[9];
      Assert.IsTrue(field.IsVisible);
      field.Value = "Some message";
      DoLoop();
      MockConsoleInfo info = Console.ReadScreen(2, 10, 20);
      Assert.AreEqual("MESSAGE: SOME MESSAG", info.ScreenText);
      field.IsVisible = false;
      DoLoop();
      Assert.IsTrue(field.IsVisible);
      info = Console.ReadScreen(2, 10, 20);
      Assert.AreEqual("MESSAGE: SOME MESSAG", info.ScreenText);
   }
}
