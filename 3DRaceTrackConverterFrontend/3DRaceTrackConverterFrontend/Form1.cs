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

    int[] reds = new int[8];

    int offset_x = 0;
    int offset_y = 0;

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

      offset_x = (int)(0.5 * panel2.Width);
      offset_y = (int)(0.5 * panel2.Height);
    }

    void PopulatePictureBoxes()
    {
      pictureBox_main.ImageLocation = trackPath;
      pictureBox_preview.ImageLocation = trackPath;
      pictureBox_main.Refresh();
      pictureBox_preview.Refresh();
    }

    private void btn_browseImage_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "";
      DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
      if (result != DialogResult.OK) // Test result.
      {
        return;
      }

      pictureBox_preview.Enabled = false;
      trackPath = openFileDialog1.FileName;
      try
      {
        PopulatePictureBoxes();
        lbl_image.Text = trackPath;
        pictureBox_preview.Enabled = true;
        DrawMasks();
        pictureBox_main.Location = new Point(0, 0);
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

      pictureBox_main.Location = new Point(pos_x + offset_x, pos_y + offset_y);
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
      openFileDialog1.Filter = "";

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

    void SetReds()
    {
      int r = -1;

      int.TryParse(m1x1.Text, NumberStyles.Integer, null, out reds[++r]);
      int.TryParse(m1y1.Text, NumberStyles.Integer, null, out reds[++r]);
                                                                     
      int.TryParse(m1x2.Text, NumberStyles.Integer, null, out reds[++r]);
      int.TryParse(m1y2.Text, NumberStyles.Integer, null, out reds[++r]);
                                                                     
      int.TryParse(m2x1.Text, NumberStyles.Integer, null, out reds[++r]);
      int.TryParse(m2y1.Text, NumberStyles.Integer, null, out reds[++r]);
                                                                     
      int.TryParse(m2x2.Text, NumberStyles.Integer, null, out reds[++r]);
      int.TryParse(m2y2.Text, NumberStyles.Integer, null, out reds[++r]);

      bool warnings = false;

      for (int i = 0; i < reds.Length; ++i)
      {
        try
        {
          (pictureBox_main.Image as Bitmap).SetPixel(reds[i], reds[++i], Color.Red);
        }
        catch (Exception)
        {
          warnings = true;
        }
      }

      if (warnings)
      {
        MessageBox.Show("Masks may be invalid.\nPlease revise for current track.");
      }
    }

    void ResetReds()
    {
      if (pictureBox_main.Image == null)
      {
        return;
      }
      
      for (int i = 0; i < reds.Length; ++i)
      {
        try
        {
          (pictureBox_main.Image as Bitmap).SetPixel(reds[i], reds[++i], Color.Gray);
        }
        catch (Exception)
        {
        }
      }
    }

    void DrawMasks()
    {
      ResetReds();
      SetReds();
      pictureBox_main.Refresh();
    }

    private int IntValidation(object sender)
    {
      TextBox t = null;
      try
      {
        t = ((TextBox)sender);
      }
      catch (Exception)
      {
        return 0;
      }

      int amount = 0;
      if (!int.TryParse(t.Text, NumberStyles.Integer, null, out amount))
      {
        t.Text = "";
      }

      return amount;
    }

    private void coordinate_Leave_x(object sender, EventArgs e)
    {
      int amount = IntValidation(sender);
      if (amount >= pictureBox_main.Image.Width)
      {
        ((TextBox)sender).Text = "";
      }
      DrawMasks();
    }

    private void coordinate_Leave_y(object sender, EventArgs e)
    {
      int amount = IntValidation(sender);
      if (amount >= pictureBox_main.Image.Height)
      {
        ((TextBox)sender).Text = "";
      }
      DrawMasks();
    }


    private void btn_load_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "Text files (*.txt)|*.txt";

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
        PopulatePictureBoxes();
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

      DrawMasks();

      int x;
      int.TryParse(m1x1.Text, NumberStyles.Integer, null, out x);
      int y;
      int.TryParse(m1y1.Text, NumberStyles.Integer, null, out y);
      pictureBox_main.Location = new Point(-1*x + offset_x, -1*y + offset_y);
    }

    private void btn_save_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "Text files (*.txt)|*.txt";

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

        Process.Start(save.FileName);
      }
    }

    private void pictureBox_main_Click(object sender, EventArgs e)
    {
      tb_x.Text = "" + ((MouseEventArgs)e).X;
      tb_y.Text = "" + ((MouseEventArgs)e).Y;
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
    
    ////////////////////////////////////////////////////////////////////////////
    
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

      if (result < 0)
      {
        MessageBox.Show("ABORTED");
        return;
      }

      if (checkBox1.Checked)
      {
        Process.Start(""+ result);
      }
    }

  }
}
