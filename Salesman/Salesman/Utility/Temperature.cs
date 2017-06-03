using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public class Temperature
    {
        public enum TemperatureFunc
        {
            Standart
        }

        private double start;

        public Double Current { get; private set; }

        private Double minT;

        private Func<Double, Double> decrease;

        public Temperature(TemperatureFunc type = TemperatureFunc.Standart, Double startTemperature = 10000000, Double minTemperature = 0.01)
        {
            start = startTemperature;
            Current = startTemperature;
            minT = minTemperature;
            if (type == TemperatureFunc.Standart)
            {
                decrease = (x) => x * 0.999997;
            }
        }

        public void Start()
        {
            Current = start;
        }

        public Temperature(Func<Double, Double> decrease, Double startTemperature = 100000, Double minTemperature = 0.1)
        {
            Current = startTemperature;
            minT = minTemperature;
            this.decrease = decrease;
        }

        public Boolean IsCold()
        {
            return Current <= minT;
        }

        public void Freezing()
        {
            Current = decrease(Current);
        }
    }
}
