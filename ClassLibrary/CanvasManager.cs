using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class CanvasManager
    {
        public int xPixel { get; set; }
        public int yPixel { get; set; }
        private double convConstX1;
        private double convConstX2;
        private double convConstY1;
        private double convConstY2;

        public CanvasManager(Graphics graphics, ComplexPoint screenBottomLeftCorner, ComplexPoint screenTopRightCorner)
        {

            // Transform from mathematical to pixel coordinates.
            //
            // The following are long-handed calulations, now replaced with more efficient calculations
            // using convConst** values.
            //       this.xPixel = (int) ((graphics.VisibleClipBounds.Size.Width) / (screenTopRightCorner.x - screenBottomLeftCorner.x) * (cmplxPoint.x - screenBottomLeftCorner.x));
            //       this.yPixel = (int) (graphics.VisibleClipBounds.Size.Height - graphics.VisibleClipBounds.Size.Height / (screenTopRightCorner.y - screenBottomLeftCorner.y) * (cmplxPoint.y - screenBottomLeftCorner.y));

            convConstX1 = graphics.VisibleClipBounds.Size.Width / (screenTopRightCorner.x - screenBottomLeftCorner.x);
            convConstX2 = convConstX1 * screenBottomLeftCorner.x;

            convConstY1 = graphics.VisibleClipBounds.Size.Height * (1.0 + screenBottomLeftCorner.y / (screenTopRightCorner.y - screenBottomLeftCorner.y));
            convConstY2 = graphics.VisibleClipBounds.Size.Height / (screenTopRightCorner.y - screenBottomLeftCorner.y);
        }

        public Pixel GetPixelLocation(ComplexPoint complexPoint)
        {
            Pixel pixel = new Pixel();
            pixel.x = (int)(convConstX1 * complexPoint.x - convConstX2);
            pixel.y = (int)(convConstY1 - convConstY2 * complexPoint.y);
            return pixel;
        }

        public ComplexPoint GetDeltaMathsCoord(ComplexPoint complexPoint)
        {
            ComplexPoint result = new ComplexPoint
            {
                x = complexPoint.x / convConstX1,
                y = complexPoint.y / convConstY2
            };
            return result;
        }

        public ComplexPoint GetAbsoluteMathsCoord(ComplexPoint pixelCoord)
        {
            ComplexPoint result = new ComplexPoint
            {
                x = (convConstX2 + pixelCoord.x) / convConstX1,
                y = (convConstY1 - pixelCoord.y) / convConstY2
            };
            return result;
        }
    }
}
