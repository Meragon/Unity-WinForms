using System.Drawing;

namespace UnityWinForms.Examples.Panels
{
    using System.Windows.Forms;
    
    public class PanelTreeView : BaseExamplePanel
    {
        public override void Initialize()
        {
            var nodesCount = 100000;

            var label = this.Create<Label>();
            var tree = this.Create<TreeView>(); 
            tree.Size = new Size(320, 400);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < nodesCount; i++)
                tree.Nodes.Add("node " + i);
            
            watch.Stop();

            label.Text = nodesCount + " nodes created in " + (int) watch.ElapsedMilliseconds + " ms.";
            
            tree.Refresh();
        }
    }
}