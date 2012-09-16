using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(TypeExtensions))]
    public class When_adding_a_generic_base_class_to_a_generic_type
    {
        static object _result;

        Because of = () => _result = typeof(GenericSomeObject<string>).GetInstanceWithBaseType<GenericBaseType<string>>();

        It should_return_an_instnace = () => _result.ShouldNotBeNull();
        It should_return_a_object_that_inherits_from_specified_base_class = () => _result.GetType().BaseType.ShouldEqual(typeof(GenericBaseType<string>));


    }
}