using System;

namespace OppaiSharp
{
    public class PPv2Parameters
    {
        public double AimStars;
        public double SpeedStars;
        public int MaxCombo = 0;
        public int CountSliders = 0, CountCircles = 0, CountSpinners = 0, CountObjects = 0;

        /// <summary> the base AR (before applying mods). </summary>
        public float BaseAR = 5.0f;

        /// <summary> the base OD (before applying mods) </summary>
        public float BaseOD = 5.0f;

        /// <summary> gamemode </summary>
        public GameMode Mode = GameMode.Standard;

        /// <summary> the mods </summary>
        public Mods Mods = Mods.NoMod;

        /// <summary> the maximum combo achieved, if -1 it will default to MaxCombo - CountMiss </summary>
        public int Combo = -1;

        /// <summary> amount of misses </summary>
        public int CountMiss;

        /// <summary> scorev1 (1) or scorev2 (2) </summary>
        public int ScoreVersion = 1;

        /// <summary> accuracy in 0..1 range </summary>
        public double Accuracy = 1;

        public PPv2Parameters()
        {
        }

        /// <param name="bm">The Beatmap object</param>
        /// <param name="d">The DiffCalc object that ran on this beatmap</param>
        /// <param name="accuracy">Accuracy in 0..1 range</param>
        /// <param name="cMiss">Amount of misses</param>
        /// <param name="combo">The combo reached by the player. At least this or <paramref name="c300"/> has to be set.</param>
        /// <param name="mods">The used mods.</param>
        public PPv2Parameters(Beatmap bm, DiffCalc d, double accuracy, int cMiss = 0, int combo = -1,
            Mods mods = Mods.NoMod)
        {
            if (bm == null)
                throw new ArgumentNullException(nameof(bm));

            //run DiffCalc if it hadn't yet
            if (d.CountSingles == 0 && Math.Abs(d.Total) <= double.Epsilon)
                d.Calc(bm, mods);

            Mode = bm.Mode;
            BaseAR = bm.AR;
            BaseOD = bm.OD;
            MaxCombo = bm.GetMaxCombo();
            CountSliders = bm.CountSliders;
            CountCircles = bm.CountCircles;
            CountSpinners = bm.CountSpinners;
            CountObjects = bm.Objects.Count;

            AimStars = d.Aim;
            SpeedStars = d.Speed;
            CountMiss = cMiss;
            Combo = combo;
            Mods = mods;

            Accuracy = accuracy;
        }

        /// <param name="bm">The beatmap, diffcalc will run on this.</param>
        /// <param name="accuracy">Accuracy</param>
        /// <param name="cMiss">Amount of misses</param>
        /// <param name="combo">The combo reached by the player. At least this or <paramref name="c300"/> has to be set.</param>
        /// <param name="mods">The used mods.</param>
        public PPv2Parameters(Beatmap bm, double accuracy, int cMiss = 0, int combo = -1, Mods mods = Mods.NoMod)
        {
            if (bm == null)
                throw new ArgumentNullException(nameof(bm));

            var d = new DiffCalc().Calc(bm, mods);

            Mode = bm.Mode;
            BaseAR = bm.AR;
            BaseOD = bm.OD;
            MaxCombo = bm.GetMaxCombo();
            CountSliders = bm.CountSliders;
            CountCircles = bm.CountCircles;
            CountSpinners = bm.CountSpinners;
            CountObjects = bm.Objects.Count;

            AimStars = d.Aim;
            SpeedStars = d.Speed;
            CountMiss = cMiss;
            Combo = combo;
            Mods = mods;

            Accuracy = accuracy;
        }

    }

    public class PPv2
    {
        public double Total { get; }
        public double Aim { get; }
        public double Speed { get; }
        public double Acc { get; }
        public Accuracy ComputedAccuracy { get; }

        /// <summary>
        /// calculates ppv2, results are stored in Total, Aim, Speed, Acc, AccPercent.
        /// See: <seealso cref="PPv2Parameters"/>
        /// </summary>
        private PPv2(double aimStars, double speedStars,
            int maxCombo, int countSliders, int countCircles, int countSpinners, int countObjects,
            float baseAR, float baseOD, GameMode mode, Mods mods,
            int combo, double accuracy, int countMiss,
            int scoreVersion, Beatmap beatmap)
        {
            if (beatmap != null)
            {
                mode = beatmap.Mode;
                baseAR = beatmap.AR;
                baseOD = beatmap.OD;
                maxCombo = beatmap.GetMaxCombo();
                countSliders = beatmap.CountSliders;
                countCircles = beatmap.CountCircles;
                countSpinners = beatmap.CountSpinners;
                countObjects = beatmap.Objects.Count;
            }

            if (mode != GameMode.Standard)
                throw new InvalidOperationException("this gamemode is not yet supported");

            if (maxCombo <= 0)
            {
                //TODO: warn "W: max_combo <= 0, changing to 1\n"
                maxCombo = 1;
            }

            if (combo < 0)
                combo = maxCombo - countMiss;

            /* accuracy -------------------------------------------- */
            ComputedAccuracy = new Accuracy(accPercent: accuracy, countObjects: countObjects, countMiss: countMiss);

            // reconstruct n300, n100 and n50 from accuracy because we dont really care about real amounts
            int count300 = ComputedAccuracy.Count300;
            int count100 = ComputedAccuracy.Count100;
            int count50 = ComputedAccuracy.Count50;
            double realAcc = ComputedAccuracy.Value();

            switch (scoreVersion)
            {
                case 1:
                    //scorev1 ignores sliders since they are free 300s
                    //and for some reason also ignores spinners
                    realAcc = new Accuracy(Math.Max(count300 - countSliders - countSpinners, 0), count100, count50,
                        countMiss).Value();

                    realAcc = Math.Max(0.0, realAcc);
                    break;

                case 2:
                    countCircles = countObjects;
                    break;

                default:
                    throw new InvalidOperationException($"unsupported scorev{scoreVersion}");
            }

            //global values ---------------------------------------
            double countObjectsOver2K = countObjects / 2000.0;

            double lengthBonus = 0.95 + 0.4 * Math.Min(1.0, countObjectsOver2K);

            if (countObjects > 2000)
                lengthBonus += Math.Log10(countObjectsOver2K) * 0.5;

            double comboBreak = Math.Pow(combo, 0.8) / Math.Pow(maxCombo, 0.8);

            //calculate stats with mods
            var mapstats = new MapStats
            {
                AR = baseAR,
                OD = baseOD
            };
            mapstats = MapStats.ModsApply(mods, mapstats, ModApplyFlags.ApplyAR | ModApplyFlags.ApplyOD);

            /* aim pp ---------------------------------------------- */
            Aim = GetPPBase(aimStars);
            Aim *= lengthBonus;
            Aim *= comboBreak;

            double aimArBonus = 0.0;
            if (mapstats.AR > 10.33)
                aimArBonus += 0.3 * (mapstats.AR - 10.33);
            else if (mapstats.AR < 8.0)
                aimArBonus += 0.1 * (8.0 - mapstats.AR);

            Aim *= 1.0 + aimArBonus * lengthBonus;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (countMiss > 0)
                Aim *= 0.97 * Math.Pow(1 - Math.Pow((double)countMiss / countObjects, 0.775), countMiss);

            double hdBonus = 1.0;
            if ((mods & Mods.Hidden) != 0)
            {
                hdBonus += 0.04f * (12.0f - mapstats.AR);
            }

            if ((mods & Mods.Flashlight) != 0)
            {
                double flBonus = 1.0 + 0.35 * Math.Min(1.0, countObjects / 200.0);
                if (countObjects > 200)
                {
                    flBonus += 0.3 * Math.Min(1, (countObjects - 200) / 300.0);
                }

                if (countObjects > 500)
                {
                    flBonus += (countObjects - 500) / 1200.0;
                }

                Aim *= flBonus;
            }

            double accBonus = accuracy;
            double odSquared = Math.Pow(mapstats.OD, 2);
            double odBonus = 0.98 + odSquared / 2500.0;

            Aim *= accBonus;
            Aim *= odBonus;
            Aim *= hdBonus;

            /* speed pp -------------------------------------------- */
            Speed = GetPPBase(speedStars);
            Speed *= lengthBonus;
            Speed *= comboBreak;
            Speed *= hdBonus;

            double speedArBonus = 0.0;
            if (mapstats.AR > 10.33)
                speedArBonus += 0.3 * (mapstats.AR - 10.33);

            Speed *= 1.0 + speedArBonus * lengthBonus;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (countMiss > 0)
                Speed *= 0.97 * Math.Pow(1 - Math.Pow((double)countMiss / countObjects, 0.775), Math.Pow(countMiss, .875));

            // Scale the speed value with accuracy and OD
            Speed *= (0.95 + odSquared / 750) * Math.Pow(accuracy, (14.5 - Math.Max(mapstats.OD, 8)) / 2);

            // Scale the speed value with # of 50s to punish doubletapping.
            Speed *= Math.Pow(0.98, count50 < countObjects / 500.0 ? 0 : count50 - countObjects / 500.0);

            /* acc pp ---------------------------------------------- */
            Acc = Math.Pow(1.52163, mapstats.OD) * Math.Pow(realAcc, 24.0) * 2.83;

            Acc *= Math.Min(1.15, Math.Pow(countCircles / 1000.0, 0.3));

            if ((mods & Mods.Hidden) != 0)
                Acc *= 1.08;

            if ((mods & Mods.Flashlight) != 0)
                Acc *= 1.02;

            /* total pp -------------------------------------------- */
            double finalMultiplier = 1.12;

            if ((mods & Mods.NoFail) != 0)
                finalMultiplier *= Math.Max(0.9, 1.0 - 0.02 * countMiss);

            if ((mods & Mods.SpunOut) != 0)
                finalMultiplier *= 1.0 - Math.Pow(countSpinners / countObjects, 0.85);

            Total = Math.Pow(Math.Pow(Aim, 1.1) + Math.Pow(Speed, 1.1) + Math.Pow(Acc, 1.1), 1.0 / 1.1) *
                    finalMultiplier;
        }

        /// <inheritdoc />
        /// <summary> See <see cref="PPv2Parameters" /> </summary>
        public PPv2(PPv2Parameters p) :
            this(p.AimStars, p.SpeedStars, p.MaxCombo, p.CountSliders,
                p.CountCircles, p.CountSpinners, p.CountObjects, p.BaseAR, p.BaseOD, p.Mode,
                p.Mods, p.Combo, p.Accuracy, p.CountMiss,
                p.ScoreVersion, null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Simplest possible call, calculates ppv2 for SS scorev1
        /// </summary>
        public PPv2(double aimStars, double speedStars, Beatmap map)
            : this(aimStars, speedStars, -1, map.CountSliders, map.CountCircles, map.CountSpinners, map.Objects.Count,
                map.AR, map.OD, map.Mode, Mods.NoMod, -1, 1, 0, 1, map)
        {
        }

        private static double GetPPBase(double stars) =>
            Math.Pow(5.0 * Math.Max(1.0, stars / 0.0675) - 4.0, 3.0) / 100000.0;
    }
}