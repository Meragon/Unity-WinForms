using System.Globalization;

namespace System.Windows.Forms
{
    using System.Drawing;
    
    public class FontDialog : Form
    {
        // Localization text.
        internal static string   locFormText            = "Font";
        internal static string   locFontName            = "Font:";
        internal static string   locFontStyle           = "Font style:";
        internal static string[] locFontStyles          = {"Regular", "Italic", "Bold", "Bold Italic"};
        internal static string   locFontSize            = "Size:";
        internal static string   locFontEffectStrikeout = "Strikeout";
        internal static string   locFontEffectUnderline = "Underline";
        internal static string   locFontEffects         = "Effects";
        internal static string   locFontSample          = "Sample";
        internal static string   locFontScript          = "Script:";
        internal static string[] locFontScripts         = {"Western"};
        internal static string   locOk                  = "OK";
        internal static string   locCancel              = "Cancel";
        
        private string    fontName  = "Arial";
        private FontStyle fontStyle = FontStyle.Regular;
        private float     fontSize  = 12;
        private bool      initialized;

        private readonly Button   buttonCancel;
        private readonly Button   buttonOk;
        private readonly CheckBox checkStrikeout;
        private readonly CheckBox checkUnderline;
        private readonly ComboBox comboScript;
        private readonly Label    labelSample;
        private readonly ListBox  listFontName;
        private readonly ListBox  listFontSize;
        private readonly ListBox  listFontStyle;
        private readonly TextBox  textFontName;
        private readonly TextBox  textFontSize;
        private readonly TextBox  textFontStyle;
        
        public FontDialog()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Size = new Size(432, 344);
            Text = locFormText;
            
            // Font name.
            var labelFontName = new Label();
            labelFontName.Location = new Point(11, 32);
            labelFontName.Text = locFontName;
            
            textFontName = new TextBox();
            textFontName.Location = new Point(11, 50);
            textFontName.Size = new Size(146, 20);
            textFontName.TabIndex = 0;
            
            listFontName = new ListBox();
            listFontName.Location = new Point(textFontName.Location.X, textFontName.Location.Y + textFontName.Height);
            listFontName.IntegralHeight = false;
            listFontName.Height = 99;
            listFontName.Width = textFontName.Width;
            listFontName.TabIndex = 3;
            listFontName.SelectedIndexChanged += ListFontNameOnSelectedIndexChanged;
            
            if (ApplicationResources.Fonts != null)
                for (int i = 0; i < ApplicationResources.Fonts.Count; i++)
                    listFontName.Items.Add(ApplicationResources.Fonts[i]);
            
            // Font style.
            var labelFontStyle = new Label();
            labelFontStyle.Location = new Point(165, 32);
            labelFontStyle.Text = locFontStyle;
            
            textFontStyle = new TextBox();
            textFontStyle.Location = new Point(165, 50);
            textFontStyle.Size = new Size(111, 20);
            textFontStyle.TabIndex = 1;
            
            listFontStyle = new ListBox();
            listFontStyle.Location = new Point(textFontStyle.Location.X, textFontStyle.Location.Y + textFontStyle.Height);
            listFontStyle.IntegralHeight = false;
            listFontStyle.Height = listFontName.Height;
            listFontStyle.Width = textFontStyle.Width;
            listFontStyle.Items.AddRange(locFontStyles);
            listFontStyle.SelectedIndex = 0;
            listFontStyle.TabIndex = 4;
            listFontStyle.SelectedIndexChanged += ListFontStyleOnSelectedIndexChanged;
            
            // Font size.
            var labelFontSize = new Label();
            labelFontSize.Location = new Point(285, 32);
            labelFontSize.Text = locFontSize;
            
            textFontSize = new TextBox();
            textFontSize.Location = new Point(285, 50);
            textFontSize.Size = new Size(54, 20);
            textFontSize.TabIndex = 2;
            
            listFontSize = new ListBox();
            listFontSize.Location = new Point(textFontSize.Location.X, textFontSize.Location.Y + textFontSize.Height);
            listFontSize.IntegralHeight = false;
            listFontSize.Height = 95;
            listFontSize.Width = textFontSize.Width;
            listFontSize.TabIndex = 5;
            listFontSize.Items.AddRange(new object[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 });
            listFontSize.SelectedIndexChanged += ListFontSizeOnSelectedIndexChanged;
            
            // Dialog buttons.
            buttonOk = new Button();
            buttonOk.Location = new Point(348, 51);
            buttonOk.Size = new Size(66, 21);
            buttonOk.TabIndex = 9;
            buttonOk.Text = locOk;
            buttonOk.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
            
            buttonCancel = new Button();
            buttonCancel.Location = new Point(348, 77);
            buttonCancel.Size = new Size(66, 21);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = locCancel;
            buttonCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            
            // Effects [Not supported].
            var groupEffects = new GroupBox();
            groupEffects.Location = new Point(11, 180);
            groupEffects.Size = new Size(146, 119);
            groupEffects.Text = locFontEffects;
            
            checkStrikeout = new CheckBox();
            checkStrikeout.Enabled = false;
            checkStrikeout.Location = new Point(10, 20);
            checkStrikeout.TabIndex = 6;
            checkStrikeout.Text = locFontEffectStrikeout;
            
            checkUnderline = new CheckBox();
            checkUnderline.Enabled = false;
            checkUnderline.Location = new Point(10, 41);
            checkUnderline.TabIndex = 7;
            checkUnderline.Text = locFontEffectUnderline;
            
            groupEffects.Controls.Add(checkStrikeout);
            groupEffects.Controls.Add(checkUnderline);
            
            // Sample.
            var groupSample = new GroupBox();
            groupSample.Location = new Point(165, 180);
            groupSample.Size = new Size(173, 72);
            groupSample.Text = locFontSample;
            
            labelSample = new Label();
            labelSample.AutoSize = false;
            labelSample.ForeColor = SystemColors.ControlText;
            labelSample.Location = new Point(0, 16);
            labelSample.Height = groupSample.Height - 16;
            labelSample.Width = groupSample.Width;
            labelSample.Text = "AaBbYyZz";
            labelSample.TextAlign = ContentAlignment.MiddleCenter;
            
            groupSample.Controls.Add(labelSample);
            
            // Script.
            var labelScript = new Label();
            labelScript.Location = new Point(165, 261);
            labelScript.Text = locFontScript;
            
            comboScript = new ComboBox();
            comboScript.Location = new Point(165, 279);
            comboScript.DropDownStyle = ComboBoxStyle.DropDownList;
            comboScript.Size = new Size(174, 20);
            comboScript.Items.AddRange(locFontScripts);
            comboScript.SelectedIndex = 0;
            comboScript.TabIndex = 8;
            
            Controls.Add(labelFontName);
            Controls.Add(textFontName);
            Controls.Add(listFontName);
            Controls.Add(labelFontStyle);
            Controls.Add(textFontStyle);
            Controls.Add(listFontStyle);
            Controls.Add(labelFontSize);
            Controls.Add(textFontSize);
            Controls.Add(listFontSize);
            Controls.Add(buttonOk);
            Controls.Add(buttonCancel);
            Controls.Add(groupEffects);
            Controls.Add(groupSample);
            Controls.Add(labelScript);
            Controls.Add(comboScript);

            AcceptButton = buttonOk;
            initialized = true;
            
            UpdateControls();
        }

        public Color Color
        {
            get { return labelSample.ForeColor; }
            set { labelSample.ForeColor = value; }
        }
        public override Font Font
        {
            get { return new Font(fontName, fontSize, fontStyle); }
            set
            {
                fontName = value.Name;
                fontStyle = value.Style;
                fontSize = value.Size;
                
                UpdateControls();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private static int FindIndex(ListBox list, string item)
        {
            for (int i = 0; i < list.Items.Count; i++)
                if (list.Items[i].ToString() == item)
                    return i;
            return 0;
        }
        private void ListFontNameOnSelectedIndexChanged(object sender, EventArgs e)
        {
            fontName = listFontName.SelectedItem.ToString();
            
            UpdateControls();
        }
        private void ListFontSizeOnSelectedIndexChanged(object sender, EventArgs e)
        {
            float.TryParse(listFontSize.SelectedItem.ToString(), out fontSize);
            
            UpdateControls();
        }
        private void ListFontStyleOnSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listFontStyle.SelectedIndex)
            {
                case 0:
                    fontStyle = FontStyle.Regular;
                    break;
                case 1:
                    fontStyle = FontStyle.Italic;
                    break;
                case 2:
                    fontStyle = FontStyle.Bold;
                    break;
                case 3:
                    fontStyle = FontStyle.Italic | FontStyle.Bold;
                    break;
            }
            
            UpdateControls();
        }
        private void UpdateControls()
        {
            if (!initialized)
                return;
            
            textFontName.Text = fontName;
            textFontSize.Text = fontSize.ToString(Application.CurrentCulture);
            textFontStyle.Text = listFontStyle.SelectedItem.ToString();
            labelSample.Font = Font;
            
            listFontName.SelectedIndexChanged -= ListFontNameOnSelectedIndexChanged;
            listFontName.SelectedIndex = FindIndex(listFontName, textFontName.Text);
            listFontName.SelectedIndexChanged += ListFontNameOnSelectedIndexChanged;
            
            listFontSize.SelectedIndexChanged -= ListFontSizeOnSelectedIndexChanged;
            listFontSize.SelectedIndex = FindIndex(listFontSize, textFontSize.Text);
            listFontSize.SelectedIndexChanged += ListFontSizeOnSelectedIndexChanged;
            
            listFontStyle.SelectedIndexChanged -= ListFontStyleOnSelectedIndexChanged;
            switch (fontStyle)
            {
                case FontStyle.Regular:
                    listFontStyle.SelectedIndex = 0;
                    break;
                case FontStyle.Italic:
                    listFontStyle.SelectedIndex = 1;
                    break;
                case FontStyle.Bold:
                    listFontStyle.SelectedIndex = 2;
                    break;
                case FontStyle.Italic | FontStyle.Bold:
                    listFontStyle.SelectedIndex = 3;
                    break;
            }
            listFontStyle.SelectedIndexChanged += ListFontStyleOnSelectedIndexChanged;
        }
    }
}