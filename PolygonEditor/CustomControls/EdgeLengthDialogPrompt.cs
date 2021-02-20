using PolygonEditor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditor.CustomControls
{
    public static class EdgeLengthDialogPrompt
    {

        public static string ShowDialog(Form parent, string text, string caption, double x, double y, double edgeLength)
        {
            Form prompt = new Form()
            {
              //  Parent = parent,
                Width = 160,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = caption,
                Left = (int)x,
                Top = (int)y,
                StartPosition = FormStartPosition.Manual,
            };
            Label textLabel = new Label() { Left = 10, Top = 10, Text = text };
            TextBox textBox = new TextBox() { Left = 10, Top = 30, Width = 100, Text = edgeLength.ToString()/*Math.Round(edgeLength, 2).ToString()*/};
            Button okButton = new Button() { Left = textBox.Left + textBox.Width + 5, Top = textBox.Top, Width = 20, Height = textBox.Height, 
                BackgroundImage = Resources.checkmark, BackgroundImageLayout = ImageLayout.Stretch};
            okButton.Click += (sender, e) =>
            {
                prompt.Close();
            };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(okButton);
            prompt.AcceptButton = okButton;
            prompt.ShowDialog();
            return IsInputValid(textBox.Text) ? textBox.Text.Replace(',', '.') : null;
        }

        private static bool IsInputValid(string text)
        {
            if (text == null || text == "") return false;
            return !text.Any(c => !(char.IsNumber(c) || c == '.' || c == ','));
        }
    }
}
