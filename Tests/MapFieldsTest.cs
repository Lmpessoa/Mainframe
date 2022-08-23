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
public sealed class MapFieldsTest {

   [TestMethod]
   public void TestTwoMethodsCannotShareName() {
      Exception ex = Assert.ThrowsException<ArgumentException>(() => new TestMap("> ¬     ¬    ", "¬Field:INP[3]", "¬Field:ROT[3]"));
      Assert.AreEqual("Duplicate field name: 'Field'", ex.Message);
   }

   [TestMethod]
   public void TestMoveFocusToField() {
      Map map = new TestMap(">  ¬XXXXXXX ", ">  ¬XXXXXXX ", "¬Field1:INP[8]", "¬Field2:INP[8]");
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocusTo(5, 1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
      Assert.AreEqual(2, map.Fields[map.CurrentFieldIndex].Left);
      Assert.AreEqual(1, map.Fields[map.CurrentFieldIndex].Top);
   }

   [TestMethod]
   public void TestMoveFocusToNowhere() {
      Map map = new TestMap(">  ¬XXXXXXX ", ">  ¬XXXXXXX ", "¬Label:ROT[8]", "¬Field:INP[8]");
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocusTo(4, 1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
      map.MoveFocusTo(8, 0);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] NO_FIELDS = new[] { "> NO FIELDS HERE" };

   [TestMethod]
   public void TestIgnoresMoveFocusNoFields() {
      Map map = new TestMap(NO_FIELDS);
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(1);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusNextNoFields() {
      Map map = new TestMap(NO_FIELDS);
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusPrevNoFields() {
      Map map = new TestMap(NO_FIELDS);
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] LABEL_ONLY = new[] { ">  ¬XXXXXXX  ", "¬Label:ROT[8]" };

   [TestMethod]
   public void TestIgnoresMoveFocusIfNotField() {
      Map map = new TestMap(LABEL_ONLY);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusNextIfNotField() {
      Map map = new TestMap(LABEL_ONLY);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusPrevIfNotField() {
      Map map = new TestMap(LABEL_ONLY);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] SINGLE_FIELD = new[] { ">  ¬XXXXXXX  ", "¬Field:INP[8]" };

   [TestMethod]
   public void TestMoveFocusToSelfSingleField() {
      Map map = new TestMap(SINGLE_FIELD);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoreMoveFocusToNonExistingField() {
      Map map = new TestMap(SINGLE_FIELD);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(2);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextToSelfSingleField() {
      Map map = new TestMap(SINGLE_FIELD);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevToSelfSingleField() {
      Map map = new TestMap(SINGLE_FIELD);
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   private static readonly string[] FIELD_AND_LABEL = new[] { ">  ¬XXXXXXX ¬XXXXXXX ", "¬Field:INP[8]", "¬Label:ROT[8]" };

   [TestMethod]
   public void TestMoveFocusNextToSelfFieldAndLabel() {
      Map map = new TestMap(FIELD_AND_LABEL);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevToSelfFieldAndLabel() {
      Map map = new TestMap(FIELD_AND_LABEL);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   private static readonly string[] TWO_FIELDS = new[] {
      "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬Field1:INP[8]", "¬Field2:INP[8]",
   };

   [TestMethod]
   public void TestMoveFocusToSameTwoFields() {
      Map map = new TestMap(TWO_FIELDS);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusToExistingField() {
      Map map = new TestMap(TWO_FIELDS);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoreMoveFocusToNonExistingFieldInSet() {
      Map map = new TestMap(TWO_FIELDS);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(2);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextTwoFields() {
      Map map = new TestMap(TWO_FIELDS);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevTwoFields() {
      Map map = new TestMap(TWO_FIELDS);
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   private static readonly string[] THREE_FIELDS = new[] {
      "> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬Field1:INP[8]", "¬Field2:INP[8]", "¬Field3:INP[8]"
   };

   [TestMethod]
   public void TestMoveFocusNextThreeFields() {
      Map map = new TestMap(THREE_FIELDS);
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevThreeFields() {
      Map map = new TestMap(THREE_FIELDS);
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(2, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextSkipsLabel() {
      Map map = new TestMap("> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬Field1:INP[8]", "¬Label:ROT[8]", "¬Field2:INP[8]");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(2, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevSkipsLabel() {
      Map map = new TestMap("> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬Field1:INP[8]", "¬Field2:INP[8]", "¬Label:ROT[8]");
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestFieldWithUnexistingType() {
      Exception ex = Assert.ThrowsException<ArgumentException>(() => new TestMap("> ¬XXXXXXX ", "¬Field:KLM[8]"));
      Assert.AreEqual("Unknown field type: 'KLM'", ex.Message);
   }

   [TestMethod]
   public void TestHiddingUndefinedField() {
      Map map = new TestMap("> ¬XXXXXXX ", "¬Field:INP[8]");
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual("Field", map.Fields[0].Name);
      Exception ex = Assert.ThrowsException<ArgumentException>(() => map.SetFieldVisible("Label", false));
      Assert.AreEqual("Field 'Label' is not defined", ex.Message);
   }

   [TestMethod]
   public void TestCheckFieldWithWidth() {
      Exception ex = Assert.ThrowsException<ArgumentException>(() => new TestMap("> ¬XXXXXXX ", "¬Field:CHK[8]"));
      Assert.AreEqual("Invalid field definition: 'Field:CHK[8]'", ex.Message);
   }

   [TestMethod]
   public void TestCheckFieldWithMask() {
      Assert.ThrowsException<FormatException>(() => new TestMap("> ¬XXXXXXX ", "¬Field:CHK(X)"));
   }

   [TestMethod]
   public void TestFieldWithOtherDefinition() {
      Exception ex = Assert.ThrowsException<ArgumentException>(() => new TestMap("> ¬XXXXXXX ", "¬Field:INP{value}"));
      Assert.AreEqual("Invalid field definition: 'Field:INP{value}'", ex.Message);
   }

   [TestMethod]
   public void TestFieldsOverlapping() {
      Exception ex = Assert.ThrowsException<ArgumentException>(() => new TestMap("> ¬XX¬XX ", "¬Field1:INP[6]", "¬Field2:INP[3]"));
      Assert.AreEqual("Fields overlap: 'Field1' and 'Field2'", ex.Message);
   }

   private sealed class TestMap : Map {
      public TestMap(params string[] contents) : base(contents) { }
   }
}
