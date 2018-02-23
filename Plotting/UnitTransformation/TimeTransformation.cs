/*
Copyright(c) <2018> <University of Washington>
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Plotting.UnitTransformation
{
    public interface IUnitMeasurement
    {
        string UnitName { get; }
        string UnitCode { get; }
        Func<double?, double?> Apply { get; }
        bool IsAllowNormalization { get; }
    }

    public class TimeSecondsMeasurement : IUnitMeasurement
    {
        public string UnitName => "s";
        public string UnitCode => "TIME_Seconds";
        public  Func<double?, double?> Apply => value => value * 1.0;
        public bool IsAllowNormalization => false;
        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Seconds ({UnitName})";
        }
    }

    public class TimeMinutesMeasurement : IUnitMeasurement
    {
        public string UnitName => "m";
        public string UnitCode => "TIME_Minutes";
        public Func<double?, double?> Apply => value => value * (1d / 60d);
        public bool IsAllowNormalization => false;
        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Minutes ({UnitName})";
        }
    }

    public class TimeHoursMeasurement : IUnitMeasurement
    {
        public string UnitName => "h";
        public string UnitCode => "TIME_Hours";
        public Func<double?, double?> Apply => value => value * (1d / 3600d);
        public bool IsAllowNormalization => false;
        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Hours ({UnitName})";
        }
    }

    public class TimeDaysMeasurement : IUnitMeasurement
    {
        public string UnitName => "d";
        public string UnitCode => "TIME_Days";
        public Func<double?, double?> Apply => value => value * (1d / 3600d * 24);
        public bool IsAllowNormalization => false;
        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Days ({UnitName})";
        }
    }

    public class CurrentAmperMeasurement : IUnitMeasurement
    {
        public string UnitName => "A";
        public string UnitCode => $"CURRENT_{UnitName}";
        public bool IsAllowNormalization => true;
        public Func<double?, double?> Apply => value => value * 0.001;
    }

    public class CurrentMilliamperMeasurement : IUnitMeasurement
    {
        public string UnitName => "mA";
        public string UnitCode => $"CURRENT_{UnitName}";
        public bool IsAllowNormalization => true;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class CurrentMicroamperMeasurement : IUnitMeasurement
    {
        public string UnitName => "μA";
        public string UnitCode => $"CURRENT_uA";
        public bool IsAllowNormalization => true;
        public Func<double?, double?> Apply => value => value * 1000.0;
    }

    public class CapacityAmperHoursMeasurement : IUnitMeasurement
    {
        public string UnitName => "Ah";
        public string UnitCode => $"CAPACITY_{UnitName}";
        public bool IsAllowNormalization => true;
        public Func<double?, double?> Apply => value => value * 0.001;
    }

    public class CapacityMilliamperHoursMeasurement : IUnitMeasurement
    {
        public string UnitName => "mAh";
        public string UnitCode => $"CAPACITY_{UnitName}";
        public bool IsAllowNormalization => true;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class TemperatureCelciusMeasurement : IUnitMeasurement
    {
        public string UnitName => "C";
        public string UnitCode => "TEMPERATURE_Celcius";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }


    public class VoltageMeasurement : IUnitMeasurement
    {
        public string UnitName => "V";
        public string UnitCode => $"VOLTAGE_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class VoltageMillivoltMeasurement : IUnitMeasurement
    {
        public string UnitName => "mV";
        public string UnitCode => $"VOLTAGE_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1000.0;
    }

    public class VoltageMicrovoltMeasurement : IUnitMeasurement
    {
        public string UnitName => "μV";
        public string UnitCode => $"VOLTAGE_uV";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1000.0 * 1000.0;
    }

    public class EnergyKiloWattHoursMeasurement : IUnitMeasurement
    {
        public string UnitName => "kWh";
        public string UnitCode => $"ENERGY_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 0.001;
    }

    public class EnergyWattHoursMeasurement : IUnitMeasurement
    {
        public string UnitName => "Wh";
        public string UnitCode => $"ENERGY_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class PowerKiloWattMeasurement : IUnitMeasurement
    {
        public string UnitName => "kW";
        public string UnitCode => $"POWER_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 0.001;
    }

    public class PowerWattMeasurement : IUnitMeasurement
    {
        public string UnitName => "W";
        public string UnitCode => $"POWER_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class ResistanceOhmMeasurement : IUnitMeasurement
    {
        public string UnitName => "Ohm";
        public string UnitCode => $"RESISTANCE_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class ResistanceMilliOhmMeasurement : IUnitMeasurement
    {
        public string UnitName => "mOhm";
        public string UnitCode => $"RESISTANCE_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1000.0;
    }

    public class ResistanceKilloOhmMeasurement : IUnitMeasurement
    {
        public string UnitName => "kOhm";
        public string UnitCode => $"RESISTANCE_{UnitName}";
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 0.001;
    }

    public class NullMeasurement : IUnitMeasurement
    {
        public string UnitName => string.Empty;
        public string UnitCode => string.Empty;
        public bool IsAllowNormalization => false;
        public Func<double?, double?> Apply => value => value * 1.0;
    }

    public class UnitMeasurementProvider
    {
        private readonly Dictionary<string, IUnitMeasurement> _timeTranformation = new Dictionary<string, IUnitMeasurement>();
        private readonly Dictionary<string, IUnitMeasurement> _currentTranformation = new Dictionary<string, IUnitMeasurement>();
        private readonly Dictionary<string, IUnitMeasurement> _capacityTranformation = new Dictionary<string, IUnitMeasurement>();

        private readonly Dictionary<string, IUnitMeasurement> _unitTranformation = new Dictionary<string, IUnitMeasurement>();


        public UnitMeasurementProvider()
        {
            Register(_unitTranformation, new TimeSecondsMeasurement());
            Register(_unitTranformation, new TimeMinutesMeasurement());
            Register(_unitTranformation, new TimeHoursMeasurement());
            Register(_unitTranformation, new TimeDaysMeasurement());

            Register(_unitTranformation, new CurrentAmperMeasurement());
            Register(_unitTranformation, new CurrentMilliamperMeasurement());
            Register(_unitTranformation, new CurrentMicroamperMeasurement());

            Register(_unitTranformation, new CapacityAmperHoursMeasurement());
            Register(_unitTranformation, new CapacityMilliamperHoursMeasurement());

            Register(_unitTranformation, new TemperatureCelciusMeasurement());

            Register(_unitTranformation, new PowerWattMeasurement());
            Register(_unitTranformation, new PowerKiloWattMeasurement());

            Register(_unitTranformation, new EnergyKiloWattHoursMeasurement());
            Register(_unitTranformation, new EnergyWattHoursMeasurement());

            Register(_unitTranformation, new VoltageMeasurement());
            Register(_unitTranformation, new VoltageMillivoltMeasurement());
            Register(_unitTranformation, new VoltageMicrovoltMeasurement());

            Register(_unitTranformation, new ResistanceOhmMeasurement());
            Register(_unitTranformation, new ResistanceMilliOhmMeasurement());
            Register(_unitTranformation, new ResistanceKilloOhmMeasurement());
        }

        public IUnitMeasurement Unit(string unitCode)
        {
            _unitTranformation.TryGetValue(unitCode, out var unit);
            return unit ?? new NullMeasurement();
        }

        public IUnitMeasurement TimeTransformation(string unitCode)
        {
            _timeTranformation.TryGetValue(unitCode, out var unitTransformation);
            return unitTransformation;
        }

        public IUnitMeasurement CurrentTransformation(string unitCode)
        {
            _currentTranformation.TryGetValue(unitCode, out var unitTransformation);
            return unitTransformation;
        }

        public IUnitMeasurement CapacityTransformation(string unitCode)
        {
            _capacityTranformation.TryGetValue(unitCode, out var unitTransformation);
            return unitTransformation;
        }

        private void Register(IDictionary<string, IUnitMeasurement> container, IUnitMeasurement unitMeasurement)
        {
            container[unitMeasurement.UnitCode] = unitMeasurement;
        }
    }
}
