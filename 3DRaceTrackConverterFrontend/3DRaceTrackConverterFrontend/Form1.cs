using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

//TODO
//AL.
//SET THE PIXELS WHEN CHANGING IN TEXTBOXES AND UPDATE VIEWS
//MAKE VIEWS MORE STABLE

namespace _3DRaceTrackConverterFrontend
{
  public partial class Form1 : Form
  {
    string trackPath = "";
    string gradientPath = "";

    string delim = " ";
    const string newline = "\r\n";

    bool isMouseDown_preview = false;

    List<string> config = new List<string>();

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      btn_browseImage.Focus();
      lbl_image.Text = lbl_gradient.Text = "";
    }

    private void btn_browseImage_Click(object sender, EventArgs e)
    {
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result != DialogResult.OK) // Test result.
      {
        return;
      }

      pictureBox_preview.Enabled = false;
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
        MessageBox.Show("Error opening image file.");
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

      s += m2x1.Text + delim;
      s += m2y1.Text + delim;
      s += m2x2.Text + delim;
      s += m2y2.Text + delim;

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

          m2x1.Text.Length == 0 ||
          m2y1.Text.Length == 0 ||
          m2x2.Text.Length == 0 ||
          m2y2.Text.Length == 0 
        );
    }

    bool PathsContainSpaces()
    {
      return (trackPath.Contains(" ") || gradientPath.Contains(" "));
    }

    private void btn_go_Click(object sender, EventArgs e)
    {
      if (AllFieldsFilled() == false)
      {
        MessageBox.Show("Not all fields valid.");
        return;
      }

      if (PathsContainSpaces() == true)
      {
        MessageBox.Show("Paths contain spaces, which are not allowed.");
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

    private void btn_load_Click(object sender, EventArgs e)
    {
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result != DialogResult.OK) // Test result.
      {
        return;
      }

      string chunk = File.ReadAllText(openFileDialog1.FileName);
      List<string> config_muddy = chunk.Split('\r').ToList();
      config.RemoveRange(0, config.Count);
      foreach (var s in config_muddy)
      {
        string n = s.Replace("\n","");
        config.Add(n);
      }

      int i = 0;

      pictureBox_preview.Enabled = false;
      trackPath = config[i++];
      try
      {
        pictureBox_main.ImageLocation = trackPath;
        pictureBox_preview.ImageLocation = trackPath;
        lbl_image.Text = trackPath;
        pictureBox_preview.Enabled = true;
      }
      catch (Exception)
      {
        MessageBox.Show("Error opening track image file.");
      }

      gradientPath = config[i++];
      try
      {
        pictureBox_gradient.ImageLocation = gradientPath;
        lbl_gradient.Text = gradientPath;
      }
      catch (Exception)
      {
        MessageBox.Show("Error opening gradient image file.");
      }

      m1x1.Text = config[i++];
      m1y1.Text = config[i++];
      m1x2.Text = config[i++];
      m1y2.Text = config[i++];


      m2x1.Text = config[i++];
      m2y1.Text = config[i++];
      m2x2.Text = config[i++];
      m2y2.Text = config[i++];
    }

    private void btn_save_Click(object sender, EventArgs e)
    {
      SaveFileDialog save = new SaveFileDialog();

      save.FileName = "config.txt";

      save.Filter = "Text File | *.txt";

      if (save.ShowDialog() == DialogResult.OK)
      {
        StreamWriter writer = new StreamWriter(save.OpenFile());

        for (int i = 0; i < config.Count; i++)
        {
          writer.WriteLine(config[i].ToString());
        }
        writer.Dispose();
        writer.Close();
      }
    }
  }
}
