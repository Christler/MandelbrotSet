using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    /// <summary>
    /// ComplexPoint class is used encapsulate a single complex point
    /// Z = x + i*y where x and y are the real and imaginary parts respectively.
    /// A number of complex arithmetic utility methods are provided.
    /// </summary>
    public class ComplexPoint
    {
        public double x { get; set; }
        public double y { get; set; }

        public double doModulus()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double doMoulusSq()
        {
            return x * x + y * y;
        }

        public ComplexPoint doCmplxSq()
        {
            ComplexPoint newComplexPoint = new ComplexPoint
            {
                x = x * x - y * y,
                y = 2 * x * y
            };

            return newComplexPoint;
        }

        public ComplexPoint doCmplxAdd(ComplexPoint arg)
        {
            x += arg.x;
            y += arg.y;

            return this;
        }

        public ComplexPoint doCmplxSqPlusConst(ComplexPoint arg)
        {
            ComplexPoint newComplexPoint = new ComplexPoint
            {
                x = x * x - y * y,
                y = 2 * x * y
            };
            newComplexPoint.x += arg.x;
            newComplexPoint.y += arg.y;
            return newComplexPoint;
        }
    }
}
