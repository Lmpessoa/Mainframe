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
public class ApplicationEventsTest {

   private static Queue<string> EventLog { get; set; }
   private Application _app;
   private MockConsole _console;
   private TestMap _map;

   [TestInitialize]
   public void Setup() {
      EventLog = new();
      _console = new();
      _map = new("Main");
      _app = new(_map, _console);
      _app.Start();
   }

   [TestMethod]
   public void TestMapSetupAndTeardown() {
      _map.Close();
      _app.DoLoopStep();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(4, EventLog.Count);
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
      Assert.AreEqual("Closing Main", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Main", EventLog.Dequeue());
      Assert.AreEqual("Closed Main", EventLog.Dequeue());
   }

   [TestMethod]
   public void TestTwoMapsSetupAndTeardown() {
      Assert.AreSame(_map, _app.CurrentMap);
      _app.DoLoopStep();
      TestMap second = new("Second");
      second.Show();
      _app.DoLoopStep();
      Assert.AreSame(second, _app.CurrentMap);
      second.Close();
      _app.DoLoopStep();
      Assert.AreSame(_map, _app.CurrentMap);
      _map.Close();
      _app.DoLoopStep();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(10, EventLog.Count);
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Main", EventLog.Dequeue());
      Assert.AreEqual("Activating Second", EventLog.Dequeue());
      Assert.AreEqual("Closing Second", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Second", EventLog.Dequeue());
      Assert.AreEqual("Closed Second", EventLog.Dequeue());
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
      Assert.AreEqual("Closing Main", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Main", EventLog.Dequeue());
      Assert.AreEqual("Closed Main", EventLog.Dequeue());
   }

   [TestMethod]
   public void TestExitingApplication() {
      _app.DoLoopStep();
      Application.Exit();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(1, EventLog.Count);
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
   }

   [TestMethod]
   public void TestChangeFields() {
      _console.SendKey(ConsoleKey.Tab);
      _app.DoLoopStep();
      _map.Close();
      _app.DoLoopStep();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(5, EventLog.Count);
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
      Assert.AreEqual("Field OptYes lost focus on Main", EventLog.Dequeue());
      Assert.AreEqual("Closing Main", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Main", EventLog.Dequeue());
      Assert.AreEqual("Closed Main", EventLog.Dequeue());
   }

   [TestMethod]
   public void TestKeyPressed() {
      _console.SendKey(ConsoleKey.F3);
      _app.DoLoopStep();
      _console.SendKey(KeyModifier.CtrlShift, ConsoleKey.O);
      _app.DoLoopStep();
      _map.Close();
      _app.DoLoopStep();
      Assert.IsFalse(Application.IsRunning);
      Assert.AreEqual(6, EventLog.Count);
      Assert.AreEqual("Activating Main", EventLog.Dequeue());
      Assert.AreEqual("Pressed key F3 on Main", EventLog.Dequeue());
      Assert.AreEqual("Pressed key Ctrl+Shift+O on Main", EventLog.Dequeue());
      Assert.AreEqual("Closing Main", EventLog.Dequeue());
      Assert.AreEqual("Deactivating Main", EventLog.Dequeue());
      Assert.AreEqual("Closed Main", EventLog.Dequeue());
   }

   public sealed class TestMap : Map {

      private readonly string _name;

      public TestMap(string name) : base("> ¬ YES   ¬ NO ", "¬OptYes:CHK(1)", "¬OptNo:CHK(1)")
         => _name = name ?? throw new ArgumentNullException(nameof(name));

      protected override void OnActivating()
         => EventLog.Enqueue($"Activating {_name}");

      protected override void OnClosed()
         => EventLog.Enqueue($"Closed {_name}");

      protected override bool OnClosing() {
         EventLog.Enqueue($"Closing {_name}");
         return true;
      }

      protected override void OnDeactivating()
         => EventLog.Enqueue($"Deactivating {_name}");

      protected override void OnKeyPressed(ConsoleKeyInfo key)
         => EventLog.Enqueue($"Pressed key {Application.SimplifyKeyInfo(key)} on {_name}");

      protected override void OnLostFocus(string fieldName)
         => EventLog.Enqueue($"Field {fieldName} lost focus on {_name}");
   }
}