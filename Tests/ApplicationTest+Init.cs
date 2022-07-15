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

public partial class ApplicationTest {

   [TestMethod]
   public void TestApplicationWithNullInitialMap() {
      Assert.ThrowsException<ArgumentNullException>(() => _ = new Application(null, Console));
   }

   [TestMethod]
   public void TestApplicationWithNullConsole() {
      Assert.ThrowsException<ArgumentNullException>(() => _ = new Application(Map, null));
   }

   [TestMethod]
   public void TestApplicationIsRunning() {
      Assert.IsTrue(Application.IsRunning);
      App.Stop();
      Assert.IsFalse(Application.IsRunning);
   }

   [TestMethod]
   public void TestCannotRunTwoApps() {
      Application app2 = new(new TestMap(), Console);
      Assert.ThrowsException<InvalidOperationException>(() => app2.Start());
   }

   [TestMethod]
   public void TestCannotRunAppTwice() {
      Assert.ThrowsException<InvalidOperationException>(() => App.Start());
   }

   [TestMethod]
   public void TestAppReturnCode() {
      Application.Exit();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(-1, App.ReturnCode);
   }

   [TestMethod]
   public void TestAppCanBeRestarted() {
      Application.Exit();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(-1, App.ReturnCode);
      App.Start();
      Assert.IsTrue(Application.IsRunning);
      Assert.AreEqual(0, App.ReturnCode);
   }

   [TestMethod]
   public void TestCannotChangeOptionsWhileRunning() {
      Assert.ThrowsException<InvalidOperationException>(() => App.UseActiveFieldBackground());
   }

   [TestMethod]
   public void TestCannotShowMapAlreadyInApp() {
      Assert.ThrowsException<InvalidOperationException>(() => Map.Show());
   }

   [TestMethod]
   public void TestSettingConsoleSize() {
      Assert.AreEqual(80, Console.WindowWidth);
      Assert.AreEqual(24, Console.WindowHeight);
      App.Stop();
      App.SetWindowSize(120, 30);
      App.Start();
      Assert.AreEqual(120, Console.WindowWidth);
      Assert.AreEqual(30, Console.WindowHeight);
      App.Stop();
      Assert.AreEqual(80, Console.WindowWidth);
      Assert.AreEqual(24, Console.WindowHeight);
   }

   [TestMethod]
   public void TestEnforcingConsoleSize() {
      App.Stop();
      Console.SetWindowSize(120, 30);
      App.Start();
      Assert.AreEqual(120, Console.WindowWidth);
      Assert.AreEqual(30, Console.WindowHeight);
      App.Stop();
      App.SetWindowSize(80, 24);
      App.Start();
      Assert.AreEqual(120, Console.WindowWidth);
      Assert.AreEqual(30, Console.WindowHeight);
      App.Stop();
      App.EnforceWindowSize();
      App.Start();
      Assert.AreEqual(80, Console.WindowWidth);
      Assert.AreEqual(24, Console.WindowHeight);
      App.Stop();
      Assert.AreEqual(120, Console.WindowWidth);
      Assert.AreEqual(30, Console.WindowHeight);
   }
}