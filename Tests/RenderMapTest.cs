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
public sealed class RenderMapTest {

   [TestMethod]
   public void TestMapWithNullContent() {
      Assert.ThrowsException<ArgumentNullException>(() => {
         Map map = new TestMap1 {
            Contents = null,
         };
      });
   }

   [TestMethod]
   public void TestMapWithoutContent() {
      Assert.ThrowsException<ArgumentException>(() => {
         Map map = new TestMap1 {
            Contents = Array.Empty<string>(),
         };
      }, "Map has no contents");
   }

   [TestMethod]
   public void TestMapWithNoValidContent() {
      Assert.ThrowsException<ArgumentException>(() => {
         Map map = new TestMap1 {
            Contents = new string[] { "", "" },
         };
      }, "Map has no contents");
   }

   [TestMethod]
   public void TestContentWithoutFields() {
      Map map = new TestMap1 {
         Contents = new string[] { "> SCREEN 001" },
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(1, fragments.Length);
      Assert.AreEqual(" SCREEN 001", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestFormatLineWithoutPrint() {
      Map map = new TestMap1 {
         Contents = new string[] {
            ":11111111111",
            "> SCREEN 001",
         },
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(1, fragments.Length);
      Assert.AreEqual(" SCREEN 001", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithInvalidLines() {
      Map map = new TestMap1 {
         Contents = new string[] {
            "> SCREEN 001",
            "+this line is discarded",
            "> LINE 02",
         },
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(2, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(2, fragments.Length);
      Assert.AreEqual(" SCREEN 001", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual(" LINE 02   ", fragments[1].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[1].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[1].BackgroundColor);
      Assert.IsTrue(fragments[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithFormattedForeground() {
      Map map = new TestMap1 {
         Contents = new string[] {
            "> SCREEN 001",
            ":        AAA",
         }
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(2, fragments.Length);
      Assert.AreEqual(" SCREEN ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsFalse(fragments[0].LineBreak);

      Assert.AreEqual("001", fragments[1].Text);
      Assert.AreEqual(ConsoleColor.Green, fragments[1].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[1].BackgroundColor);
      Assert.IsTrue(fragments[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestContentWithFormattedBackground() {
      Map map = new TestMap1 {
         Contents = new string[] {
            "> SCREEN 001",
            ":        EEE",
            ":11111111111"
         }
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(2, fragments.Length);
      Assert.AreEqual(" SCREEN ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual(ConsoleColor.DarkBlue, fragments[0].BackgroundColor);
      Assert.IsFalse(fragments[0].LineBreak);

      Assert.AreEqual("001", fragments[1].Text);
      Assert.AreEqual(ConsoleColor.Yellow, fragments[1].ForegroundColor);
      Assert.AreEqual(ConsoleColor.DarkBlue, fragments[1].BackgroundColor);
      Assert.IsTrue(fragments[1].LineBreak);

      Assert.AreEqual(0, map.Fields.Count);
   }

   [TestMethod]
   public void TestSingleFieldLocation() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬XXXXXXX ", "¬XXXXXXXX" },
      };
      Assert.AreEqual(11, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(1, fragments.Length);
      Assert.AreEqual("           ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);
   }

   [TestMethod]
   public void TestDoubleFieldLocation() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬XXXXXXX : ¬XXXXXXX ", "¬ROT:8", "¬XXXXXXXX" },
      };
      Assert.AreEqual(22, map.Width);
      Assert.AreEqual(1, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(1, fragments.Length);
      Assert.AreEqual("           :          ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual(2, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is Label);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);

      Assert.IsFalse(map.Fields[1] is Label);
      Assert.AreEqual(13, map.Fields[1].Left);
      Assert.AreEqual(0, map.Fields[1].Top);
      Assert.AreEqual(8, map.Fields[1].Width);
   }

   [TestMethod]
   public void TestDoubleFieldLocationMultipleLines() {
      Map map = new TestMap1 {
         Contents = new string[] {
            ">  ¬XXXXXXX ",
            ">       TEST: ¬XXXXXXX ",
            "¬ROT:8",
            "¬XXXXXXXX",
         },
      };
      Assert.AreEqual(22, map.Width);
      Assert.AreEqual(2, map.Height);
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(2, fragments.Length);
      Assert.AreEqual("                      ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].BackgroundColor);
      Assert.IsTrue(fragments[0].LineBreak);

      Assert.AreEqual("       TEST:          ", fragments[1].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[1].ForegroundColor);
      Assert.AreEqual((ConsoleColor) (-1), fragments[1].BackgroundColor);
      Assert.IsTrue(fragments[1].LineBreak);

      Assert.AreEqual(2, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is Label);
      Assert.AreEqual(2, map.Fields[0].Left);
      Assert.AreEqual(0, map.Fields[0].Top);
      Assert.AreEqual(8, map.Fields[0].Width);

      Assert.IsFalse(map.Fields[1] is Label);
      Assert.AreEqual(13, map.Fields[1].Left);
      Assert.AreEqual(1, map.Fields[1].Top);
      Assert.AreEqual(8, map.Fields[1].Width);
   }

   [TestMethod]
   public void TestReadOnlyFieldValue() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬XXXXXXX ", "¬ROT:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is Label);
      Label field = (Label) map.Fields[0];
      Assert.IsFalse(field.IsDirty);
      field.Value = "01/01";
      Assert.IsTrue(field.IsDirty);
      Assert.AreEqual("01/01", field.Value);
   }

   [TestMethod]
   public void TestEditableFieldValue() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬XXXXXXX ", "¬XXXXXXXX" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is TextField);
      TextField field = (TextField) map.Fields[0];
      Assert.IsFalse(field.IsDirty);
      field.Value = "01/01";
      Assert.IsTrue(field.IsDirty);
      Assert.AreEqual("01/01", field.Value);
      Assert.AreEqual("01/01\t\t\t", field.GetInnerValue());
   }

   [TestMethod]
   public void TestFieldIgnoresSameValue() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬XXXXXXX ", "¬XXXXXXXX" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is TextField);
      TextField field = (TextField) map.Fields[0];
      Assert.IsFalse(field.IsDirty);
      field.Value = "01/01";
      Assert.IsTrue(field.IsDirty);
      Assert.AreEqual("01/01", field.Value);
      Assert.AreEqual("01/01\t\t\t", field.GetInnerValue());
      field.IsDirty = false;
      Assert.IsFalse(field.IsDirty);
      field.Value = "01/01";
      Assert.IsFalse(field.IsDirty);
   }

   [TestMethod]
   public void TestMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.SetMessage("Test message");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test mes", ((StatusMessage) map.Fields[0]).Value);
      Assert.AreEqual(MessageKind.Info, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestErrorMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.SetError("Test error");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test err", ((StatusMessage) map.Fields[0]).Value);
      Assert.AreEqual(MessageKind.Error, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestAlertMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.SetAlert("Test alert");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test ale", ((StatusMessage) map.Fields[0]).Value);
      Assert.AreEqual(MessageKind.Alert, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestSuccessMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.SetSuccess("Test success");
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test suc", ((StatusMessage) map.Fields[0]).Value);
      Assert.AreEqual(MessageKind.Success, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestDirectSetMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      ((StatusMessage) map.Fields[0]).Value = "Test message";
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("Test mes", ((StatusMessage) map.Fields[0]).Value);
      Assert.AreEqual(MessageKind.None, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestClearMessageField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬XXXXXXX ", "¬MSG:8" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is StatusMessage);
      Assert.IsFalse(map.Fields[0].IsDirty);
      map.SetError("Test error");
      map.Fields[0].IsDirty = false;
      Assert.AreEqual("Test err", ((StatusMessage) map.Fields[0]).Value);
      Assert.IsFalse(map.Fields[0].IsDirty);
      Assert.AreEqual(MessageKind.Error, ((StatusMessage) map.Fields[0]).Kind);
      map.ClearMessage();
      Assert.AreEqual(null, ((StatusMessage) map.Fields[0]).Value);
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual(MessageKind.None, ((StatusMessage) map.Fields[0]).Kind);
   }

   [TestMethod]
   public void TestDuplicateMessageField() {
      InvalidFieldException ex = Assert.ThrowsException<InvalidFieldException>(() => {
         Map map = new TestMap1 {
            Contents = new string[] { ">  ¬XXXXXXX   ¬XXX ", "¬MSG:8", "¬MSG:4" },
         };
      });
      Assert.AreEqual(13, ex.FieldLeft);
      Assert.AreEqual(0, ex.FieldTop);
   }

   [TestMethod]
   public void TestNullValueForField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬AAAAAAA ", "¬AAAAAAAA" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      Assert.AreEqual("", field.Value);
      field.Value = "ABCD";
      Assert.IsTrue(map.Fields[0].IsDirty);
      Assert.AreEqual("ABCD", field.Value);
      field.Value = null;
      Assert.AreEqual("", field.Value);
   }

   [TestMethod]
   public void TestValidValueForTextField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬AAAAAAA ", "¬AAAAAAAA" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      field.Value = "ABCD";
      Assert.IsTrue(field.IsDirty);
      Assert.AreEqual("ABCD", field.Value);
   }

   [TestMethod]
   public void TestInvalidValueForTextField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬AAAAAAA ", "¬AAAAAAAA" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      Assert.ThrowsException<ArgumentException>(() => field.Value = "1234");
   }

   [TestMethod]
   public void TestValidValueForNumberField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬9999999 ", "¬99999999" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      field.Value = "1234";
      Assert.IsTrue(field.IsDirty);
      Assert.AreEqual("1234", field.Value);
   }

   [TestMethod]
   public void TestInvalidValueForNumberField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬9999999 ", "¬99999999" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      Assert.ThrowsException<ArgumentException>(() => field.Value = "ABCD");
   }

   [TestMethod]
   public void TestInvalidMaskForField() {
      Map map = new TestMap1 {
         Contents = new string[] { ">  ¬------- ", "¬--------" },
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.IsTrue(map.Fields[0] is InputField);
      TextField field = (TextField) map.Fields[0];
      Assert.ThrowsException<InvalidDataException>(() => field.Value = "ABCD");
   }


   [DataTestMethod]
   [DataRow(ConsoleKey.F3, false, false)]
   [DataRow(ConsoleKey.F10, false, true)]
   [DataRow(ConsoleKey.F10, true, false)]
   public void TestMapKeyPressed(ConsoleKey key, bool shift, bool result) {
      TestMap1 map = new();
      Assert.IsFalse(map.F10Pressed);

      map.DidKeyPress(new('\0', key, shift, false, false));
      Assert.AreEqual(result, map.F10Pressed);
   }

   [TestMethod]
   public void TestContentFromResource() {
      Map map = new TestMap2();
      MapFragment[] fragments = map.ContentFragments.ToArray();
      Assert.AreEqual(4, fragments.Length);

      Assert.AreEqual("  SCREEN ", fragments[0].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[0].ForegroundColor);
      Assert.IsFalse(fragments[0].LineBreak);

      Assert.AreEqual("002", fragments[1].Text);
      Assert.AreEqual(ConsoleColor.Yellow, fragments[1].ForegroundColor);
      Assert.IsTrue(fragments[1].LineBreak);

      Assert.AreEqual("            ", fragments[2].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[2].ForegroundColor);
      Assert.IsTrue(fragments[2].LineBreak);

      Assert.AreEqual(" OPTION:    ", fragments[3].Text);
      Assert.AreEqual((ConsoleColor) (-1), fragments[3].ForegroundColor);
      Assert.IsTrue(fragments[3].LineBreak);

      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(9, map.Fields[0].Left);
      Assert.AreEqual(2, map.Fields[0].Top);
      Assert.AreEqual(2, map.Fields[0].Width);
      Assert.IsFalse(map.Fields[0] is Label);
   }


   [TestMethod]
   public void TestReorderNonExistingField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬ ", "> ¬ ", "> ¬ ", "¬X", "¬9", "¬A" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => map.Fields.Move(7, 2));
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => map.Fields.Move(-1, 2));
   }

   [TestMethod]
   public void TestReorderField() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬ ", "> ¬ ", "> ¬ ", "¬X", "¬9", "¬A" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("X", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("9", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[2]).InputMask);
      map.Fields.Move(0, 1);
      Assert.AreEqual("9", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("X", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[2]).InputMask);
   }

   [TestMethod]
   public void TestReorderFieldToBeforeStart() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬ ", "> ¬ ", "> ¬ ", "¬X", "¬9", "¬A" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("X", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("9", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[2]).InputMask);
      map.Fields.Move(1, -2);
      Assert.AreEqual("9", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("X", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[2]).InputMask);
   }

   [TestMethod]
   public void TestReorderFieldToBeyondEnd() {
      Map map = new TestMap1 {
         Contents = new string[] { "> ¬ ", "> ¬ ", "> ¬ ", "¬X", "¬9", "¬A" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual("X", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("9", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[2]).InputMask);
      map.Fields.Move(1, 7);
      Assert.AreEqual("X", ((TextField) map.Fields[0]).InputMask);
      Assert.AreEqual("A", ((TextField) map.Fields[1]).InputMask);
      Assert.AreEqual("9", ((TextField) map.Fields[2]).InputMask);
   }


   private sealed class TestMap1 : Map {

      public bool F10Pressed { get; set; } = false;

      [CommandKey(ConsoleKey.F10)]
      public void KeyPressed()
         => F10Pressed = true;
   }

   private sealed class TestMap2 : Map { }
}
