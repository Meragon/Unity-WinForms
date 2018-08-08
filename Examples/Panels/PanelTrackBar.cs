namespace UnityWinForms.Examples.Panels
{
    using System.Drawing;
    using System.Windows.Forms;
    
    public class PanelTrackBar : BaseExamplePanel
    {
        public override void Initialize()
        {
            var label = this.Create<Label>("Value: ");
            var track = this.Create<TrackBar>();
            track.Orientation = Orientation.Vertical;
            track.TickStyle = TickStyle.Both;
            track.ValueChanged += (sender, args) => label.Text = "Value: "+ track.Value;
        }
    }
}