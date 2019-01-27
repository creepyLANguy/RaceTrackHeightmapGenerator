﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

    bool shouldRestrictPaintsOnResize = false;

    int magnify_X = 50;
    int magnify_Y = 30;
    int magnificationScale = 5;
    IntPtr deskDC = GetDC(IntPtr.Zero);
    Graphics g;

    Bitmap bmpSmall;

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
      btn_browseImage.Focus();
      lbl_image.Text = lbl_gradient.Text = "";

      offset_x = (int)(0.5 * panel2.Width);
      offset_y = (int)(0.5 * panel2.Height);

      g = Graphics.FromHdc(deskDC);
      g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
      //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
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
      openFileDialog1.Filter = "Bitmap files (*.bmp)|*.bmp";

      DialogResult result = openFileDialog1.ShowDialog();
      if (result != DialogResult.OK)
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

        groupBox1.Enabled = true;
        groupBox2.Enabled = true;
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
      openFileDialog1.Filter = "Bitmap files (*.bmp)|*.bmp";

      DialogResult result = openFileDialog1.ShowDialog();
      if (result != DialogResult.OK)
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
      openFileDialog1.Filter = "Track Data files (*.trk)|*.trk";

      DialogResult result = openFileDialog1.ShowDialog();
      if (result != DialogResult.OK)
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
        groupBox1.Enabled = true;
        groupBox2.Enabled = true;
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
      openFileDialog1.Filter = "Track Data files (*.trk)|*.trk";

      SaveFileDialog save = new SaveFileDialog();

      save.FileName = "config.trk";

      save.Filter = "Track Data file | *.trk";

      if (save.ShowDialog() == DialogResult.OK)
      {
        StreamWriter writer = new StreamWriter(save.OpenFile());

        writer.WriteLine(trackPath);
        writer.WriteLine(gradientPath);
        writer.WriteLine(m1x1.Text);
        writer.WriteLine(m1y1.Text);
        writer.WriteLine(m1x2.Text);
        writer.WriteLine(m1y2.Text);
        writer.WriteLine(m2x1.Text);
        writer.WriteLine(m2y1.Text);
        writer.WriteLine(m2x2.Text);
        writer.WriteLine(m2y2.Text);

        writer.Dispose();
        writer.Close();

        Process.Start("notepad.exe", save.FileName);
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
    
    void Lock()
    {
      panel_all.Enabled = false;
      lbl_processing2.Visible = true;
    }

    void Unlock()
    {
      panel_all.Enabled = true;
      lbl_processing2.Visible = false;
    }

    bool PathsContainSpaces()
    {
      return (trackPath.Contains(" ") || gradientPath.Contains(" "));
    }

    private void btn_go_Click(object sender, EventArgs e)
    {
      Lock();

      if (AllFieldsFilled() == false)
      {
        MessageBox.Show("Not all fields valid.");
        Unlock();
        return;
      }

      if (PathsContainSpaces() == true)
      {
        MessageBox.Show("Paths contain spaces, which are not allowed.");
        Unlock();
        return;
      }

      Process process = new Process();
      process.StartInfo.FileName = "TrackEdgeDetector.exe";
      process.StartInfo.Arguments = GetArgsString();
      process.Start();
      process.WaitForExit();
      int result1 = process.ExitCode;

      if (result1 < 0)
      {
        MessageBox.Show("ABORTED - " + process.StartInfo.FileName);
        Unlock();
        return;
      }

      if (checkBox1.Checked)
      {
        //Process.Start(""+ result1);
      }

      process.StartInfo.FileName = "TryMakeGradient.exe";
      process.StartInfo.Arguments = gradientPath;
      process.Start();
      process.WaitForExit();
      int result2 = process.ExitCode;

      if (result2 < 0)
      {
        MessageBox.Show("ABORTED - " + process.StartInfo.FileName);
        Unlock();
        return;
      }

      if (checkBox1.Checked)
      {
        //Process.Start(""+ result2);
      }

      process.StartInfo.FileName = "TrackGradientShader.exe";
      process.StartInfo.Arguments = result1 + delim + result2;
      process.Start();
      process.WaitForExit();
      int result3 = process.ExitCode;

      if (result3 < 0)
      {
        MessageBox.Show("ABORTED - " + process.StartInfo.FileName);
        Unlock();
        return;
      }

      if (checkBox1.Checked)
      {
        Process.Start(Application.StartupPath);
        Process.Start("" + result3 + ".bmp");
      }

      Unlock();
    }

    private void btn_m1p1_Click(object sender, EventArgs e)
    {
      if (tb_x.Text.Length <= 0 || tb_y.Text.Length <= 0)
      {
        return;
      }
      if (pictureBox_main.Image == null)
      {
        return;
      }

      m1x1.Text = tb_x.Text;
      m1y1.Text = tb_y.Text;
      coordinate_Leave_x(m1x1, null);
      coordinate_Leave_y(m1y1, null);
    }

    private void btn_m1p2_Click(object sender, EventArgs e)
    {
      if (tb_x.Text.Length <= 0 || tb_y.Text.Length <= 0)
      {
        return;
      }
      if (pictureBox_main.Image == null)
      {
        return;
      }

      m1x2.Text = tb_x.Text;
      m1y2.Text = tb_y.Text;
      coordinate_Leave_x(m1x2, null);
      coordinate_Leave_y(m1y2, null);
    }

    private void btn_m2p1_Click(object sender, EventArgs e)
    {
      if (tb_x.Text.Length <= 0 || tb_y.Text.Length <= 0)
      {
        return;
      }
      if (pictureBox_main.Image == null)
      {
        return;
      }

      m2x1.Text = tb_x.Text;
      m2y1.Text = tb_y.Text;
      coordinate_Leave_x(m2x1, null);
      coordinate_Leave_y(m2y1, null);
    }

    private void btn_m2p2_Click(object sender, EventArgs e)
    {
      if (tb_x.Text.Length <= 0 || tb_y.Text.Length <= 0)
      {
        return;
      }
      if (pictureBox_main.Image == null)
      {
        return;
      }

      m2x2.Text = tb_x.Text;
      m2y2.Text = tb_y.Text;
      coordinate_Leave_x(m2x2, null);
      coordinate_Leave_y(m2y2, null);
    }

    private void Form1_ResizeBegin(object sender, EventArgs e)
    {
      if (shouldRestrictPaintsOnResize)
      {
        SuspendLayout();
      }
    }

    private void Form1_ResizeEnd(object sender, EventArgs e)
    {
      if (shouldRestrictPaintsOnResize)
      {
        ResumeLayout(true);
      }
    }

    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("User32.dll")]
    public static extern void ReleaseDC(IntPtr dc, IntPtr hwnd);

    private Bitmap CaptureScreen()
    {
      Rectangle bounds = Screen.GetBounds(Point.Empty);
      Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
      }
      return bmp;
    }

    private void pictureBox_main_MouseLeave(object sender, EventArgs e)
    {
      if (magnifyToggle.Checked)
      {
        Refresh();
      }
    }

    private void pictureBox_main_MouseMove(object sender, MouseEventArgs e)
    {
      if (magnifyToggle.Checked == false)
      {
        return;
      }

      int mouse_x = Cursor.Position.X;
      int mouse_y = Cursor.Position.Y;
      
      bmpSmall = CaptureScreen().Clone
        (
          new Rectangle
          (
          mouse_x - (magnify_X / 2),
          mouse_y - (magnify_Y / 2),
          magnify_X, magnify_Y
          ),
        PixelFormat.Format32bppRgb
        );

      bmpSmall.SetPixel(bmpSmall.Width / 2, bmpSmall.Height / 2, Color.Green);

      Refresh();
      g.DrawImage(bmpSmall, mouse_x + 15, mouse_y + 15, magnify_X * magnificationScale, magnify_Y * magnificationScale);
    }

  }
}
