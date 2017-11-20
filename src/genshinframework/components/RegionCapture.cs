using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace genshinframework.components
{
	/// <summary>
	/// Must be attached to a form
	/// </summary>
	public class RegionCapture : Form
	{
		public enum CaptureMode:int{
			RegionSelect = 0
		}

		Point m_StartPoint, m_CurrentPoint;
		public Rectangle m_EstimatedRegion = new Rectangle();
		Color m_OriginalBackColor;

		Bitmap m_Bitty;
		CaptureMode m_CaptureMode;

		bool m_regionSelect, drawRegionSelect, cancelRegionSelect;

		const Keys captureKey = Keys.Enter;
		const Keys cancelKey = Keys.Escape;

		public event EventHandler CaptureFinished;
		
		public RegionCapture()
		{
			this.m_CaptureMode = CaptureMode.RegionSelect;

			this.WindowState = FormWindowState.Maximized;
			this.FormBorderStyle = FormBorderStyle.None;
			this.DoubleBuffered = true;
			this.Opacity = 0.75f;
			this.TransparencyKey = Color.White;
			m_OriginalBackColor = this.BackColor; //Cache first BG Color before setting a new value;
			this.BackColor = Color.White;

		

			this.Load += RegionCapture_Load;
			this.MouseDown += RegionCapture_MouseDown;
			this.MouseMove += RegionCapture_MouseMove;
			this.MouseUp += RegionCapture_MouseUp;
			this.KeyUp += RegionCapture_KeyUp;
		}

		private void RegionCapture_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case captureKey:
					break;
				case cancelKey:
                    this.Hide();
					break;
				default:
					break;
			}
			//throw new NotImplementedException();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if(this.drawRegionSelect && !cancelRegionSelect)
			{
				using (Brush br = new SolidBrush(Color.White))
				{
					Rectangle desiredRegion = new Rectangle(
						this.m_EstimatedRegion.X + 1,
						this.m_EstimatedRegion.Y + 1,
						this.m_EstimatedRegion.Width - 1,
						this.m_EstimatedRegion.Height - 1);
					g.SetClip(desiredRegion, System.Drawing.Drawing2D.CombineMode.Exclude);
					g.FillRectangle(br, this.m_EstimatedRegion);
				}
			}

			//BG Color
			using(Brush br = new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke)))
			{
				g.FillRectangle(br, this.ClientRectangle);
			}

			//Border
			using(Pen p = new Pen(Color.DimGray, 2.5f))
			{
				g.DrawRectangle(p, this.m_EstimatedRegion);
			}

			if (this.cancelRegionSelect)
			{
				g.Clear(Color.FromArgb(255, Color.WhiteSmoke));
			}

			//base.OnPaint(e);
		}
		private void RegionCapture_MouseUp(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					if(this.m_CaptureMode >= CaptureMode.RegionSelect)
					{
						if(this.m_regionSelect && (!(this.m_EstimatedRegion.Width <= 0) && !(this.m_EstimatedRegion.Height <= 0)))
						{
							if(this.m_CaptureMode == CaptureMode.RegionSelect)
							{

								this.m_Bitty = new Bitmap(
									this.m_EstimatedRegion.Width,
									this.m_EstimatedRegion.Height,
									PixelFormat.Format24bppRgb);
								using(Graphics g = Graphics.FromImage(this.m_Bitty))
								{
									g.CopyFromScreen(
										this.m_EstimatedRegion.X, this.m_EstimatedRegion.Y,
										0, 0, this.m_EstimatedRegion.Size);

                                    this.HandleEvent();
								}
							}
							this.m_regionSelect = false;
						}
					}
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					if(this.m_CaptureMode >= CaptureMode.RegionSelect)
					{
						if(!(this.m_EstimatedRegion.Width <= 0) && !(this.m_EstimatedRegion.Height <= 0))
						{
							this.cancelRegionSelect = true;
							this.Refresh();
							this.cancelRegionSelect = false;
							this.m_EstimatedRegion = Rectangle.Empty;
						}
					}
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
			 //throw new NotImplementedException();
		}

		private void RegionCapture_MouseMove(object sender, MouseEventArgs e)
		{
			//if(this.m_CaptureMode == CaptureMode.RegionSelect)

			switch (e.Button)
			{
				case MouseButtons.Left:
					//region select
					if(this.m_CaptureMode >= CaptureMode.RegionSelect)
					{
						this.m_regionSelect = true;
						int desiredX = 0, desiredY = 0;
						int w = 0, h = 0;

						this.drawRegionSelect = true;

						this.m_CurrentPoint = e.Location;

						if((this.m_StartPoint.X > this.m_CurrentPoint.X) && (this.m_StartPoint.Y < this.m_CurrentPoint.Y))
						{
							h = Math.Abs(this.m_CurrentPoint.Y - this.m_StartPoint.Y);
							w = Math.Abs(this.m_CurrentPoint.X - this.m_StartPoint.X);
							desiredX = (this.m_StartPoint.X) - w;
							desiredY = (this.m_StartPoint.Y - h) + h;
						}
						else if((this.m_StartPoint.X < this.m_CurrentPoint.X) && (this.m_StartPoint.Y > this.m_CurrentPoint.Y))
						{
							//1st quadrant
							h = Math.Abs(this.m_CurrentPoint.Y - this.m_StartPoint.Y);
							w = Math.Abs(this.m_CurrentPoint.X - this.m_StartPoint.X);
							desiredX = (this.m_StartPoint.X + w) - w;
							desiredY = (this.m_StartPoint.Y - h);
						}
						else if(this.m_StartPoint.X > this.m_CurrentPoint.X)
						{
							//2nd q
							w = Math.Abs(this.m_CurrentPoint.X - this.m_StartPoint.X);
							h = Math.Abs(this.m_CurrentPoint.Y - this.m_StartPoint.Y);
							desiredX = this.m_CurrentPoint.X;
							desiredY = this.m_CurrentPoint.Y;
						}
						else
						{
							//4th
							w = (this.m_CurrentPoint.X - this.m_StartPoint.X);
							h = (this.m_CurrentPoint.Y - this.m_StartPoint.Y);
							desiredX = this.m_StartPoint.X;
							desiredY = this.m_StartPoint.Y;
						}

						this.m_EstimatedRegion = new Rectangle(desiredX, desiredY, w, h);
						this.Refresh();
					}
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
			 //throw new NotImplementedException();
		}

		private void RegionCapture_MouseDown(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					if(this.m_CaptureMode >= CaptureMode.RegionSelect)
					{
						this.m_StartPoint = e.Location;
					}
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
			 //throw new NotImplementedException();
		}

		private void RegionCapture_Load(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Maximized;
			this.FormBorderStyle = FormBorderStyle.None;
			//throw new NotImplementedException();
		}

		public void HandleEvent()
		{
			HandledEventArgs handledEventArgs = new HandledEventArgs(false);
			if (this.CaptureFinished != null)
			{
				this.CaptureFinished(this, handledEventArgs);
			}
		}

		public Bitmap GetBitty()
		{
			return m_Bitty;
		}

		public Rectangle GetSelectedArea()
		{
			return m_EstimatedRegion;
		}

		public void SetCursor(Cursor c)
		{
			this.Cursor = c;
		}

		public void SetForm(Form f)
		{

		}

        public void CameraCapture()
        {

            this.m_Bitty = new Bitmap(
                this.m_EstimatedRegion.Width,
                this.m_EstimatedRegion.Height,
                PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(this.m_Bitty))
            {
                g.CopyFromScreen(
                    this.m_EstimatedRegion.X, this.m_EstimatedRegion.Y,
                    0, 0, this.m_EstimatedRegion.Size);
            }
        }
	}
}
