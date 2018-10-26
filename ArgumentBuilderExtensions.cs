using System.Collections.Generic;

namespace Vitevic.Shared
{
    // Had to move IEnumerable overloads here to avoid conflicts for null's:
    // The call is ambiguous between the following methods or properties: 'ArgumentBuilder.Add(string)' and 'ArgumentBuilder.Add(IEnumerable<string>)'
    public static class ArgumentBuilderExtensions
    {
        public static void Add(this ArgumentBuilder ab, IEnumerable<string> arguments)
        {
            foreach( var arg in arguments )
            {
                ab.Add(arg);
            }
        }
        public static void Add(this ArgumentBuilder ab, bool condition, IEnumerable<string> arguments)
        {
            if( condition )
            {
                ab.Add(arguments);
            }
        }
    }
}
