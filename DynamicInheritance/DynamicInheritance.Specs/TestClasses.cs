namespace DynamicInheritance.Specs
{
    public interface IBaseType
    {
        string GetBaseName();
    }

    public class BaseType : IBaseType
    {
        public string name = "peach";
        public BaseType()
        {
            string p = "s";
        }
        public string GetBaseName()
        {
            return "BaseType";
        }
    }

    public class GenericBaseType<T>
    {
        public string GetBaseName()
        {
            return string.Format("BaseType<{0}>", typeof(T));
        }
    }

    public class GenericSomeObject<T>
    {
        
    }

    public interface ISomeObject
    {
        string GetSuperName();
        string AddHashTag(string orig);
        string UseInterface(IAmAnInterface inter);
    }

    public class SomeObject : ISomeObject
    {
        public string name;
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

        public string GetField()
        {
            return name;
        }
    }

    public interface IAmAnInterface
    {
        string DoSomething();
    }
}
