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
public sealed class MapRenderTest {

   [TestMethod]
   public void TestMapWithNullContent() {
      Assert.ThrowsException<ArgumentNullException>(() => {
         Map map = new TestMap1(null);
      });
   }

   [TestMethod]
   public void TestMapWithoutContent() {
      Assert.ThrowsException<ArgumentException>(() => {
         Map map = new TestMap1();
      }, "Map has no contents");
   }

   [TestMethod]
   public void TestMapWithNoValidContent() {
      Assert.ThrowsException<ArgumentException>(() => {
         Map map = new TestMap1("", "");
      }, "Map has no contents");
   }

   [TestMethod]
   public void TestContentWithoutFields() {
      Map map = new TestMap1("> SCREEN 001");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(1, parts.Length);
      Assert.AreEqual(" SCREEN 001", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestFormatLineWithoutPrint() {
      Map map = new TestMap1(":11111111111", "> SCREEN 001");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(1, parts.Length);
      Assert.AreEqual(" SCREEN 001", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithInvalidLines() {
      Map map = new TestMap1("> SCREEN 001", "+this line is discarded", "> LINE 02");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(2, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(2, parts.Length);
      Assert.AreEqual(" SCREEN 001", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual(" LINE 02   ", parts[1].Text);
      Assert.AreEqual(MapPartColor.Default, parts[1].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[1].BackgroundColor);
      Assert.IsTrue(parts[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithFormattedForeground() {
      Map map = new TestMap1("> SCREEN 001", ":        AAA");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(2, parts.Length);
      Assert.AreEqual(" SCREEN ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsFalse(parts[0].LineBreak);

      Assert.AreEqual("001", parts[1].Text);
      Assert.AreEqual(MapPartColor.Green, parts[1].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[1].BackgroundColor);
      Assert.IsTrue(parts[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithFormattedBackground() {
      Map map = new TestMap1("> SCREEN 001", ":        EEE", ":11111111111");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(2, parts.Length);
      Assert.AreEqual(" SCREEN ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.DarkBlue, parts[0].BackgroundColor);
      Assert.IsFalse(parts[0].LineBreak);

      Assert.AreEqual("001", parts[1].Text);
      Assert.AreEqual(MapPartColor.Yellow, parts[1].ForegroundColor);
      Assert.AreEqual(MapPartColor.DarkBlue, parts[1].BackgroundColor);
      Assert.IsTrue(parts[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestSingleFieldLocation() {
      Map map = new TestMap1(">  ¬XXXXXXX ", "¬Field:INP[8]");
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(1, parts.Length);
      Assert.AreEqual("           ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);
   }

   [TestMethod]
   public void TestDoubleFieldLocation() {
      Map map = new TestMap1(">  ¬XXXXXXX : ¬XXXXXXX ", "¬Label:ROT[8]", "¬Field:INP[8]");
      Assert.AreEqual(22, map.Width);
      Assert.AreEqual(1, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(1, parts.Length);
      Assert.AreEqual("           :          ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual(2, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsReadOnly);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);

      Assert.IsFalse(map.Fields[1].IsReadOnly);
      Assert.AreEqual(13, map.Fields[1].Left);
      Assert.AreEqual(0, map.Fields[1].Top);
      Assert.AreEqual(8, map.Fields[1].Width);
   }

   [TestMethod]
   public void TestDoubleFieldLocationMultipleLines() {
      Map map = new TestMap1(">  ¬XXXXXXX ", ">       TEST: ¬XXXXXXX ", "¬Label:ROT[8]", "¬Field:INP[8]");
      Assert.AreEqual(22, map.Width);
      Assert.AreEqual(2, map.Height);
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(2, parts.Length);
      Assert.AreEqual("                      ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[0].BackgroundColor);
      Assert.IsTrue(parts[0].LineBreak);

      Assert.AreEqual("       TEST:          ", parts[1].Text);
      Assert.AreEqual(MapPartColor.Default, parts[1].ForegroundColor);
      Assert.AreEqual(MapPartColor.Default, parts[1].BackgroundColor);
      Assert.IsTrue(parts[1].LineBreak);

      Assert.AreEqual(2, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsReadOnly);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);

      Assert.IsFalse(map.Fields[1].IsReadOnly);
      Assert.AreEqual(13, map.Fields[1].Left);
      Assert.AreEqual(1, map.Fields[1].Top);
      Assert.AreEqual(8, map.Fields[1].Width);
   }

   [TestMethod]
   public void TestReadOnlyFieldValue() {
      Map map = new TestMap1(">  ¬XXXXXXX ", "¬Label:ROT[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsReadOnly);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.Fields[0].SetValue("01/01");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("01/01", map["Label"]);
   }

   [TestMethod]
   public void TestEditableFieldValue() {
      Map map = new TestMap1(">  ¬XXXXXXX ", "¬Field:INP[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.Fields[0].SetValue("01/01");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("01/01", map["Field"]);
      Assert.AreEqual("01/01\0\0\0", map.Fields[0].Value);
   }

   [TestMethod]
   public void TestFieldIgnoresSameValue() {
      Map map = new TestMap1(">  ¬XXXXXXX ", "¬Field:INP[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.Fields[0].SetValue("01/01");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("01/01", map["Field"]);
      Assert.AreEqual("01/01\0\0\0", map.Fields[0].Value);
      map.Fields[0].IsDirty = false;
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.Fields[0].SetValue("01/01");
      Assert.IsFalse(map.Fields[0].IsDirty);
   }

   [TestMethod]
   public void TestMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.SetMessage("Test message");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test mes", map.Fields[0].Value);
      Assert.AreEqual(StatusFieldSeverity.Info, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestErrorMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.SetError("Test error");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test err", map.Fields[0].Value);
      Assert.AreEqual(StatusFieldSeverity.Error, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestAlertMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.SetAlert("Test alert");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test ale", map.Fields[0].Value);
      Assert.AreEqual(StatusFieldSeverity.Alert, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestSuccessMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.SetSuccess("Test success");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test suc", map.Fields[0].Value);
      Assert.AreEqual(StatusFieldSeverity.Success, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestDirectSetMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.Fields[0].SetValue("Test message");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test mes", map["Status"]);
      Assert.AreEqual(StatusFieldSeverity.None, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestClearMessageField() {
      Map map = new TestMap1("> ¬XXXXXXX ", "¬Status:STA[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsStatus);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.IsNotNull(map.StatusField);
      Assert.AreSame(map.Fields[0], map.StatusField);
      map.SetError("Test error");
      map.Fields[0].IsDirty = false;
      Assert.AreEqual("Test err", map["Status"]);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.AreEqual(StatusFieldSeverity.Error, map.StatusField!.Severity);
      map.ClearMessage();
      Assert.AreEqual("", map["Status"]);
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual(StatusFieldSeverity.None, map.StatusField!.Severity);
   }

   [TestMethod]
   public void TestDuplicateMessageField() {
      InvalidFieldException ex = Assert.ThrowsException<InvalidFieldException>(() => {
         Map map = new TestMap1(">  ¬XXXXXXX   ¬XXX ", "¬Status1:STA[8]", "¬Status2:STA[4]");
      });
      Assert.AreEqual(13, ex.FieldLeft);
      Assert.AreEqual(0, ex.FieldTop);
   }

   [TestMethod]
   public void TestValueForFieldWithFreeMask1() {
      Map map = new TestMap1(">  ¬XXXXXXXX ", "¬Field:INP(XXXXXXXXX)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.AreEqual(9, map.Fields[0].Width);
      Assert.AreEqual("", map["Field"]);
      map.Fields[0].SetValue("A-B+C/D E");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("A-B+C/D E", map["Field"]);
      map.Fields[0].SetValue(null);
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestValueForFieldWithFreeMask2() {
      Map map = new TestMap1(">  ¬XXXXXXXX ", "¬Field:INP(*********)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.AreEqual(9, map.Fields[0].Width);
      Assert.AreEqual("", map["Field"]);
      map.Fields[0].SetValue("A-B+C/D E");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("A-B+C/D E", map["Field"]);
      map.Fields[0].SetValue(null);
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestValueForFieldWithFreeMask3() {
      Map map = new TestMap1(">  ¬XXXXXXXX ", "¬Field:INP(         )");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.AreEqual(9, map.Fields[0].Width);
      Assert.AreEqual("", map["Field"]);
      map.Fields[0].SetValue("A-B+C/D E");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("A-B+C/D E", map["Field"]);
      map.Fields[0].SetValue(null);
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestNullValueForField() {
      Map map = new TestMap1(">  ¬AAAAAAA ", "¬Field:INP(AAAAAAAA)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.AreEqual("", map["Field"]);
      map.Fields[0].SetValue("ABCD");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("ABCD", map["Field"]);
      map.Fields[0].SetValue(null);
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestValidValueForTextField() {
      Map map = new TestMap1(">  ¬AAAAAAA ", "¬Field:INP(AAAAAAAA)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      map.Fields[0].SetValue("ABCD");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("ABCD", map["Field"]);
   }

   [TestMethod]
   public void TestInvalidValueForTextField() {
      Map map = new TestMap1(">  ¬AAAAAAA ", "¬Field:INP(AAAAAAAA)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.IsFalse(map.Fields[0].SetValue("1234"));
      map["Field"] = "1234";
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestValidValueForNumberField() {
      Map map = new TestMap1(">  ¬9999999 ", "¬Field:INP(99999999)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      map.Fields[0].SetValue("1234");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("1234", map["Field"]);
   }

   [TestMethod]
   public void TestInvalidValueForNumberField() {
      Map map = new TestMap1(">  ¬9999999 ", "¬Field:INP(99999999)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.IsFalse(map.Fields[0].SetValue("ABCD"));
      map["Field"] = "ABCD";
      Assert.AreEqual("", map["Field"]);
   }

   [TestMethod]
   public void TestInvalidMaskForField() {
      Map map = new TestMap1(">  ¬------- ", "¬Field:INP(--------)");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0].IsEditable);
      Assert.ThrowsException<InvalidDataException>(() => map.Fields[0].SetValue("ABCD"));
   }


   [DataTestMethod]
   [DataRow(ConsoleKey.F3, false, false)]
   [DataRow(ConsoleKey.F10, false, true)]
   [DataRow(ConsoleKey.F10, true, false)]
   public void TestMapKeyPressed(ConsoleKey key, bool shift, bool result) {
      TestMap2 map = new();
      Assert.IsFalse(map.F10Pressed);
      ConsoleCursor cursor = new(0, 0);

      map.DidKeyPress(new('\0', key, shift, false, false), cursor);
      Assert.AreEqual(result, map.F10Pressed);
   }

   [TestMethod]
   public void TestContentFromResource() {
      Map map = new TestMap2();
      MapPart[] parts = map.Parts.ToArray();
      Assert.AreEqual(4, parts.Length);

      Assert.AreEqual("  SCREEN ", parts[0].Text);
      Assert.AreEqual(MapPartColor.Default, parts[0].ForegroundColor);
      Assert.IsFalse(parts[0].LineBreak);

      Assert.AreEqual("002", parts[1].Text);
      Assert.AreEqual(MapPartColor.Yellow, parts[1].ForegroundColor);
      Assert.IsTrue(parts[1].LineBreak);

      Assert.AreEqual("            ", parts[2].Text);
      Assert.AreEqual(MapPartColor.Default, parts[2].ForegroundColor);
      Assert.IsTrue(parts[2].LineBreak);

      Assert.AreEqual(" OPTION:    ", parts[3].Text);
      Assert.AreEqual(MapPartColor.Default, parts[3].ForegroundColor);
      Assert.IsTrue(parts[3].LineBreak);

      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(9, map.Fields[0].Left);
      Assert.AreEqual(2, map.Fields[0].Top);
      Assert.AreEqual(2, map.Fields[0].Width);
      Assert.IsFalse(map.Fields[0].IsReadOnly);
   }


   [TestMethod]
   public void TestReorderNonExistingField() {
      Map map = new TestMap1("> ¬ ", "> ¬ ", "> ¬ ", "¬F1:INP(X)", "¬F2:INP(X)", "¬F3:INP(X)");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => map.Fields.Move(7, 2));
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => map.Fields.Move(-1, 2));
   }

   [TestMethod]
   public void TestReorderField() {
      Map map = new TestMap1("> ¬ ", "> ¬ ", "> ¬ ", "¬F1:INP(X)", "¬F2:INP(X)", "¬F3:INP(X)");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("F1", map.Fields[0].Name);
      Assert.AreEqual("F2", map.Fields[1].Name);
      Assert.AreEqual("F3", map.Fields[2].Name);
      map.Fields.Move(0, 1);
      Assert.AreEqual("F2", map.Fields[0].Name);
      Assert.AreEqual("F1", map.Fields[1].Name);
      Assert.AreEqual("F3", map.Fields[2].Name);
   }

   [TestMethod]
   public void TestReorderFieldToBeforeStart() {
      Map map = new TestMap1("> ¬ ", "> ¬ ", "> ¬ ", "¬F1:INP(X)", "¬F2:INP(X)", "¬F3:INP(X)");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("F1", map.Fields[0].Name);
      Assert.AreEqual("F2", map.Fields[1].Name);
      Assert.AreEqual("F3", map.Fields[2].Name);
      map.Fields.Move(1, -2);
      Assert.AreEqual("F2", map.Fields[0].Name);
      Assert.AreEqual("F1", map.Fields[1].Name);
      Assert.AreEqual("F3", map.Fields[2].Name);
   }

   [TestMethod]
   public void TestReorderFieldToBeyondEnd() {
      Map map = new TestMap1("> ¬ ", "> ¬ ", "> ¬ ", "¬F1:INP(X)", "¬F2:INP(X)", "¬F3:INP(X)");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("F1", map.Fields[0].Name);
      Assert.AreEqual("F2", map.Fields[1].Name);
      Assert.AreEqual("F3", map.Fields[2].Name);
      map.Fields.Move(1, 7);
      Assert.AreEqual("F1", map.Fields[0].Name);
      Assert.AreEqual("F3", map.Fields[1].Name);
      Assert.AreEqual("F2", map.Fields[2].Name);
   }


   private sealed class TestMap1 : Map {

      public TestMap1(params string[] contents) : base(contents) { }
   }

   private sealed class TestMap2 : Map {

      public bool F10Pressed { get; set; } = false;

      [CommandKey(ConsoleKey.F10)]
      public void KeyPressed()
         => F10Pressed = true;
   }
}
