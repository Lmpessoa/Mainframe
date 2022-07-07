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
public class ApplicationInitTest : ApplicationTest {

   private Field MockRegister(string args) => new TextField();

   [TestMethod]
   public void TestFieldRegisterWithoutPrefix() {
      Assert.ThrowsException<ArgumentNullException>(() => Field.Register(null, MockRegister));
   }

   [TestMethod]
   public void TestFieldRegisterWithInvalidPrefix() {
      Assert.ThrowsException<ArgumentException>(() => Field.Register("de", MockRegister));
      Assert.ThrowsException<ArgumentException>(() => Field.Register("dext", MockRegister));
      Assert.ThrowsException<ArgumentException>(() => Field.Register("de1", MockRegister));
      Assert.ThrowsException<ArgumentException>(() => Field.Register("de-", MockRegister));
   }

   [TestMethod]
   public void TestFieldregisterWithDuplicatePrefix() {
      Assert.ThrowsException<ArgumentException>(() => Field.Register("chk", MockRegister));
   }

   [TestMethod]
   public void TestFieldRegisterWithoutCreator() {
      Assert.ThrowsException<ArgumentNullException>(() => Field.Register("XXX", null));
   }

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
}