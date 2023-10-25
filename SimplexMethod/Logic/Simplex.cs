using SimplexMethod.Models;

namespace SimplexMethod.Logic;

public record SimplexSnap(
    double[] B,
    double[][] Matrix,
    double[] M,
    double[] F,
    int[] C,
    double[] FunctionVariables,
    bool IsMDone,
    bool[] MArray);

public class Simplex
{
    private readonly Function _function;
    private double[] _functionVariables;
    private double[][] _matrix;
    private double[] _b;
    private bool[] _m;
    private double[] M;
    private double[] _f;
    private int[] _c;
    private bool _isMDone = false;

    public Simplex(Function function, Constraint[] constraints)
    {
        _function = function.IsExtraMax ? function : Canonize(function);

        GetMatrix(constraints);
        GetFunctionArray();
        GetMandF();

        _f = _f.Select(val => -val).ToArray();
    }

    public Tuple<List<SimplexSnap>, SimplexResult> GetResult()
    {
        var snaps = new List<SimplexSnap> { new(_b, _matrix, M, _f, _c, _functionVariables, _isMDone, _m) };
        var result = NextStep();
        var i = 0;

        while (result.Result == SimplexResult.NotYetFound && i < 100)
        {
            CalculateSimplexTableau(result.Index);
            snaps.Add(new SimplexSnap(_b, _matrix, M, _f, _c, _functionVariables, _isMDone, _m));
            result = NextStep();
            i++;
        }

        return new Tuple<List<SimplexSnap>, SimplexResult>(snaps, result.Result);
    }

    private void CalculateSimplexTableau((int, int) Xij)
    {
        _c[Xij.Item2] = Xij.Item1;
        var newJRow = new double[_matrix.Length];

        for (int i = 0; i < _matrix.Length; i++)
        {
            newJRow[i] = _matrix[i][Xij.Item2] / _matrix[Xij.Item1][Xij.Item2];
        }

        var newB = new double[_b.Length];

        for (int i = 0; i < _b.Length; i++)
        {
            if (i == Xij.Item2)
            {
                newB[i] = _b[i] / _matrix[Xij.Item1][Xij.Item2];
            }
            else
            {
                newB[i] = _b[i] - _b[Xij.Item2] / _matrix[Xij.Item1][Xij.Item2] * _matrix[Xij.Item1][i];
            }
        }

        _b = newB;
        var newMatrix = new double[_matrix.Length][];

        for (int i = 0; i < _matrix.Length; i++)
        {
            newMatrix[i] = new double[_c.Length];
            for (int j = 0; j < _c.Length; j++)
            {
                if (j == Xij.Item2)
                {
                    newMatrix[i][j] = newJRow[i];
                }
                else
                {
                    newMatrix[i][j] = _matrix[i][j] - newJRow[i] * _matrix[Xij.Item1][j];
                }
            }
        }

        _matrix = newMatrix;
        GetMandF();
    }

    private void GetMandF()
    {
        M = new double[_matrix.Length];
        _f = new double[_matrix.Length];

        for (var i = 0; i < _matrix.Length; i++)
        {
            double sumF = 0;
            double sumM = 0;
            for (var j = 0; j < _matrix[0].Length; j++)
            {
                if (_m[_c[j]])
                {
                    sumM -= _matrix[i][j];
                }
                else
                {
                    sumF += _functionVariables[_c[j]] * _matrix[i][j];
                }
            }
            M[i] = _m[i] ? sumM + 1 : sumM;
            _f[i] = sumF - _functionVariables[i];
        }
    }

    private SimplexIndexResult NextStep()
    {
        var columnM = GetIndexOfNegativeElementWithMaxAbsoluteValue(M);

        if (_isMDone || columnM == -1)
        {
            _isMDone = true;
            var columnF = GetIndexOfNegativeElementWithMaxAbsoluteValue(_f);

            if (columnF != -1)
            {
                int row = GetIndexOfMinimalRatio(_matrix[columnF], _b);

                if (row != -1)
                {

                    return new SimplexIndexResult((columnF, row), SimplexResult.NotYetFound);
                }
                else
                {
                    return new SimplexIndexResult(null, SimplexResult.Unbounded);
                }
            }
            else
            {
                return new SimplexIndexResult(null, SimplexResult.Found);
            }
        }
        else
        {
            var row = GetIndexOfMinimalRatio(_matrix[columnM], _b);

            return row != -1
                ? new SimplexIndexResult((columnM, row), SimplexResult.NotYetFound)
                : new SimplexIndexResult(null, SimplexResult.Unbounded);
        }
    }

    private int GetIndexOfNegativeElementWithMaxAbsoluteValue(double[] array)
    {
        var index = -1;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0)
            {
                if (!_isMDone || (_isMDone && !_m[i]))
                {
                    if (index == -1)
                    {
                        index = i;
                    }
                    else if (Math.Abs(array[i]) > Math.Abs(array[index]))
                    {
                        index = i;
                    }
                }
            }
        }
        return index;
    }

    private int GetIndexOfMinimalRatio(double[] column, double[] b)
    {
        int index = -1;

        for (int i = 0; i < column.Length; i++)
        {
            if (column[i] > 0 && b[i] > 0)
            {
                if (index == -1)
                {
                    index = i;
                }
                else if (b[i] / column[i] < b[index] / column[index])
                {
                    index = i;
                }
            }
        }

        return index;
    }

    private void GetFunctionArray()
    {
        var funcVars = new double[_matrix.Length];
        for (int i = 0; i < _matrix.Length; i++)
        {
            funcVars[i] = i < _function.Variables.Length ? _function.Variables[i] : 0;
        }

        _functionVariables = funcVars;
    }

    private Function Canonize(Function function)
    {
        var newFuncVars = function.Variables.Select(val => -val).ToArray();
        return new Function(newFuncVars, -function.C, true);
    }

    private double[][] AppendColumn(double[][] matrix, double[] column)
    {
        var newMatrix = new double[matrix.Length + 1][];

        for (int i = 0; i < matrix.Length; i++)
        {
            newMatrix[i] = matrix[i];
        }

        newMatrix[matrix.Length] = column;
        return newMatrix;
    }

    private T[] Append<T>(T[] array, T element)
    {
        var newArray = new T[array.Length + 1];

        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }

        newArray[array.Length] = element;
        return newArray;
    }

    private double[] GetColumn(double value, int place, int length)
    {
        var newColumn = new double[length];

        for (int k = 0; k < length; k++)
        {
            newColumn[k] = k == place ? value : 0;
        }

        return newColumn;
    }

    private void GetMatrix(Constraint[] constraints)
    {
        foreach (var constraint in constraints)
        {
            if (constraint.B < 0)
            {
                for (int j = 0; j < constraint.Variables.Length; j++)
                {
                    constraint.Variables[j] = -constraint.Variables[j];
                }

                constraint.Sign = constraint.Sign switch
                {
                    ">=" => "<=",
                    "<=" => ">=",
                    _ => constraint.Sign
                };
            }
        }

        var matrix = new double[constraints.First().Variables.Length][];

        for (int i = 0; i < constraints.First().Variables.Length; i++)
        {
            matrix[i] = new double[constraints.Length];
            for (int j = 0; j < constraints.Length; j++)
            {
                matrix[i][j] = constraints[j].Variables[i];
            }
        }

        var appendixMatrix = new double[0][];
        var Bs = new double[constraints.Length];

        for (int i = 0; i < constraints.Length; i++)
        {
            var current = constraints[i];
            Bs[i] = current.B;

            appendixMatrix = current.Sign switch
            {
                ">=" => AppendColumn(appendixMatrix, GetColumn(-1, i, constraints.Length)),
                "<=" => AppendColumn(appendixMatrix, GetColumn(1, i, constraints.Length)),
                _ => appendixMatrix
            };
        }

        var newMatrix = new double[constraints.First().Variables.Length + appendixMatrix.Length][];

        for (var i = 0; i < constraints.First().Variables.Length; i++)
        {
            newMatrix[i] = matrix[i];
        }

        for (var i = constraints.First().Variables.Length; i < constraints.First().Variables.Length + appendixMatrix.Length; i++)
        {
            newMatrix[i] = appendixMatrix[i - constraints.First().Variables.Length];
        }

        var hasBasicVar = new bool[constraints.Length];
        _c = new int[constraints.Length];
        var ci = 0;

        for (int i = 0; i < newMatrix.Length; i++)
        {
            var hasOnlyNulls = true;
            var hasOne = false;
            var onePosition = (0, 0);

            for (int j = 0; j < constraints.Length; j++)
            {
                if (newMatrix[i][j] == 1)
                {
                    if (hasOne)
                    {
                        hasOnlyNulls = false;
                        break;
                    }
                    else
                    {
                        hasOne = true;
                        onePosition = (i, j);
                    }
                }
                else if (newMatrix[i][j] != 0)
                {
                    hasOnlyNulls = false;
                    break;
                }
            }

            if (hasOnlyNulls && hasOne)
            {
                hasBasicVar[onePosition.Item2] = true;
                _c[ci] = onePosition.Item1;
                ci++;
            }
        }

        _m = new bool[newMatrix.Length];

        for (int i = 0; i < newMatrix.Length; i++)
        {
            _m[i] = false;
        }

        for (int i = 0; i < constraints.Length; i++)
        {
            if (!hasBasicVar[i])
            {
                var basicColumn = new double[constraints.Length];

                for (int j = 0; j < constraints.Length; j++)
                {
                    basicColumn[j] = j == i ? 1 : 0;
                }

                newMatrix = AppendColumn(newMatrix, basicColumn);
                _m = Append(_m, true);
                _c[ci] = newMatrix.Length - 1;
                ci++;
            }
        }

        _b = Bs;
        this._matrix = newMatrix;
    }
}