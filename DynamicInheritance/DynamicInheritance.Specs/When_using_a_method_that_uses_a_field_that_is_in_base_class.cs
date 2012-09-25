using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    public class When_using_a_method_in_sub_class_that_reads_a_field_that_is_in_base_class : WithSomeObject
    {
        Because of = () => Result = (string)Subject.GetField();
        It should_use_the_field_in_the_base_class = () => Result.ShouldEqual("peach");
    }

    public class When_using_a_method_in_sub_class_that_reads_a_field_that_is_in_sub_class : WithSomeObject
    {
        Because of = () => Result = (string)Subject.GetSubField();
        It should_use_the_field_in_the_base_class = () => Result.ShouldEqual("cherry");
    }

    public class When_using_a_sub_class_method_that_sets_the_primitive_field_of_base_class :WithSomeObject
    {
        Because of = () => Result = Subject.SetField("boom");
        It should_return_the_saved_string = () => Result.ShouldEqual("boom");
    }

    public class When_using_a_sub_class_method_that_sets_a_nonprimitive_field_of_base_class : WithSomeObject
    {
        static SomeFieldType _obj = new SomeFieldType();
        Because of = () => _result = Subject.SetMember(_obj);
        It should_return_the_saved_string = () => _result.ShouldEqual<SomeFieldType>(_obj);
        static SomeFieldType _result;
    }
}
