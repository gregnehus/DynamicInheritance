namespace DynamicInheritance.Specs
{
    public class BaseType
    {
        public string GetBaseName()
        {
            return "BaseType";
        }
    }

    public class SomeObject
    {
        public string GetSuperName()
        {
            return "SomeObject";
        } 
        public string AddHashTag(string orig)
        {
            return "#" + orig;
        }

        public string UseInterface(IAmAnInterface inter)
        {
            return inter.DoSomething();
        }
    }

    public interface IAmAnInterface
    {
        string DoSomething();
    }
}
