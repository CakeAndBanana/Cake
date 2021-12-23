using System;
using System.Collections.Generic;

namespace OppaiSharp
{
    public partial class DiffCalc
    {
        /// <summary>default Value for singletap_threshold <see cref="Calc()" /></summary>
        public const double DefaultSingletapThreshold = 125.0;

        /// <summary>Star rating</summary>
        public double Total { get; private set; }

        /// <summary>Aim stars</summary>
        public double Aim { get; private set; }

        public double AimDifficulty { get; private set; }
        public double AimLengthBonus { get; private set; } /* unused for now */

        /// <summary>Speed stars</summary>
        public double Speed { get; private set; }

        public double SpeedDifficulty { get; private set; }
        public double SpeedLengthBonus { get; private set; } /* unused for now */

        /// <summary>The number of notes that are considered singletaps by the difficulty calculator</summary>
        public int CountSingles { get; private set; }

        /// <summary>The number of taps slower or equal to the singletap threshold value</summary>
        public int CountSinglesThreshold { get; private set; }

        /// <summary>
        /// The beatmap we want to calculate the difficulty for. Must be set or passed to Calc() explicitly. Persists 
        /// across Calc() calls unless it's changed or explicity passed to Calc().
        /// See: <see cref="Calc(OppaiSharp.Beatmap)"/>, <see cref="Calc(OppaiSharp.Beatmap, Mods)"/>, <see cref="Calc(OppaiSharp.Beatmap, Mods, double)"/>
        /// </summary>
        public Beatmap Beatmap { get; set; }

        private double speedMul;
        private readonly List<double> strains = new List<double>(512);

        public DiffCalc() => Reset();

        /// <summary>Sets up the instance for re-use by resetting fields</summary>
        private void Reset()
        {
            Total = Aim = Speed = 0.0;
            CountSingles = CountSinglesThreshold = 0;
            speedMul = 1.0;
        }

        public override string ToString() => $"{Total} stars ({Aim} aim, {Speed} speed)";

        /// <summary>
        /// Calculates beatmap difficulty and stores it in total, aim, speed, nsingles, nsingles_speed fields.
        /// </summary>
        /// <param name="mods"></param>
        /// <param name="singletapThreshold">
        /// The smallest milliseconds interval that will be considered singletappable. for example, 125ms is 240 1/2 
        /// singletaps ((60000 / 240) / 2)
        /// </param>
        /// <returns>Itself</returns>
        public DiffCalc Calc(Mods mods, double singletapThreshold)
        {
            Reset();

            var mapstats = new MapStats { CS = Beatmap.CS };
            mapstats = MapStats.ModsApply(mods, mapstats, ModApplyFlags.ApplyCS);
            speedMul = mapstats.Speed;

            double radius = (PlayfieldWidth / 16.0) * (1.0 - 0.7 * (mapstats.CS - 5.0) / 5.0);

            //positions are normalized on circle radius so that we can calc as if everything was the same circlesize
            double scalingFactor = 52.0 / radius;

            if (radius < CirclesizeBuffThreshold)
                scalingFactor *= 1.0 + Math.Min(CirclesizeBuffThreshold - radius, 5.0) / 50.0;

            Vector2 normalizedCenter = new Vector2(PlayfieldCenter) * scalingFactor;

            //calculate normalized positions
            for (int i = 0; i < Beatmap.Objects.Count; i++)
            {
                HitObject obj = Beatmap.Objects[i];

                if ((obj.Type & HitObjectType.Spinner) != 0)
                {
                    obj.Normpos = new Vector2(normalizedCenter);
                }
                else
                {
                    Vector2 pos;

                    if ((obj.Type & HitObjectType.Slider) != 0)
                        pos = ((Slider)obj.Data).Position;
                    else if ((obj.Type & HitObjectType.Circle) != 0)
                        pos = ((Circle)obj.Data).Position;
                    else
                    {
                        //TODO: warn $"W: unknown object type {obj.Type:X8}\n"
                        pos = new Vector2();
                    }

                    obj.Normpos = new Vector2(pos) * scalingFactor;
                }

                if (i >= 2)
                {
                    var prev1 = Beatmap.Objects[i - 1];
                    var prev2 = Beatmap.Objects[i - 2];
                    Vector2 v1 = prev2.Normpos - prev1.Normpos;
                    Vector2 v2 = obj.Normpos - prev1.Normpos;
                    double dot = Vector2.Dot(v1, v2);
                    double det = v1.X * v2.Y - v1.Y * v2.X;
                    obj.Angle = Math.Abs(Math.Atan2(det, dot));
                }
                else
                {
                    obj.Angle = double.NaN;
                }
            }

            //speed and aim stars
            Speed = CalcIndividual(StrainType.Speed);
            Aim = CalcIndividual(StrainType.Aim);

            Speed = Math.Sqrt(Speed) * StarScalingFactor;
            Aim = Math.Sqrt(Aim) * StarScalingFactor;
            if ((mods & Mods.TouchDevice) != 0)
                Aim = Math.Pow(Aim, 0.8);

            //total stars
            Total = Aim + Speed + Math.Abs(Speed - Aim) * ExtremeScalingFactor;

            //singletap stats
            for (int i = 1; i < Beatmap.Objects.Count; ++i)
            {
                HitObject prev = Beatmap.Objects[i - 1];
                HitObject curr = Beatmap.Objects[i];

                if (curr.IsSingle)
                    CountSingles++;

                if ((curr.Type & (HitObjectType.Circle | HitObjectType.Slider)) == 0)
                    continue;

                double interval = (curr.Time - prev.Time) / speedMul;

                if (interval >= singletapThreshold)
                    CountSinglesThreshold++;
            }

            return this;
        }

        /// <returns>
        /// <see cref="Calc(Mods,double)"/> with <seealso cref="DefaultSingletapThreshold"/> as second parameter
        /// </returns>
        public DiffCalc Calc(Mods mods) => Calc(mods, DefaultSingletapThreshold);

        /// <returns>
        /// <see cref="Calc(Mods,double)"/> with <seealso cref="Mods.NoMod"/> and 
        /// <seealso cref="DefaultSingletapThreshold"/> as parameters
        /// </returns>
        public DiffCalc Calc() => Calc(Mods.NoMod, DefaultSingletapThreshold);

        /// <summary>
        /// Sets beatmap field and calls <see cref="Calc(Mods, double)"/>
        /// </summary>
        public DiffCalc Calc(Beatmap beatmap, Mods mods, double singletapThreshold)
        {
            Beatmap = beatmap;
            return Calc(mods, singletapThreshold);
        }

        /// <summary>
        /// Sets beatmap field and calls <see cref="Calc(Mods, double)"/> with 
        /// <seealso cref="DefaultSingletapThreshold"/> as second parameter
        /// </summary>
        public DiffCalc Calc(Beatmap beatmap, Mods mods) => Calc(beatmap, mods, DefaultSingletapThreshold);

        /// <summary>
        /// Sets beatmap field and calls <see cref="Calc(OppaiSharp.Beatmap, Mods, double)"/> with 
        /// <seealso cref="Mods.NoMod"/> and <seealso cref="DefaultSingletapThreshold"/> as parameters
        /// </summary>
        public DiffCalc Calc(Beatmap beatmap) => Calc(beatmap, Mods.NoMod, DefaultSingletapThreshold);

        private double CalcIndividual(StrainType type)
        {
            strains.Clear();

            double strainStep = StrainStep * speedMul;
            double intervalEnd = strainStep;
            double maxStrain = 0.0;

            //calculate all strains
            for (int i = 0; i < Beatmap.Objects.Count; ++i)
            {
                HitObject obj = Beatmap.Objects[i];
                HitObject prev = i > 0
                    ? Beatmap.Objects[i - 1]
                    : null;

                if (prev != null)
                    DiffStrain(type, obj, prev, speedMul);

                while (obj.Time > intervalEnd)
                {
                    //add max strain for this interval
                    strains.Add(maxStrain);

                    if (prev != null)
                    {
                        //decay last object's strains until the next interval and use that as the initial max strain
                        double decay = Math.Pow(DecayBase[type], (intervalEnd - prev.Time) / 1000.0);
                        maxStrain = prev.Strains[type] * decay;
                    }
                    else
                        maxStrain = 0.0;

                    intervalEnd += strainStep;
                }

                maxStrain = Math.Max(maxStrain, obj.Strains[type]);
            }

            //weigh the top strains sorted from highest to lowest
            double weight = 1.0;
            double difficulty = 0.0;

            strains.Sort();
            strains.Reverse();

            foreach (double strain in strains)
            {
                difficulty += strain * weight;
                weight *= DecayWeight;
            }

            return difficulty;
        }

        private static double DiffSpacingWeight(StrainType type, double distance,
            double deltaTime,
            double prevDistance, double prevDeltaTime,
            double angle)
        {
            double angleBonus;
            double strainTime = Math.Max(deltaTime, 50.0f);

            switch (type)
            {
                case StrainType.Aim:
                    {
                        double result = 0;
                        double prevStrainTime = Math.Max(prevDeltaTime, 50.0f);
                        if (!(Double.IsNaN(angle) || Double.IsInfinity(angle)) && angle > AimAngleBonusBegin)
                        {
                            angleBonus = Math.Sqrt(
                                Math.Max(prevDistance - AngleBonusScale, 0)
                                * Math.Pow(Math.Sin(angle - AimAngleBonusBegin), 2)
                                * Math.Max(distance - AngleBonusScale, 0)
                            );
                            result = 1.5f * Math.Pow(Math.Max(0, angleBonus), 0.99)
                                     / Math.Max(AimTimingThreshold, prevStrainTime);
                        }

                        var weightedDistance = Math.Pow(distance, 0.99);
                        return Math.Max(
                            result + weightedDistance /
                            Math.Max(AimTimingThreshold, strainTime),
                            weightedDistance / strainTime
                        );
                    }
                case StrainType.Speed:
                    {
                        distance = Math.Min(distance, SingleSpacing);
                        deltaTime = Math.Max(deltaTime, MaxSpeedBonus);
                        double speedBonus = 1.0f;
                        if (deltaTime < MinSpeedBonus)
                        {
                            speedBonus += Math.Pow((MinSpeedBonus - deltaTime) / 40.0f, 2);
                        }

                        angleBonus = 1.0f;
                        if (!(Double.IsNaN(angle) || Double.IsInfinity(angle)) && angle < SpeedAngleBonusBegin)
                        {
                            double s = Math.Sin(1.5 * (SpeedAngleBonusBegin - angle));
                            angleBonus += Math.Pow(s, 2) / 3.57f;
                            if (angle < Math.PI / 2)
                            {
                                angleBonus = 1.28f;
                                if (distance < AngleBonusScale && angle < Math.PI / 4)
                                {
                                    angleBonus += (1 - angleBonus)
                                                  * Math.Min((AngleBonusScale - distance) / 10, 1);
                                }
                                else if (distance < AngleBonusScale)
                                {
                                    angleBonus += (1 - angleBonus)
                                                  * Math.Min((AngleBonusScale - distance) / 10, 1)
                                                  * Math.Sin((Math.PI / 2 - angle) * 4 / Math.PI);
                                }
                            }
                        }

                        return (
                                   (1 + (speedBonus - 1) * 0.75f) *
                                   angleBonus *
                                   (0.95f + speedBonus * Math.Pow(distance / SingleSpacing, 3.5))
                               ) / strainTime;
                    }
                default:
                    throw new InvalidOperationException("this difficulty type does not exist");
            }
        }

        /// <summary>
        /// Calculates the strain for one difficulty type and stores it in <paramref name="obj"/>. This assumes that 
        /// <see cref="HitObject.Normpos"/> is already computed. This also sets <see cref="HitObject.IsSingle"/> if 
        /// <paramref name="type"/> is <seealso cref="StrainType.Speed"/>.
        /// </summary>
        private static void DiffStrain(StrainType type, HitObject obj, HitObject prev, double speedMul)
        {
            double value = 0.0;
            double timeElapsed = (obj.Time - prev.Time) / speedMul;
            double decay =
                Math.Pow(DecayBase[type], timeElapsed / 1000.0);

            obj.DeltaTime = timeElapsed;

            /* this implementation doesn't account for sliders */
            if ((obj.Type & (HitObjectType.Slider | HitObjectType.Circle)) != 0)
            {
                double distance = (obj.Normpos - prev.Normpos).Length;
                obj.DeltaDistance = distance;

                if (type == StrainType.Speed)
                {
                    obj.IsSingle = distance > SingleSpacing;
                }

                value = DiffSpacingWeight(type, distance, timeElapsed, prev.DeltaDistance, prev.DeltaTime, obj.Angle);
                value *= WeightScaling[type];
            }

            obj.Strains[type] = prev.Strains[type] * decay + value;
        }
    }
}