using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using genshinframework.components;

namespace dev.genshiAI.forms
{
	public partial class ChefBotForm : Form
	{
		RegionCapture rg;
        String jsonContent;
        object jsonObject;
        JArray jsonArray;
        JObject jObj;

        List<string> similarity;
        List<Bitmap> tempaltes;

        ak.component.Chef chef;


        Hotkey hk_start;
        Hotkey hk_stop;

		public ChefBotForm()
		{
			InitializeComponent();
            rg = new RegionCapture();
            rg.CaptureFinished += Rg_CaptureFinished;
            this.Load += ChefBotForm_Load;

            chef = new ak.component.Chef();
            tempaltes = new List<Bitmap>();


          
        }

        private void Hk_stop_Pressed(object sender, HandledEventArgs e)
        {
            this.timer1.Stop();
        }

        private void Hk_start_Pressed(object sender, HandledEventArgs e)
        {
            timer1.Enabled = true;
            this.timer1.Start();
            this.timer1.Interval = int.Parse((string.IsNullOrEmpty(this.textBox4.Text)) ? "0.77" : this.textBox4.Text);
        }

        private void Rg_CaptureFinished(object sender, EventArgs e)
        {
            this.pictureBox1.Image = rg.GetBitty();
            chef.setupChefTable(rg.m_EstimatedRegion);
            this.label2.Text = chef.chefTable.ToString();
        }

        private void ChefBotForm_Load(object sender, EventArgs e)
        {
          
            //if (hk_start.Registered)
            //    hk_start.Unregister();
            //if (hk_stop.Registered)
            //    hk_stop.Unregister();

            hk_start = new Hotkey(Keys.F10, true, false, false, false);
            hk_stop = new Hotkey(Keys.F11, true, false, false, false);
            hk_start.Register(this);
            hk_stop.Register(this);

            hk_start.Pressed += Hk_start_Pressed;
            hk_stop.Pressed += Hk_stop_Pressed;
           

        }

        private void button1_Click(object sender, EventArgs e)
		{
			rg.Show();
		}

        //Load Recipe
        private void button3_Click(object sender, EventArgs e)
        {
            //is.openFileDialog1.ShowDialog();
            DialogResult dlgRes = this.openFileDialog1.ShowDialog();
            if(dlgRes == DialogResult.OK)
            {
                Console.WriteLine(this.openFileDialog1.FileName);
                using (StreamReader reader = new StreamReader(this.openFileDialog1.FileName))
                {

                    this.jsonContent = reader.ReadToEnd();
                    this.label1.Text = jsonContent;
                    jObj = JObject.Parse(jsonContent);
                    jsonArray = JArray.FromObject(jObj["wrongIngredient"]);

                    //Qualified Directory
                    string fileDir = this.openFileDialog1.FileName.Replace(this.openFileDialog1.SafeFileName,"");
                    Console.WriteLine(fileDir);
                    foreach (var item in jsonArray)
                    {
                        string img = fileDir + item + "";
                        Console.WriteLine(fileDir+ item+ "");
                        Console.WriteLine(File.Exists(fileDir + item + ""));
                        this.tempaltes.Add((Bitmap)Image.FromFile(img));
                    }
                    this.chef.setRecipe(this.tempaltes);
                }
            }
        }

        //Image Template Matching
        Bitmap image1, image2;
        Point click;
        private void button2_Click(object sender, EventArgs e)
        {
           
            chef.similarityOffset = float.Parse((string.IsNullOrEmpty(this.textBox2.Text)) ? "0.80" : this.textBox2.Text);
            chef.treshold = float.Parse((string.IsNullOrEmpty(this.textBox3.Text)) ? "0.77" : this.textBox3.Text);
            rg.CameraCapture();
            chef.chefEuclidFilter(rg.GetBitty(), out similarity, out click, out image1, out image2);
            this.pictureBox2.Image = image1;
            this.pictureBox3.Image = image2;
            this.textBox1.Lines = similarity.ToArray();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        int tick = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            button4.Text = "start" + tick;
            tick++;
            if (tick >= 100)
                tick = 0;
            chef.similarityOffset = float.Parse((string.IsNullOrEmpty(this.textBox2.Text)) ? "0.90" : this.textBox2.Text);
            chef.treshold = float.Parse((string.IsNullOrEmpty(this.textBox3.Text)) ? "0.75" : this.textBox3.Text);
            rg.CameraCapture();
            chef.chefEuclidFilter(rg.GetBitty(), out similarity, out click, out image1, out image2);

            //perform click
            if (click.X <= -1 || click.Y <= -1)
            {
                //do nothing
            }
            else
            {
                //do click
                Cursor.Position = click;
                //Console.WriteLine("Click " + click);
                genshinframework.VInput.PerformLeftClick();
            }

            // this.pictureBox2.Image = image1;
            //this.pictureBox3.Image = image2;
            //this.pictureBox1.Image = rg.GetBitty();
            this.textBox1.Lines = similarity.ToArray();
        }
    }
}
