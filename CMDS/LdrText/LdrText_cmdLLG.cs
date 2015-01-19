using Base_Tools45;

namespace LdrText
{
    public class LdrText_cmdLLG
    {
        public static void
        cmdLLG(string zone, double N, double E, out string lat, out string lng)
        {
            lat = "";
            lng = "";
            zoneData zD = getZoneData(zone);
            double Np = N - zD.No;
            double Ep = E - zD.Eo;
            double Rp = zD.Ro - Np;
            double ratio = Ep / Rp;
            double gam = System.Math.Atan(Ep / Rp);       //gam result is in radians
            double Lo = DMS_DecimalDegrees(zD.Lo);
            double sinBo = System.Math.Sin((zD.Bo / 180) * System.Math.PI);
            double lam = Lo - (gam / System.Math.PI * 180) / sinBo;

            lng = string.Format("Long = {0}", Conv.DDtoDMS(lam, 4));

            double u = Np - Ep * System.Math.Tan(gam / 2);    //gam is already in radians
            double phiDelta = u * (zD.G1 + u * (zD.G2 + u * (zD.G3 + u * zD.G4)));
            double phi = zD.Bo + phiDelta;
            lat = string.Format("Lat = {0}", Conv.DDtoDMS(phi, 4));
        }

        private static double
        DMS_DecimalDegrees(DMS dms)
        {
            double d = 0;

            d = dms.Degree + dms.Minute / 60 + dms.Seconds / 3600;

            return d;
        }

        private static zoneData
        getZoneData(string zone)
        {
            zoneData zD = new zoneData();

            switch (zone)
            {
                case "401":
                    zD.Bs = new DMS { Degree = 40, Minute = 0, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 41, Minute = 40, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 39, Minute = 20, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 122, Minute = 0, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 40.8351061249; //N
                    zD.SinBo = 0.653884305400;
                    zD.Rb = 24791796.351;
                    zD.Ro = 24244708.924;
                    zD.No = 2187504.093;
                    zD.K = 40314310.136;
                    zD.ko = 0.999894636561;
                    zD.Mo = 20872882.401;
                    zD.ro = 20913107.780;

                    zD.L1 = 364300.5191;
                    zD.L2 = 31.6772;
                    zD.L3 = 18.4872;
                    zD.L4 = 0.069800;

                    zD.G1 = 0.000002744986448;
                    zD.G2 = -0.000000000000000655192;
                    zD.G3 = -0.00000000000000000000104884;
                    zD.G4 = -0.0000000000000000000000000000096167;

                    zD.F1 = 0.999894636561;
                    zD.F2 = 0.00000000000000114329;
                    zD.F3 = 0.0000000000000000000000155;
                    break;

                case "402":
                    zD.Bs = new DMS { Degree = 38, Minute = 20, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 39, Minute = 50, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 37, Minute = 40, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 122, Minute = 0, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 39.0846839219; //N
                    zD.SinBo = 0.630468335285;
                    zD.Rb = 26311590.850;
                    zD.Ro = 25795162.985;
                    zD.No = 2156844.531;
                    zD.K = 41077187.051;
                    zD.ko = 0.999914672977;
                    zD.Mo = 20866980.555;
                    zD.ro = 20909305.294;

                    zD.L1 = 364197.5131;
                    zD.L2 = 31.3198;
                    zD.L3 = 18.4998;
                    zD.L4 = 0.065577;

                    zD.G1 = 0.000002745762818;
                    zD.G2 = -0.000000000000000648347;
                    zD.G3 = -0.00000000000000000000105080;
                    zD.G4 = -0.0000000000000000000000000000089858;

                    zD.F1 = 0.999914672977;
                    zD.F2 = 0.00000000000000114370;
                    zD.F3 = 0.0000000000000000000000146;
                    break;

                case "403":
                    zD.Bs = new DMS { Degree = 37, Minute = 04, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 38, Minute = 26, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 36, Minute = 30, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 120, Minute = 30, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 37.7510694363; //N
                    zD.SinBo = 0.612232038295;
                    zD.Rb = 27512330.711;
                    zD.Ro = 27056804.050;
                    zD.No = 2095943.327;
                    zD.K = 41747209.726;
                    zD.ko = 0.999929178853;
                    zD.Mo = 20862522.855;
                    zD.ro = 20906428.003;

                    zD.L1 = 364119.7127;
                    zD.L2 = 30.9692;
                    zD.L3 = 18.5086;
                    zD.L4 = 0.062493;

                    zD.G1 = 0.000002746349509;
                    zD.G2 = -0.000000000000000641501;
                    zD.G3 = -0.00000000000000000000105230;
                    zD.G4 = -0.0000000000000000000000000000085291;

                    zD.F1 = 0.999929178853;
                    zD.F2 = 0.00000000000000114398;
                    zD.F3 = 0.0000000000000000000000138;
                    break;

                case "404":
                    zD.Bs = new DMS { Degree = 36, Minute = 0, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 37, Minute = 15, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 35, Minute = 20, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 119, Minute = 0, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 36.6258593071; //N
                    zD.SinBo = 0.596587149880;
                    zD.Rb = 28652263.494;
                    zD.Ro = 28181724.783;
                    zD.No = 2110955.377;
                    zD.K = 42378478.327;
                    zD.ko = 0.999940761703;
                    zD.Mo = 20858793.249;
                    zD.ro = 20904016.591;

                    zD.L1 = 364054.6183;
                    zD.L2 = 30.6211;
                    zD.L3 = 18.5174;
                    zD.L4 = 0.060308;

                    zD.G1 = 0.000002746840562;
                    zD.G2 = -0.000000000000000634643;
                    zD.G3 = -0.00000000000000000000105351;
                    zD.G4 = -0.0000000000000000000000000000081324;

                    zD.F1 = 0.999940761703;
                    zD.F2 = 0.00000000000000114427;
                    zD.F3 = 0.0000000000000000000000133;
                    break;

                case "405":
                    zD.Bs = new DMS { Degree = 34, Minute = 02, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 35, Minute = 28, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 33, Minute = 30, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 118, Minute = 0, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 34.7510553142; //N
                    zD.SinBo = 0.570011896174;
                    zD.Rb = 30648744.932;
                    zD.Ro = 30193453.753;
                    zD.No = 2095707.846;
                    zD.K = 43578078.311;
                    zD.ko = 0.999922127209;
                    zD.Mo = 20851897.173;
                    zD.ro = 20899279.068;

                    zD.L1 = 363934.2590;
                    zD.L2 = 29.9356;
                    zD.L3 = 18.5303;
                    zD.L4 = 0.057234;

                    zD.G1 = 0.000002747748987;
                    zD.G2 = -0.000000000000000621091;
                    zD.G3 = -0.00000000000000000000105565;
                    zD.G4 = -0.0000000000000000000000000000074567;

                    zD.F1 = 0.999922127209;
                    zD.F2 = 0.00000000000000114477;
                    zD.F3 = 0.0000000000000000000000125;
                    break;

                case "406":
                    zD.Bs = new DMS { Degree = 32, Minute = 47, Seconds = 0, Heading = "N" };
                    zD.Bn = new DMS { Degree = 33, Minute = 53, Seconds = 0, Heading = "N" };
                    zD.Bb = new DMS { Degree = 32, Minute = 10, Seconds = 0, Heading = "N" };
                    zD.Lo = new DMS { Degree = 116, Minute = 15, Seconds = 0, Heading = "W" };

                    zD.Nb = 1640416.667;
                    zD.Eo = 6561666.667;

                    zD.Bo = 33.3339229447; //N
                    zD.SinBo = 0.549517575763;
                    zD.Rb = 32270577.813;
                    zD.Ro = 31845868.317;
                    zD.No = 2065126.163;
                    zD.K = 44625982.642;
                    zD.ko = 0.999954142490;
                    zD.Mo = 20847750.958;
                    zD.ro = 20896729.860;

                    zD.L1 = 363861.8950;
                    zD.L2 = 29.3368;
                    zD.L3 = 18.5396;
                    zD.L4 = 0.053054;

                    zD.G1 = 0.000002748295465;
                    zD.G2 = -0.000000000000000608981;
                    zD.G3 = -0.00000000000000000000105713;
                    zD.G4 = -0.0000000000000000000000000000071424;

                    zD.F1 = 0.999954142490;
                    zD.F2 = 0.00000000000000114504;
                    zD.F3 = 0.0000000000000000000000118;
                    break;
            }
            return zD;
        }

        private struct DMS
        {
            internal int Degree;
            internal int Minute;
            internal double Seconds;
            internal string Heading;
        }

        private struct zoneData
        {
            internal DMS Bs;
            internal DMS Bn;
            internal DMS Bb;
            internal DMS Lo;
            internal double Nb;
            internal double Eo;

            internal double Bo;
            internal double SinBo;
            internal double Rb;
            internal double Ro;
            internal double No;
            internal double K;
            internal double ko;
            internal double Mo;
            internal double ro;

            internal double L1;
            internal double L2;
            internal double L3;
            internal double L4;

            internal double G1;
            internal double G2;
            internal double G3;
            internal double G4;

            internal double F1;
            internal double F2;
            internal double F3;
        }
    }
}
