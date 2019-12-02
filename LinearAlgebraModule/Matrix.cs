
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace LinearAlgebraModule
{
    public class Matrix
    {
        private double[,] container;

        public Matrix(List<List<double>> rows)
        {
            this.Rows = rows;
        }

        public Matrix(IEnumerable<double> zigzagElements, int size)
        {
            this.Rows = IterateZigzagBypass(zigzagElements, size);
        }

        public List<List<double>> Rows
        {
            get
            {
                return Enumerable.Range(0, this.container.GetLength(0))
                    .ToList().Select(x => this.IterateRow(x).ToList()).ToList();
            }

            private set
            {
                this.container = this.TransformRowsToMatrix(value);
            }
        }

        public List<List<double>> Columns
        {
            get
            {
                return Enumerable.Range(0, this.container.GetLength(1))
                    .ToList()
                    .Select(x => this.IterateColumn(x).ToList()).ToList();
            }

            private set
            {
                this.container = this.TransformColumnsToMatrix(value);
            }
        }

        public IEnumerable<double> GetMainDiagonal
        {
            get
            {
                foreach (var row in this.Rows)
                {
                    yield return row.Skip(this.GetRowIndex(row)).Take(1).First();
                }
            }
        }

        public IEnumerable<double> GetSideDiagonal
        {
            get
            {
                foreach (var element in this.IterateSideDiagonal(this.Rows.Count - 1))
                {
                    yield return element;
                }
            }
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Contract.Requires(m1.Columns.Count == m2.Rows.Count);
            var result = new List<List<double>>();
            for (int i = 0; i < m1.Rows.Count; i++)
            {
                var row = new List<double>();
                for (int j = 0; j < m2.Columns.Count; j++)
                {
                    row.Add(m1.Rows[i].Zip(m2.Columns[j], (x, y) => x * y).Sum());
                }
                result.Add(row);
            }
            return new Matrix(result);
        }

        public static IEnumerable<sbyte> GetBypass(IEnumerable<sbyte> bytes, int size)
        {
            var list = new List<List<double>>();
            for (int i=0; i<bytes.Count()/size; i++)
            {
                list.Add(new List<double>(bytes.Skip(i * size).Take(size).Select(x => (double)x)));
            }
            return new Matrix(list).SideDiagonalBypass().Select(x => (sbyte)x);
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Contract.Requires(m1.Rows.Count == m2.Rows.Count);
            var result = new List<List<double>>();
            Enumerable.Range(0, m1.Rows.Count)
                .ToList()
                .ForEach(x => result.Add(m1.Rows[x].Zip(m2.Rows[x], (y, z) => y + z).ToList()));
            return new Matrix(result);
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Contract.Requires(m1.Rows.Count == m2.Rows.Count);
            var result = new List<List<double>>();
            Enumerable.Range(0, m1.Rows.Count)
                .ToList()
                .ForEach(x => result.Add(m1.Rows[x].Zip(m2.Rows[x], (y, z) => y - z).ToList()));
            return new Matrix(result);
        }

        public static Matrix operator *(Matrix m, double coefficient)
        {
            List<List<double>> result = m.Rows.Select(x => x.Select(y => y * coefficient).ToList()).ToList();
            return new Matrix(result);
        }

        public static Matrix operator /(Matrix m, double coefficient)
        {
            List<List<double>> result = m.Rows.Select(x => x.Select(y => y / coefficient).ToList()).ToList();
            return new Matrix(result);
        }

        public static bool operator ==(Matrix m1, Matrix m2)
        {
            if (m1.Rows.Count != m2.Rows.Count || m1.Columns.Count != m2.Columns.Count) return false;
            var result = Enumerable.Range(0, m1.Rows.Count)
                .Select(x => m1.Rows[x].SequenceEqual(m2.Rows[x]));
            return !result.Contains(false);
        }

        public static bool operator !=(Matrix m1, Matrix m2)
        {
            if (m1.Rows.Count != m2.Rows.Count || m1.Columns.Count != m2.Columns.Count) return true;
            var result = Enumerable.Range(0, m1.Rows.Count)
                .Select(x => m1.Rows[x].SequenceEqual(m2.Rows[x]));
            return result.Contains(false);
        }

        public static Matrix GetAllianceMatrix(Matrix m)
        {
            var rows = new List<List<double>>(m.Rows);
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows.First().Count; j++)
                {
                    rows[i][j] = m.GetAlgebraicComplement(i, j);
                }
            }
            return new Matrix(rows).Transpose();
        }

        public static Matrix GetReverseMatrix(Matrix m)
        {
            Contract.Requires(m.Rows.Count == m.Columns.Count && m.Rows.Count != 0);
            return GetAllianceMatrix(m) / GetMatrixDeterminant(m);
        }

        public IEnumerable<double> SideDiagonalBypass()
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                var diagonal = this.IterateSideDiagonal(i).ToList();
                if (i % 2 == 1)
                {
                    diagonal.Reverse();
                }
                foreach (var element in diagonal)
                {
                    yield return element;
                }
            }
        }

        public double GetSecondOrderDeterminant()
        {
            Contract.Requires(this.Rows.Count == 2 && this.Columns.Count == 2);
            return this.GetMainDiagonal.Aggregate((x, y) => x * y) - this.GetSideDiagonal.Aggregate((x, y) => x * y);
        }

        public double GetMinor(int row, int column)
        {
            Matrix reduced = this.ReduceMatrix(row, column);
            return GetMatrixDeterminant(reduced);
        }

        public double GetAlgebraicComplement(int row, int column)
        {
            return Math.Pow(-1, row + column) * this.GetMinor(row, column);
        }

        public static double GetMatrixDeterminant(Matrix matrix)
        {
            Contract.Requires(matrix.Rows.Count > 0 && matrix.Columns.Count > 0);
            if (matrix.Rows.Count == 2 && matrix.Columns.Count == 2) return matrix.GetSecondOrderDeterminant();
            var firstRow = matrix.Rows.First();
            if (matrix.Rows.Count == 1) return firstRow.First();
            double result = 0;
            for (int i = 0; i < matrix.Columns.Count; i++)
            {
                result += matrix.Rows[0][i] * matrix.GetAlgebraicComplement(0, i);
            }
            return result;
        }

        public Matrix Transpose()
        {
            return new Matrix(this.Columns);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix matrix &&
                   EqualityComparer<List<List<double>>>.Default.Equals(Rows, matrix.Rows);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private IEnumerable<double> IterateRow(int row)
        {
            for (int i = 0; i < this.container.GetLength(1); i++)
            {
                yield return this.container[row, i];
            }
        }

        private static List<List<double>> IterateZigzagBypass(IEnumerable<double> bypass, int size)
        {
            var result = new List<List<double>>();
            Enumerable.Range(0, size).ToList()
                .ForEach(x => result.Add(new List<double>()));
            int additional = 1;
            int index = 1;
            int accumulatedLength = 0;
            while (index != 0)
            {
                var sequence = bypass.Skip(accumulatedLength).Take(index).ToList();
                accumulatedLength += sequence.Count;
                if (sequence.Count % 2 == 0)
                {
                    sequence.Reverse();
                }
                for (int i=0; i<sequence.Count; i++)
                {
                    if (additional == 1)
                    {
                        result[sequence.Count - i - 1].Add(sequence[i]);
                    }
                    else
                    {
                        result[size - i - 1].Add(sequence[i]);
                    }
                }
                if (index == size)
                {
                    additional = -1;
                }
                index += additional;
            }
            return result;
        }

        private IEnumerable<double> IterateSideDiagonal(int index)
        {
            foreach (var row in this.Rows)
            {
                yield return row.Skip(index + 1 - this.GetRowIndex(row) - 1).Take(1).First();
            }
        }

        private IEnumerable<double> IterateColumn(int column)
        {
            for (int i = 0; i < this.container.GetLength(0); i++)
            {
                yield return this.container[i, column];
            }
        }

        private Matrix ReduceMatrix(int row, int column)
        {
            List<List<double>> rows = new List<List<double>>();
            foreach (var matrixRow in this.Rows)
            {
                if (this.GetRowIndex(matrixRow) != row)
                {
                    matrixRow.RemoveAt(column);
                    rows.Add(matrixRow);
                }
            }
            return new Matrix(rows);
        }

        private int GetRowIndex(List<double> row)
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                if (row.SequenceEqual(this.Rows[i])) return i;
            }
            return -1;
        }

        private int GetColumnIndex(List<double> column)
        {
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (column.SequenceEqual(this.Columns[i])) return i;
            }
            return -1;
        }

        private double[,] TransformColumnsToMatrix(List<List<double>> columns)
        {
            Contract.Requires(columns.Count > 0 && columns.First().Count > 0);
            double[,] result = new double[columns.First().Count, columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < columns.First().Count; j++)
                {
                    result[j, i] = columns[i][j];
                }
            }
            return result;
        }

        private double[,] TransformRowsToMatrix(List<List<double>> rows)
        {
            Contract.Requires(rows.Count > 0 && rows.First().Count > 0);
            double[,] result = new double[rows.Count, rows.First().Count];
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows.First().Count; j++)
                {
                    result[i, j] = rows[i][j];
                }
            }
            return result;
        }
    }
}
