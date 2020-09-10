using System;
using System.Linq;
using testPr.Exceptions;
using testPr.Graphs.Components;

namespace testPr.Graphs
{
    internal static class GraphPointsConverter
    {
        internal static GraphDot[] StringTo2DGraphDots(string configRow)
        {
            const int _dementionsCount = 2;//количество координат для определения точки

            var _points = configRow.Split(')').Select(p => p.Replace(" ", "")
                                                                    .Replace(",(", "")
                                                                    .Replace("(", ""))
                                                    .Where(x => x != "").ToList();

            GraphDot[] _XYDots = new GraphDot[_points.Count()];

            for (int i = 0; i < _points.Count();)
            {
                var dots = _points[i].Split(';');
                double _xValue, _yValue;
                if (dots.Count() == _dementionsCount
                    && Double.TryParse(dots[0], out _xValue)
                    && Double.TryParse(dots[1], out _yValue))
                {
                    _XYDots[i].X = _xValue;
                    _XYDots[i].Y = _yValue;
                }
                else
                {
                    throw new ParsingException($"Произошла ошибка при попытке формирования точек графика по данным из App.config - точка : ({_points[i]})");
                }
                i++;
            }

            return _XYDots;
        }
    }
}
