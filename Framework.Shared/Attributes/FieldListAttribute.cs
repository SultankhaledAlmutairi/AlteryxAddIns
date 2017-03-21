using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AlteryxRecordInfoNet;

namespace OmniBus.Framework.Attributes
{
    /// <summary>
    ///     For designers, specified a fixed list of valid values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FieldListAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldListAttribute" /> class.
        /// </summary>
        /// <param name="values">List of valid values.</param>
        public FieldListAttribute(params object[] values)
        {
            this.DictionaryLookUp = values.ToDictionary(ToLabel, v => v);
            this.StandardValuesCollection =
                new TypeConverter.StandardValuesCollection(values.Select(ToLabel).ToList());
        }

        /// <summary>
        ///     Gets a dictionary of names mapping back to value.
        /// </summary>
        public Dictionary<string, object> DictionaryLookUp { get; }

        /// <summary>
        ///     Gets a <see cref="TypeConverter.StandardValuesCollection" /> needed for type converters.
        /// </summary>
        public TypeConverter.StandardValuesCollection StandardValuesCollection { get; }

        /// <summary>
        ///     Gets a copy of the names and value in the original order of the values list.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> OrderedDictionary
            =>
                this.StandardValuesCollection.Cast<string>()
                    .Select(k => new KeyValuePair<string, object>(k, this.DictionaryLookUp[k]));

        private static string ToLabel(object v)
            => v is FieldType ? v.ToString().Substring(5).Replace("_", string.Empty) : v.ToString();
    }
}