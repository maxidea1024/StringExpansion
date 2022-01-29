using System;
using System.Text;

namespace StringExpansion
{
    /// <summary>
    /// A class that contains optional values ​​related to string expansion.
    /// </summary>
    public class StringExpansionOptions
    {
        /// <summary>An object that provides the value of a variable used in StringExpander in the form of a string.</summary>
        public IVarProvider VarProvider { get; set; }

        /// <summary>
        /// Set whether to throw an exception when the variable is not found.
        /// If you set it to not throw an exception, nothing is printed.
        /// </summary>
        public bool ThrowIfVariableNotExists { get; set; } = true;

        /// <summary>
        /// Separator character to designate the variable to be substituted.
        /// Usually '$' or '%' characters are used.
        /// </summary>
        public char VarDelimiterChar { get; set; } = '%';

        /// <summary>
        /// This is the starting parenthesis character surrounding the variable name in the string.
        /// </summary>
        public char BeginBraceChar { get; set; } = '(';

        /// <summary>
        /// This is the ending parenthesis character surrounding the variable name in the string.
        /// </summary>
        public char EndBraceChar { get; set; } = ')';
    }

    /// <summary>
    /// Expand the string through string substitution.
    /// </summary>
    public class StringExpander
    {
        /// <summary>
        /// Maximum number of recursive calls. Used to prevent stack overflow due to circular references.
        /// </summary>
        public const int MaxRecursionDepth = 8;

        /// <summary>
        /// Expand the string through string substitution.
        /// </summary>
        /// <param name="source">Source string to expand.</param>
        /// <param name="options">Options for string expansion.</param>
        /// <returns>Expanded string.</returns>
        public static string Expand(string source, StringExpansionOptions options)
        {
            // In case of an empty string, it is returned as is.
            if (string.IsNullOrEmpty(source))
                return source;

            // If there are no delimiters, just return with input string.
            if (!source.Contains(options.VarDelimiterChar))
                return source;

            // Expand the string through string substitution.
            var result = new StringBuilder();
            ExpandInternal(result, source, options, 0);
            return result.ToString();
        }

        private static void ExpandInternal(StringBuilder result, string source, StringExpansionOptions options, int depth)
        {
            // Check the maximum number of recursive calls. Prevents stack overflow due to circular references.
            if (depth > MaxRecursionDepth)
                throw new Exception("Maximum recursive call Depth exceeded. Check out circular references.");

            // In case of an empty string, it is returned as is.
            if (string.IsNullOrEmpty(source))
                return;

            // If there are no delimiters, just append input string.
            if (!source.Contains(options.VarDelimiterChar))
            {
                result.Append(source);
                return;
            }

            int offset = 0;
            while (offset < source.Length)
            {
                // Is this a variable reference?
                if (source[offset] == options.VarDelimiterChar)
                {
                    // Parse the name of variable.

                    offset++;

                    if (offset == source.Length)
                        throw new Exception("No content after the delimiter.");


                    // Escaping

                    if (source[offset] == options.VarDelimiterChar)
                    {
                        result.Append(options.VarDelimiterChar);
                        offset++;
                        continue;
                    }


                    string varName;

                    if (source[offset] == options.BeginBraceChar)
                    {
                        offset++;

                        int varOffset = offset;

                        // Parse until the end of variable, skipping pairs of '(' and ')'
                        int level = 1;
                        while (level > 0 && offset < source.Length)
                        {
                            if (source[offset] == options.BeginBraceChar)
                                level++;
                            else if (source[offset] == options.EndBraceChar)
                                level--;

                            if (level > 0)
                                offset++; // don't skip over the last parenthesis
                        }

                        if (level != 0)
                            throw new Exception("The number of parentheses does not match.");

                        varName = source[varOffset..offset].Trim();
                        offset++;
                    }
                    else
                    {
                        varName = ParseIdentifierAndAdvance(source, ref offset);
                    }

                    // Empty variable name?
                    if (string.IsNullOrEmpty(varName))
                        throw new Exception("Variable name must not be empty.");

                    // If the preceding variable does not exist, the variable specified in the replacement syntax after ':' is used.
                    string alternative = null;

                    // After checking whether a substitution variable is specified, if a substitution variable is specified,
                    // after getting the substitution variable name
                    // If the specified variable does not exist, a substitution variable is used.
                    int colon = varName.IndexOf(':');
                    if (colon >= 0)
                    {
                        alternative = varName[(colon + 1)..].Trim();
                        varName = varName.Substring(0, colon).Trim();
                    }

                    // Get the value of the variable from the specified IVarProvider.
                    string value = options.VarProvider.GetVar(varName);
                    if (value == null)
                    {
                        if (!string.IsNullOrEmpty(alternative))
                        {
                            // Recursive
                            ExpandInternal(result, alternative, options, depth + 1);
                        }
                        else
                        {
                            if (options.ThrowIfVariableNotExists)
                            {
                                throw new Exception($"Variable `{varName}` not exists.");
                            }
                            else
                            {
                                // Just ignore(don't append)
                            }
                        }
                    }
                    else
                    {
                        // Recursive
                        ExpandInternal(result, value, options, depth + 1);
                    }
                }
                else
                {
                    // Just plain character output.
                    result.Append(source[offset++]);
                }
            }
        }

        static string ParseIdentifierAndAdvance(string source, ref int offset)
        {
            // Skip leading whitespace. But, could this cause ambiguity?
            //while (offset < source.Length && char.IsWhiteSpace(source[offset]))
            //    offset++;

            // The first character must be '_' or an letter.
            if (offset >= source.Length ||
                !(source[offset] == '_' || char.IsLetter(source[offset])))
                return "";

            int identifierOffset = offset;
            while (offset < source.Length)
            {
                if (!(source[offset] == '_' || char.IsLetterOrDigit(source[offset])))
                    break;
                offset++;
            }

            string identifier = source[identifierOffset..offset];
            return identifier;
        }
    }
}
