using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;

namespace Grading
{
    public class Grading_SetupSlope
    {
        private int _intSide;
        private Point3d _pnt3dA;
        private Point3d _pnt3dB;
        private double _slope;
        private Boolean _boolDoFirst;
        private string _strSurfaceTAR;
        private string _strSurfaceDES;
        private List<Point3d> _pnts3dB1;
        private List<Point3d> _pnts3dB2;
        private List<Point3d> _pnts3dD;
        private List<Point3d> _pnts3dR;
        private double _elev;
        private double _B1Width;
        private double _B2Width;
        private double _B1Slope;
        private double _B2Slope;
        private bool _doB1;
        private bool _doB2;
        private int _interval;

        private double pi = System.Math.PI;

        public Grading_SetupSlope(int intSide, Point3d pnt3dA, Point3d pnt3dB, double slope, ref Boolean boolDoFirst, string strSurfaceTAR, string strSurfaceDES,
            ref List<Point3d> pnts3dB1, ref List<Point3d> pnts3dB2, ref List<Point3d> pnts3dD, ref List<Point3d> pnts3dR, double B1Width,
            double B2Width, double B1Slope, double B2Slope, bool doB1, bool doB2, int interval, double elev = 0.0)
        {
            _intSide = intSide;
            _pnt3dA = pnt3dA;
            _pnt3dB = pnt3dB;
            _slope = slope;
            _boolDoFirst = boolDoFirst;
            _strSurfaceTAR = strSurfaceTAR;
            _strSurfaceDES = strSurfaceDES;
            _pnts3dB1 = pnts3dB1;
            _pnts3dB2 = pnts3dB2;
            _pnts3dD = pnts3dD;
            _pnts3dR = pnts3dR;
            _B1Width = B1Width;
            _B2Width = B2Width;
            _B1Slope = B1Slope;
            _B2Slope = B2Slope;
            _doB1 = doB1;
            _doB2 = doB2;
            _interval = interval;
            _elev = elev;
        }

        internal void setupSlope()
        {
            double dblAngAB = _pnt3dA.getDirection(_pnt3dB);

            Point3d pnt3dX;
            bool exists = false;

            List<Point3d> pnts3dT = new List<Point3d>();

            double dblAng = dblAngAB + pi / 2 * _intSide;
            pnts3dT = getIntermediates(_pnt3dA, _pnt3dB, dblAngAB, _interval);
            for (int i = 0; i < pnts3dT.Count; i++)
            {
                _pnts3dR.Add(pnts3dT[i]);
            }

            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnt3dX = pnts3dT[i];
                if (_doB1 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, _B1Width, _B1Slope);
                    _pnts3dB1.Add(pnt3dX);
                }
                if (_doB2 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, _B2Width, _B2Slope);
                }

                Vector3d v3d = new Vector3d(System.Math.Cos(dblAng), System.Math.Sin(dblAng), _slope);
                Point3d pnt3d0 = Pub.pnt3dO;

                if (_strSurfaceTAR != "")
                {
                    ObjectId idSurf = Surf.getSurface(_strSurfaceTAR, out exists);
                    TinSurface surf = idSurf.getEnt() as TinSurface;
                    try
                    {
                        pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
                    }
                    catch (System.Exception)
                    {
                        v3d = new Vector3d(v3d.X, v3d.Y, -_slope);
                        pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
                    }
                }
                else
                {
                    try
                    {
                        double elevDiff = _elev - pnt3dX.Z;
                        if (elevDiff > 0)
                        {
                            pnt3d0 = pnt3dX.traverse(dblAng, elevDiff / _slope, _slope);
                        }
                        else
                        {
                            pnt3d0 = pnt3dX.traverse(dblAng, System.Math.Abs(elevDiff) / _slope, -_slope);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " Grading_SetupSlope.cs: line: 122");
                    }
                }

                _pnts3dD.Add(pnt3d0);

                if (_doB2 == true)
                {
                    pnt3dX = pnt3d0.traverse(dblAng + pi, _B2Width, -_B2Slope);
                    _pnts3dB2.Add(pnt3dX);
                }
            }
        }

        private static List<Point3d> getIntermediates(Point3d pnt3dA, Point3d pnt3dB, double angAB, int interval)
        {
            List<Point3d> pnts3dX = new List<Point3d>();
            double len = pnt3dA.getDistance(pnt3dB);
            int num = (int)System.Math.Truncate(len / interval);
            double slope = (pnt3dB.Z - pnt3dA.Z) / len;

            for (int i = 0; i < num; i++)
            {
                pnts3dX.Add(pnt3dA.traverse(angAB, i * interval, slope));
            }

            pnts3dX.Add(pnt3dB);

            return pnts3dX;
        }// end  getIntermediates
    }
}
