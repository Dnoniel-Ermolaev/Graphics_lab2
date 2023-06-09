﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Ermolaev_tomogramm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        public int currentLayer = 0;
        public bool needReload = false;
        private void glControl1_Paint_1(object sender, PaintEventArgs e)
        {
            if (loaded)
            {

                if (radioButton1.Checked)
                    view.DrawQuads(currentLayer);
                else if (radioButton3.Checked)
                    view.DrawQuadstrip(currentLayer);

                else
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();

                }
                glControl1.SwapBuffers();//загружает наш буффер в буффер экрана
            }
            //glControl1.Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
                currentLayer = trackBar1.Value;
                needReload = true;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
        // чтобы Application.Idle работала автоматически
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer(fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void openBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                trackBar1.Refresh();
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        //задание минимума для TF 
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.minimum = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.window = trackBar3.Value;
            needReload = true;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
           
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            view.SetupView(glControl1.Width, glControl1.Height);
            //glControl1.Invalidate();
        }
    }
}
