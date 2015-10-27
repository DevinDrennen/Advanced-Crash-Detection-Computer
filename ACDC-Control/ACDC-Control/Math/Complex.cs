using System;
using Microsoft.SPOT;

namespace ACDC_Control.Math.Types
{
    public struct Complex
    {
        #region Special Complex

        public static readonly Complex Unit = new Complex(1, 1);
        
        public static readonly Complex Zero = new Complex();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the real component.
        /// </summary>
        public double Real
        { get; private set; }

        /// <summary>
        /// Gets the imaginary component.
        /// </summary>
        public double Imaginary
        { get; private set; }

        /// <summary>
        /// Gets the magnitude (or absolute value).
        /// </summary>
        public double Magnitude
        { get; private set; }

        /// <summary>
        /// Gets the phase.
        /// </summary>
        public double Phase
        { get; private set; }

        #endregion

        /// <summary>
        /// Create a complex number with floats for real and imaginary components.
        /// </summary>
        /// <param name="real">Real component.</param>
        /// <param name="img">Imaginary component.</param>
        public Complex(double real = 0, double img = 0)
        {
            Real = real;
            Imaginary = img;
            Magnitude = System.Math.Sqrt(real * real + img * img);
            Phase = System.Math.Atan2(img, real);
        }

        #region Operators

        /// <summary>
        /// Add a complex number to another.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }

        /// <summary>
        /// Subtract a complex number by another.
        /// </summary>
        /// <param name="c1">Number being subtracted from.</param>
        /// <param name="c2">Number being subtracted.</param>
        /// <returns></returns>
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }

        /// <summary>
        /// Negate a complex number.
        /// </summary>
        /// <param name="c">Complex number to negate.</param>
        /// <returns></returns>
        public static Complex operator -(Complex c)
        {
            return new Complex(-c.Real, -c.Imaginary);
        }

        /// <summary>
        /// Multiply two complex numbers.
        /// </summary>
        /// <param name="c1">First complex factor.</param>
        /// <param name="c2">Second complex factor.</param>
        /// <returns></returns>
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Real * c2.Real + -(c1.Imaginary * c2.Imaginary), c1.Real * c2.Imaginary + c1.Imaginary * c2.Real);
        }

        /// <summary>
        /// Multiply a complex number by a regular double.
        /// </summary>
        /// <param name="c1">Complex factor.</param>
        /// <param name="f1">Float factor.</param>
        /// <returns></returns>
        public static Complex operator *(Complex c1, double f1)
        {
            return new Complex(f1 * c1.Real, f1 * c1.Imaginary);
        }

        /// <summary>
        /// Multiply a complex number by a regular double.
        /// </summary>
        /// <param name="f1">Float factor.</param>
        /// <param name="c1">Complex factor.</param>
        /// <returns></returns>
        public static Complex operator *(double f1, Complex c1)
        {
            return new Complex(f1 * c1.Real, f1 * c1.Imaginary);
        }

        /// <summary>
        /// Divide a complex number by another.
        /// </summary>
        /// <param name="dividend">Complex number to be divided.</param>
        /// <param name="divisor">Complex number to divide by.</param>
        /// <returns></returns>
        public static Complex operator /(Complex dividend, Complex divisor)
        {
            // Algorithm is:
            // ((ac + bd) / (c^2 + d^2)) + ((bc - ad) / (c^2 + d^2)i
            // where dividend is a + bi, and divisor is b + ci 

            double a = dividend.Real, b = dividend.Imaginary;
            double c = divisor.Real, d = divisor.Imaginary;

            return new Complex(((a * c + b * d) / (c * c + d * d)), ((b * c - a * d) / (c * c + d * d)));
        }

        /// <summary>
        /// Devide a complex number by a regular double.
        /// </summary>
        /// <param name="dividend">Complex number to be divided.</param>
        /// <param name="divisor">Float to divide by.</param>
        /// <returns></returns>
        public static Complex operator /(Complex dividend, double divisor)
        {
            return new Complex(dividend.Real / divisor, dividend.Imaginary / divisor);
        }

        /// <summary>
        /// Check if two Complex numbers are equal.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Complex c1, Complex c2)
        {
            return c1.Real.Equals(c2.Real) && c1.Imaginary.Equals(c2.Imaginary);
        }

        /// <summary>
        /// Check if two Complex numbers are not equal.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Complex c1, Complex c2)
        {
            return !(c1 == c2);
        }

        /// <summary>
        /// Implicitly convert float to the complex number with said float as the real component.
        /// </summary>
        /// <param name="real"></param>
        public static implicit operator Complex(double real)
        {
            return new Complex(real);
        }

        /// <summary>
        /// Explicitly convert complex number to the string representation.
        /// </summary>
        /// <param name="c1"></param>
        public static explicit operator string (Complex c1)
        {
            return c1.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the comjugate.
        /// </summary>
        /// <returns></returns>
        public Complex Conjugate()
        {
            return new Complex(Real, -Imaginary);
        }

        /// <summary>
        /// Print out text representation of the complex number.
        /// </summary>
        /// <param name="realFormat">Formating for real part float</param>
        /// <param name="imgFormat">Formating for imaginary part float</param>
        /// <returns></returns>
        public string ToString(string realFormat, string imgFormat)
        {
            return "(" + Real.ToString(realFormat) + " + " + Imaginary.ToString(imgFormat) + "i)";
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Check if this complex number is equal to an object passed
        /// </summary>
        /// <param name="obj">Any object which can be casted to Complex type.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this == (Complex)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)Real;
        }

        /// <summary>
        /// Print out text representation of the complex number.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + Real + " + " + Imaginary + "i)";
        }

        #endregion
    }
}
