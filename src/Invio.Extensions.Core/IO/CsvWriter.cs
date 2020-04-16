using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Invio.Extensions.IO {
    /// <summary>
    /// Writes heterogeneous lists of objects and dictionaries as a series of delimited value rows
    /// separated by newlines.
    /// </summary>
    public sealed class CsvWriter : IDisposable {
        private static readonly Encoding DefaultEncoding =
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private const Int32 DefaultBufferSize = 1024;

        private readonly IDictionary<Type, Func<Object, Object>[]> propertyCache =
            new Dictionary<Type, Func<Object, Object>[]>();
        private readonly TextWriter writer;
        private readonly Boolean disposeWriter;
        private readonly CsvWriterOptions.Immutable options;
        private readonly Regex specialCharacterRegex;
        private IList<String> fields;

        /// <summary>
        /// Creates an instance that will write output to the specified stream using the default
        /// encoding (UTF8) and options.
        /// </summary>
        /// <param name="stream">The stream to output the csv data to.</param>
        public CsvWriter(Stream stream) :
            this(new StreamWriter(stream, DefaultEncoding, DefaultBufferSize, leaveOpen: true), true, new CsvWriterOptions()) {
        }

        /// <summary>
        /// Creates an instance that will write output to the specified stream using the default
        /// encoding (UTF8) and the specified options.
        /// </summary>
        /// <param name="stream">The stream to output the csv data to.</param>
        /// <param name="options">The csv formatting options.</param>
        public CsvWriter(Stream stream, CsvWriterOptions options) :
            this(new StreamWriter(stream, DefaultEncoding, DefaultBufferSize, leaveOpen: true), true, options) {
        }

        /// <summary>
        /// Creates an instance that will write output to the specified stream using the specified
        /// encoding and the default options.
        /// </summary>
        /// <param name="stream">The stream to output the csv data to.</param>
        /// <param name="encoding">The encoding to use when writing data to the stream.</param>
        public CsvWriter(Stream stream, Encoding encoding) :
            this(new StreamWriter(stream, encoding, DefaultBufferSize, leaveOpen: true), true, new CsvWriterOptions()) {
        }

        /// <summary>
        /// Creates an instance that will write output to the specified stream using the specified
        /// encoding and options.
        /// </summary>
        /// <param name="stream">The stream to output the csv data to.</param>
        /// <param name="encoding">The encoding to use when writing data to the stream.</param>
        /// <param name="options">The csv formatting options.</param>
        public CsvWriter(Stream stream, Encoding encoding, CsvWriterOptions options) :
            this(new StreamWriter(stream, encoding, DefaultBufferSize, leaveOpen: true), true, options) {
        }

        /// <summary>
        /// Creates an instance that will write output to the specified TextWriter using the
        /// default options.
        /// </summary>
        /// <param name="writer">The writer to output the csv data to.</param>
        public CsvWriter(TextWriter writer) : this(writer, false, new CsvWriterOptions()) {
        }

        /// <summary>
        /// Creates an instance that will write output to the specified TextWriter using the
        /// specified options.
        /// </summary>
        /// <param name="writer">The writer to output the csv data to.</param>
        /// <param name="options">The csv formatting options.</param>
        public CsvWriter(TextWriter writer, CsvWriterOptions options) :
            this(
                writer ?? throw new ArgumentNullException(nameof(writer)),
                false,
                options  ?? throw new ArgumentNullException(nameof(options))) {
        }

        private CsvWriter(TextWriter writer, Boolean ownWriter, CsvWriterOptions options) {
            this.writer = writer;
            this.disposeWriter = ownWriter;
            this.options = options.ToImmutable();
            this.specialCharacterRegex = new Regex(
                $"[\r\n{this.options.FieldSeparator}{this.options.QuoteCharacter}]"
            );
        }

        /// <summary>
        /// Writes the specified list of fields as a header row.
        /// </summary>
        /// <remarks>
        /// Unless <see cref="SetFields" /> is called after this method, the set of header values
        /// will be used as the fields to output for each row.
        /// </remarks>
        /// <param name="header">
        /// A list of strings that will be written as a csv header row.
        /// </param>
        /// <exception cref="ArgumentNullException">The headers parameter is null.</exception>
        public void WriteHeader(IEnumerable<String> header) {
            this.WriteHeaderImpl(header, false);
        }

        /// <summary>
        /// Asynchronously writes the specified list of fields as a header row.
        /// </summary>
        /// <remarks>
        /// Unless <see cref="SetFields" /> is called after this method, the set of header values
        /// will be used as the fields to output for each row.
        /// </remarks>
        /// <param name="header">
        /// A list of strings that will be written as a csv header row.
        /// </param>
        /// <exception cref="ArgumentNullException">The headers parameter is null.</exception>
        public Task WriteHeaderAsync(IEnumerable<String> header) {
            return this.WriteHeaderImpl(header, true);
        }


        /// <summary>
        /// Generates a header row based on the specified object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the object specified implements <see cref="IDictionary" />, the
        /// <see cref="IDictionary.Keys" /> collection will be used to generate the header row
        /// (it is assumed that all keys are of the type <see cref="String" />). If the data is not
        /// a dictionary then the header is based on the list of public instance properties.
        /// </para>
        /// <para>
        /// When a header is generated, the set of fields is stored and used to retrieve and order
        /// data on subsequent calls to <see cref="WriteObject" />.
        /// </para>
        /// </remarks>
        /// <param name="data">The object to use to generate the header row.</param>
        public void WriteHeader(Object data) {
            this.WriteHeaderImpl(data, false);
        }

        /// <summary>
        /// Asynchronously generates a header row based on the specified object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the object specified implements <see cref="IDictionary" />, the
        /// <see cref="IDictionary.Keys" /> collection will be used to generate the header row
        /// (it is assumed that all keys are of the type <see cref="String" />). If the data is not
        /// a dictionary then the header is based on the list of public instance properties.
        /// </para>
        /// <para>
        /// When a header is generated, the set of fields is stored and used to retrieve and order
        /// data on subsequent calls to <see cref="WriteObject" />.
        /// </para>
        /// </remarks>
        /// <param name="data">The object to use to generate the header row.</param>
        public Task WriteHeaderAsync(Object data) {
            return this.WriteHeaderImpl(data, true);
        }

        /// <summary>
        /// Sets the field names to use when writing objects.
        /// </summary>
        /// <remarks>
        /// This method provides explicit control of the fields to serialize and their order.
        /// This method makes it possible to use strings for the header row that do not exactly
        /// match the dictionary keys or property names of the data objects.
        /// </remarks>
        /// <param name="fieldNames">
        /// The list of field names to serialize for each data object.
        /// </param>
        public void SetFields(IEnumerable<String> fieldNames) {
            this.fields = new List<String>(fieldNames);
            this.propertyCache.Clear();
        }

        /// <summary>
        /// Writes an object as a csv row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the object specified implements <see cref="IDictionary" />, values will be retrieved
        /// using the index operator (it is assumed that all keys are of the type
        /// <see cref="String" />). If the data is not a dictionary then values will be retrieved
        /// from public instance properties. If a header has not been generated and an explicit
        /// list of fields has not been set (see <see cref="SetFields" />) then all keys or
        /// public properties will be serialized, and all subsequent calls to this method will use
        /// the same set of fields in the same order.
        /// </para>
        /// </remarks>
        /// <param name="data">The data to serialize.</param>
        public void WriteObject(Object data) {
            this.WriteObjectImpl(data, false);
        }

        /// <summary>
        /// Asynchronously writes an object as a csv row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the object specified implements <see cref="IDictionary" />, values will be retrieved
        /// using the index operator (it is assumed that all keys are of the type
        /// <see cref="String" />). If the object implements <see cref="IEnumerable" /> then each
        /// item it contains will be treated as a field (in this case any previously specified list
        /// of fields is ignored). If the data is not a dictionary then values will be retrieved
        /// from public instance properties. If a header has not been generated and an explicit
        /// list of fields has not been set (see <see cref="SetFields" />) then all keys or
        /// public properties will be serialized, and all subsequent calls to this method will use
        /// the same set of fields in the same order.
        /// </para>
        /// </remarks>
        /// <param name="data">The data to serialize.</param>
        public Task WriteObjectAsync(Object data) {
            return this.WriteObjectImpl(data, true);
        }

        /// <summary>
        /// Flushes any buffered data to the output writer.
        /// </summary>
        public void Flush() {
            this.writer.Flush();
        }

        /// <summary>
        /// Flushes any buffered data to the output writer.
        /// </summary>
        public Task FlushAsync() {
            return this.writer.FlushAsync();
        }

        void IDisposable.Dispose() {
            if (this.disposeWriter) {
                this.writer?.Dispose();
            }
        }

        private Task WriteHeaderImpl(IEnumerable<String> header, Boolean async) {
            if (header == null) {
                throw new ArgumentNullException(nameof(header));
            }

            this.fields = new List<String>(header);
            this.propertyCache.Clear();
            var line = String.Join(
                $"{this.options.FieldSeparator}",
                this.fields.Select(this.QuoteAsNeeded)
            );

            if (async) {
                return this.writer.WriteLineAsync(line);
            } else {
                this.writer.WriteLine(line);
                return null;
            }
        }

        private Task WriteHeaderImpl(Object data, Boolean async) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            IEnumerable<String> headerFields;
            if (data is IDictionary dict) {
                headerFields = dict.Keys.Cast<String>();
            } else {
                var properties = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                headerFields = properties.Where(p => p.CanRead).Select(p => p.Name);
            }

            return this.WriteHeaderImpl(headerFields, async);
        }

        private Task WriteObjectImpl(Object data, Boolean async) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            String line;
            switch (data) {
                case IDictionary dict:
                    if (this.fields == null) {
                        this.fields = new List<String>(dict.Keys.Cast<String>());
                    }

                    line = String.Join(
                        $"{this.options.FieldSeparator}",
                        this.fields.Select(f => dict[f]?.ToString()).Select(this.QuoteAsNeeded)
                    );
                    break;
                case IEnumerable enumerable:
                    line = String.Join(
                        $"{this.options.FieldSeparator}",
                        enumerable.Cast<Object>().Select(v => v?.ToString()).Select(QuoteAsNeeded)
                    );
                    break;
                default:
                    if (this.fields == null) {
                        var properties = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        this.fields = properties.Where(p => p.CanRead).Select(p => p.Name).ToList();
                    }

                    var getters = this.GetPropertyGetters(data.GetType());

                    line = String.Join(
                        $"{this.options.FieldSeparator}",
                        getters.Select(getter => getter(data)?.ToString()).Select(this.QuoteAsNeeded)
                    );
                    break;
            }

            if (async) {
                return this.writer.WriteLineAsync(line);
            } else {
                this.writer.WriteLine(line);
                return null;
            }
        }

        private Func<Object, Object>[] GetPropertyGetters(Type type) {
            if (!this.propertyCache.TryGetValue(type, out var getters)) {
                var properties =
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.CanRead)
                        .ToDictionary(p => p.Name);
                getters =
                    this.fields.Select(f => CreateGetter(properties[f])).ToArray();
                this.propertyCache.Add(type, getters);
            }

            return getters;
        }

        private Func<Object, Object> CreateGetter(PropertyInfo property) {
            var instance = Expression.Parameter(typeof(Object), "instance");

            var body = Expression.Convert(
                Expression.Call(
                    Expression.Convert(
                        instance,
                        property.DeclaringType ??
                            throw new ArgumentException("The specified property has no DeclaringType.", nameof(property))),
                    property.GetMethod
                ),
                typeof(Object)
            );

            return Expression.Lambda<Func<Object, Object>>(body, instance).Compile();
        }

        private String QuoteAsNeeded(String str) {
            if (str != null && (this.options.QuoteAllFields || this.specialCharacterRegex.IsMatch(str))) {
                return str.Quote(
                    quoteCharacter: this.options.QuoteCharacter,
                    escapeCharacter: this.options.EscapeCharacter,
                    specialCharacters: this.options.AllowQuotedNewline ? null : new[] { '\n', '\r' },
                    escapeSequences: this.options.AllowQuotedNewline ? null : new[] { "n", "r" }
                );
            } else {
                return str;
            }
        }
    }
}
