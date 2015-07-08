namespace NameProvider
{
    using System;
    using System.Collections.Generic;

    public sealed class ExampleNameBucketProvider : NameBucketProvider<ExampleNameBucketProvider>
    {
        public override IEnumerable<string> Fetch<TEnum>(TEnum nameType)
        {
            if (typeof(TEnum) != typeof(MaleFemaleGendersAndSurname))
                throw new NotSupportedException(
                    $"{nameof(ExampleNameBucketProvider)} only provides name buckets for {nameof(MaleFemaleGendersAndSurname)}");

            switch (EnumHelpers.Parse<MaleFemaleGendersAndSurname>(nameType))
            {
                case MaleFemaleGendersAndSurname.Male:
                    return new[] { "Aaron", "Matt", "Steve" };
                case MaleFemaleGendersAndSurname.Female:
                    return new[] { "Amelia", "Melanie", "Lyn" };
                case MaleFemaleGendersAndSurname.Surname:
                    return new[] { "Duffin", "Northey", "Stevens" };
                default:
                    throw new ArgumentOutOfRangeException(nameof(nameType));
            }
        }
    }
}