using System;

namespace DD1Queuing
{
    class QueueLength
    {
        static void Main(string[] args)
        {
            float RampFlowRate = 1500;
            float RampFreeFlowSpeedMiPerHr = 35;
            float RampPctHeavyVeh = 0;
            float AvgSmallAutoLengthFt = (14.57f + 16.7f + 16.22f + 14.78f + 15.57f + 15.53f) / 6;
            float AvgLargeAutoLengthFt = (16.4f + 18.98f + 16.94f + 19.31f) / 4;
            float AvgSmallTruckLengthFt = 29;
            float AvgLargeTruckLengthFt = (55 + 68.5f + 74.6f) / 3;
            float PctSmallAuto = 0.6f * (1 - RampPctHeavyVeh);
            float PctLargeAuto = 0.4f * (1 - RampPctHeavyVeh);
            float PctSmallTruck = RampPctHeavyVeh / 2;
            float PctLargeTruck = RampPctHeavyVeh / 2;
            float AvgVehLengthFt = PctSmallAuto * AvgSmallAutoLengthFt + PctLargeAuto * AvgLargeAutoLengthFt + PctSmallTruck * AvgSmallTruckLengthFt + PctLargeTruck * AvgLargeTruckLengthFt;
            float AvgHeadwayInQueue = 29 - AvgVehLengthFt;
            int QueueStorageFt = 3000;
            int DetectorLengthFt = 6;
            float QueueStorageVeh = ((QueueStorageFt - DetectorLengthFt - AvgVehLengthFt) / (AvgVehLengthFt + AvgHeadwayInQueue)) + 1;
            int UpdateInterval = 20;
            int AnalysisPeriodSec = 3600;
            int MeteringRateMax = 1800;
            float MeteringRate = 900;

            float RampCapacity;
            if (RampFreeFlowSpeedMiPerHr > 50)
                RampCapacity = 2200;
            else if (RampFreeFlowSpeedMiPerHr > 40)
                RampCapacity = 2100;
            else if (RampFreeFlowSpeedMiPerHr > 30)
                RampCapacity = 2000;
            else if (RampFreeFlowSpeedMiPerHr > 20)
                RampCapacity = 1900;
            else
                RampCapacity = 1800;

            float QueueGrowthRateVehPerSec = (Math.Min(RampFlowRate, RampCapacity) - MeteringRate) / 3600;
            float QueueGrowthRateMin = (Math.Min(RampFlowRate, RampCapacity) - MeteringRateMax) / 3600; //queue growth rate when max metering rate is activated
            float t1 = 0; float t2 = 0; float t3 = 0; float t4 = 0;
            float q1 = 0; float q2 = 0; float q3 = 0; float q4 = 0;
            if (QueueGrowthRateVehPerSec > 0)
            {
                t1 = Math.Min(QueueStorageVeh / QueueGrowthRateVehPerSec, AnalysisPeriodSec); //time (sec passed from the beginning) takes the queue reaches the advance queue detector
                q1 = Math.Max(QueueGrowthRateVehPerSec * t1, 0);
                t2 = Math.Max(Math.Min(t1 + UpdateInterval, AnalysisPeriodSec) - t1, 0);
                q2 = Math.Max(QueueGrowthRateVehPerSec * t1 + QueueGrowthRateMin * t2, 0);
                while (q2 > QueueStorageFt & t1 + t2 < AnalysisPeriodSec)
                {
                    for (int i = 2; i <= Math.Floor((AnalysisPeriodSec - t1) / UpdateInterval); i++)
                    {
                        t2 = Math.Max(Math.Min(t1 + i * UpdateInterval, AnalysisPeriodSec) - t1, 0);
                        q2 = Math.Max(QueueGrowthRateVehPerSec * t1 + QueueGrowthRateMin * t2, 0);
                    }
                }
                if (t1 + t2 < AnalysisPeriodSec)
                {
                    t3 = Math.Max(Math.Min((QueueStorageVeh - q2) / QueueGrowthRateVehPerSec, AnalysisPeriodSec), 0);
                    q3 = Math.Max(QueueGrowthRateVehPerSec * (t1 + t3) + QueueGrowthRateMin * t2, 0);
                    t4 = Math.Max(Math.Min(t1 + t2 + t3 + UpdateInterval, AnalysisPeriodSec) - t1 - t2 - t3, 0);
                    q4 = Math.Max(QueueGrowthRateVehPerSec * (t1 + t3) + QueueGrowthRateMin * (t2 + t4), 0);
                    while (q4 > QueueStorageVeh & t1 + t2 + t3 + t4 < AnalysisPeriodSec)
                    {
                        for (int j = 2; j <= Math.Floor((AnalysisPeriodSec - t1 - t2 - t3) / UpdateInterval); j++)
                        {
                            t4 = Math.Max(Math.Min(t1 + t2 + t3 + j * UpdateInterval, AnalysisPeriodSec) - t1 - t2 - t3, 0);
                            q4 = Math.Max(QueueGrowthRateVehPerSec * (t1 + t3) + QueueGrowthRateMin * (t2 + t4), 0);
                        }
                    }
                }
            }
            float PctTimeMaxMetering = (t2 + t4) * UpdateInterval / AnalysisPeriodSec;
        }
    }
}