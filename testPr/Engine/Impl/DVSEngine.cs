using System;
using System.Configuration;
using testPr.Exceptions;
using testPr.Graphs;
using testPr.Graphs.Impl;

namespace testPr.Engine.Impl
{
    internal class DVSEngine : IEngine
    {
        #region параметры тестируемого двигателя
        private double _I_value;    //Момент инерции
        private double _T_overheat; //Температура перегрева
        private double _HM_value;   //Коэффициент зависимости скорости нагрева от крутящего момента
        private double _HV_value;   //Коэффициент зависимости скорости нагрева от скорости вращения коленвала
        private double _HC_value;   //Коэффициент зависимости скорости охлаждения от температуры двигателя и окружающей среды

        private double _crankshaftSpeed = 0.0;  //Скорость вращения коленвала
        private double _engineTemperature = 0.0;//Температура двигателя

        private double _roomTemperature = 0.0;

        private double _torque            //Крутящий момент
        {
            get { return _graph.GetY(_crankshaftSpeed); }
        }
        private double _acceleration
        {
            get
            {
                if (_torque > 0 && _I_value > 0)
                    return _torque / _I_value;
                else throw new Exception("M и I должны быть определены и иметь значение > 0");
            }
        }

        private double _heatingSpeed
        {
            get { return (GetCurrentHeatingSpeed() - GetCurrentСoolingSpeed()) * _timeStep; }
        }

        IGraph<double, double> _graph;
        #endregion

        private double _timeStep = 0.10;
        private double _dt = 0.0;

        internal DVSEngine(double roomTemp)
        {
            _engineTemperature = _roomTemperature = roomTemp;
            SetParams();
        }

        public void Simulate()
        {
            SimulateOverheat();
        }

        private void SimulateOverheat()
        {
            Console.WriteLine("Симуляция работы ДВС для определения времени перегрева");
            while (_engineTemperature < _T_overheat)
            {
                _engineTemperature += _heatingSpeed;
                _crankshaftSpeed += _acceleration * _timeStep;
                Console.WriteLine($"Прошло {_dt:0.##} секунд. Температура двигателя: {_engineTemperature:0.#} градусов цельсия при {_crankshaftSpeed:0.##} оборотах коленвала в секунду");
                if (_engineTemperature < _T_overheat)
                    MakeTimeStep();
            }
            Console.WriteLine($"Прошло {_dt:0.##} секунд. Температура двигателя: {_engineTemperature:0.#} градусов цельсия при температуре перегрева: {_T_overheat}");
        }

        private void MakeTimeStep()
        {
            _dt += _timeStep;
        }

        private double GetCurrentHeatingSpeed()
        {
            return _torque * _HM_value + Math.Pow(_crankshaftSpeed, 2) * _HV_value;
        }

        private double GetCurrentСoolingSpeed()
        {
            return _HC_value * (_roomTemperature - _engineTemperature);
        }

        private void SetParams()
        {
            var configSection = (ConfigurationManager.GetSection("DVSEngine") as System.Collections.Hashtable);
            if (!Double.TryParse(((string)configSection["I_value"]).Replace(".", ","), out _I_value) ||
                !Double.TryParse(((string)configSection["T_overheat"]).Replace(".", ","), out _T_overheat) ||
                !Double.TryParse(((string)configSection["Hm_value"]).Replace(".", ","), out _HM_value) ||
                !Double.TryParse(((string)configSection["Hv_value"]).Replace(".", ","), out _HV_value) ||
                !Double.TryParse(((string)configSection["HC_value"]).Replace(".", ","), out _HC_value))
            {
                throw new ParsingException($"Произошла ошибка при попытке установки тестовых параметров двигателя из App.Config");
            }
            _graph = new PLInterpolationGraph();
        }
    }
}