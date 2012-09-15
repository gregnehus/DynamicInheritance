using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    public class When_adding_a_base_class_to_a_defined_type
    {
        static object _result;

        Because of = () => _result =  typeof(SomeObject).GetInstanceWithBaseType<BaseType>();
        It should_return_an_instance = () => _result.ShouldNotBeNull();
        It should_return_an_object_that_inherits_from_specified_base_class = () => _result.GetType().BaseType.ShouldEqual(typeof(BaseType));
    }
}