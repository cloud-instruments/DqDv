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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using DataLayer;
using Dqdv.Types;
using log4net;
using Plotting;
using Cycle = Plotting.Cycle;
using DataPoint = Dqdv.Types.DataPoint;
using Project = Plotting.Project;

namespace WebApp.Services
{
    class ProjectDataRepository : IProjectDataRepository
    {
        #region Logging

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Private fields

        private readonly DqdvContext _db;
        private readonly ProjectPlotCache _cache;

        #endregion

        #region Constructor

        public ProjectDataRepository(DqdvContext db, ProjectPlotCache cache)
        {
            _db = db;
            _cache = cache;
        }

        #endregion

        #region IProjectDataRepository implementation

        public Project GetProject(int id, string user)
        {
            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return new Project
                {
                    Name = "Unknown",
                };
            }

            return new Project
            {
                Id = id,
                Name = project.Name,
                Mass = project.Mass,
                ActiveMaterialFraction = project.ActiveMaterialFraction,
                TheoreticalCapacity = project.TheoreticalCapacity,
                Area = project.Area,
                IsAveragePlot = project.IsAveragePlot
            };
        }

        public List<DataPoint> GetPoints(int id, string user)
        {
            var key = "Points_" + id.ToString(CultureInfo.InvariantCulture);
            var points = (List<DataPoint>)_cache.Get(key);
            if (points != null)
                return points;

            points = LoadPoints(id, user);
            if (points == null)
                return new List<DataPoint>();

            _cache.Set(key, points, new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration, RemovedCallback = OnCacheItemRemoved });
            return points;
        }

        public List<Cycle> GetCycles(int id, string user)
        {
            var key = "Cycles_" + id.ToString(CultureInfo.InvariantCulture);
            var cycles = (List<Cycle>)_cache.Get(key);
            if (cycles != null)
                return cycles;

            cycles = LoadCycles(id, user);
            if (cycles == null)
                return new List<Cycle>();

            _cache.Set(key, cycles, new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration, RemovedCallback = OnCacheItemRemoved });
            return cycles;
        }

        #endregion

        #region Private methods

        private List<DataPoint> LoadPoints(int projectId, string user)
        {
            if (!IsProjectReady(projectId))
                return null;

            var timer = Stopwatch.StartNew();

            var points = _db.DataPoints
                .Where(p => p.ProjectId == projectId)
                .OrderBy(p => p.Index)
                .Select(p =>
                    new DataPoint
                    {
                        CycleIndex = p.CycleIndex,
                        CycleStep = (CycleStep)p.CycleStep,
                        Time = p.Time,
                        Current = p.Current,
                        Voltage = p.Voltage,
                        Capacity = p.Capacity,
                        Energy = p.Energy,
                        Power = p.Power,
                        Temperature = p.Temperature
                    }).ToList();

            Log.Info($"ProjectDataRepository.LoadPoints: LoadPoints({projectId}): {points.Count} points, {timer.ElapsedMilliseconds} ms");
            return points;
        }

        private List<Cycle> LoadCycles(int projectId, string trace)
        {
            Log.Info($"ProjectDataRepository.LoadCycles: LoadCycles({projectId}): {trace}");

            if (!IsProjectReady(projectId))
                return null;

            var timer = Stopwatch.StartNew();
            var cycles = _db.Cycles
                .AsNoTracking()
                .Where(c => c.ProjectId == projectId)
                .OrderBy(c => c.Index)
                .ToList()
                .Select(c =>
                    new Cycle
                    {
                        Index = c.Index,
                        FirstPointIndex = c.FirstPointIndex,
                        PointCount = c.PointCount,
                        EndCurrent = c.EndCurrent,
                        DischargeEndCurrent = c.DischargeEndCurrent,
                        MidVoltage = c.MidVoltage,
                        EndVoltage = c.EndVoltage,
                        DischargeEndVoltage = c.DischargeEndVoltage,
                        ChargeCapacity = c.ChargeCapacity,
                        ChargeCapacityRetention = c.ChargeCapacityRetention,
                        DischargeCapacity = c.DischargeCapacity,
                        DischargeCapacityRetention = c.DischargeCapacityRetention,
                        Power = c.Power,
                        DischargePower = c.DischargePower,
                        ChargeEnergy = c.ChargeEnergy,
                        DischargeEnergy = c.DischargeEnergy,
                        Temperature = c.Temperature,
                        ResistanceOhms = c.ResistanceOhms,
                        DischargeResistance = c.DischargeResistance,
                        StartCurrent = c.StartCurrent,
                        StartDischargeCurrent = c.StartDischargeCurrent,
                        StartDischargeVoltage = c.StartDischargeVoltage,
                        StartChargeVoltage = c.StartChargeVoltage,
                        StatisticMetaData = new Plotting.StatisticMetaData
                        {
                            EndCurrentStdDev = c.StatisticMetaData?.EndCurrentStdDev,
                            DischargeEndCurrentStdDev = c.StatisticMetaData?.DischargeEndCurrentStdDev,
                            MidVoltageStdDev = c.StatisticMetaData?.MidVoltageStdDev,
                            EndVoltageStdDev = c.StatisticMetaData?.EndVoltageStdDev,
                            DischargeEndVoltageStdDev = c.StatisticMetaData?.DischargeEndVoltageStdDev,
                            ChargeCapacityStdDev = c.StatisticMetaData?.ChargeCapacityStdDev,
                            DischargeCapacityStdDev = c.StatisticMetaData?.DischargeCapacityStdDev,
                            ChargeEnergyStdDev = c.StatisticMetaData?.ChargeEnergyStdDev,
                            PowerStdDev = c.StatisticMetaData?.PowerStdDev,
                            DischargePowerStdDev = c.StatisticMetaData?.DischargePowerStdDev,
                            ResistanceOhmsStdDev = c.StatisticMetaData?.ResistanceOhmsStdDev,
                            DischargeResistanceStdDev = c.StatisticMetaData?.DischargeResistanceStdDev,
                            CoulombicEfficiencyStdDev = c.StatisticMetaData?.CoulombicEfficiencyStdDev,
                            CoulombicEfficiencyAverage = c.StatisticMetaData?.CoulombicEfficiencyAverage,
                            DischargeEnergyStdDev = c.StatisticMetaData?.DischargeEnergyStdDev,
                            ChargeCapacityRetentionStdDev = c.StatisticMetaData?.ChargeCapacityRetentionStdDev,
                            DischargeCapacityRetentionStdDev = c.StatisticMetaData?.DischargeCapacityRetentionStdDev
                        }
                    }).ToList();

            Log.Info($"ProjectDataRepository.LoadCycles: LoadCycles({projectId}): {cycles.Count} cycles, {timer.ElapsedMilliseconds} ms");
            return cycles;
        }

        private bool IsProjectReady(int projectId)
        {
            var project = _db.Projects.Find(projectId);
            return project != null && project.IsReady && !project.Failed;
        }

        private static void OnCacheItemRemoved(CacheEntryRemovedArguments args)
        {
            Log.InfoFormat("ProjectDataRepository.OnCacheItemRemoved: Cache item '{0}' has been removed from cached because of: {1}", args.CacheItem.Key, args.RemovedReason);
        }

        #endregion
    }
}
