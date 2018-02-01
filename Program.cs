using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCodeToCSharpConversion
{
    class Program
    {
        static void Main(string[] args)
        {
            string strFileName = "train_dataset.csv";
            DataTable dt = GetDataFromCSVFile(strFileName);
            double[][] matrix = ConvertDataTableToMatrices(dt);

        }

        private static double[][] ConvertDataTableToMatrices(DataTable dt)
        {
            double[] y = dt.AsEnumerable().Select(x => Convert.ToDouble(x[0])).ToArray<double>();
            double[] X1 = dt.AsEnumerable().Select(x => Convert.ToDouble(x[1])).ToArray<double>();
            double[] X2 = dt.AsEnumerable().Select(x => Convert.ToDouble(x[2])).ToArray<double>();
            double[] X3 = dt.AsEnumerable().Select(x => Convert.ToDouble(x[3])).ToArray<double>();
            double[] X4 = dt.AsEnumerable().Select(x => Convert.ToDouble(x[4])).ToArray<double>();

            double[][] matrix = new double[][] { y, X1, X2, X3, X4 };

            CreateVars(matrix);

            return matrix;
        }

        private static void CreateVars(double[][] matrix)
        {
            int MATRIX_ROWS = matrix[0].Length;
            int MATRIX_COLUMNS = matrix.Length - 1;

            double[,] X = new double[MATRIX_ROWS, MATRIX_COLUMNS];

            for (int i = 0; i < MATRIX_ROWS; i++)
            {
                for (int j = 0; j < MATRIX_COLUMNS; j++)
                {
                    X[i, j] = matrix[j][i];
                }
            }

            double[] y = new double[MATRIX_ROWS];

            for (int i = 0; i < MATRIX_ROWS; i++)
            {
                y[i] = matrix[0][i];

            }
            double[] theta = new double[] { 1, 1, 1, 1, 1 };
            double result = OrdinaryLeastSquares(theta, y, X);

        }

        //Ordinary Least Square Regression
        private static double OrdinaryLeastSquares(double[] theta, double[] y, double[,] X)
        {
            double[] beta = theta.Skip(1).ToArray<double>();
            double sigma2 = theta[1];

            if (sigma2 <= 0)
            {
                return double.NaN;
            }

            int n = X.GetLength(0);
            double[,] e = MatrixOperations(y, MatrixOperations(X, beta, "Multiply"), "Subtract");
            double[,] TransposeProduct = MatrixOperations(MatrixTranspose(e), e, "Multiply");
            double FinalResult = ((-n / 2) * Math.Log(2 * Math.PI)) - ((n / 2) * Math.Log(sigma2)) - MatrixDivide(TransposeProduct, 2 * sigma2)[0, 0];

            return -FinalResult;
        }

        private static double[,] MatrixDivide(double[,] Matrix1, double divisor)
        {
            int Row_Num = Matrix1.GetLength(0);
            int Col_Num = Matrix1.GetLength(1);

            double[,] ResultMatrix = new double[Row_Num, Col_Num];

            for (int i = 0; i < Row_Num; i++)
            {
                for (int j = 0; j < Col_Num; j++)
                {
                    ResultMatrix[i, j] = Convert.ToDouble(Matrix1[i, j] / divisor);
                }
            }

            return ResultMatrix;
        }

        private static double[,] MatrixTranspose(double[,] Matrix1)
        {
            int Row_Num = Matrix1.GetLength(0);
            int Col_Num = Matrix1.GetLength(1);

            double[,] ResultMatrix = new double[Col_Num, Row_Num];

            for (int i = 0; i < Col_Num; i++)
            {
                for (int j = 0; j < Row_Num; j++)
                {
                    ResultMatrix[i, j] = Matrix1[j, i];
                }
            }

            return ResultMatrix;
        }


        private static double[,] MatrixOperations(double[] Matrix1, double[,] Matrix2, string Operation)
        {
            int Row_Num = Matrix1.GetLength(0);
            int Col_Num = Matrix2.GetLength(1);
            double[,] ResultMatrix;


            ResultMatrix = new double[Row_Num, Col_Num];

            int dimension = ResultMatrix.Rank;
            for (int i = 0; i < Row_Num; i++)
            {
                for (int j = 0; j < Col_Num; j++)
                {

                    ResultMatrix[i, j] = 0;
                    for (int k = 0; k < dimension - 1; k++)
                    {
                        if (Operation == "Multiply")
                        {
                            ResultMatrix[i, j] += Matrix1[k] * Matrix2[i, k];
                        }
                        else if (Operation == "Subtract")
                        {
                            ResultMatrix[i, j] = Matrix1[k] - Matrix2[i, k];
                        }
                    }
                }
            }

            return ResultMatrix;
        }

        private static double[,] MatrixOperations(double[,] Matrix1, double[] Matrix2, string Operation)
        {
            int Row_Num = Matrix1.GetLength(0);
            int Col_Num = 1;

            double[,] ResultMatrix = new double[Row_Num, Col_Num];
            int dimension = ResultMatrix.Rank;
            for (int i = 0; i < Row_Num; i++)
            {
                for (int j = 0; j < Col_Num; j++)
                {

                    ResultMatrix[i, j] = 0;
                    for (int k = 0; k < dimension; k++)
                    {
                        if (Operation == "Multiply")
                        {
                            ResultMatrix[i, j] += Matrix1[i, k] * Matrix2[k];
                        }
                        else if (Operation == "Subtract")
                        {
                            ResultMatrix[i, j] = Matrix1[i, k] - Matrix2[k];
                        }

                    }
                }
            }

            return ResultMatrix;
        }

        private static double[,] MatrixOperations(double[,] Matrix1, double[,] Matrix2, string Operation)
        {
            int Row_Num = Matrix1.GetLength(0);
            int Col_Num = Matrix2.GetLength(1);

            double[,] ResultMatrix = new double[Row_Num, Col_Num];
            int dimension = ResultMatrix.Rank;
            for (int i = 0; i < Row_Num; i++)
            {
                for (int j = 0; j < Col_Num; j++)
                {

                    ResultMatrix[i, j] = 0;
                    for (int k = 0; k < dimension; k++)
                    {
                        if (Operation == "Multiply")
                        {
                            ResultMatrix[i, j] += Matrix1[i, k] * Matrix2[k, j];
                        }
                        else if (Operation == "Subtract")
                        {
                            ResultMatrix[i, j] = Matrix1[i, k] - Matrix2[k, j];
                        }

                    }
                }
            }

            return ResultMatrix;
        }


        private static DataTable GetDataFromCSVFile(string strFileName)
        {
            string[] Lines = File.ReadAllLines(strFileName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }

            return dt;
        }


    }
}
