using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject("Inheritance")]
    public class When_adding_a_base_class_to_a_defined_type
    {
        static object _result;

        Because of = () => _result =  typeof(SomeObject).GetInstanceWithBaseType<BaseType>();
        It should_return_an_instance = () => _result.ShouldNotBeNull();
        It should_return_an_object_that_inherits_from_specified_base_class = () => _result.GetType().BaseType.ShouldEqual(typeof(BaseType));
        It should_be_able_to_cast_to_the_interface_of_the_base_type = () => (_result as IBaseType).ShouldNotBeNull();
        It should_be_able_to_cast_to_the_interface_of_the_sub_type = () => (_result as ISomeObject).ShouldNotBeNull();
    }
}