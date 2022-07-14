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

namespace Lmpessoa.Mainframe;

/// <summary>
/// 
/// </summary>
public sealed partial class Application {

   /// <summary>
   /// 
   /// </summary>
   public static bool IsRunning
      => _current is not null && _current._maps.Any();

   /// <summary>
   /// 
   /// </summary>
   /// <param name="code"></param>
   public static void Exit(int code = -1) {
      if (IsRunning) {
         _current!.ReturnCode = code;
         _current.Stop();
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="map"></param>
   /// <returns></returns>
   public static int Run(Map map)
      => new Application(map).Run();

   /// <summary>
   /// 
   /// </summary>
   /// <param name="initialMap"></param>
   public Application(Map initialMap) : this(initialMap, SystemConsole.Instance) { }

   /// <summary>
   /// 
   /// </summary>
   public int ReturnCode { get; private set; } = 0;


   /// <summary>
   /// 
   /// </summary>
   public void PreserveGivenFieldValues() {
      AssertAppIsNotRunning();
      PreserveValues = true;
   }

   /// <summary>
   /// 
   /// </summary>
   /// <returns></returns>
   public int Run() {
      try {
         Start();
         while (IsRunning) {
            DoLoopStep();
         }
      } finally {
         Stop();
      }
      return ReturnCode;
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="width"></param>
   /// <param name="height"></param>
   public void SetConsoleSize(int width, int height) {
      AssertAppIsNotRunning();
      _consoleSize = (Math.Max(80, width), Math.Max(24, height));
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="left"></param>
   /// <param name="top"></param>
   public void SetDefaultWindowPosition(int left, int top) {
      AssertAppIsNotRunning();
      _windowPos = (Math.Max(left, -1), Math.Max(top, -1));
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="maxIdleTime"></param>
   public void SetMaxIdleTime(uint maxIdleTime) {
      AssertAppIsNotRunning();
      _maxIdleTime = maxIdleTime;
   }

   /// <summary>
   /// 
   /// </summary>
   public void UseActiveFieldBackground() {
      AssertAppIsNotRunning();
      _colors.UseActiveFieldBackground();
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="fgcolor"></param>
   /// <param name="bgcolor"></param>
   public void UseActiveFieldColors(ConsoleColor fgcolor, ConsoleColor bgcolor) {
      AssertAppIsNotRunning();
      _colors.UseActiveFieldColors(fgcolor, bgcolor);
   }

   /// <summary>
   /// 
   /// </summary>
   public void UseBulletsInPasswordFields() {
      AssertAppIsNotRunning();
      _passwordChar = '\u25CF';
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="color"></param>
   public void UseCommandColorInBackground(ConsoleColor color) {
      AssertAppIsNotRunning();
      _colors.UseCommandColors(_colors.DefaultBackgroundColor, color);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="color"></param>
   public void UseCommandColorInForeground(ConsoleColor color) {
      AssertAppIsNotRunning();
      _colors.UseCommandColors(color, _colors.DefaultBackgroundColor);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="border"></param>
   public void UseWindowBorder(WindowBorder border) {
      AssertAppIsNotRunning();
      _border = border;
   }



   internal static void SetCursorPosition(int left, int top) {
      if (_current is not null) {
         _current.Console.SetCursorPosition(left, top);
      }
   }

   internal Application(Map initialMap, IConsole console) {
      _initialMap = initialMap ?? throw new ArgumentNullException(nameof(initialMap));
      Console = console ?? throw new ArgumentNullException(nameof(console));
      _consoleSize = (Math.Max(80, Console.WindowWidth), Math.Max(24, Console.WindowHeight));
      _colors = new(console);
   }

   internal Map? CurrentMap
      => _maps.Any() ? _maps.Peek() : null;

   internal bool PreserveValues { get; private set; }

   internal static void Push(Map map) {
      int left = 0;
      int top = 0;
      if (_current is not null) {
         if (_current.CurrentMap is Map previous) {
            previous.CursorLastPosition = (_current.Console.CursorLeft, _current.Console.CursorTop);
            previous.WillDeactivate();
         }
         if (map.IsInWindow) {
            if (_current._windowPos == (-1, -1)) {
               left = (_current._consoleSize.Width - map.Width) / 2;
               top = (_current._consoleSize.Height - map.Height) / 2;
            } else {
               (left, top) = _current._windowPos;
               if (_current._border != WindowBorder.None) {
                  left += 1;
                  top += 1;
               }
            }
         }
         map.Top = top;
         map.Left = left;
         map.Application = _current;
         map.WillActivate();
         map.FocusOnNextField();
         _current._maps.Push(map);
         _current._viewDirty = true;
      }
   }

   internal static void Pop(Map map) {
      if (_current is not null && _current.CurrentMap == map) {
         if (map.ShouldClose()) {
            map.WillDeactivate();
            _current._maps.Pop();
            map.DidClose();
            if (_current.CurrentMap is Map previous) {
               (int cleft, int ctop) = previous.CursorLastPosition;
               _current.Console.SetCursorPosition(cleft, ctop);
               previous.WillActivate();
            }
            _current._viewDirty = true;
         }
      }
   }

   internal bool InsertMode { get; private set; } = true;

   internal bool UsesActiveFieldColors
      => _colors.UsesActiveFieldColors;

   internal void DoLoopStep() {
      if (CurrentMap is Map map) {
         HandleIfKeyPressed();
         if (_viewDirty || map.Fields.Any(f => f.IsDirty)) {
            int cleft = Console.CursorLeft;
            int ctop = Console.CursorTop;
            Console.CursorVisible = false;
            RedrawMapIfNeeded();
            RedrawFieldsIfNeeded();
            _viewDirty = false;
            Console.CursorLeft = cleft;
            Console.CursorTop = ctop;
            if (CurrentMap?.Fields.Any() ?? false) {
               map.MoveFocusTo(cleft, ctop);
               Console.CursorVisible = true;
            }
         }
      }
   }

   internal void Start() {
      if (IsRunning) {
         throw new InvalidOperationException();
      }
      ReturnCode = 0;
      _current = this;
      _initCtrlC = Console.TreatControlCAsInput;
      _initCurSize = Console.CursorSize;
      _initWinSize = (Console.WindowWidth, Console.WindowHeight);
      //EnforceTerminalSize();
      _restoreConsole = true;
      Console.TreatControlCAsInput = true;
      _inactiveSince = DateTime.Now;
      _initialMap.Show();
   }

   internal void Stop() {
      foreach (Map map in _maps) {
         map.Application = null;
      }
      _maps.Clear();
      if (_restoreConsole) {
         //RestoreConsoleSize();
         Console.ResetColor();
         Console.Clear();
         Console.CursorSize = _initCurSize;
         Console.CursorVisible = true;
         Console.TreatControlCAsInput = _initCtrlC;
      }
      _restoreConsole = false;
      _current = null;
   }
}
