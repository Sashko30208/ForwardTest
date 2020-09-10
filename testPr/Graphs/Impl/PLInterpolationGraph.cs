using System;
using System.Configuration;
using System.Linq;
using testPr.Exceptions;
using testPr.Graphs.Components;

namespace testPr.Graphs.Impl
{
    /// <summary>
    /// Piecewise linear interpolation
    /// </summary>
    internal class PLInterpolationGraph : IGraph<double, double>
    {
        private const int _minPointsCount = 2;

        private GraphDot[] _XYDots;
        internal GraphDot[] XYDots
        {
            get
            {
                return _XYDots;
            }
            set
            {
                if (value != null && value.Length > _minPointsCount && CheckGraphValues(value))
                    _XYDots = value;
                else
                    throw new GraphDeclarationException($"График должен быть задан не менее чем {_minPointsCount} точками с неотрицательными величинами Х и У, которые расположены по возрастанию относительно оси Х");
            }
        }

        internal PLInterpolationGraph()
        {
            SetPoints();
        }

        private void SetPoints()
        {
            try
            {
                var configSection = (ConfigurationManager.GetSection("DVSEngineCrankshaftGraph") as System.Collections.Hashtable);
                var graphDots = GraphPointsConverter.StringTo2DGraphDots((string)configSection["MV_values"]);
                XYDots = graphDots;
            }
            catch (GraphDeclarationException declatationEx)
            { throw declatationEx; }
            catch (ParsingException parsEx)
            {
                throw parsEx;
            }
            catch (Exception ex)
            {
                throw new ParsingException($"Произошла ошибка при попытке чтения данных о графике зависимости крутящего момента M, вырабатываемого двигателем, от скорости вращения коленвала V из App.Config:" + "/n" + ex.Message);
            }
        }

        public double GetY(double valX)
        {
            if (_XYDots.Length < _minPointsCount)
            {
                throw new GraphDeclarationException("Не указано достаточное количество точек для задания графика (График не инициализирован)");
            }

            GraphDot lowerPoint = XYDots.Where(p => p.X <= valX).OrderByDescending(p => p.X).FirstOrDefault();
            GraphDot higherPoint = XYDots.Where(p => p.X >= valX).OrderBy(p => p.X).FirstOrDefault();

            double dX = higherPoint.X - lowerPoint.X;
            if (dX == 0)
            {
                return higherPoint.Y;
            }
            else
            {
                double dY = higherPoint.Y - lowerPoint.Y;
                double coeff = dY / dX;
                return (valX - lowerPoint.X) * coeff + lowerPoint.Y;
            }
        }

        /// <summary>
        /// Проверка, что точки указаны строго в порядке возрастания по оси Х и положительны
        /// </summary>
        /// <param name="dots">список точек</param>
        /// <returns>корректность точек</returns>
        private bool CheckGraphValues(GraphDot[] dots)
        {
            bool checkResult = true;
            if (dots[0].X >= 0 && dots[0].Y >= 0)
            {
                int dotsLength = dots.Length;
                for (int dot = 1; dot < dotsLength; dot++)
                {
                    if (dots[dot].X <= dots[dot - 1].X || dots[dot].X < 0 || dots[dot].Y < 0)
                    { checkResult = false; break; }
                }
            }
            else
            {
                checkResult = false;
            }
            return checkResult;
        }
    }
}
