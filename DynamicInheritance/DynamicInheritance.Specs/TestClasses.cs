namespace DynamicInheritance.Specs
{
    public interface IBaseType
    {
        string GetBaseName();
    }

    public class BaseType : IBaseType
    {
        protected string name = "peach";
        public SomeFieldType member;
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
        string SetField(string str);
        object SetMember(SomeFieldType blah);
    }

    public class SomeObject : ISomeObject
    {
        public string name;
        public object member;
        public string subName = "cherry";
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

        public string SetField(string str)
        {
            name = str;
            return GetField();
        }

        public object SetMember(SomeFieldType blah)
        {
            member = blah;
            return GetMember();
        }

        public object GetMember()
        {
            return member;
        }
        public string GetSubField()
        {
            return subName;
        }

    }
    public class SomeFieldType{}
    public interface IAmAnInterface
    {
        string DoSomething();
    }
}
