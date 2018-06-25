namespace UnityWinForms
{
    using UnityEngine;

    using UnityWinForms.Examples;

    public class UnitWinFormsExamples : MonoBehaviour
    {
        private void Start()
        {
            var form = new FormExamples();

            form.Show();
        }
    }
}