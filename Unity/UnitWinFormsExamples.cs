namespace UnityWinForms
{
    using UnityEngine;

    using UnityWinForms.Examples;

    public class UnitWinFormsExamples : MonoBehaviour
    {
        public static Material s_chartGradient;
        public Material ChartGradient;
        
        private void Start()
        {
            s_chartGradient = ChartGradient;
            
            var form = new FormExamples();

            form.Show();
        }
    }
}