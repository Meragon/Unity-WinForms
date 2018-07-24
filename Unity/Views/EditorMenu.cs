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
            var imgFileDialogBack = images.Find(x => x.name == "filedialog_back");
            var imgFileDialogFile = images.Find(x => x.name == "filedialog_file");
            var imgFileDialogFolder = images.Find(x => x.name == "filedialog_folder");
            var imgFileDialogRefresh = images.Find(x => x.name == "filedialog_refresh");
            var imgFileDialogUp = images.Find(x => x.name == "filedialog_up");
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

                var objResources = obj.Resources;

                if (objResources.Images.ArrowDown == null) objResources.Images.ArrowDown = imgArrowDown;
                if (objResources.Images.ArrowLeft == null) objResources.Images.ArrowLeft = imgArrowLeft;
                if (objResources.Images.ArrowRight == null) objResources.Images.ArrowRight = imgArrowRight;
                if (objResources.Images.ArrowUp == null) objResources.Images.ArrowUp = imgArrowUp;
                if (objResources.Images.Checked == null) objResources.Images.Checked = imgChecked;
                if (objResources.Images.Circle == null) objResources.Images.Circle = imgCircle;
                if (objResources.Images.Close == null) objResources.Images.Close = imgClose;
                if (objResources.Images.Cursors.Hand == null) objResources.Images.Cursors.Hand = imgCursorHand;
                if (objResources.Images.Cursors.Help == null) objResources.Images.Cursors.Help = imgCursorHelp;
                if (objResources.Images.Cursors.HSplit == null) objResources.Images.Cursors.HSplit = imgCursorHSplit;
                if (objResources.Images.Cursors.IBeam == null) objResources.Images.Cursors.IBeam = imgCursorIBeam;
                if (objResources.Images.Cursors.SizeAll == null) objResources.Images.Cursors.SizeAll = imgCursorSizeAll;
                if (objResources.Images.Cursors.SizeNESW == null) objResources.Images.Cursors.SizeNESW = imgCursorSizeNESW;
                if (objResources.Images.Cursors.SizeNS == null) objResources.Images.Cursors.SizeNS = imgCursorSizeNS;
                if (objResources.Images.Cursors.SizeNWSE == null) objResources.Images.Cursors.SizeNWSE = imgCursorSizeNWSE;
                if (objResources.Images.Cursors.SizeWE == null) objResources.Images.Cursors.SizeWE = imgCursorSizeWE;
                if (objResources.Images.Cursors.VSplit == null) objResources.Images.Cursors.VSplit = imgCursorVSplit;
                if (objResources.Images.CurvedArrowDown == null) objResources.Images.CurvedArrowDown = imgCArrowDown;
                if (objResources.Images.CurvedArrowLeft == null) objResources.Images.CurvedArrowLeft = imgCArrowLeft;
                if (objResources.Images.CurvedArrowRight == null) objResources.Images.CurvedArrowRight = imgCArrowRight;
                if (objResources.Images.CurvedArrowUp == null) objResources.Images.CurvedArrowUp = imgCArrowUp;
                if (objResources.Images.DateTimePicker == null) objResources.Images.DateTimePicker = imgDateTimePicker;
                if (objResources.Images.DropDownRightArrow == null) objResources.Images.DropDownRightArrow = imgDDRightArrow;
                if (objResources.Images.FileDialogBack == null) objResources.Images.FileDialogBack = imgFileDialogBack;
                if (objResources.Images.FileDialogFile == null) objResources.Images.FileDialogFile = imgFileDialogFile;
                if (objResources.Images.FileDialogFolder == null) objResources.Images.FileDialogFolder = imgFileDialogFolder;
                if (objResources.Images.FileDialogRefresh == null) objResources.Images.FileDialogRefresh = imgFileDialogRefresh;
                if (objResources.Images.FileDialogUp == null) objResources.Images.FileDialogUp = imgFileDialogUp;
                if (objResources.Images.FormResize == null) objResources.Images.FormResize = imgFormResize;
                if (objResources.Images.NumericDown == null) objResources.Images.NumericDown = imgNumericDown;
                if (objResources.Images.NumericUp == null) objResources.Images.NumericUp = imgNumericUp;
                if (objResources.Images.RadioButton_Checked == null) objResources.Images.RadioButton_Checked = imgRadioChecked;
                if (objResources.Images.RadioButton_Hovered == null) objResources.Images.RadioButton_Hovered = imgRadioHovered;
                if (objResources.Images.RadioButton_Unchecked == null) objResources.Images.RadioButton_Unchecked = imgRadioUnchecked;
                if (objResources.Images.TreeNodeCollapsed == null) objResources.Images.TreeNodeCollapsed = imgTreeCollapsed;
                if (objResources.Images.TreeNodeExpanded == null) objResources.Images.TreeNodeExpanded = imgTreeExpanded;
            }

            Debug.Log("UWF: Resources validated.");
        }
    }
}
#endif