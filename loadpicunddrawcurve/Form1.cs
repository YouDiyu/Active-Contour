using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace loadpicunddrawcurve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap oriimage = null;
        Bitmap oriimage0;
        Bitmap oriimage1;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog opd = new OpenFileDialog();
                opd.DefaultExt = ".bmp";
                opd.Filter = "Image Files(*.bmp,*.jpg,*.png,*.TIF)|*.bmp;*.jpg;*.png;*.TIF||";
                if (DialogResult.OK != opd.ShowDialog(this))
                {
                    return;
                }
                Image tmpImage = Image.FromFile(opd.FileName);
                Size pansiz = panel1.ClientSize;
                Size tmpsiz = tmpImage.Size;
                if (tmpsiz.Width > pansiz.Width || tmpsiz.Height > pansiz.Height)
                {
                    double rImage = tmpsiz.Width * 1.0 / tmpsiz.Height;
                    double rWnd = pansiz.Width * 1.0 / pansiz.Height;
                    if (rImage < rWnd) // image more high
                    {
                        
                        tmpsiz.Height = pansiz.Height;
                        tmpsiz.Width = (int)(tmpsiz.Height * rImage);
                    }
                    else //image is more wide
                    {

                        tmpsiz.Width = pansiz.Width;
                        tmpsiz.Height = (int)(pansiz.Width / rImage);
                    }
                }
                panel1.Size = tmpsiz;
                oriimage = new Bitmap(tmpImage, tmpsiz);
                Color pixcolor = Color.FromArgb(0);
                double pixval =0;
                for(int i = 0; i < oriimage.Height; i++)
                {
                    for(int j = 0; j < oriimage.Width; j++)
                    {
                        pixcolor = oriimage.GetPixel(j, i);
                        pixval = pixcolor.R * 0.3 + pixcolor.G * 0.59 + pixcolor.B * 0.11;
                        oriimage.SetPixel(j,i, Color.FromArgb(Convert.ToInt32(pixval), Convert.ToInt32(pixval), Convert.ToInt32(pixval)));
                    }
                }
                oriimage1 = new Bitmap(oriimage, tmpsiz);
                //for (int i = 2; i < oriimage.Height - 2; i++)
                //{
                //    for (int j = 2; j < oriimage.Width - 2; j++)
                //    {
                //        pixval = (oriimage.GetPixel(j - 2, i - 2).R + oriimage.GetPixel(j - 1, i - 2).R + oriimage.GetPixel(j, i - 2).R * 2 + oriimage.GetPixel(j + 1, i - 2).R + oriimage.GetPixel(j + 2, i - 2).R
                //            + oriimage.GetPixel(j - 2, i - 1).R + oriimage.GetPixel(j - 1, i - 1).R * 2 + oriimage.GetPixel(j, i - 1).R * 4 + oriimage.GetPixel(j + 1, i - 1).R * 2 + oriimage.GetPixel(j + 2, i - 1).R
                //            + oriimage.GetPixel(j - 2, i).R * 2 + oriimage.GetPixel(j - 1, i).R * 4 + oriimage.GetPixel(j, i).R * 8 + oriimage.GetPixel(j + 1, i).R * 4 + oriimage.GetPixel(j + 2, i).R * 2
                //            + oriimage.GetPixel(j - 2, i + 1).R + oriimage.GetPixel(j - 1, i + 1).R * 2 + oriimage.GetPixel(j, i + 1).R * 4 + oriimage.GetPixel(j + 1, i + 1).R * 2 + oriimage.GetPixel(j + 2, i + 1).R
                //            + oriimage.GetPixel(j - 2, i + 2).R + oriimage.GetPixel(j - 1, i + 2).R + oriimage.GetPixel(j, i + 2).R * 2 + oriimage.GetPixel(j + 1, i + 2).R + oriimage.GetPixel(j + 2, i + 2).R) / 52;
                //        oriimage1.SetPixel(j, i, Color.FromArgb(Convert.ToInt32(pixval), Convert.ToInt32(pixval), Convert.ToInt32(pixval)));
                //    }
                //}
                panel1.BackgroundImage = oriimage1;
                textBox1.AppendText("Image load"+"\n");
                panel1.Refresh();
                oriimage0 =new Bitmap(oriimage1, tmpsiz);
                //oriimage=>grau;oriimage1=>nach gaussian;oriimage0=>copy von oriimage1
            }
            catch
            {
                textBox1.AppendText("fehler!");
            }
        }
        bool mousedown = false;
        bool mouseup = false;
        bool mousemove = false;
        List<Point> ptlist = null;
        Point pt;
        Graphics graf = null;
        Pen pen = new Pen(Color.Red);
        private void button2_Click(object sender, EventArgs e)
        {
            if (mousedown == false)
            {
                mousedown = true;
                button2.Enabled = false;
            }
            else
            {
                return; 
            }
        }

        private void MouseDown_1(object sender, MouseEventArgs e)
        {
            if(mousedown == true)
            {
                mousemove = true;
                ptlist = new List<Point>();
                graf = Graphics.FromImage(oriimage1);
                //pt = new Point(e.Location.X, e.Location.Y);  //nur fuer test
                //textBox1.AppendText(Convert.ToDouble(pt.X) + "," + Convert.ToDouble(pt.Y) + "\n");   //nur fuer test
            }
            else
            {
                return;
            }
        }
        long mouseTick = 0;
        private void MouseMove_1(object sender, MouseEventArgs e)
        {
            if (mousemove == true)
            {
                long ticks = DateTime.Now.Ticks;
                if ((ticks - mouseTick) < 100 * 10000)
                {
                    return;
                }
                mouseTick = ticks;
                mouseup = true;
                Point mittelpt = new Point(0, 0);
                Rectangle oriimagerec = new Rectangle(mittelpt, oriimage1.Size);
                if (oriimagerec.Contains(e.Location))
                {
                    pt = new Point(e.Location.X, e.Location.Y);
                    ptlist.Add(pt);
                    graf.DrawRectangle(pen, pt.X - 1, pt.Y - 1, 3, 3);
                    if (ptlist.Count > 1)
                    {
                        graf.DrawLine(pen, ptlist[ptlist.Count - 2], ptlist[ptlist.Count - 1]);
                    }
                    graf.Save();
                    panel1.Refresh();
                }
            }
            else
            {
                return;
            }
        }

        private void MouseUp_1(object sender, MouseEventArgs e)
        {
            if(mouseup == true)
            {
                mousedown = false;
                mousemove = false;
                mouseup = false;
                button2.Enabled = true;
                for(int i =0; i<ptlist.Count; i++)
                {
                    textBox1.AppendText(Convert.ToDouble(ptlist[i].X) +","+ Convert.ToDouble(ptlist[i].Y)+"\n");
                }
                graf.DrawLine(pen, ptlist[0], ptlist[ptlist.Count - 1]);
                graf.Save();
                panel1.Refresh();
            }
            else
            {
                return;
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            oriimage1 = new Bitmap(oriimage0);
            panel1.BackgroundImage = oriimage1;
            textBox1.AppendText("reload");
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            oriimage1 = new Bitmap(oriimage0);
            int interations = 0;                             //nummer der Iterationen
            interations = Convert.ToInt32(textBox2.Text);
            double alpha, beta, Wline, Wedge, Wterm;
            alpha = Convert.ToDouble(textBox3.Text);
            beta = Convert.ToDouble(textBox4.Text);
            Wline = Convert.ToDouble(textBox5.Text);
            Wedge = Convert.ToDouble(textBox6.Text);
            Wterm = Convert.ToDouble(textBox7.Text);
            for(int iter=0; iter<= interations; iter++)      //Anfang der Iteration
            {
                for(int p = 0; p < ptlist.Count; p++)        //alle Points in Ptlist
                {
                    Point copyPt = ptlist[p];
                    Point offsetpt;
                    int newx = 0;
                    int newy = 0;//Koordinate des Neuen Points
                    double Emin = 0; 
                    
                    //textBox1.AppendText("attention!"+Convert.ToString(copyPt.X) + "," + Convert.ToString(copyPt.Y));
                    for(int k = 0; k < 9; k++)               //3*3Matrix 
                    {
                        Offset offset = Nodeoffset(k);
                        offsetpt = new Point(copyPt.X + offset.X, copyPt.Y + offset.Y);
                        ptlist[p] = offsetpt;
                        //Esnake(Egsm)=alpha*Eint(vs)+beta*Eint(vss)+Wline*Eline+Wedge*Eedge+Wterm*Eterm
                        //Eint
                        double Egsm = 0;
                        double GsmDis=0;
                        double EinzDis=0;
                        double avgDis=0;
                        double EintVs = 0;
                        double EinzEint = 0;
                        double EintVss = 0; 
                        for(int t = 1; t < ptlist.Count; t++)//Durchschnittsdistance
                        {
                            EinzDis = Math.Sqrt(Math.Pow((ptlist[t].X - ptlist[t - 1].X),2) + Math.Pow((ptlist[t].Y - ptlist[t - 1].Y),2));
                            //EinzDis = p(i)-p(i-1)
                            GsmDis = GsmDis + EinzDis;
                        }
                        avgDis = (GsmDis + Math.Sqrt(Math.Pow((ptlist[0].X - ptlist[ptlist.Count - 1].X),2) + Math.Pow((ptlist[0].Y - ptlist[ptlist.Count - 1].Y),2))) / ptlist.Count;
                        for (int t = 1; t < ptlist.Count; t++)
                        {
                            EinzEint = Math.Pow(Math.Sqrt(Math.Pow((ptlist[t].X - ptlist[t - 1].X), 2) + Math.Pow((ptlist[t].Y - ptlist[t - 1].Y), 2)), 2);
                            //EinzEint = (avg - |p(i)-p(i-1)|)^2  Wenn EinzDis == avgDis, bekommt mann EinzEint(min)??????
                            EintVs = EintVs + EinzEint;
                        }
                        EintVs = EintVs + Math.Pow(Math.Sqrt(Math.Pow((ptlist[0].X - ptlist[ptlist.Count - 1].X),2) + Math.Pow((ptlist[0].Y - ptlist[ptlist.Count - 1].Y),2)), 2);
                        for(int t=1;  t < ptlist.Count - 1; t++)
                        {
                            EinzEint = Math.Pow(ptlist[t + 1].X + ptlist[t - 1].X - 2 * ptlist[t].X, 2) + Math.Pow(ptlist[t + 1].Y + ptlist[t - 1].Y - 2 * ptlist[t].Y, 2);
                            EintVss = EintVss + EinzEint;
                        }
                        EintVss = EintVss + Math.Pow(ptlist[ptlist.Count - 2].X + ptlist[0].X - 2 * ptlist[ptlist.Count - 1].X, 2) + Math.Pow(ptlist[ptlist.Count - 2].Y + ptlist[0].Y - 2 * ptlist[ptlist.Count - 1].Y, 2)
                            + Math.Pow(ptlist[ptlist.Count - 1].X + ptlist[1].X - 2 * ptlist[0].X, 2) + Math.Pow(ptlist[ptlist.Count - 1].Y + ptlist[1].Y - 2 * ptlist[0].Y, 2);
                        //EintVss = P(i+1)-2Pi+P(i-1)
                        double Eedge=0;
                        double Iy = 0;
                        double Ix = 0;
                        //Iy = -a11-2a12-a13+a31+a32+a33; Ix = -a11-2a21-a31+a13+2a23+a33
                        int x = offsetpt.X;
                        int y = offsetpt.Y;
                        if (x > 1 && x < oriimage1.Width && y > 1 && y < oriimage1.Height)
                        {
                            Iy = -oriimage1.GetPixel(x - 1, y - 1).R - 2 * oriimage1.GetPixel(x - 1, y).R - oriimage1.GetPixel(x - 1, y + 1).R
                                + oriimage1.GetPixel(x + 1, y - 1).R + 2 * oriimage1.GetPixel(x + 1, y).R + oriimage1.GetPixel(x + 1, y + 1).R;
                            Ix = -oriimage1.GetPixel(x - 1, y - 1).R - 2 * oriimage1.GetPixel(x, y - 1).R - oriimage1.GetPixel(x + 1, y - 1).R
                                + oriimage1.GetPixel(x - 1, y + 1).R + 2 * oriimage1.GetPixel(x, y + 1).R + oriimage1.GetPixel(x + 1, y + 1).R;
                            Eedge = Math.Sqrt(Ix * Ix + Iy * Iy);
                            double Eterm = 0;
                            double Cx, Cy, Cxx, Cyy, Cxy;
                            Cx = oriimage1.GetPixel(x, y).R - oriimage1.GetPixel(x, y - 1).R;
                            Cy = oriimage1.GetPixel(x, y).R - oriimage1.GetPixel(x - 1, y).R;
                            Cyy = oriimage1.GetPixel(x, y).R - 2 * oriimage1.GetPixel(x - 1, y).R + oriimage1.GetPixel(x - 2, y).R;
                            Cxx = oriimage1.GetPixel(x, y).R - 2 * oriimage1.GetPixel(x, y - 1).R + oriimage1.GetPixel(x, y - 2).R;
                            Cxy = oriimage1.GetPixel(x, y).R + oriimage1.GetPixel(x - 1, y - 1).R - oriimage1.GetPixel(x - 1, y).R - oriimage1.GetPixel(x, y - 1).R;
                            Eterm = (Cyy * Cx * Cx - 2 * Cxy * Cx * Cy + Cxx * Cy * Cy) / Math.Pow(1 + Cx * Cx + Cy * Cy, 3 / 2);
                            double Eline = oriimage1.GetPixel(ptlist[p].X, ptlist[p].Y).R;
                            //Eterm = (Cyy*Cx^2 - 2*Cxy*Cx*Cy + Cxx*Cy^2)/（(1 +Cx^2 + Cy^2)^(3/2)）
                            //Die Energie von offsetpt
                            Egsm = alpha * EintVs + beta * EintVss + Wline * Eline - Wedge * Eedge + Wterm + Eterm;
                            //textBox1.AppendText(Convert.ToString(Egsm) + "\n");
                            if (k == 0)
                            {
                                Emin = Egsm;
                                newx = offsetpt.X;
                                newy = offsetpt.Y;
                            }
                            else
                            {
                                if (Egsm <= Emin)
                                {
                                    Emin = Egsm;
                                    newx = offsetpt.X;
                                    newy = offsetpt.Y;
                                }
                            }
                        }
                        //textBox1.AppendText("change" + Convert.ToString(offsetpt.X) + "," + Convert.ToString(offsetpt.Y));
                        //ptlist[p]=offsetpt(min Energie)
                    }
                    
                    offsetpt = new Point(newx, newy);
                    ptlist[p] = offsetpt;
                   
                }
            }
            //Draw!
            graf = Graphics.FromImage(oriimage1);
            foreach(Point pt in ptlist)
            {
                graf.DrawRectangle(pen, pt.X - 1, pt.Y - 1, 3, 3);
            }
            for(int i = 1; i < ptlist.Count; i++)
            {
                graf.DrawLine(pen, ptlist[i - 1], ptlist[i]);
            }
            graf.DrawLine(pen, ptlist[0], ptlist[ptlist.Count - 1]);
            graf.Save();
            panel1.BackgroundImage = oriimage1;
            panel1.Refresh();
        }


        public class Offset
        {
            public int X;
            public int Y;
            public Offset(int x,int y)
            {
                X = x;
                Y = y;
            }
            public Offset()
            {
                X = 0;
                Y = 0;
            }
        }
        private Offset Nodeoffset(int KK)
        {
            int offsetX = 0;
            int offsetY = 0;
            /* 0 1 2
             * 3 4 5
             * 6 7 8
             */
            switch (KK)
            {
                case 0:
                    offsetX = -1;
                    offsetY = -1;
                    break;
                case 1:
                    offsetX = 0;
                    offsetY = -1;
                    break;
                case 2:
                    offsetX = 1;
                    offsetY = -1;
                    break;
                case 3:
                    offsetX = -1;
                    offsetY = 0;
                    break;
                case 4:
                    offsetX = 0;
                    offsetY = 0;
                    break;
                case 5:
                    offsetX = 1;
                    offsetY = 0;
                    break;
                case 6:
                    offsetX = -1;
                    offsetY = 1;
                    break;
                case 7:
                    offsetX = 0;
                    offsetY = 1;
                    break;
                case 8:
                    offsetX = 1;
                    offsetY = 1;
                    break;
            }
            return new Offset(offsetX, offsetY);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            oriimage1 = new Bitmap(oriimage0);
            double ttttttt = oriimage1.GetPixel(ptlist[4].X, ptlist[4].Y).R;
            textBox1.AppendText(Convert.ToString(ttttttt)+"\n");
        }
    }
}
