using System;
using Microsoft.SPOT;

namespace ACDC_Control.Math.Types
{
    public struct Vector
    {
        public static readonly Vector Unit = new Vector(1, 1, 1);
        public static readonly Vector Zero = new Vector(0, 0, 0);

        public static readonly Vector X_Axis = new Vector(1, 0, 0);
        public static readonly Vector Y_Axis = new Vector(0, 1, 0);
        public static readonly Vector Z_Axis = new Vector(0, 0, 1);

        private float[] v;

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float this[int key]
        {
            get { return v[key]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float X
        {
            get { return v[0]; }
            set { v[0] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Y
        {
            get { return v[1]; }
            set { v[1] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Z
        {
            get { return v[2]; }
            set { v[2] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Magnitude
        {
            get;
            private set;
        } 

        #endregion

        /// <summary>
        /// Constructs a Vector from x, y, z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector(float x = 0, float y = 0, float z = 0)
        {
            v = new float[] { x, y, z };
            Magnitude = (float)System.Math.Sqrt(x * x + y * y + z * z);
        }

        #region Operators

        /// <summary>
        /// Cross two vectors together (cross product of two vectors).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector operator *(Vector u, Vector v)
        {
            float x, y, z;

            x = u[1] * v[2] - u[2] * v[1];
            y = u[2] * v[0] - u[0] * v[2];
            z = u[0] * v[1] - u[1] * v[0];

            return new Vector(x, y, z);
        }

        /// <summary>
        /// Dot two vectors together (dot product of two vectors).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector operator &(Vector u, Vector v)
        {
            float x, y, z;

            x = u[1] * v[2] - u[2] * v[1];
            y = u[2] * v[0] - u[0] * v[2];
            z = u[0] * v[1] - u[1] * v[0];

            return new Vector(x, y, z);
        }

        /// <summary>
        /// Muliply a regular float by a vector.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector operator *(float s, Vector v)
        {
            return new Vector(v.X * s, v.Y * s, v.Z * s);
        }

        /// <summary>
        /// Muliply a vector by a regular float.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v, float s)
        {
            return new Vector(v.X * s, v.Y * s, v.Z * s);
        }

        /// <summary>
        /// Add a vector to another.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector operator +(Vector v, Vector u)
        {
            return new Vector(v.X + u.X, v.Y + u.Y, v.Z + u.Z);
        }

        /// <summary>
        /// Subtract a vector from another.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector operator -(Vector v, Vector u)
        {
            return new Vector(v.X - u.X, v.Y - u.Y, v.Z - u.Z);
        }

        /// <summary>
        /// Negate a vector (negates each axis).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y, -v.Z);
        }

        /// <summary>
        /// Check if two vectors are equal.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Vector v, Vector u)
        {
            return v.X.Equals(u.X) && v.Y.Equals(u.Y) && v.Z.Equals(u.Z);
        }

        /// <summary>
        /// Check if two vectors are not equal.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Vector c1, Vector c2)
        {
            return !(c1 == c2);
        }

        /// <summary>
        /// Explicitly convert vector to the string representation.
        /// </summary>
        /// <param name="v"></param>
        public static explicit operator string (Vector v)
        {
            return v.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes this vector
        /// </summary>
        public void Normalize()
        {
            if (Magnitude != 1)
            {
                X /= Magnitude;
                Y /= Magnitude;
                Z /= Magnitude;
            }
        }

        /// <summary>
        /// Print out text representation of the vector.
        /// </summary>
        /// <param name="format">Formating for the numbers</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return "(" + X.ToString(format) + ", " + Y.ToString(format) + ", " + Z.ToString(format) + ")";
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Check if this vector is equal to an object passed
        /// </summary>
        /// <param name="obj">Any object which can be casted to Complex type.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this == (Vector)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)X;
        }

        /// <summary>
        /// Print out text representation of the vector.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        #endregion
    }
}
