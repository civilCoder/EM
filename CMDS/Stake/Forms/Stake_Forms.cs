namespace Stake.Forms
{
    public sealed class Stake_Forms
    {
        private static readonly Stake_Forms stakeForms = new Stake_Forms();

        public static Stake_Forms sForms
        {
            get
            {
                return stakeForms;
            }
        }

        private Stake_Forms()
        {
            fAddPoint = new Forms.frmAddPoint();
            fBldgNames = new Forms.frmBldgNames();
            fExport = new Forms.frmExport();
            fGrid = new Forms.frmGrid();
            fGridLabelEdit = new Forms.frmGridLabelEdit();
            fMisc = new Forms.frmMisc();
            fPickXref = new Forms.frmPickXref();
            fPoints = new Forms.frmPoints();
            fStake = new Forms.frmStake();
            fStaRange = new Forms.frmStaRange();
        }

        public Forms.frmAddPoint fAddPoint { get; set; }             
        public Forms.frmBldgNames fBldgNames { get; set; }         
        public Forms.frmExport fExport { get; set; }     
        public Forms.frmGrid fGrid { get; set; }      
        public Forms.frmGridLabelEdit fGridLabelEdit { get; set; }
        public Forms.frmMisc fMisc { get; set; }        
        public Forms.frmPoints fPoints { get; set; }       
        public Forms.frmStake fStake { get; set; }       
        public Forms.frmStaRange fStaRange { get; set; }
        public Forms.frmPickXref fPickXref { get; set; }
    }
}