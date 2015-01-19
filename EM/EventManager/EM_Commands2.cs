using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

using Base_Tools45;
using Base_Tools45.C3D;

using System.Collections.Generic;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

//[assembly: CommandClass(typeof(EventManager.EM_Commands))]

namespace EventManager
{
	public partial class EM_Commands
	{
		Editor ed = BaseObjs._editor;

		[CommandMethod("VX")]
		public void cmdVX()
		{
			ed.Command("-VPOINT");
		}


		[CommandMethod("COUT")]
		public void cmdCOUT()
		{
			Cout.CadOUT.doCadOUT("cmdCOUT");
		}


		[CommandMethod("COUTX")]
		public void cmdCOUTX()
		{
			Cout.CadOUT.doCadOUT("cmdCOUTX");
		}

		[CommandMethod("ABL")]
		public void cmdABL()
		{
			try
			{
				ProcessFigures.ABL.addBreakline();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 55");
			}
		}

		[CommandMethod("CO", "ALR", CommandFlags.UsePickSet)]
		public void cmdALR()
		{
			try
			{
				Bubble.BB_Ldr.addLdr();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 68");
			}
		}

		[CommandMethod("GRADING", "AJP", CommandFlags.UsePickSet)]
		public void cmdAJP()
		{
			try
			{
				Grading.Cmds.cmdAJP.AJP();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 81");
			}
		}

		[CommandMethod("ABC")]
		public void cmdABC()
		{
			try
			{
				Grading.Cmds.cmdABC.ABC("cmdABC");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 94");
			}
		}

		[CommandMethod("ABG")]
		public void cmdABG()
		{
			try
			{
				Grading.Cmds.cmdABC.ABC("cmdABG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 107");
			}
		}

		[CommandMethod("arcg")]
		public void cmdARCG()
		{
			try
			{
				acedPostCommand("(arcg_x)\r");
				//Grading.Cmds.cmdARCG.ARCG();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 121");
			}
		}

		[CommandMethod("GRADING", "AV", CommandFlags.UsePickSet)]
		public void cmdAV()
		{
			try
			{
				Grading.Cmds.cmdAV.AV();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 134");
			}
		}

		[CommandMethod("GRADING", "AVG", CommandFlags.UsePickSet)]
		public void cmdAVG()
		{
			try
			{
				Grading.Grading_Palette.gPalette.showPalettes(vis: true, index: 0);
				Grading.Grading_Palette.gPalette.pGradeFloor.Initialize_Form();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 148");
			}
		}

		[CommandMethod("CO", "BB", CommandFlags.UsePickSet)]
		public void cmdBB()
		{
			try
			{
				using (BaseObjs._acadDoc.LockDocument())
				{
					Draw.bubble(1024);
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 164");
			}
		}

		[CommandMethod("GRADING", "BC", CommandFlags.UsePickSet)]
		public void cmdBC()
		{
			try
			{
				Grading.Cmds.cmdBC.BC("cmdBC");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 177");
			}
		}

		[CommandMethod("GRADING", "BC1", CommandFlags.UsePickSet)]
		public void cmdBC1()
		{
			try
			{
				Grading.Cmds.cmdBC1.BC1("cmdBC1");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 190");
			}
		}

		[CommandMethod("GRADING", "BD", CommandFlags.UsePickSet)]
		public void cmdBD()
		{
			try
			{
				Grading.Cmds.cmdBD.BD();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 203");
			}
		}

		[CommandMethod("GRADING", "BFL", CommandFlags.UsePickSet)]
		public void cmdBFL()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdFL", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 217");
			}
		}

		[CommandMethod("GRADING", "BG", CommandFlags.UsePickSet)]
		public void cmdBG()
		{
			try
			{
				Grading.Cmds.cmdBC.BC("cmdBG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 230");
			}
		}

		[CommandMethod("GRADING", "BL", CommandFlags.UsePickSet)]
		public void cmdBL()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdBL", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 244");
			}
		}

		[CommandMethod("GRADING", "BV", CommandFlags.UsePickSet)]
		public void cmdBV()
		{
			try
			{
				//acedPostCommand("undo mark\r");
				Grading.Cmds.cmdBV.BV();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 258");
			}
		}

		[CommandMethod("C", CommandFlags.UsePickSet | CommandFlags.Redraw)]
		public void cmdC()
		{
			try
			{
				acedPostCommand(".COPY\r");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 271");
			}
		}

		[CommandMethod("GRADING", "CBF", CommandFlags.UsePickSet)]
		public void cmdCFD()
		{
			try
			{
				Grading.Cmds.cmdCBF.CBF();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 284");
			}
		}

		[CommandMethod("CB", CommandFlags.UsePickSet)]
		public void cmdCB()
		{
			try
			{
				Bubble.BB_Copy.copyBub();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 297");
			}
		}

		[CommandMethod("CBB")]
		public void cmdCBB()
		{
			try
			{
				ConvertToXData.Convert_App.convertBubble();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 310");
			}
		}

		[CommandMethod("CO", "CDLTXD", CommandFlags.UsePickSet)]
		public void cmdCDLTXD()
		{
			try
			{
				ConvertToXData.Convert_App.convertDictionaryLinksToXData();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 336");
			}
		}

		[CommandMethod("CO", "CF0", CommandFlags.UsePickSet)]
		public void cmdCFO()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdCF0");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 349");
			}
		}

		[CommandMethod("CO", "cGS", CommandFlags.UsePickSet)]
		public void cmdConvertGS()
		{
			Point3d pnt3dPick = Pub.pnt3dO;
			try
			{
				Entity ent = Select.selectEntity(typeof(DBText), "select GS Text", "", out pnt3dPick);

				if (ent == null)
					return;

				ConvertToXData.Convert_App.convertGSItem(ent.ObjectId);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 368");
			}
		}

		[CommandMethod("GRADING", "CPXD", CommandFlags.UsePickSet)]
		public void cmdCPXD()
		{
			try
			{
				Grading.Cmds.cmdCPXD.checkPntXData();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 381");
			}
		}

		[CommandMethod("CO", "DA1", CommandFlags.UsePickSet)]
		public void cmdDA1()
		{
			try
			{
				DimPL.DimPL_App.dimpl(0.09, DimPL.DimPL_Global.maxRows090, 2.0);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 394");
			}
		}

		[CommandMethod("CO", "DA1t", CommandFlags.UsePickSet)]
		public void cmdDA1t()
		{
			try
			{
				DimPL.DimPL_App.dimpl(0.09, DimPL.DimPL_Global.maxRows090, 1.5);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 407");
			}
		}

		[CommandMethod("CO", "DA2", CommandFlags.UsePickSet)]
		public void cmdDA2()
		{
			try
			{
				DimPL.DimPL_App.dimpl(0.075, DimPL.DimPL_Global.maxRows075, 2.0);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 420");
			}
		}

		[CommandMethod("CO", "DA2t", CommandFlags.UsePickSet)]
		public void cmdDA2t()
		{
			try
			{
				DimPL.DimPL_App.dimpl(0.075, DimPL.DimPL_Global.maxRows075, 1.5);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 433");
			}
		}

		[CommandMethod("GRADING", "DBL", CommandFlags.UsePickSet)]
		public void cmdDBL()
		{
			var varResponse = MessageBox.Show("Warning:\r\rSelecting YES will delete all 3dBreaklines on layer CPNT-BRKLINES\r and remove all Breaklines from surface CPNT-ON.\r\rCONTINUE?", "DELETE BREAKLINES: SURFACE & DRAWING", MessageBoxButtons.OKCancel);
			if (varResponse == DialogResult.Yes)
			{
				try
				{
					using (BaseObjs._acadDoc.LockDocument())
					{
						Grading.Cmds.cmdDBL.deleteBreaklinesInSurface("CPNT-ON");
						Grading.Cmds.cmdDBL.deleteBreaklinesInDwg();
					}
				}
				catch (System.Exception ex)
				{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 453");
				}
			}
			else
			{
				return;
			}
		}

		[CommandMethod("CO", "DEP", CommandFlags.UsePickSet)]
		public void cmdDEP()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdDEP");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 471");
			}
		}

		[CommandMethod("CO", "DL", CommandFlags.UsePickSet)]
		public void cmdDL()
		{
			SelectionSet ss = Select.buildSSet(typeof(CogoPoint), false);

			if (ss.Count == 0)
			{
				BaseObjs.write("\nNo Cogo Points selected.\n");
				return;
			}

			double xOff, yOff;

			UserInput.getXYoffset(out xOff, out yOff);
			Vector3d vec3d = new Vector3d(xOff, yOff, 0);

			try
			{
				using (Transaction TR = BaseObjs.startTransactionDb())
				{
					ObjectId[] idObjs = ss.GetObjectIds();
					foreach (ObjectId id in idObjs)
					{
						CogoPoint cogoPnt = (CogoPoint)TR.GetObject(id, OpenMode.ForWrite);
						cogoPnt.ResetLabel();
						cogoPnt.LabelLocation = cogoPnt.LabelLocation + vec3d;
					}

					TR.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 508");
			}
		}

		[CommandMethod("CO", "FFD", CommandFlags.UsePickSet)]
		public void cmdFD()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdFFD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 521");
			}
		}

		[CommandMethod("CO", "FF", CommandFlags.UsePickSet)]
		public void cmdFF()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdFF");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 534");
			}
		}

		[CommandMethod("CO", "FL", CommandFlags.UsePickSet)]
		public void cmdFL()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdFL");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 547");
			}
		}

		[CommandMethod("CO", "FLX", CommandFlags.UsePickSet)]
		public void cmdFLX()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdFLX");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 560");
			}
		}

		[CommandMethod("CLKS")]
		public void cmdFLKS()
		{
			try
			{

				//fixGradeTagLinks.App.executeCLKS();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 574");
			}
		}

		[CommandMethod("CO", "G", CommandFlags.UsePickSet)]
		public void cmdG()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 587");
			}
		}

		[CommandMethod("GRADING", "GB", CommandFlags.UsePickSet)]
		public void cmdGB()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdGB", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 601");
			}
		}

		[CommandMethod("GRADING", "GD", CommandFlags.UsePickSet)]
		public void cmdGD()
		{
			try
			{
				Grading.Cmds.cmdGD.GD();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 614");
			}
		}

		[CommandMethod("CO", "GQ", CommandFlags.UsePickSet)]
		public void cmdGQ()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGQ");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 627");
			}
		}


		[CommandMethod("GRADING", "GRADE", CommandFlags.UsePickSet)]
		public void cmdGrade()
		{
			try
			{
				Grading.Grading_Palette.gPalette.showPalettes(vis: true, index: 2);
				Grading.Grading_Palette.gPalette.pGradeFloor.Initialize_Form();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 642");
			}
		}

		[CommandMethod("CO", "GS", CommandFlags.UsePickSet)]
		public void cmdGS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 655");
			}
		}

		[CommandMethod("CO", "GS0", CommandFlags.UsePickSet)]
		public void cmdGS0()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGS0");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 668");
			}
		}

		[CommandMethod("CO", "GS2", CommandFlags.UsePickSet)]
		public void cmdGS2()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGS2");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 681");
			}
		}

		[CommandMethod("CO", "GS3", CommandFlags.UsePickSet)]
		public void cmdGS3()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGS3");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 694");
			}
		}

		[CommandMethod("CO", "GSE", CommandFlags.UsePickSet)]
		public void cmdGSE()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGSE");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 707");
			}
		}

		[CommandMethod("CO", "GSS", CommandFlags.UsePickSet)]
		public void cmdGSS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGSS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 720");
			}
		}

		[CommandMethod("CO", "GSX", CommandFlags.UsePickSet)]
		public void cmdGSX()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGSX");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 733");
			}
		}

		[CommandMethod("CO", "GX", CommandFlags.UsePickSet)]
		public void cmdGX()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdGX");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 746");
			}
		}

		[CommandMethod("CO", "HB", CommandFlags.UsePickSet)]
		public void cmdHB()
		{
			try
			{
				Draw.bubble(6);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 759");
			}
		}

		[CommandMethod("CO", "LD", CommandFlags.UsePickSet)]
		public void cmdLD()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdLD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 772");
			}
		}

		[CommandMethod("GRADING", "MB", CommandFlags.UsePickSet)]
		public void cmdMB()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdMB", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 786");
			}
		}

		[CommandMethod("GRADING", "MBL", CommandFlags.UsePickSet)]
		public void cmdMBL()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdMBL", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 800");
			}
		}

		[CommandMethod("GRADING", "GBM", CommandFlags.UsePickSet)]
		public void cmdGBM()
		{
			ObjectId idPoly = ObjectId.Null;
			try
			{
				BrkLine.makeBreakline(apps.lnkBrks, "cmdGBM", out idPoly);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 814");
			}
		}

		[CommandMethod("GRADING", "MCE", CommandFlags.UsePickSet)]
		public void cmdMCE()
		{
			try
			{
				Grading.Cmds.cmdMCE.MCE();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 827");
			}
		}

		[CommandMethod("GRADING", "MCG", CommandFlags.UsePickSet)]
		public void cmdMCG()
		{
			try
			{
				Grading.Cmds.cmdMCG.MCG();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 840");
			}
		}

		[CommandMethod("GRADING", "MR", CommandFlags.UsePickSet)]
		public void cmdMR()
		{
			try
			{
				Grading.Cmds.cmdRDR.MR();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 853");
			}
		}

		[CommandMethod("CO", "PLBDS", CommandFlags.UsePickSet)]
		public void cmdPBDSL()
		{
			try
			{
				acedPostCommand("(plbds_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLBDS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 867");
			}
		}

		[CommandMethod("CO", "PFA", CommandFlags.UsePickSet)]
		public void cmdPFA()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFA");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 880");
			}
		}

		[CommandMethod("CO", "PFBEG", CommandFlags.UsePickSet)]
		public void cmdPFBEG()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFBEG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 893");
			}
		}

		[CommandMethod("CO", "PFDIM", CommandFlags.UsePickSet)]
		public void cmdPFDIM()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFDIM");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 906");
			}
		}

		[CommandMethod("CO", "PFDIML", CommandFlags.UsePickSet)]
		public void cmdPFDIML()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFDIML");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 919");
			}
		}

		[CommandMethod("CO", "PFDIMS", CommandFlags.UsePickSet)]
		public void cmdPFDIMS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFDIMS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 932");
			}
		}

		[CommandMethod("CO", "PFDM", CommandFlags.UsePickSet)]
		public void cmdPFDM()
		{
			try
			{
				acadApp.RunMacro("Module4.pfdm");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFDM");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 946");
			}
		}

		[CommandMethod("CO", "PFDP", CommandFlags.UsePickSet)]
		public void cmdPFDP()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFDP");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 959");
			}
		}

		[CommandMethod("CO", "PFED", CommandFlags.UsePickSet)]
		public void cmdPFED()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFED");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 972");
			}
		}

		[CommandMethod("CO", "PFEND", CommandFlags.UsePickSet)]
		public void cmdPFEND()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFEND");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 985");
			}
		}

		[CommandMethod("CO", "PFES", CommandFlags.UsePickSet)]
		public void cmdPFES()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFES");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 998");
			}
		}

		[CommandMethod("CO", "PFI", CommandFlags.UsePickSet)]
		public void cmdPFI()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFI");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1011");
			}
		}

		[CommandMethod("CO", "PFIS", CommandFlags.UsePickSet)]
		public void cmdPFIS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFIS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1024");
			}
		}

		[CommandMethod("CO", "PFS", CommandFlags.UsePickSet)]
		public void cmdPFS()
		{
			try
			{
				acadApp.RunMacro("Module4.pfs");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1038");
			}
		}

		[CommandMethod("CO", "PFSDE", CommandFlags.UsePickSet)]
		public void cmdPFSDE()
		{
			try
			{
				acadApp.RunMacro("Module4.pfsde");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFSDE");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1052");
			}
		}

		[CommandMethod("CO", "PFSDM", CommandFlags.UsePickSet)]
		public void cmdPFSDM()
		{
			try
			{
				acadApp.RunMacro("Module4.pfsdm");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFSDM");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1066");
			}
		}

		[CommandMethod("CO", "PFSDP", CommandFlags.UsePickSet)]
		public void cmdPFSDP()
		{
			try
			{
				acadApp.RunMacro("Module4.pfsdp");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFSDP");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1080");
			}
		}

		[CommandMethod("CO", "PFSI", CommandFlags.UsePickSet)]
		public void cmdPFSI()
		{
			try
			{
				acedPostCommand("(pfssinv_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFSI");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1094");
			}
		}

		[CommandMethod("CO", "PFSIE", CommandFlags.UsePickSet)]
		public void cmdPFSIE()
		{
			try
			{
				acedPostCommand("(pfstainv_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFSIE");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1108");
			}
		}

		[CommandMethod("CO", "PFXI", CommandFlags.UsePickSet)]
		public void cmdPFXI()
		{
			try
			{
				acedPostCommand("(pfsecinv_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPFXI");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1122");
			}
		}

		[CommandMethod("CO", "PLA", CommandFlags.UsePickSet)]
		public void cmdPLA()
		{
			try
			{
				acedPostCommand("(pla_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLA");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1136");
			}
		}

		[CommandMethod("CO", "PLBD", CommandFlags.UsePickSet)]
		public void cmdPLBD()
		{
			try
			{
				acedPostCommand("(plbd_x)\r");
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLBD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1150");
			}
		}

		[CommandMethod("CO", "PLBEG", CommandFlags.UsePickSet)]
		public void cmdPLBEG()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLBEG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1163");
			}
		}

		[CommandMethod("CO", "PLCD", CommandFlags.UsePickSet)]
		public void cmdPLCD()
		{
			try
			{
				acedPostCommand("(plcd_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLCD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1177");
			}
		}

		[CommandMethod("CO", "PLEND", CommandFlags.UsePickSet)]
		public void cmdPLEND()
		{
			try
			{
				acedPostCommand("(plend_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLEND");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1191");
			}
		}

		[CommandMethod("CO", "PLMH", CommandFlags.UsePickSet)]
		public void cmdPLMH()
		{
			try
			{
				acadApp.RunMacro("Module4.PLMH");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLMH");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1205");
			}
		}

		[CommandMethod("CO", "PLSCO", CommandFlags.UsePickSet)]
		public void cmdPLSCO()
		{
			try
			{
				acedPostCommand("(plssco_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLSCO");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1219");
			}
		}

		[CommandMethod("CO", "PLSMH", CommandFlags.UsePickSet)]
		public void cmdPLSMH()
		{
			try
			{
				acedPostCommand("(plssmh_x)\r");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLSMH");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1233");
			}
		}

		[CommandMethod("CO", "PLX", CommandFlags.UsePickSet)]
		public void cmdPLX()
		{
			try
			{
				acadApp.RunMacro("Module4.plx");
				//LdrText.LdrText_ProcessCmds.ProcessCmds("cmdPLX");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1247");
			}
		}

		[CommandMethod("PPF2", CommandFlags.UsePickSet)]
		public void cmdPPF2()
		{
			try
			{
				ProcessPointFile.PPF_APP.runApp();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1260");
			}
		}

		[CommandMethod("GRADING", "RB", CommandFlags.Redraw)]
		public void cmdRB()
		{
			try
			{
				Grading.Cmds.cmdRB.RB();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1273");
			}
		}

		[CommandMethod("GRADING", "RB0", CommandFlags.Redraw)]
		public void cmdRB0()
		{
			try
			{
				Grading.Cmds.cmdABL.removeBreaklinewWith0vertices();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1286");
			}
		}

		[CommandMethod("GRADING", "RBA", CommandFlags.Redraw)]
		public void cmdRBA()
		{
			try
			{
				Grading.Cmds.cmdRB.RBA();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1299");
			}
		}

		[CommandMethod("GRADING", "RDRd", CommandFlags.NoActionRecording)]
		public void cmdRDR()
		{
			try
			{
				Grading.Cmds.cmdRDR.RDR();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1312");
			}
		}

		[CommandMethod("GRADING", "RDR", CommandFlags.NoActionRecording)]
		public void cmdRDRold()
		{
			try
			{
				acedPostCommand("(rdr_x)\r");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1325");
			}
		}

		[CommandMethod("GRADING", "RL", CommandFlags.UsePickSet)]
		public void cmdRL()
		{
			try
			{
				Grading.Cmds.cmdRL.RL(apps.lnkBrks, "cmdRL");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1338");
			}
		}

		[CommandMethod("GRADING", "RPJ", CommandFlags.UsePickSet)]
		public void cmdRPJ()
		{
			try
			{
				Grading.Cmds.cmdS.S(Pub.pnt3dO, "CPNT-JOIN");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1351");
			}
		}

		[CommandMethod("CO", "RQ", CommandFlags.UsePickSet)]
		public void cmdRQ()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdRQ");
				//acedPostCommand("(rq_x)\r");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1365");
			}
		}

		[CommandMethod("riser")]
		public void cmdRiser_x()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdRiser");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1378");
			}
		}

		[CommandMethod("GRADING", "RT", CommandFlags.UsePickSet)]
		public void cmdRT()
		{
			try
			{
				Grading.Cmds.cmdRL.RL(apps.lnkBrks, "cmdRT");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1391");
			}
		}

		[CommandMethod("GRADING", "RTd", CommandFlags.UsePickSet)]
		public void cmdRTd()
		{
			try
			{
				Grading.Cmds.cmdRTD.RTD(apps.lnkBrks);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1404");
			}
		}

		[CommandMethod("GRADING", "RTr", CommandFlags.UsePickSet)]
		public void cmdRTr()
		{
			try
			{
				Grading.Cmds.cmdRTx.SetPointBySlopeFromRef3dPoly();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1417");
			}
		}

		[CommandMethod("GRADING", "S", CommandFlags.UsePickSet)]
		public void cmdS()
		{
			try
			{
				Grading.Cmds.cmdS.S(Pub.pnt3dO, "CPNT-ON");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1430");
			}
		}

		[CommandMethod("SA", CommandFlags.UsePickSet)]
		public void cmdSA()
		{
			try
			{
				short[] colors = {
					10,
					20,
					30,
					40,
					50,
					60,
					70,
					80,
					90,
					100
				};
				Surf_Analysis.doSurfaceAnalysis("EXIST", 10, colors);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1455");
			}
		}

		[CommandMethod("CO", "SB", CommandFlags.UsePickSet)]
		public void cmdSB()
		{
			try
			{
				Draw.bubble(4);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1468");
			}
		}

		[CommandMethod("CO", "SBD", CommandFlags.UsePickSet)]
		public void cmdSBD()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSBD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1481");
			}
		}

		[CommandMethod("CO", "SDE", CommandFlags.UsePickSet)]
		public void cmdSDE()
		{
			try
			{
				//acadApp.RunMacro("Module4.sta_desc_elev");
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSDE");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1495");
			}
		}

		[CommandMethod("CO", "SDS", CommandFlags.UsePickSet)]
		public void cmdSDS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSDS");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1508");
			}
		}

		[CommandMethod("CO", "SE", CommandFlags.UsePickSet)]
		public void cmdSE()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSE");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1521");
			}
		}

		[CommandMethod("CO", "SED", CommandFlags.UsePickSet)]
		public void cmdSED()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSED");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1534");
			}
		}

		[CommandMethod("GRADING", "SG", CommandFlags.UsePickSet)]
		public void slopeGrade()
		{
			Grading.Cmds.cmdSG.SG();
		}

		[CommandMethod("CO", "SS", CommandFlags.UsePickSet)]
		public void cmdSS()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSL");       //same as cmdSL
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1553");
			}
		}

		[CommandMethod("GRADING", "SSD", CommandFlags.UsePickSet)]
		public void cmdSSD()
		{
			try
			{
				Grading.Cmds.cmdS.S(Pub.pnt3dO, "UTL-SD");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1566");
			}
		}

		[CommandMethod("GRADING", "SSE", CommandFlags.UsePickSet)]
		public void cmdSSW()
		{
			try
			{
				Grading.Cmds.cmdS.S(Pub.pnt3dO, "UTL-SEW");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1579");
			}
		}

		[CommandMethod("GRADING", "SX", CommandFlags.UsePickSet)]
		public void cmdSX()
		{
			try
			{
				Grading.Cmds.cmdS.S(Pub.pnt3dO, "EXIST");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1592");
			}
		}

		[CommandMethod("CO", "TB", CommandFlags.UsePickSet)]
		public void cmdTB()
		{
			try
			{
				Draw.bubble(3);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1605");
			}
		}

		[CommandMethod("GRADING", "TP1", CommandFlags.Interruptible | CommandFlags.Transparent)]
		public void cmdTP1(){
			try
			{
				Grading.Cmds.cmdTP.TP1();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1617");
			}           
		}

		[CommandMethod("GRADING", "TP2", CommandFlags.Interruptible | CommandFlags.Transparent)]
		public void cmdTP2()
		{
			try
			{
				Grading.Cmds.cmdTP.TP2();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1630");
			}
		}

		[CommandMethod("GRADING", "TP3", CommandFlags.Interruptible | CommandFlags.Transparent)]
		public void cmdTP3()
		{
			try
			{
				Grading.Cmds.cmdTP.TP3();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1643");
			}
		}

		[CommandMethod("GRADING", "TP4", CommandFlags.Interruptible | CommandFlags.Transparent)]
		public void cmdTP4()
		{
			try
			{
				Grading.Cmds.cmdTP.TP4();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1656");
			}
		}


		[CommandMethod("GRADING", "UBP", CommandFlags.UsePickSet)]
		public void cmdUBP()
		{
			try
			{
				Grading.Cmds.cmdUBP.updateBrkLines_Pnts();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1670");
			}
		}

		[CommandMethod("GRADING", "UC", CommandFlags.UsePickSet)]
		public void cmdUC()
		{
			try
			{
				Grading.Cmds.cmdUC1.updateControl();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1683");
			}
		}

		[CommandMethod("GRADING", "uPL", CommandFlags.UsePickSet)]
		public void cmdUPL()
		{
			TypedValue[] TVs = new TypedValue[1];
			TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
			SelectionSet ss = Select.buildSSet(TVs);
			ObjectId[] ids = ss.GetObjectIds();
			List<string> styles = new List<string>();
			CogoPoint cogoPnt = null;
			using (BaseObjs._acadDoc.LockDocument())
			{
				using (Transaction TR = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId id in ids)
					{
						try
						{
							cogoPnt = (CogoPoint)id.GetObject(OpenMode.ForRead);
							ObjectId idStyle = cogoPnt.LabelStyleId;
							LabelStyle style = (LabelStyle)idStyle.GetObject(OpenMode.ForRead);
							if (!styles.Contains(style.Name))
							{
								styles.Add(style.Name);
							}
						}
						catch (System.Exception ex)
						{
							string nameLayer = cogoPnt.Layer;
							if (!styles.Contains(nameLayer))
							{
								styles.Add(nameLayer);
							}
							BaseObjs.writeDebug(string.Format("{0} EM_Commands2.cs: line: 1115", ex.Message));
						}
					}
					TR.Commit();
				}
			}
		}

		[CommandMethod("VB", CommandFlags.UsePickSet)]
		public void cmdVB()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdVB");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1736");
			}
		}

		[CommandMethod("CO", "VG", CommandFlags.UsePickSet)]
		public void cmdVG()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdVG");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1749");
			}
		}

		[CommandMethod("CO", "WQ", CommandFlags.UsePickSet)]
		public void cmdWQ()
		{
			try
			{
				LdrText.LdrText_ProcessCmds.ProcessCmds("cmdWQ");
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1762");
			}
		}

		[CommandMethod("WS")]
		public void cmdWS()
		{
			try
			{
				WSP.WSP_App.addWaterSurfaceProfile();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1775");
			}
		}

		[CommandMethod("GRADING", "XO", CommandFlags.UsePickSet)]
		public void cmdXO()
		{
			try
			{
				Grading.Cmds.cmdXO.XO();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1788");
			}
		}

		[CommandMethod("CO", "CON", CommandFlags.UsePickSet)]
		public void conLabel()
		{
			try
			{
				BaseObjs.sendCommand("(CONTG_x)\r");
				//LabelContours.ContourLabels.doLabels();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1802");
			}
		}

		[CommandMethod("PF")]
		public void pf()
		{
			try
			{
				ProcessFigures.PF.doProcessFigures();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1815");
			}
		}

		[CommandMethod("UBL")]
		public void ubl()
		{
			try
			{
				ProcessFigures.UBL.updateBrkLines();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1828");
			}
		}

		[CommandMethod("GRADING", "CPA", CommandFlags.UsePickSet)]
		public void checkPointsAlign()
		{
			try
			{
				SDrain.SDrain_App.checkPointsToAlign();
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1841");
			}
		}

		[CommandMethod("WDX")]
		public void cmdWDX()
		{
			Wall.Wall_Form.frmWall1 fWall1 = Wall.Wall_Forms.wForms.fWall1;
			fWall1.opt1_SITE.Checked = true;
			fWall1.btn3_AlignMake.Visible = false;

			try
			{
				Application.ShowModelessDialog(Application.MainWindow.Handle, fWall1, false);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1858");
			}
		}

		[CommandMethod("WD")]
		public void cmdWD()
		{
			try{
				Application.ShowModelessDialog(Application.MainWindow.Handle, Wall.Wall_Forms.wForms.fWall2, false);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1870");
			}			            
		}

		[CommandMethod("BSA")]
		public void cmdBSA()
		{
			try
			{
				Application.ShowModelessDialog(Application.MainWindow.Handle, Wall.Wall_Forms.wForms.fWall3, false);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1883");
			}
		}

		[CommandMethod("BSA2")]
		public void cmdBSA2()
		{
			try
			{
				Application.ShowModelessDialog(Application.MainWindow.Handle, Wall.Wall_Forms.wForms.fWall4, false);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " EM_Commands2.cs: line: 1896");
			}
		}
	}
}
