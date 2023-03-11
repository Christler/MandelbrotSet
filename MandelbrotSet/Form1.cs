using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MandelbrotSet
{
    public partial class Form1 : Form
    {
        private double yMin = -2;                                 // Default minimum Y for the set to render.
        private double yMax = 2;                                  // Default maximum Y for the set to render.
        private double xMin = -3;                                 // Default minimum X for the set to render.
        private double xMax = 1.5;                                  // Default maximum X for the set to render.
        private int maxIterations = 100;                             // Default maximum number of iterations for Mandelbrot calculation.
        private int numColors = 0;                                  // Default number of colours to use in colour table.
        private int zoomScale = 5;                                  // Default amount to zoom in by.
        private CanvasManager canvasManager;                        // Used for conversions between math and pixel coordinates.
        private Graphics graphics;                                  // Graphics object: all graphical rendering is done on this object.
        private Bitmap bitmap;                                      
        private ColorTable colorTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateGraphicsObj();
            populateTextBoxes();
            progressBar1.Maximum = bitmap.Height;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;
            progressBar1.Style = ProgressBarStyle.Continuous;
        }

        private void CreateGraphicsObj()
        {
            bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);
        }

        private void RenderImage()
        {
            try
            {
                // Clear any existing graphics content.
                graphics.Clear(Color.Black);

                numColors = maxIterations;

                // If colourTable is not yet created or maxIterations has changed, create colourTable.
                if ((colorTable == null) || (maxIterations != colorTable.kMax) || (numColors != colorTable.nColour))
                {
                    colorTable = new ColorTable(numColors, maxIterations);
                }

                ComplexPoint screenBottomLeft = new ComplexPoint
                {
                    x = xMin,
                    y = yMin
                };
                ComplexPoint screenTopRight = new ComplexPoint
                {
                    x = xMax,
                    y = yMax
                };

                // Create pixel manager. This sets up the scaling factors used when
                // converting from mathemathical to screen (pixel units) using the
                canvasManager = new CanvasManager(graphics, screenBottomLeft, screenTopRight);
                int xyPixelStep = 1;
                ComplexPoint pixelStep = new ComplexPoint
                {
                    x = xyPixelStep,
                    y = xyPixelStep
                }; 
                ComplexPoint xyStep = canvasManager.GetDeltaMathsCoord(pixelStep);

                int height = (int)graphics.VisibleClipBounds.Size.Height;
                double modulusSquared;
                Color color;
                Color previousColor = Color.Red;

                // Main loop, nested over Y (outer) and X (inner) values.
                int lineNumber = 0;
                int yPix = bitmap.Height - 1;
                
                for (double y = yMin; y < yMax; y += xyStep.y)
                {
                    int xPix = 0;
                    for (double x = xMin; x < xMax; x += xyStep.x)
                    {
                        // Create complex point C = x + i*y.
                        ComplexPoint c = new ComplexPoint
                        {
                            x = x,
                            y = y
                        };

                        
                        ComplexPoint zk = new ComplexPoint
                        {
                            x = 0,
                            y = 0
                        };

                        // Do the main Mandelbrot calculation. Iterate until the equation
                        int i = 0;
                        do
                        {
                            zk = zk.doCmplxSqPlusConst(c);
                            modulusSquared = zk.doMoulusSq();
                            i++;
                        } while ((modulusSquared <= 4.0) && (i < maxIterations));

                        if (i < maxIterations)
                        {

                            if (i >= numColors)
                            {
                                color = previousColor;
                            }
                            else
                            {
                                color = colorTable.GetColor(i);
                                previousColor = color;
                            }

                            if ((xPix < bitmap.Width) && (yPix >= 0))
                            {
                                bitmap.SetPixel(xPix, yPix, color);
                            }

                        }
                        xPix += xyPixelStep;
                    }
                    yPix -= xyPixelStep;
                    lineNumber++;

                    if ((lineNumber % 120) == 0)
                    {
                        Refresh();
                    }

                    if (lineNumber < bitmap.Height)
                    {
                        progressBar1.Value = lineNumber;
                    }
                    else
                    {
                        progressBar1.Value = bitmap.Height;
                    }
                }
                Refresh();
            }
            catch (Exception e2)
            {
                throw e2;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj = e.Graphics;
            graphicsObj.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            graphicsObj.Dispose();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            maxIterations = Convert.ToInt32(txtIterations.Text);

            RenderImage();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            yMin += 0.2;
            yMax += 0.2;
            populateTextBoxes();
            RenderImage();
        }

        private void btnMoveRight_Click(object sender, EventArgs e)
        {
            xMin += 0.2;
            xMax += 0.2;
            populateTextBoxes();
            RenderImage();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            yMin -= 0.2;
            yMax -= 0.2;
            populateTextBoxes();
            RenderImage();
        }

        private void btnMoveLeft_Click(object sender, EventArgs e)
        {
            xMin -= 0.2;
            xMax -= 0.2;
            populateTextBoxes();
            RenderImage();
        }

        private void populateTextBoxes()
        {
            txtIterations.Text = maxIterations.ToString();
            txtXMax.Text = xMax.ToString();
            txtXMin.Text = xMin.ToString();
            txtYMax.Text = yMax.ToString();
            txtYMin.Text = yMin.ToString();
            numZoomScale.Value = zoomScale;
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            zoomScale = Convert.ToInt32(numZoomScale.Value);
            double x = Convert.ToDouble(e.X);
            double y = Convert.ToDouble(e.Y);
            ComplexPoint pixelLocation1 = new ComplexPoint
            {
                x = (int)(x - (bitmap.Width / (zoomScale)) / 4),
                y = (int)(y - (bitmap.Height / (zoomScale)) / 4)
            };

            ComplexPoint pixelLocation2 = new ComplexPoint
            {
                x = (int)(x + (bitmap.Width / (zoomScale)) / 4),
                y = (int)(y + (bitmap.Height / (zoomScale)) / 4)
            };
            ComplexPoint zoomCoord1 = canvasManager.GetAbsoluteMathsCoord(pixelLocation1);
            ComplexPoint zoomCoord2 = canvasManager.GetAbsoluteMathsCoord(pixelLocation2);
            if (zoomCoord2.x < zoomCoord1.x)
            {
                double temp = zoomCoord1.x;
                zoomCoord1.x = zoomCoord2.x;
                zoomCoord2.x = temp;
            }
            if (zoomCoord2.y < zoomCoord1.y)
            {
                double temp = zoomCoord1.y;
                zoomCoord1.y = zoomCoord2.y;
                zoomCoord2.y = temp;
            }
            yMin = zoomCoord1.y;
            yMax = zoomCoord2.y;
            xMin = zoomCoord1.x;
            xMax = zoomCoord2.x;
            populateTextBoxes();
            RenderImage();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            yMin = -2.0;                                 
            yMax = 2.0;                                  
            xMin = -3.0;                                 
            xMax = 1.5;
            populateTextBoxes();
            RenderImage();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            RenderImage();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
