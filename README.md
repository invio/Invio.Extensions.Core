# Invio.Extensions.Core

General purpose extensions to components of the .Net Core Framework.

[![Appveyor](https://ci.appveyor.com/api/projects/status/74ub7qe2eitxuj4d?svg=true)](https://ci.appveyor.com/project/invio/invio-extensions-core/branch/master)
[![Build Status](https://travis-ci.org/invio/Invio.Extensions.Core.svg?branch=master)](https://travis-ci.org/invio/Invio.Extensions.Core)
[![Coverage](https://coveralls.io/repos/github/invio/Invio.Extensions.Core/badge.svg?branch=master)](https://coveralls.io/github/invio/Invio.Extensions.Core?branch=master)
[![NuGet](https://img.shields.io/nuget/v/Invio.Extensions.Core.svg)](https://www.nuget.org/packages/Invio.Extensions.Core/)

# Installation

The latest version of this package is available on NuGet. To install, run the following command:

```
PM> Install-Package Invio.Extensions.Core
```

# Capabilities

## String Extensions

### Quote

Wrap strings in quotation marks, escaping existing quotes.

```csharp
Console.WriteLine(
    "it's a good day to quote.".Quote(quoteCharacter: '\'')
);
```

Displays

```
'it\'s a good day to quote'
```

Also escapes existing escape characters and supports repeat-quote escaping.

```csharp
Console.WriteLine("foo\bar".Quote());
// "foo\\bar"

Console.WriteLine(
    "it's \"quoted\" already".Quote(escapeCharacter: '"')
);
// "it's ""quoted"" already"
```

## IListExtensions

### Deconstruction

The `IListExtensions.Deconstruct` extension methods allow lists to be assigned to tuple literals, extracting up to the first five values.

```csharp
var (first, second, rest) = new[] { "foo", "bar", "baz", "..." };
```

In this example `first` will be "foo", `second` will be "bar", and third will be an `IEnumerable<String>` containing `{ "baz", "..." }`.

Attempting to extract more values than exist in the list will result in an `ArgumentOutOfRangeException`.

### Shuffle

The `IListExtensions.Shuffle` extension methods allow mutable lists to have their items placed in a random order.

```csharp
var list = new [] { "foo", "bar", "biz" };
list.Shuffle();

foreach (var item in list) {
  Console.WriteLine(item);
}

// Something like of the following:
//  "biz"
//  "foo"
//  "bar"
```

In this example the order when each `item` that is printed to the console is non-deterministic. It may be the original order of the items.

## TaskExtensions

### Cast&lt;T>

Because there is no `ITask<out T>` if you have a `Task<List<T>>` and you need to pass it to a function that expects `Task<IEnumerable<T>>` it is necessary to either use an async function to await the result, cast it, and then return the new result; or to use `ContinueWith`. The `Cast<T>` extension method abridges this syntax:

```csharp
// given
Task<List<String>> someTask;
// instead of this
someTask.ContinueWith(t => (IEnumerable<String>)t.Result);
// or this
Task.FromResult(
	(IEnumerable<String>)
	(await someTask.ConfigureAwait(false)));
// use this
someTask.Cast<IEnumerable<String>>();
```

### ContinueWithResult

If you have a `Task<T>` and you want to run some synchronous transformation function on its result when it completes, it is either necessary to await the result, or use `ContinueWith` however `ContinueWith` executes the continuation whether the original task succeeds or fails, and accessing `Task<T>.Result` obfuscates exceptions by wrapping them in an `AggregateException`.

```csharp
Task<String> someTask;
someTask.ContinueWithResult(str => str.ToUpper());
```
