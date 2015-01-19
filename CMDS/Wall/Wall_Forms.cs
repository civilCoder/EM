namespace Wall
{
    public sealed class Wall_Forms
    {
        private static readonly Wall_Forms wallForms = new Wall_Forms();

        public static Wall_Forms wForms
        {
            get
            {
                return wallForms;
            }
        }

        private Wall_Forms()
        {
            fWall1 = new Wall_Form.frmWall1();
            fWall2 = new Wall_Form.frmWall2();
            fWall3 = new Wall_Form.frmWall3();
            fWall4 = new Wall_Form.frmWall4();
        }


        public Wall_Form.frmWall1 fWall1 { get; set; }
        public Wall_Form.frmWall2 fWall2 { get; set; }
        public Wall_Form.frmWall3 fWall3 { get; set; }
        public Wall_Form.frmWall4 fWall4 { get; set; }   
    }
}
