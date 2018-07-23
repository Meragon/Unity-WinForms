#if UNITY_EDITOR
namespace Unity.Views
{
    using System.Collections.Generic;
    using System.Linq;

    using Unity.API;

    using UnityEditor;

    using UnityEngine;

    public static class EditorMenu
    {
        [MenuItem("Window/UnityWinForms/AppControlList")]
        public static void ShowControlList()
        {
            EditorWindow.GetWindow(typeof(AppControlList), false, "AppControlList");
        }

        [MenuItem("Window/UnityWinForms/SWF Inspector")]
        public static EditorWindow ShowInspector()
        {
            return EditorWindow.GetWindow(typeof(ControlInspector), false, "SWF Inspector");
        }

        [MenuItem("Window/UnityWinForms/Validate resources")]
        public static void ValidateResources()
        {
            var uwfObjects = Resources.FindObjectsOfTypeAll<UnityWinForms>();
            var fonts = Resources.FindObjectsOfTypeAll<Font>();
            var images = Resources.LoadAll<Texture2D>("").ToList();

            var imgArrowDown = images.Find(x => x.name == "arrow_down");
            var imgArrowLeft = images.Find(x => x.name == "arrow_left");
            var imgArrowRight = images.Find(x => x.name == "arrow_right");
            var imgArrowUp = images.Find(x => x.name == "arrow_up");
            var imgChecked = images.Find(x => x.name == "checked");
            var imgCircle = images.Find(x => x.name == "circle");
            var imgClose = images.Find(x => x.name == "close");

            var imgCursorHand = images.Find(x => x.name == "hand");
            var imgCursorHelp = images.Find(x => x.name == "help");
            var imgCursorHSplit = images.Find(x => x.name == "hsplit");
            var imgCursorIBeam = images.Find(x => x.name == "ibeam");
            var imgCursorSizeAll = images.Find(x => x.name == "sizeall");
            var imgCursorSizeNESW = images.Find(x => x.name == "sizenesw");
            var imgCursorSizeNS = images.Find(x => x.name == "sizens");
            var imgCursorSizeNWSE = images.Find(x => x.name == "sizenwse");
            var imgCursorSizeWE = images.Find(x => x.name == "sizewe");
            var imgCursorVSplit = images.Find(x => x.name == "vsplit");

            var imgCArrowDown = images.Find(x => x.name == "curved_arrow_down");
            var imgCArrowLeft = images.Find(x => x.name == "curved_arrow_left");
            var imgCArrowRight = images.Find(x => x.name == "curved_arrow_right");
            var imgCArrowUp = images.Find(x => x.name == "curved_arrow_up");
            var imgDateTimePicker = images.Find(x => x.name == "datetimepicker");
            var imgDDRightArrow = images.Find(x => x.name == "dropdown_rightarrow");
            var imgFormResize = images.Find(x => x.name == "form_resize");
            var imgNumericDown = images.Find(x => x.name == "numeric_down");
            var imgNumericUp = images.Find(x => x.name == "numeric_up");
            var imgRadioChecked = images.Find(x => x.name == "radioButton_checked");
            var imgRadioHovered = images.Find(x => x.name == "radioButton_hovered");
            var imgRadioUnchecked = images.Find(x => x.name == "radioButton_unchecked");
            var imgTreeCollapsed = images.Find(x => x.name == "treenode_collapsed");
            var imgTreeExpanded = images.Find(x => x.name == "treenode_expanded");

            for (int i = 0; i < uwfObjects.Length; i++)
            {
                var obj = uwfObjects[i];
                if (obj.Resources.Fonts == null)
                    obj.Resources.Fonts = new List<Font>();
                if (obj.Resources.Fonts.Count == 0 && fonts != null && fonts.Length > 0)
                {
                    foreach (var font in fonts)
                        obj.Resources.Fonts.Add(font);
                }

                if (obj.Resources.Images.ArrowDown == null) obj.Resources.Images.ArrowDown = imgArrowDown;
                if (obj.Resources.Images.ArrowLeft == null) obj.Resources.Images.ArrowLeft = imgArrowLeft;
                if (obj.Resources.Images.ArrowRight == null) obj.Resources.Images.ArrowRight = imgArrowRight;
                if (obj.Resources.Images.ArrowUp == null) obj.Resources.Images.ArrowUp = imgArrowUp;
                if (obj.Resources.Images.Checked == null) obj.Resources.Images.Checked = imgChecked;
                if (obj.Resources.Images.Circle == null) obj.Resources.Images.Circle = imgCircle;
                if (obj.Resources.Images.Close == null) obj.Resources.Images.Close = imgClose;
                if (obj.Resources.Images.Cursors.Hand == null) obj.Resources.Images.Cursors.Hand = imgCursorHand;
                if (obj.Resources.Images.Cursors.Help == null) obj.Resources.Images.Cursors.Help = imgCursorHelp;
                if (obj.Resources.Images.Cursors.HSplit == null) obj.Resources.Images.Cursors.HSplit = imgCursorHSplit;
                if (obj.Resources.Images.Cursors.IBeam == null) obj.Resources.Images.Cursors.IBeam = imgCursorIBeam;
                if (obj.Resources.Images.Cursors.SizeAll == null) obj.Resources.Images.Cursors.SizeAll = imgCursorSizeAll;
                if (obj.Resources.Images.Cursors.SizeNESW == null) obj.Resources.Images.Cursors.SizeNESW = imgCursorSizeNESW;
                if (obj.Resources.Images.Cursors.SizeNS == null) obj.Resources.Images.Cursors.SizeNS = imgCursorSizeNS;
                if (obj.Resources.Images.Cursors.SizeNWSE == null) obj.Resources.Images.Cursors.SizeNWSE = imgCursorSizeNWSE;
                if (obj.Resources.Images.Cursors.SizeWE == null) obj.Resources.Images.Cursors.SizeWE = imgCursorSizeWE;
                if (obj.Resources.Images.Cursors.VSplit == null) obj.Resources.Images.Cursors.VSplit = imgCursorVSplit;
                if (obj.Resources.Images.CurvedArrowDown == null) obj.Resources.Images.CurvedArrowDown = imgCArrowDown;
                if (obj.Resources.Images.CurvedArrowLeft == null) obj.Resources.Images.CurvedArrowLeft = imgCArrowLeft;
                if (obj.Resources.Images.CurvedArrowRight == null) obj.Resources.Images.CurvedArrowRight = imgCArrowRight;
                if (obj.Resources.Images.CurvedArrowUp == null) obj.Resources.Images.CurvedArrowUp = imgCArrowUp;
                if (obj.Resources.Images.DateTimePicker == null) obj.Resources.Images.DateTimePicker = imgDateTimePicker;
                if (obj.Resources.Images.DropDownRightArrow == null) obj.Resources.Images.DropDownRightArrow = imgDDRightArrow;
                if (obj.Resources.Images.FormResize == null) obj.Resources.Images.FormResize = imgFormResize;
                if (obj.Resources.Images.NumericDown == null) obj.Resources.Images.NumericDown = imgNumericDown;
                if (obj.Resources.Images.NumericUp == null) obj.Resources.Images.NumericUp = imgNumericUp;
                if (obj.Resources.Images.RadioButton_Checked == null) obj.Resources.Images.RadioButton_Checked = imgRadioChecked;
                if (obj.Resources.Images.RadioButton_Hovered == null) obj.Resources.Images.RadioButton_Hovered = imgRadioHovered;
                if (obj.Resources.Images.RadioButton_Unchecked == null) obj.Resources.Images.RadioButton_Unchecked = imgRadioUnchecked;
                if (obj.Resources.Images.TreeNodeCollapsed == null) obj.Resources.Images.TreeNodeCollapsed = imgTreeCollapsed;
                if (obj.Resources.Images.TreeNodeExpanded == null) obj.Resources.Images.TreeNodeExpanded = imgTreeExpanded;
            }

            Debug.Log("UWF: Resources validated.");
        }
    }
}
#endif