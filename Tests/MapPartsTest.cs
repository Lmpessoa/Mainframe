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
public sealed class MapPartsTest {

   [TestMethod]
   public void TestSimpleLine() {
      MapPart[] result = MapPart.Parse("  TEST  ").ToArray();
      Assert.AreEqual(1, result.Length);
      AssertFragmentIs(result[0], "  TEST  ", MapPartColor.Default, MapPartColor.Default, true);
   }

   [TestMethod]
   public void TestTwoPartCommandFirst() {
      MapPart[] result = MapPart.Parse("PF10 EXIT",
                                               "++++     ").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "PF10", MapPartColor.Highlight, MapPartColor.Highlight, false);
      AssertFragmentIs(result[1], " EXIT", MapPartColor.Default, MapPartColor.Default, true);
   }

   [TestMethod]
   public void TestTwoPartCommandFirstTrimmed() {
      MapPart[] result = MapPart.Parse("PF10 EXIT",
                                               "++++").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "PF10", MapPartColor.Highlight, MapPartColor.Highlight, false);
      AssertFragmentIs(result[1], " EXIT", MapPartColor.Default, MapPartColor.Default, true);
   }

   [TestMethod]
   public void TestTwoPartCommandLast() {
      MapPart[] result = MapPart.Parse("EXIT -> PF10",
                                               "        ++++").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "EXIT -> ", MapPartColor.Default, MapPartColor.Default, false);
      AssertFragmentIs(result[1], "PF10", MapPartColor.Highlight, MapPartColor.Highlight, true);
   }

   [TestMethod]
   public void TestThreePartCommand() {
      MapPart[] result = MapPart.Parse("   PF10 EXIT",
                                               "   ++++").ToArray();
      Assert.AreEqual(3, result.Length);
      AssertFragmentIs(result[0], "   ", MapPartColor.Default, MapPartColor.Default, false);
      AssertFragmentIs(result[1], "PF10", MapPartColor.Highlight, MapPartColor.Highlight, false);
      AssertFragmentIs(result[2], " EXIT", MapPartColor.Default, MapPartColor.Default, true);
   }

   [TestMethod]
   public void TestCommandIgnoresBackground() {
      MapPart[] result = MapPart.Parse("PF10", "++++", "0000").ToArray();
      Assert.AreEqual(1, result.Length);
      AssertFragmentIs(result[0], "PF10", MapPartColor.Highlight, MapPartColor.Highlight, true);
   }

   [TestMethod]
   public void TestTwoPartWithSameBackground() {
      MapPart[] result = MapPart.Parse("SCREEN 001",
                                               "FFFFFFFEEE",
                                               "1111111111").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "SCREEN ", MapPartColor.White, MapPartColor.DarkBlue, false);
      AssertFragmentIs(result[1], "001", MapPartColor.Yellow, MapPartColor.DarkBlue, true);
   }

   [TestMethod]
   public void TestTwoPartWithSameForeground() {
      MapPart[] result = MapPart.Parse("SCREEN 001",
                                               "FFFFFFFFFF",
                                               "1111111222").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "SCREEN ", MapPartColor.White, MapPartColor.DarkBlue, false);
      AssertFragmentIs(result[1], "001", MapPartColor.White, MapPartColor.DarkGreen, true);
   }

   [TestMethod]
   public void TestMisalignedColours() {
      MapPart[] result = MapPart.Parse("DOUBLECOLORS",
                                               "111116666666",
                                               "000000088888").ToArray();
      Assert.AreEqual(3, result.Length);
      AssertFragmentIs(result[0], "DOUBL", MapPartColor.DarkBlue, MapPartColor.Black, false);
      AssertFragmentIs(result[1], "EC", MapPartColor.DarkYellow, MapPartColor.Black, false);
      AssertFragmentIs(result[2], "OLORS", MapPartColor.DarkYellow, MapPartColor.DarkGray, true);
   }

   private static void AssertFragmentIs(MapPart fragment, string expectedText,
         MapPartColor expectedFore, MapPartColor expectedBack, bool lineBreak) {
      Assert.AreEqual(expectedText, fragment.Text);
      Assert.AreEqual(expectedFore, fragment.ForegroundColor);
      Assert.AreEqual(expectedBack, fragment.BackgroundColor);
      Assert.AreEqual(lineBreak, fragment.LineBreak);
   }
}
