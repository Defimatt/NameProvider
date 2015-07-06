using static System.Linq.Enumerable;
using static NameProvider.FullNameType;

namespace NameProvider
{
    internal class Example
    {
        private static void Main(string[] args)
        {
            var fnp = new FullNameGenerator();
            
            var maleNames = fnp.Names(Male);
            var femaleNames = fnp.Names(Female);

            fnp = new FullNameGenerator();
            var allNames = fnp.AllNames(Male).Take(200000).ToList();
        }
    }
}