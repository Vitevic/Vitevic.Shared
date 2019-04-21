// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections;
using System.Collections.Generic;

namespace Vitevic.Shared
{
    /// This class are IEnumerable<string> to allow syntax like this:
    /// 
    public class ArgumentBuilder : IEnumerable<string>
    {
        public const string DefaultArgumentSeparator = " ";

        private readonly List<string> _arguments = new List<string>();

        public string ArgumentSeparator { get; set; }
        public IReadOnlyList<string> Arguments { get => _arguments; }

        public ArgumentBuilder(string argumentSeparator = DefaultArgumentSeparator)
        {
            ArgumentSeparator = argumentSeparator;
        }

        public void Add(string argument)
        {
            if( !string.IsNullOrWhiteSpace(argument) )
            {
                _arguments.Add(argument);
            }
        }
        public void Add(bool condition, string argument)
        {
            if( condition )
            {
                Add(argument);
            }
        }
        public void Add(bool condition, string argument, string elseArgument)
        {
            if( condition )
            {
                Add(argument);
            }
            else
            {
                Add(elseArgument);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _arguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(ArgumentSeparator, _arguments);
        }
    }
}
