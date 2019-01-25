using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace _3DRaceTrackConverterFrontend
{
  public partial class Form1 : Form
  {
    string trackPath = "";
    string gradientPath = "";

    string delim = ",";

    bool isMouseDown_preview = false;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      btn_browseImage.Focus();
    }

    private void btn_browseImage_Click(object sender, EventArgs e)
    {
      pictureBox_preview.Enabled = false;

      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result != DialogResult.OK) // Test result.
      {
        return;
      }

      trackPath = openFileDialog1.FileName;
      try
      {
        pictureBox_main.ImageLocation = trackPath;
        pictureBox_preview.ImageLocation = trackPath;
        lbl_image.Text = trackPath;
        pictureBox_preview.Enabled = true;
      }
      catch (Exception)
      {
        MessageBox.Show("Error opening file.");
      }
    }

    private void RecenterPictureBox_Main(MouseEventArgs e)
    {
      Point cursorPos = new Point(e.X, e.Y);

      int pos_x = -1 * (int)(pictureBox_main.Image.Width * ((double)cursorPos.X / pictureBox_preview.Width));
      int pos_y = -1 * (int)(pictureBox_main.Image.Height * ((double)cursorPos.Y / pictureBox_preview.Height));

      pictureBox_main.Location = new Point(pos_x, pos_y);
    }

    private void pictureBox_preview_MouseDown(object sender, MouseEventArgs e)
    {
      isMouseDown_preview = true;
      RecenterPictureBox_Main(e);
    }

    private void pictureBox_preview_MouseUp(object sender, MouseEventArgs e)
    {
      isMouseDown_preview = false;
    }

    private void pictureBox_preview_MouseMove(object sender, MouseEventArgs e)
    {
      if (isMouseDown_preview == false)
      {
        return;
      }

      RecenterPictureBox_Main(e);
    }

    private void btn_gradient_Click(object sender, EventArgs e)
    {
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result != DialogResult.OK) // Test result.
      {
        return;
      }

      gradientPath = openFileDialog1.FileName;
      try
      {
        pictureBox_gradient.ImageLocation = gradientPath;
        lbl_gradient.Text = gradientPath;
      }
      catch (Exception)
      {
        MessageBox.Show("Error opening file.");
      }
    }

    string GetArgsString()
    {
      string s = "";

      s += trackPath + delim;

      s += m1x1.Text + delim;
      s += m1y1.Text + delim;
      s += m1x2.Text + delim;
      s += m1y2.Text + delim;
      s += m1dx.Text + delim;
      s += m1dy.Text + delim;

      s += m2x1.Text + delim;
      s += m2y1.Text + delim;
      s += m2x2.Text + delim;
      s += m2y2.Text + delim;
      s += m2dx.Text + delim;
      s += m2dy.Text + delim;

      return s;
    }

    bool AllFieldsFilled()
    {
      return 
        !(
          trackPath.Length == 0 ||
          gradientPath.Length == 0 ||

          m1x1.Text.Length == 0 ||
          m1y1.Text.Length == 0 ||
          m1x2.Text.Length == 0 ||
          m1y2.Text.Length == 0 ||
          m1dx.Text.Length == 0 ||
          m1dy.Text.Length == 0 ||

          m2x1.Text.Length == 0 ||
          m2y1.Text.Length == 0 ||
          m2x2.Text.Length == 0 ||
          m2y2.Text.Length == 0 ||
          m2dx.Text.Length == 0 ||
          m2dy.Text.Length == 0
        );
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (AllFieldsFilled() == false)
      {
        MessageBox.Show("Not all fields valid.");
        return;
      }

      Process process = new Process();
      process.StartInfo.FileName = "TrackEdgeDetector.exe";
      process.StartInfo.Arguments = GetArgsString(); ;
      process.Start();
      process.WaitForExit();
      int result = process.ExitCode;
    }

    private void coordinate_Leave(object sender, EventArgs e)
    {
      TextBox t = null;
      try
      {
        t = ((TextBox)sender);
      }
      catch (Exception)
      {
        return;
      }

      int amount = 0;
      if (!int.TryParse(t.Text, NumberStyles.Integer, null, out amount))
      {
        t.Text = "";
      }
    }
  }
}
