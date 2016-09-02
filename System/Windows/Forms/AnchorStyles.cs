using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    [Flags]
    public enum AnchorStyles
    {
        // Сводка:
        //     Элемент управления, не привязанный к краям своего контейнера.
        None = 0,
        //
        // Сводка:
        //     Элемент управления, привязанный к верхнему краю своего контейнера.
        Top = 1,
        //
        // Сводка:
        //     Элемент управления, привязанный к нижнему краю своего контейнера.
        Bottom = 2,
        //
        // Сводка:
        //     Элемент управления, привязанный к левому краю своего контейнера.
        Left = 4,
        //
        // Сводка:
        //     Элемент управления, привязанный к правому краю своего контейнера.
        Right = 8,

        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
        All = Top | Bottom | Left | Right
    }
}
