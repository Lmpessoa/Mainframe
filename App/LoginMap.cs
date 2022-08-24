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

namespace Lmpessoa.Mainframe.Test;

internal class LoginMap : Map {

   private const int LOGIN = 0;
   private const int PASSWORD = 1;
   private const int SYS_NAME = 2;
   private const int NEW_PASSWORD = 3;
   private const int CURR_DATE = 4;
   private const int CURR_TIME = 5;

   public LoginMap()
      => Fields.Move(2, 3);

   protected override void OnActivating() {
      CurrentDate = DateOnly.FromDateTime(DateTime.Now);
      CurrentTime = TimeOnly.FromDateTime(DateTime.Now);
   }

   [CommandKey(ConsoleKey.F1)]
   public void ShowHelp()
      => SetError("NÃO EXISTE AJUDA DISPONÍVEL NESTA TELA");

   [CommandKey(ConsoleKey.F3)]
   public void ConfirmClose()
      => new ModalMap().ShowWindow();

   public string Login {
      get => ((TextField) Fields[LOGIN]).Value;
      set => ((TextField) Fields[LOGIN]).Value = value;
   }

   public string Password {
      get => ((TextField) Fields[PASSWORD]).Value;
      set => ((TextField) Fields[PASSWORD]).Value = value;
   }

   public string SystemName {
      get => ((TextField) Fields[SYS_NAME]).Value;
      set => ((TextField) Fields[SYS_NAME]).Value = value;
   }

   public string NewPassword {
      get => ((TextField) Fields[NEW_PASSWORD]).Value;
      set => ((TextField) Fields[NEW_PASSWORD]).Value = value;
   }

   public DateOnly CurrentDate {
      get => DateOnly.ParseExact(((Label) Fields[CURR_DATE]).Value ?? "01/01/1970", "dd/MM/yyyy");
      set => ((Label) Fields[CURR_DATE]).Value = value.ToString("dd/MM/yyyy");
   }

   public TimeOnly CurrentTime {
      get => TimeOnly.ParseExact(((Label) Fields[CURR_TIME]).Value ?? "00:00:00", "HH:mm:ss");
      set => ((Label) Fields[CURR_TIME]).Value = value.ToString("HH:mm:ss");
   }
}
