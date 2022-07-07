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
public sealed class FieldMapTest {

   [TestMethod]
   public void TestMoveFocusToField() {
      Map map = new TestMap {
         Contents = new string[] { ">  ¬XXXXXXX ", ">  ¬XXXXXXX ", "¬XXXXXXXX", "¬XXXXXXXX" },
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocusTo(5, 1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
      Assert.AreEqual(2, map.Fields[map.CurrentFieldIndex].Left);
      Assert.AreEqual(1, map.Fields[map.CurrentFieldIndex].Top);
   }

   [TestMethod]
   public void TestMoveFocusToNowhere() {
      Map map = new TestMap {
         Contents = new string[] { ">  ¬XXXXXXX ", ">  ¬XXXXXXX ", "¬ROT:8", "¬XXXXXXXX" },
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocusTo(4, 1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
      map.MoveFocusTo(8, 0);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] NO_FIELDS = new string[] { "> NO FIELDS HERE" };

   [TestMethod]
   public void TestIgnoresMoveFocusNoFields() {
      Map map = new TestMap {
         Contents = NO_FIELDS,
      };
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(1);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusNextNoFields() {
      Map map = new TestMap {
         Contents = NO_FIELDS,
      };
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusPrevNoFields() {
      Map map = new TestMap {
         Contents = NO_FIELDS,
      };
      Assert.AreEqual(0, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] LABEL_ONLY = new string[] { ">  ¬XXXXXXX  ", "¬ROT:8" };

   [TestMethod]
   public void TestIgnoresMoveFocusIfNotField() {
      Map map = new TestMap {
         Contents = LABEL_ONLY,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusNextIfNotField() {
      Map map = new TestMap {
         Contents = LABEL_ONLY,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoresMoveFocusPrevIfNotField() {
      Map map = new TestMap {
         Contents = LABEL_ONLY,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   private static readonly string[] SINGLE_FIELD = new string[] { ">  ¬XXXXXXX  ", "¬XXXXXXXX" };

   [TestMethod]
   public void TestMoveFocusToSelfSingleField() {
      Map map = new TestMap {
         Contents = SINGLE_FIELD,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoreMoveFocusToNonExistingField() {
      Map map = new TestMap {
         Contents = SINGLE_FIELD,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(2);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextToSelfSingleField() {
      Map map = new TestMap {
         Contents = SINGLE_FIELD,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevToSelfSingleField() {
      Map map = new TestMap {
         Contents = SINGLE_FIELD,
      };
      Assert.AreEqual(1, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   private static readonly string[] FIELD_AND_LABEL = new string[] { ">  ¬XXXXXXX ¬XXXXXXX ", "¬XXXXXXXX", "¬ROT:8" };

   [TestMethod]
   public void TestMoveFocusNextToSelfFieldAndLabel() {
      Map map = new TestMap {
         Contents = FIELD_AND_LABEL,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevToSelfFieldAndLabel() {
      Map map = new TestMap {
         Contents = FIELD_AND_LABEL,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   private static readonly string[] TWO_FIELDS = new string[] {
      "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬XXXXXXXX", "¬XXXXXXXX",
   };

   [TestMethod]
   public void TestMoveFocusToSameTwoFields() {
      Map map = new TestMap {
         Contents = TWO_FIELDS,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(0);
      Assert.AreEqual(0, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusToExistingField() {
      Map map = new TestMap {
         Contents = TWO_FIELDS,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(1);
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestIgnoreMoveFocusToNonExistingFieldInSet() {
      Map map = new TestMap {
         Contents = TWO_FIELDS,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.MoveFocus(2);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextTwoFields() {
      Map map = new TestMap {
         Contents = TWO_FIELDS,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevTwoFields() {
      Map map = new TestMap {
         Contents = TWO_FIELDS,
      };
      Assert.AreEqual(2, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   private static readonly string[] THREE_FIELDS = new string[] {
      "> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬XXXXXXXX", "¬XXXXXXXX", "¬XXXXXXXX"
   };

   [TestMethod]
   public void TestMoveFocusNextThreeFields() {
      Map map = new TestMap {
         Contents = THREE_FIELDS,
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevThreeFields() {
      Map map = new TestMap {
         Contents = THREE_FIELDS,
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(2, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusNextSkipsLabel() {
      Map map = new TestMap {
         Contents = new string[] { "> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬XXXXXXXX", "¬ROT:8", "¬XXXXXXXX" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(0, map.CurrentFieldIndex);
      map.FocusOnNextField();
      Assert.AreEqual(2, map.CurrentFieldIndex);
   }

   [TestMethod]
   public void TestMoveFocusPrevSkipsLabel() {
      Map map = new TestMap {
         Contents = new string[] { "> ¬XXXXXXX ", "> ¬XXXXXXX ", "> ¬XXXXXXX ", "¬XXXXXXXX", "¬XXXXXXXX", "¬ROT:8" },
      };
      Assert.AreEqual(3, map.Fields.Count);
      Assert.AreEqual(-1, map.CurrentFieldIndex);
      map.FocusOnPreviousField();
      Assert.AreEqual(1, map.CurrentFieldIndex);
   }

   private sealed class TestMap : Map { }
}
