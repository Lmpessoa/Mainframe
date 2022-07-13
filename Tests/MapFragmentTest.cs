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
public sealed class MapFragmentTest {

   [TestMethod]
   public void TestSimpleLine() {
      MapFragment[] result = MapFragment.Parse("  TEST  ").ToArray();
      Assert.AreEqual(1, result.Length);
      AssertFragmentIs(result[0], "  TEST  ", (ConsoleColor) (-1), (ConsoleColor) (-1), true);
   }

   [TestMethod]
   public void TestTwoPartCommandFirst() {
      MapFragment[] result = MapFragment.Parse("PF10 EXIT",
                                               "++++     ").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "PF10", (ConsoleColor) 16, (ConsoleColor) 16, false);
      AssertFragmentIs(result[1], " EXIT", (ConsoleColor) (-1), (ConsoleColor) (-1), true);
   }

   [TestMethod]
   public void TestTwoPartCommandFirstTrimmed() {
      MapFragment[] result = MapFragment.Parse("PF10 EXIT",
                                               "++++").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "PF10", (ConsoleColor) 16, (ConsoleColor) 16, false);
      AssertFragmentIs(result[1], " EXIT", (ConsoleColor) (-1), (ConsoleColor) (-1), true);
   }

   [TestMethod]
   public void TestTwoPartCommandLast() {
      MapFragment[] result = MapFragment.Parse("EXIT -> PF10",
                                               "        ++++").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "EXIT -> ", (ConsoleColor) (-1), (ConsoleColor) (-1), false);
      AssertFragmentIs(result[1], "PF10", (ConsoleColor) 16, (ConsoleColor) 16, true);
   }

   [TestMethod]
   public void TestThreePartCommand() {
      MapFragment[] result = MapFragment.Parse("   PF10 EXIT",
                                               "   ++++").ToArray();
      Assert.AreEqual(3, result.Length);
      AssertFragmentIs(result[0], "   ", (ConsoleColor) (-1), (ConsoleColor) (-1), false);
      AssertFragmentIs(result[1], "PF10", (ConsoleColor) 16, (ConsoleColor) 16, false);
      AssertFragmentIs(result[2], " EXIT", (ConsoleColor) (-1), (ConsoleColor) (-1), true);
   }

   [TestMethod]
   public void TestCommandIgnoresBackground() {
      MapFragment[] result = MapFragment.Parse("PF10", "++++", "0000").ToArray();
      Assert.AreEqual(1, result.Length);
      AssertFragmentIs(result[0], "PF10", (ConsoleColor) 16, (ConsoleColor) 16, true);
   }

   [TestMethod]
   public void TestTwoPartWithSameBackground() {
      MapFragment[] result = MapFragment.Parse("SCREEN 001",
                                               "FFFFFFFEEE",
                                               "1111111111").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "SCREEN ", ConsoleColor.White, ConsoleColor.DarkBlue, false);
      AssertFragmentIs(result[1], "001", ConsoleColor.Yellow, ConsoleColor.DarkBlue, true);
   }

   [TestMethod]
   public void TestTwoPartWithSameForeground() {
      MapFragment[] result = MapFragment.Parse("SCREEN 001",
                                               "FFFFFFFFFF",
                                               "1111111222").ToArray();
      Assert.AreEqual(2, result.Length);
      AssertFragmentIs(result[0], "SCREEN ", ConsoleColor.White, ConsoleColor.DarkBlue, false);
      AssertFragmentIs(result[1], "001", ConsoleColor.White, ConsoleColor.DarkGreen, true);
   }

   [TestMethod]
   public void TestMisalignedColours() {
      MapFragment[] result = MapFragment.Parse("DOUBLECOLORS",
                                               "111116666666",
                                               "000000088888").ToArray();
      Assert.AreEqual(3, result.Length);
      AssertFragmentIs(result[0], "DOUBL", ConsoleColor.DarkBlue, ConsoleColor.Black, false);
      AssertFragmentIs(result[1], "EC", ConsoleColor.DarkYellow, ConsoleColor.Black, false);
      AssertFragmentIs(result[2], "OLORS", ConsoleColor.DarkYellow, ConsoleColor.DarkGray, true);
   }

   private static void AssertFragmentIs(MapFragment fragment, string expectedText,
         ConsoleColor expectedFore, ConsoleColor expectedBack, bool lineBreak) {
      Assert.AreEqual(expectedText, fragment.Text);
      Assert.AreEqual(expectedFore, fragment.ForegroundColor);
      Assert.AreEqual(expectedBack, fragment.BackgroundColor);
      Assert.AreEqual(lineBreak, fragment.LineBreak);
   }
}
