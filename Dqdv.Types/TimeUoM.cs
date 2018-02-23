
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

namespace Dqdv.Types
{
    public enum TimeUoM
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2,
        Days = 3
    }

    public enum VoltageUoM
    {
        V  = 0,
        mV = 1,
        uV = 2,
        kV = 3,
        MV = 4
    }

    public enum EnergyUoM
    {
        Wh = 0,
        muWh = 1,
        mWh = 2,
        kWh = 3,
        MWh = 4,
        GWh = 5,
    }

    public enum PowerUoM
    {
        W = 0,
        muW = 1,
        mW = 2,
        kW = 3,
        MW = 4,
        GW = 5,
    }

    public enum ResistanceUoM
    {
        Ohm = 0,
        muOhm = 1,
        mOhm = 2,
        kOhm = 3,
        MOhm = 4,
        GOhm = 5
    }

    public enum TemperatureUoM
    {
        Celcius = 0,
        F = 1,
    }
}
