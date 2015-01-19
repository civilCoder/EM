namespace EventManager
{
    public class EM_CBoolClass
    {
        public bool val;

        public EM_CBoolClass(bool val)
        {
            this.val = val;
        }

        public override string ToString()
        {
            return (val.ToString());
        }
    }
}