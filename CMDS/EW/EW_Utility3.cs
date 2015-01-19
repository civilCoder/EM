
using Autodesk.AutoCAD.DatabaseServices;

using Base_Tools45;

namespace EW
{
	public static class EW_Utility3
	{
		public const string Point_Groups = "CPNT-ON, CPNT-ST, CPNT-TRANS, CPNT-MISC, UTL-SEW, UTL-SD, UTL-WAT, UTL-MISC, SPNT, EXIST";
		

		public static void
		setUpLayers()
		{
			ObjectId idLayer = ObjectId.Null;

			idLayer = Layer.manageLayers("_XX-AUTO DRIVE");
			Layer.modifyLayer(idLayer, 63, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-AUTO PARKING");
			Layer.modifyLayer(idLayer, 203, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-BUILDING ADJACENT LANDSCAPING");
			Layer.modifyLayer(idLayer, 235, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-CONCRETE APRON");
			Layer.modifyLayer(idLayer, 180, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-DOCK APRON");
			Layer.modifyLayer(idLayer, 6, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-FLOOR SLAB_A");
			Layer.modifyLayer(idLayer, 30, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-FLOOR SLAB_B");
			Layer.modifyLayer(idLayer, 180, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-FLOOR SLAB_C");
			Layer.modifyLayer(idLayer, 14, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-FRONT LANDSCAPING");
			Layer.modifyLayer(idLayer, 35, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-OFFICE SLAB AND SAND");
			Layer.modifyLayer(idLayer, 153, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_XX-TRUCK PAVING");
			Layer.modifyLayer(idLayer, 114, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("_YY-K BRACE");
			Layer.modifyLayer(idLayer, 45, LineWeight.LineWeight035);

			idLayer = Layer.manageLayers("GRADING LIMIT");
			Layer.modifyLayer(idLayer, 1, LineWeight.LineWeight035);

            LinetypeTableRecord lttr = LineType.getLinetypeTableRecord("HIDDEN");
            using(var tr = BaseObjs.startTransactionDb()){
                LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(idLayer, OpenMode.ForWrite);
                ltr.LinetypeObjectId = lttr.ObjectId;
            }
		}
	}
}
