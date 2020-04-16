using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Invio.Extensions.IO;
using Xunit;

namespace Invio.Extensions.Core.Tests.IO {
    public class CsvWriterUnitTests {
        public static IEnumerable<Object[]> Header_Object_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new { Foo = 1, Bar = 2, Baz = 3 },
                    $"Foo,Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    new { Foo = 1, Bar = 2, Baz = 3 },
                    $"Foo\tBar\tBaz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true },
                    new { Foo = 1, Bar = 2, Baz = 3 },
                    $"\"Foo\",\"Bar\",\"Baz\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\'' },
                    new { Foo = 1, Bar = 2, Baz = 3 },
                    $"'Foo','Bar','Baz'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(Header_Object_TestData))]
        public async Task WriteHeader_Object(CsvWriterOptions options, Object data, String expected) {
            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteHeaderAsync(data);
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }

        public static IEnumerable<Object[]> Header_Dictionary_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new SortedDictionary<String, Object> {
                        { "Foo", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar,Baz,Foo{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new SortedDictionary<String, Object> {
                        { "Fo o", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar,Baz,Fo o{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new SortedDictionary<String, Object> {
                        { "Fo,o", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar,Baz,\"Fo,o\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new SortedDictionary<String, Object> {
                        { "Fo\no", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar,Baz,\"Fo\\no\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { AllowQuotedNewline = true },
                    new SortedDictionary<String, Object> {
                        { "Fo\no", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar,Baz,\"Fo\no\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    new SortedDictionary<String, Object> {
                        { "Foo", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"Bar\tBaz\tFoo{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true },
                    new SortedDictionary<String, Object> {
                        { "Foo", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"\"Bar\",\"Baz\",\"Foo\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\'' },
                    new SortedDictionary<String, Object> {
                        { "Foo", 1 },
                        { "Bar", 2 },
                        { "Baz", 3 }
                    },
                    $"'Bar','Baz','Foo'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(Header_Dictionary_TestData))]
        public async Task WriteHeader_Dictionary(CsvWriterOptions options, IDictionary data, String expected) {
            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteHeaderAsync(data);
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }

        public static IEnumerable<Object[]> Header_Explicit_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new[] {
                        "Foo",
                        "Bar",
                        "Baz"
                    },
                    $"Foo,Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new[] {
                        "Fo o",
                        "Bar",
                        "Baz"
                    },
                    $"Fo o,Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new[] {
                        "Fo,o",
                        "Bar",
                        "Baz"
                    },
                    $"\"Fo,o\",Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions(),
                    new[] {
                        "Fo\no",
                        "Bar",
                        "Baz"
                    },
                    $"\"Fo\\no\",Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { AllowQuotedNewline = true },
                    new[] {
                        "Fo\no",
                        "Bar",
                        "Baz"
                    },
                    $"\"Fo\no\",Bar,Baz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    new[] {
                        "Foo",
                        "Bar",
                        "Baz"
                    },
                    $"Foo\tBar\tBaz{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true },
                    new[] {
                        "Foo",
                        "Bar",
                        "Baz"
                    },
                    $"\"Foo\",\"Bar\",\"Baz\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\'' },
                    new[] {
                        "Foo",
                        "Bar",
                        "Baz"
                    },
                    $"'Foo','Bar','Baz'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(Header_Explicit_TestData))]
        public async Task WriteHeader_Explicit(
            CsvWriterOptions options,
            String[] fields,
            String expected) {

            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteHeaderAsync(fields);
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }

        public static IEnumerable<Object[]> WriteObject_WithHeader_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    $"Foo,Bar,Baz{Environment.NewLine}37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    $"Foo\tBar\tBaz{Environment.NewLine}37\t73\t\"Test,\tquoting\\\\\"{Environment.NewLine}3.142\t2.718\t\"Test\\nNewline\"{Environment.NewLine}0\t1\t\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { AllowQuotedNewline = true },
                    $"Foo,Bar,Baz{Environment.NewLine}37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteCharacter = '\'' },
                    $"Foo,Bar,Baz{Environment.NewLine}37,73,'Test,\tquoting\\\\'{Environment.NewLine}3.142,2.718,'Test\\nNewline'{Environment.NewLine}0,1,Nested \"Quotes\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\''},
                    $"'Foo','Bar','Baz'{Environment.NewLine}'37','73','Test,\tquoting\\\\'{Environment.NewLine}'3.142','2.718','Test\\nNewline'{Environment.NewLine}'0','1','Nested \"Quotes\"'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(WriteObject_WithHeader_TestData))]
        public async Task WriteObject_WithHeader(CsvWriterOptions options, String expected) {
            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteHeaderAsync(new { Foo = 1, Bar = 2, Baz = 3 });

                    await writer.WriteObjectAsync(
                        new { Foo = 37, Bar = 73, Baz = "Test,\tquoting\\", Herp = "Derp" }
                    );
                    await writer.WriteObjectAsync(
                        new Dictionary<String, Object> {
                            { "Foo", 3.142m },
                            { "Bar", 2.718m },
                            { "Baz", "Test\nNewline" },
                            { "Nope", "Dope" }
                        }
                    );
                    await writer.WriteObjectAsync(new Object[] { 0, 1, "Nested \"Quotes\"" });
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }


        public static IEnumerable<Object[]> WriteObject_NoHeader_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    $"37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    $"37\t73\t\"Test,\tquoting\\\\\"{Environment.NewLine}3.142\t2.718\t\"Test\\nNewline\"{Environment.NewLine}0\t1\t\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { AllowQuotedNewline = true },
                    $"37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteCharacter = '\'' },
                    $"37,73,'Test,\tquoting\\\\'{Environment.NewLine}3.142,2.718,'Test\\nNewline'{Environment.NewLine}0,1,Nested \"Quotes\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\''},
                    $"'37','73','Test,\tquoting\\\\'{Environment.NewLine}'3.142','2.718','Test\\nNewline'{Environment.NewLine}'0','1','Nested \"Quotes\"'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(WriteObject_NoHeader_TestData))]
        public async Task WriteObject_NoHeader(CsvWriterOptions options, String expected) {
            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteObjectAsync(
                        new { Foo = 37, Bar = 73, Baz = "Test,\tquoting\\" }
                    );
                    await writer.WriteObjectAsync(
                        new Dictionary<String, Object> {
                            { "Foo", 3.142m },
                            { "Bar", 2.718m },
                            { "Baz", "Test\nNewline" },
                            { "Nope", "Dope" }
                        }
                    );
                    await writer.WriteObjectAsync(new Object[] { 0, 1, "Nested \"Quotes\"" });
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }

        public static IEnumerable<Object[]> WriteObject_ExplicitFieldList_TestData {
            get {
                yield return new Object[] {
                    new CsvWriterOptions(),
                    $"\"Column, Bar\",Foo Column,\"\\\"Baz\\\"\"{Environment.NewLine}37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { FieldSeparator = '\t' },
                    $"Column, Bar\tFoo Column\t\"\\\"Baz\\\"\"{Environment.NewLine}37\t73\t\"Test,\tquoting\\\\\"{Environment.NewLine}3.142\t2.718\t\"Test\\nNewline\"{Environment.NewLine}0\t1\t\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { AllowQuotedNewline = true },
                    $"\"Column, Bar\",Foo Column,\"\\\"Baz\\\"\"{Environment.NewLine}37,73,\"Test,\tquoting\\\\\"{Environment.NewLine}3.142,2.718,\"Test\nNewline\"{Environment.NewLine}0,1,\"Nested \\\"Quotes\\\"\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteCharacter = '\'' },
                    $"'Column, Bar',Foo Column,\"Baz\"{Environment.NewLine}37,73,'Test,\tquoting\\\\'{Environment.NewLine}3.142,2.718,'Test\\nNewline'{Environment.NewLine}0,1,Nested \"Quotes\"{Environment.NewLine}"
                };
                yield return new Object[] {
                    new CsvWriterOptions { QuoteAllFields = true, QuoteCharacter = '\''},
                    $"'Column, Bar','Foo Column','\"Baz\"'{Environment.NewLine}'37','73','Test,\tquoting\\\\'{Environment.NewLine}'3.142','2.718','Test\\nNewline'{Environment.NewLine}'0','1','Nested \"Quotes\"'{Environment.NewLine}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(WriteObject_ExplicitFieldList_TestData))]
        public async Task WriteObject_ExplicitFieldList(CsvWriterOptions options, String expected) {
            using (var buffer = new MemoryStream()) {
                using (var writer = new CsvWriter(buffer, options)) {
                    await writer.WriteHeaderAsync(new[] { "Column, Bar", "Foo Column", "\"Baz\"" });

                    writer.SetFields(new[] { "Bar", "Foo", "Baz" });

                    await writer.WriteObjectAsync(
                        new { Foo = 73, Bar = 37, Baz = "Test,\tquoting\\", Herp = "Derp" }
                    );
                    await writer.WriteObjectAsync(
                        new Dictionary<String, Object> {
                            { "Foo", 2.718m },
                            { "Bar", 3.142m },
                            { "Baz", "Test\nNewline" },
                            { "Nope", "Dope" }
                        }
                    );
                    await writer.WriteObjectAsync(new Object[] { 0, 1, "Nested \"Quotes\"" });
                }

                buffer.Position = 0;
                using (var reader = new StreamReader(buffer)) {
                    Assert.Equal(expected, await reader.ReadToEndAsync());
                }
            }
        }
    }
}
