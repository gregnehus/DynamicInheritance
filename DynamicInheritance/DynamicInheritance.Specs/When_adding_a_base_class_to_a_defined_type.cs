using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof (ObjectExtensions))]
    public class When_adding_a_base_class_to_a_defined_type
    {
        static SomeObject _super = new SomeObject();
        static Type _result;

        Because of = () => _result =  _super.AddBaseType<BaseType>();
        It should_return_a_non_null_type = () => _result.ShouldNotBeNull();
        It should_return_a_type_that_inherits_from_specified_base_class = () => _result.BaseType.ShouldEqual(typeof(BaseType));
        It should_return_a_type_that_can_be_instantiated = () => Activator.CreateInstance(_result).ShouldNotBeNull();

    }

    [Subject(typeof(ObjectExtensions))]
    public class When_using_a_class_instantiated_from_dynamic_type
    {
        static SomeObject _super = new SomeObject();
        static Type _resultType;
        static dynamic _subject;

        Establish context = () => _resultType = _super.AddBaseType<BaseType>();

        Because of = () => _subject = Activator.CreateInstance(_resultType);

        It should_retain_the_original_method_functionality = () => _subject.GetSuperName().ShouldEqual("SomeObject");

    }
}