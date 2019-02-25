# Unity Windows Forms

This is WinForms wrapper (including System.Drawing). Here you can create base controls and make your own custom controls. The reason why I made it? Because now I can quickly create GUI through the code. But, yeah, right now there is a poor implementation of interaction with the editor.

### Controls atm
- Button;
- CheckBox;
- ComboBox;
- DateTimePicker;
- FontDialog;
- Form;
- GroupBox;
- HScrollBar;
- Label;
- LinkLabel;
- ListBox;
- MenuStrip;
- MonthCalendar;
- NumericUpDown
- OpenFileDialog;
- Panel;
- PictureBox;
- ProgressBar;
- RadioButton;
- SaveFileDialog;
- SplitContainer;
- TabControl;
- TextBox;
- Timer;
- ToolStrip;
- ToolTip;
- TrackBar;
- TreeView;
- VScrollBar;

Other controls: 
- BitmapLabel;
- ColorPicker (replacement for ColorDialog);
- Highchart (replacement for Chart);
- RepeatButton;
- TableView (replacement for DataGridView);

### Usage

1. Attach **UnityWinForms** script to **GameObject**;
2. Add **Arial** font to resources;
3. Add other fonts and images; 
4. Create **Form** control in your **MonoBehaviour** script;
```sh
public class GameGuiController : MonoBehaviour
{
	void Start()
	{
		var form = new Form();
		form.Show();
		
		// Or show a message.
		// 		MessageBox.Show("Hello World.");
	}
}
```

### Screenshots
![scr1](https://i.imgur.com/z7ol9jq.png)
![scr1](https://i.imgur.com/oifeDMo.png) 

### From other projects using Unity-WinForms
![scr1](https://i.imgur.com/YJ3Y5BD.png)
![scr1](http://i.imgur.com/nZUFZCe.png)
