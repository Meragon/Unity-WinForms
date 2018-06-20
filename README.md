# Unity Windows Forms

This is WinForms wrapper (including System.Drawing). Here you can create base controls and make your own custom controls. The reason why I made it? Because now I can quickly create GUI through the code. But, yeah, right now there is a poor implementation of interaction with the editor.

### Controls atm
- Button;
- CheckBox;
- ComboBox;
- DateTimePicker;
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
- TabControl;
- TextBox;
- Timer;
- ToolStrip;
- ToolTip;
- TreeView;
- VScrollBar;

Other controls: 
- BitmapLabel;
- ColorPicker;
- RepeatButton;
- TableView;

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
![scr1](http://i.imgur.com/LCQsFgv.png)

### From other projects using Unity-WinForms
![scr1](http://i.imgur.com/njQZbCP.png)
![scr1](http://i.imgur.com/I9H0AWt.png)
![scr1](http://i.imgur.com/nZUFZCe.png)
![scr1](http://i.imgur.com/GpiWviP.png)
