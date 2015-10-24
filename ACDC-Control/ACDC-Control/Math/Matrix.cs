using System;
using Microsoft.SPOT;

namespace ACDC_Control.Math.Types
{
    public struct Matrix
    {
        private float[][] matrix;

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m">Row</param>
        /// <param name="n">Column</param>
        /// <returns></returns>
        public float this[uint m, uint n]
        {
            get
            {
                if (m < M && n < N)
                    return matrix[m][n];
                else
                    throw new IndexOutOfRangeException("Index out of range of matrix.");
            }

            set
            {
                if (m < M && n < N)
                    matrix[m][n] = value;
                else
                    throw new IndexOutOfRangeException("Index out of range of matrix.");
            }
        }

        /// <summary>
        /// Number of rows
        /// </summary>
        public uint M
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of rows
        /// </summary>
        public uint N
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat">2D float array with matrix values.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        public Matrix(float[][] mat, uint rows, uint cols)
        {
            if (mat.Length != rows)
                throw new Exception("Invalid number of rows in matrix!");

            for(int i = 0; i < mat.Length; i++)
                if(mat[i].Length != cols)
                    throw new Exception("Invalid number of columns in matrix row " + i);

            matrix = mat;
            M = rows;
            N = cols;
        }

        /// <summary>
        /// Create an empty matrix with a set number of rows and columns.
        /// </summary>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        public Matrix(uint rows, uint cols)
        {
            matrix = new float[rows][];

            for (int i = 0; i < rows; i++)
                matrix[i] = new float[cols];
            
            M = rows;
            N = cols;
        }

        #region Operators

        /// <summary>
        /// Multiply a matrix by another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.N != b.M)
                throw new ArgumentException("Matrix multiplication dimension error. a.N (rows) must equal b.M (columns)");

            Matrix resultant = new Matrix(a.M, b.N);
            float sum;

            for (uint i = 0; i < a.M; i++)
            {
                for (uint j = 0; j < b.N; j++)
                {
                    sum = 0;

                    for (uint k = 0; k < a.N; k++)
                        sum += a[i, k] * b[k, j];

                    resultant[i, j] = sum;
                }
            }

            return resultant;
        }

        /// <summary>
        /// Muliply a matrix float by a matrix.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix operator *(float s, Matrix a)
        {
            return a * s;
        }

        /// <summary>
        /// Muliply a matrix by a regular float.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, float s)
        {
            Matrix resultant = new Matrix(a.M, a.N);

            for (uint m = 0; m < a.M; m++)
                for (uint n = 0; n < a.N; n++)
                    resultant[m, n] = s * a[m, n];

            return resultant;
        }

        /// <summary>
        /// Add a matrix to another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.M != b.M || a.N != b.N)
                throw new ArgumentException("Matrix subtraction dimension error. Matrix A and B must have equal dimensions.");

            Matrix resultant = new Matrix(a.M, a.N);

            for (uint m = 0; m < a.M; m++)
                for (uint n = 0; n < a.N; n++)
                    resultant[m, n] = a[m, n] + b[m, n];

            return resultant;
        }

        /// <summary>
        /// Subtract a matrix from another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if(a.M != b.M || a.N != b.N)
                throw new ArgumentException("Matrix subtraction dimension error. Matrix A and B must have equal dimensions.");

            Matrix resultant = new Matrix(a.M, a.N);

            for (uint m = 0; m < a.M; m++)
                for (uint n = 0; n < a.N; n++)
                    resultant[m, n] = a[m, n] - b[m, n];

            return resultant;
        }

        /// <summary>
        /// Negate a matrix (negates each element).
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a)
        {
            Matrix resultant = new Matrix(a.M, a.N);

            for (uint m = 0; m < a.M; m++)
                for (uint n = 0; n < a.N; n++)
                    resultant[m, n] = -a[m, n];

            return resultant;
        }

        /// <summary>
        /// Check if two matricies are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.M == b.M && a.N == b.N)
            {
                for (uint m = 0; m < a.M; m++)
                    for (uint n = 0; n < a.N; n++)
                        if (a[m, n] != b[m, n])
                            return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if two matricies are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Explicitly convert matrix to the string representation.
        /// </summary>
        /// <param name="a"></param>
        public static explicit operator string(Matrix a)
        {
            return a.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create an identity matrix of n by n size.
        /// </summary>
        /// <param name="n">Size of (square) matrix.</param>
        public static Matrix CreateIdentityMatrix(uint n)
        {
            Matrix resultant = new Matrix(n, n);

            for (uint i = 0; i < n; i++)
                resultant[i, i] = 1;

            return resultant;
        }

        /// <summary>
        /// Create a row vector from a Vector type.
        /// </summary>
        /// <param name="v">Vector to convert</param>
        public static Matrix CreateRowVector(Vector v)
        {
            Matrix resultant = new Matrix(1, 3);
            resultant[0, 0] = v.X;
            resultant[0, 1] = v.Y;
            resultant[0, 2] = v.Z;

            return resultant;
        }

        /// <summary>
        /// Create a column vector from a Vector type.
        /// </summary>
        /// <param name="v">Vector to convert</param>
        public static Matrix CreateColumnVector(Vector v)
        {
            Matrix resultant = new Matrix(3, 1);
            resultant[0, 0] = v.X;
            resultant[1, 0] = v.Y;
            resultant[2, 0] = v.Z;

            return resultant;
        }

        /// <summary>
        /// Get transpose of this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix GetTranspose()
        {
            Matrix resultant = new Matrix(N, M);

            for (uint m = 0; m < M; m++)
                for (uint n = 0; n < N; n++)
                    resultant[n, m] = matrix[m][n];

            return resultant;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Check if this matrix is equal to an object passed
        /// </summary>
        /// <param name="obj">Any object which can be casted to matrix type.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this == (Matrix)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)M;
        }

        /// <summary>
        /// Print out text representation of the matrix.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";

            for (uint i = 0; i < M; i++)
            {
                for (uint j = 0; j < N - 1; j++)
                    result += matrix[i][j].ToString() + ",";
                result += matrix[i][N-1] + "\n";
            }

            return result;
        }

        #endregion

    }
}
