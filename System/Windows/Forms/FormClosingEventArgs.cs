using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public class FormClosingEventArgs : CancelEventArgs
    {
        public FormClosingEventArgs(CloseReason closeReason, bool cancel)
        {
            _closeReason = closeReason;
        }

        private CloseReason _closeReason;
        public CloseReason CloseReason { get { return _closeReason; } }
    }

    public enum CloseReason
    {
        // Сводка:
        //     Причина закрытия не была определена или не может быть определена.
        None = 0,
        //
        // Сводка:
        //     Операционная система закрывает все приложения перед завершением работы.
        WindowsShutDown = 1,
        //
        // Сводка:
        //     Родительская форма этой формы многодокументного интерфейса (MDI) закрывается.
        MdiFormClosing = 2,
        //
        // Сводка:
        //     Пользователь закрывает форму, используя пользовательский интерфейс, например
        //     нажатием кнопки Закрыть в окне формы, выбором параметра Закрыть в системном
        //     меню окна или нажатием сочетания клавиш ALT+F4.
        UserClosing = 3,
        //
        // Сводка:
        //     Диспетчер задач Microsoft Windows закрывает приложение.
        TaskManagerClosing = 4,
        //
        // Сводка:
        //     Форма-владелец закрывается.
        FormOwnerClosing = 5,
        //
        // Сводка:
        //     Был вызван метод System.Windows.Forms.Application.Exit() класса System.Windows.Forms.Application.
        ApplicationExitCall = 6,
    }
}
