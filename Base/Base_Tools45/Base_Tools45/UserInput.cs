using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;

namespace Base_Tools45
{
	/// <summary>
	///
	/// </summary>
	public static class UserInput
	{
		public static Point3d
		getPnt3d(string CMD)
		{
			Editor ED = BaseObjs._editor;

			string strPrompt = "";

			if (CMD == "FL")
			{
				strPrompt = "\nPick FL Point: ";
			}
			if (CMD == "G")
			{
				strPrompt = "\nPick Grade Point: ";
			}
			if (CMD == "FF")
			{
				strPrompt = "\nPick FF Point";
			}

			PromptPointOptions PPO = new PromptPointOptions(strPrompt);
			PPO.UseBasePoint = false;

			object objSV = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("osmode");
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("osmode", 8);

			PromptPointResult PPR = ED.GetPoint(PPO);

			if (PPR.Status == PromptStatus.OK)
			{
				Point3d pntRet = PPR.Value;
				return pntRet;
			}

			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("osmode", objSV.ToString());

			return Pub.pnt3dO;
		}

		public static Point3d
		getPnt3d(Point3d pntBase, string strPrompt, int osMode)
		{
			object mode = SnapMode.getOSnap();
			SnapMode.setOSnap(osMode);

			try
			{
				Editor ED = BaseObjs._editor;
				PromptPointOptions PPO = new PromptPointOptions(strPrompt);
				if (pntBase != Pub.pnt3dO)
				{
					PPO.UseBasePoint = true;
					PPO.BasePoint = pntBase;
				}
				else
				{
					PPO.UseBasePoint = false;
				}

				PromptPointResult PPR = ED.GetPoint(PPO);

				if (PPR.Status == PromptStatus.OK)
				{
					Point3d pntRet = PPR.Value;
					return pntRet;
				}
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 85");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}

			return Pub.pnt3dO;
		}

		public static Point3d
		getPoint(string strPrompt, Point3d pnt3d, out bool escape, out PromptStatus promptStatus, int osMode)
		{
			object mode = SnapMode.getOSnap();
			SnapMode.setOSnap(osMode);

			promptStatus = PromptStatus.Error;
			escape = false;
			Point3d pnt3dX = Pub.pnt3dO;
			Editor ED = BaseObjs._editor;
			PromptPointResult PPR = null;

			PromptPointOptions PPO = new PromptPointOptions(strPrompt);
			PPO.AllowNone = true;
			if (pnt3d != pnt3dX)
			{
				PPO.AllowNone = true;
				PPO.UseBasePoint = true;
				PPO.BasePoint = pnt3d;
				PPO.UseDashedLine = true;
			}

			try
			{
				PPR = ED.GetPoint(PPO);
				switch (PPR.Status)
				{
					case PromptStatus.Cancel:
						escape = true;
						break;

					case PromptStatus.Other:
						pnt3dX = Pub.pnt3dO;
						break;

					case PromptStatus.OK:
						pnt3dX = PPR.Value;
						break;
				}
				promptStatus = PPR.Status;
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 138");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}
			return pnt3dX;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="strPrompt"></param>
		/// <returns></returns>
		public static Point3d
		getPoint(string strPrompt, out PromptStatus ps, int osMode)
		{
			object mode = SnapMode.getOSnap();
			SnapMode.setOSnap(osMode);

			Point3d pnt3dX = Pub.pnt3dO;
			Editor ed = BaseObjs._editor;
			PromptPointResult ppr = null;
			ps = PromptStatus.Cancel;

			PromptPointOptions ppo = new PromptPointOptions(strPrompt);
			ppo.AllowNone = true;

			try
			{
				ppr = ed.GetPoint(ppo);
				ps = ppr.Status;
				if (ps == PromptStatus.OK)
				{
					pnt3dX = ppr.Value;
				}
				else
				{
					ed.WriteMessage("Point selection canceled");
					pnt3dX = Pub.pnt3dO;
				}
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 182");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}
			return pnt3dX;
		}

		public static string
		getPoint(string prompt1, out ObjectId idCgPnt, out Point3d pnt3d, Point3d pnt3dBase, int osMode, bool round = true)
		{
			Object mode = SnapMode.getOSnap();
			idCgPnt = ObjectId.Null;
			string elev = string.Empty;
			pnt3d = Pub.pnt3dO;

			Point3d pnt3dX = Pub.pnt3dO;

			ObjectId idCogoPnt = ObjectId.Null;
			bool escape;
			PromptStatus ps;
			try
			{
				if (pnt3dBase != Pub.pnt3dO)
				{
					pnt3dX = UserInput.getPoint(prompt1, pnt3dBase, out escape, out ps, osMode);
				}
				else
				{
					pnt3dX = UserInput.getPoint(prompt1, out ps, osMode);
				}

				if (pnt3dX == Pub.pnt3dO)
					return elev;

				ObjectIdCollection ids = new ObjectIdCollection();
				BaseObjs._db.forEachMS<CogoPoint>(
                    cg =>                                 //linq
                    {
                        if (cg.Location == pnt3dX)
                        {
                            idCogoPnt = cg.ObjectId;
                            ids.Add(idCogoPnt);
                        }
                    });

				switch (ids.Count)
				{
					case 0:
						if (round)
							elev = string.Format("{0:F2}", pnt3d.Z);
						else
							elev = pnt3d.Z.ToString();
						pnt3d = pnt3dX;
						break;

					case 1:
						if (round)
							elev = string.Format("{0:F2}", idCogoPnt.getCogoPntElevation());
						else
							elev = idCogoPnt.getCogoPntElevation().ToString();
							
						idCgPnt = idCogoPnt;
						pnt3d = pnt3dX;
						break;

					case 2:
						Application.ShowAlertDialog("Multiple CogoPoints at same location. Exiting...");
						break;
				}
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 256");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}
			return elev;
		}

		public static string
		getCogoPoint(string prompt1, out ObjectId idCgPnt, Point3d pnt3dBase, int osMode)
		{
			Object mode = SnapMode.getOSnap();
			idCgPnt = ObjectId.Null;
			string elev = string.Empty;

			Point3d pnt3dX = Pub.pnt3dO;

			ObjectId idCogoPnt = ObjectId.Null;
			bool escape;
			PromptStatus ps;
			try
			{
				if (pnt3dBase != Pub.pnt3dO)
				{
					pnt3dX = UserInput.getPoint(prompt1, pnt3dBase, out escape, out ps, osMode);
				}
				else
				{
					pnt3dX = UserInput.getPoint(prompt1, out ps, osMode);
				}

				if (pnt3dX == Pub.pnt3dO)
					return elev;

				ObjectIdCollection ids = new ObjectIdCollection();
				BaseObjs._db.forEachMS<CogoPoint>(cg =>
				{
					if (cg.Location == pnt3dX)
					{
						idCogoPnt = cg.ObjectId;
						ids.Add(idCogoPnt);
					}
				});

				switch (ids.Count)
				{
					case 0:
						break;

					case 1:
						elev = string.Format("{0:F2}", idCogoPnt.getCogoPntElevation());
						idCgPnt = idCogoPnt;
						break;

					case 2:
						Application.ShowAlertDialog("Multiple CogoPoints at same location. Exiting...");
						break;
				}
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 318");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}
			return elev;
		}

		public static string
		getCogoPoint(string prompt1, out ObjectId idCgPnt, ObjectId idCgPntBase, int osMode)
		{
			Object mode = SnapMode.getOSnap();
			string elev = string.Empty;
			ObjectId idCogoPnt = ObjectId.Null;
			idCgPnt = ObjectId.Null;

			Point3d pnt3dBase = Pub.pnt3dO;
			Point3d pnt3dX = Pub.pnt3dO;

			if (idCgPntBase != ObjectId.Null)
				pnt3dBase = idCgPntBase.getCogoPntCoordinates();

			bool escape;
			PromptStatus ps;
			try
			{
				if (pnt3dBase != Pub.pnt3dO)
				{
					pnt3dX = UserInput.getPoint(prompt1, pnt3dBase, out escape, out ps, osMode);
				}
				else
				{
					pnt3dX = UserInput.getPoint(prompt1, out ps, osMode);
				}

				if (pnt3dX == Pub.pnt3dO)
					return elev;

				ObjectIdCollection ids = new ObjectIdCollection();
				BaseObjs._db.forEachMS<CogoPoint>(cg =>
				{
					if (cg.Location == pnt3dX)
					{
						idCogoPnt = cg.ObjectId;
						ids.Add(idCogoPnt);
					}
				});

				switch (ids.Count)
				{
					case 0:
						break;

					case 1:
						elev = string.Format("{0:F2}", idCogoPnt.getCogoPntElevation());
						idCgPnt = idCogoPnt;
						break;

					case 2:
						Application.ShowAlertDialog("Multiple CogoPoints at same location. Exiting...");
						break;
				}
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 384");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}

			return elev;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="deflt"></param>
		/// <param name="prompt"></param>
		/// <param name="keywords"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool
		getUserInput(string deflt, string prompt, string keywords, out string result, bool spaces = false)
		{
			bool escape = false;
			Editor ed = BaseObjs._editor;
			PromptStringOptions pso = new PromptStringOptions(prompt);

			pso.UseDefaultValue = true;
			pso.DefaultValue = deflt;
			pso.AllowSpaces = spaces;
			pso.AppendKeywordsToMessage = true;

			string[] keyW = keywords.Split(new Char[] {
				' '
			});
			foreach (string s in keyW)
			{
				pso.Keywords.Add(s);
			}

			pso.SetMessageAndKeywords(prompt, keywords);

			PromptResult pr = ed.GetString(pso);

			if (pr.Status == PromptStatus.OK)
			{
				result = pr.StringResult.ToUpper();
			}
			else if (pr.Status == PromptStatus.None)
			{
				result = deflt;
			}
			else if (pr.Status == PromptStatus.Cancel)
			{
				escape = true;
				result = string.Empty;
			}
			else
				result = string.Empty;
			return escape;
		}

		public static bool
		getUserInput(string prompt, out string result, bool spaces = false)
		{
			bool escape = false;
			Editor ed = BaseObjs._editor;
			PromptStringOptions pso = new PromptStringOptions(prompt);
			pso.AllowSpaces = spaces;
			result = string.Empty;
			PromptResult pr = ed.GetString(pso);

			switch (pr.Status)
			{
				case PromptStatus.OK:
					result = pr.StringResult.ToUpper();
					break;

				case PromptStatus.Cancel:
					escape = true;
					break;

				case PromptStatus.None:
					result = "";
					break;
			}
			return escape;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="deflt"></param>
		/// <param name="prompt"></param>
		/// <param name="result"></param>
		public static bool
		getUserInput(string deflt, string prompt, out string result, bool spaces = false)
		{
			bool escape = false;
			Editor ed = BaseObjs._editor;
			PromptStringOptions pso = new PromptStringOptions(prompt);

			pso.UseDefaultValue = true;
			pso.DefaultValue = deflt;
			pso.AllowSpaces = spaces;
			result = string.Empty;

			PromptResult pr = ed.GetString(pso);

			switch (pr.Status)
			{
				case PromptStatus.OK:
					result = pr.StringResult.ToUpper();
					if (result == "")
						result = deflt;
					break;

				case PromptStatus.Cancel:
					escape = true;
					break;

				case PromptStatus.None:
					result = deflt;
					break;
			}
			return escape;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="result"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static bool
		getUserInput(string message, out double result, double def = -99.99, bool allow0 = true, bool allowNone = true)
		{
			bool escape = false;
			Editor ed = BaseObjs._editor;
			PromptDoubleOptions pdo = new PromptDoubleOptions(message);
			pdo.AllowNone = allowNone;
			pdo.AllowNegative = true;
			pdo.AllowZero = allow0;
			pdo.UseDefaultValue = true;

			if (def != -99.99)
			{
				pdo.DefaultValue = def;
			}
			PromptDoubleResult pdr = ed.GetDouble(pdo);
			if (pdr.Status == PromptStatus.OK)
			{
				escape = false;
				result = Convert.ToDouble(pdr.Value);
			}
			else if (pdr.Status == PromptStatus.None)
			{
				escape = false;
				result = def;
			}
			else if (pdr.Status == PromptStatus.Cancel)
			{
				escape = true;
				result = -99.99;
			}
			else
				result = -99.99;
			return escape;
		}

		public static bool
		getUserInputDoubleAsString(string message, out string result, string def = "-99.99")
		{
			bool escape = false;
			Editor ed = BaseObjs._editor;
			PromptDoubleOptions pdo = new PromptDoubleOptions(message);
			pdo.AllowNone = true;
			pdo.AllowNegative = true;
			pdo.AllowZero = true;

			if (def != "-99.99")
			{
				double defd = 0;
				double.TryParse(def, out defd);
				pdo.DefaultValue = defd;
			}
			PromptDoubleResult pdr = ed.GetDouble(pdo);
			if (pdr.Status == PromptStatus.OK)
			{
				escape = false;
				result = pdr.Value.ToString();
			}
			else if (pdr.Status == PromptStatus.None)
			{
				escape = false;
				result = def;
			}
			else if (pdr.Status == PromptStatus.Cancel)
			{
				escape = true;
				result = "-99.99";
			}
			else
				result = "-99.99";
			return escape;
		}

		public static string
		getUserInput(string strElev, string CMD)
		{
			Editor ED = BaseObjs._editor;
			PromptStringOptions PSO = null;
			PromptKeywordOptions PKO = null;
			PromptResult PR = null;

			String strPrompt = "";
			String strTop = "";
			String strBot = "";

			if (CMD == "FL")
			{
				strPrompt = "\nEnter FL Elevation:";
			}
			if (CMD == "FF")
			{
				strPrompt = string.Format("\nEnter FF Elevation <{0}>:", strElev);
			}

			PSO = new PromptStringOptions(strPrompt);
			PSO.AllowSpaces = false;
			PSO.DefaultValue = strElev;

			PR = ED.GetString(PSO);

			if (PR.Status == PromptStatus.OK)
			{
				strTop = PR.StringResult.ToUpper();
			}
			else
			{
				strTop = "";
			}

			if (CMD == "FL")
			{
				PKO = new PromptKeywordOptions("\nEnter Bottom Text -");
				PKO.Keywords.Add("fS");
				PKO.Keywords.Add("Fl");
				PKO.Keywords.Add("Other");
				PKO.Keywords.Default = "Fl";

				PR = ED.GetKeywords(PKO);

				if (PR.Status == PromptStatus.OK)
				{
					if (PR.StringResult == "Other")
					{
						PSO = new PromptStringOptions("\nEnter Description:");
						PSO.AllowSpaces = false;
						PR = ED.GetString(PSO);

						if (PR.Status == PromptStatus.OK)
						{
							strBot = PR.StringResult.ToUpper();
						}
						else
						{
							strBot = "";
						}
					}
					else
					{
						strBot = PR.StringResult.ToUpper();
					}
				}
				else
				{
					strBot = "";
				}
			}// end if "FL"

			if (CMD == "FF")
			{
				PSO = new PromptStringOptions("\nEnter Floor Slab Thickness:");
				PSO.AllowSpaces = false;
				PSO.DefaultValue = "0.50";
				PR = ED.GetString(PSO);

				if (PR.Status == PromptStatus.OK)
				{
					strBot = PR.StringResult.ToUpper();
				}
				else
				{
					strBot = "";
				}
			}// end if "FF"

			return string.Format("{0}\\P{1}", strTop, strBot);
		}

		public static string
		getUserInputGrade(string strElev, string CMD)
		{
			Editor ED = BaseObjs._editor;

			PromptStringOptions PSO = null;
			PromptKeywordOptions PKO = null;
			PromptResult PR = null;

			String strTop = "";
			String strBot = "";

			String strTopElev = "";
			String strTopDesc = "";
			String strBotElev = "";
			String strBotDesc = "";

			PSO = new PromptStringOptions("\nEnter Bottom Elevation:");
			PSO.AllowSpaces = false;
			PSO.DefaultValue = strElev;

			PR = ED.GetString(PSO);

			if (PR.Status == PromptStatus.OK)
			{
				strBotElev = PR.StringResult.ToUpper();
			}
			else
			{
				strBotElev = "";
			}

			PKO = new PromptKeywordOptions("\nEnter Bottom Label -");
			PKO.Keywords.Add("fS");
			PKO.Keywords.Add("Fl");
			PKO.Keywords.Add("Other");
			PKO.Keywords.Default = "Fl";

			PR = ED.GetKeywords(PKO);

			PR = ED.GetString(PSO);

			if (PR.Status == PromptStatus.OK)
			{
				if (PR.StringResult == "Other")
				{
					PSO = new PromptStringOptions("\nEnter Description:");
					PSO.AllowSpaces = false;
					PR = ED.GetString(PSO);

					if (PR.Status == PromptStatus.OK)
					{
						strBotDesc = PR.StringResult.ToUpper();
					}
					else
					{
						strBotDesc = "";
					}
				}
				else
				{
					strBotDesc = PR.StringResult.ToUpper();
				}
			}
			else
			{
				strBotDesc = "";
			}

			PKO = new PromptKeywordOptions("\nEnter Top Label");
			PKO.Keywords.Add("TC");
			PKO.Keywords.Add("Other");
			PKO.Keywords.Default = "TC";

			PR = ED.GetKeywords(PKO);

			if (PR.Status == PromptStatus.OK)
			{
				if (PR.StringResult == "Other")
				{
					PSO = new PromptStringOptions("\nEnter Description:");
					PSO.AllowSpaces = false;
					PR = ED.GetString(PSO);

					if (PR.Status == PromptStatus.OK)
					{
						strBotDesc = PR.StringResult.ToUpper();
					}
					else
					{
						strBotDesc = "";
					}
				}
				else
				{
					strTopDesc = PR.StringResult.ToUpper();
				}
			}
			else
			{
				strTopDesc = "";
			}

			PSO = new PromptStringOptions("\nEnter Curb Height:");
			PSO.AllowSpaces = false;
			PSO.DefaultValue = "0.50";
			PR = ED.GetString(PSO);

			if (PR.Status == PromptStatus.OK)
			{
				strBotElev = PR.StringResult.ToUpper();
			}
			else
			{
				strBotElev = "";
			}

			strTop = string.Format("{0} {1}", strTopElev, strTopDesc);
			strBot = string.Format("{0} {1}", strBotElev, strBotDesc);

			return string.Format("{0}\\P{1}", strTop, strBot);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="deflt"></param>
		/// <param name="result"></param>
		/// <param name="messageAndKeywords"></param>
		/// <param name="globalKeywords"></param>
		/// <returns></returns>
		public static bool
		getUserInputKeyword(string deflt, out string result, string messageAndKeywords, string globalKeywords)
		{
			result = "";
			bool escape = false;
			Editor ed = BaseObjs._editor;

			PromptKeywordOptions pko = new PromptKeywordOptions(messageAndKeywords);

			pko.AllowNone = true;
			pko.AppendKeywordsToMessage = true;
			pko.AllowArbitraryInput = true;

			string[] keyW = globalKeywords.Split(new Char[] {
				' '
			});

			List<string> keys = new List<string>();
			foreach (object s in keyW)
			{
				pko.Keywords.Add(s.ToString());
				keys.Add(s.ToString());
			}
			if (keys.Contains(deflt))
				pko.Keywords.Default = deflt;
			else
			{
				pko.Keywords.Add(deflt);
				pko.Keywords.Default = deflt;
			}

			pko.SetMessageAndKeywords(messageAndKeywords, globalKeywords);

			PromptResult pr = ed.GetKeywords(pko);
			switch (pr.Status)
			{
				case PromptStatus.Cancel:
					escape = true;
					break;

				case PromptStatus.Error:
					escape = false;
					break;

				case PromptStatus.None:
					result = deflt;
					escape = false;
					break;

				case PromptStatus.OK:
					result = pr.StringResult;
					escape = false;
					break;

				case PromptStatus.Keyword:
					result = pr.StringResult;
					escape = false;
					break;
			}
			return escape;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="xOff"></param>
		/// <param name="yOff"></param>
		public static void
		getXYoffset(out double xOff, out double yOff)
		{
			getUserInput("Enter X Offset: ", out xOff);
			getUserInput("Enter Y Offset: ", out yOff);
		}

		public static Double
		pickAngle(Point3d pnt3dBase, int osMode)
		{
			object mode = SnapMode.getOSnap();
			SnapMode.setOSnap(osMode);

			Editor ED = BaseObjs._editor;
			Point3d pnt3d;
			PromptPointOptions PPO = new PromptPointOptions("\nSelect Last Point: ");
			PPO.UseBasePoint = true;
			PPO.BasePoint = pnt3dBase;

			double dblRotation = 0.0;
			try
			{
				PromptPointResult PPR = ED.GetPoint(PPO);
				if (PPR.Status == PromptStatus.OK)
				{
					pnt3d = PPR.Value;
				}
				else
				{
					return -999.999;
				}

				dblRotation = Base_Tools45.Measure.getAzRadians(pnt3dBase, pnt3d);
			}
			catch (System.Exception ex)
			{
                BaseObjs.writeDebug(ex.Message + " UserInput.cs: line: 918");
			}
			finally
			{
				SnapMode.setOSnap((int)mode);
			}

			return dblRotation;
		}

		public static PromptStatus
		getUserInputInt(string prmpt, bool bIntAllowArbitraryInput, bool bIntAllowNone,
			bool bIntAllowZero, bool bIntAllowNegative, bool bIntUseDefault,
			int def, out int answer)
		{
			PromptIntegerOptions prOpts = new PromptIntegerOptions(prmpt);
			prOpts.AllowArbitraryInput = bIntAllowArbitraryInput;
			prOpts.AllowNone = bIntAllowNone;
			prOpts.AllowZero = bIntAllowZero;
			prOpts.AllowNegative = bIntAllowNegative;
			prOpts.UseDefaultValue = bIntUseDefault;
			if (bIntUseDefault)
				prOpts.DefaultValue = def;

			Editor ed = BaseObjs._editor;

			PromptIntegerResult prRes = ed.GetInteger(prOpts);
			if (prRes.Status == PromptStatus.OK)
				answer = prRes.Value;
			else
				answer = 0; //bogus value

			return prRes.Status;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prmpt"></param>
		/// <param name="def"></param>
		/// <param name="answer"></param>
		/// <returns></returns>
		public static PromptStatus
		getUserInputYesNo(string prmpt, bool def, out bool answer)
		{
			Editor ed = BaseObjs._editor;

			answer = false;

			string defStr = (def) ? "Yes" : "No";
			PromptKeywordOptions prOpts = new PromptKeywordOptions(prmpt);
			prOpts.Keywords.Add("Yes");
			prOpts.Keywords.Add("No");
			prOpts.Keywords.Default = defStr;

			PromptResult prRes = ed.GetKeywords(prOpts);
			if (prRes.Status == PromptStatus.OK)
			{
				if (prRes.StringResult == "Yes")
					answer = true;
			}

			return prRes.Status;
		}
	}
}
