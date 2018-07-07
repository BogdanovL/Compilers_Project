namespace ASTBuilder
{
    public abstract class AbstractAttr
    {
        string type;

        public AbstractAttr(string s)
        {
            this.type = s;
        }
        public dynamic typeInfo { get; internal set; }

    }

    public class Attributes : AbstractAttr
    {
        public Attributes(string s) : base(s)
        {

        }

    }
    
}