# Multi URL Picker for Umbraco 7

[![NuGet release](https://img.shields.io/nuget/v/RJP.UmbracoMultiUrlPicker.svg)](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/multi-url-picker)

Allows editors to pick and sort multiple URLs, it uses Umbraco's link picker which supports internal and external links and media.

## Installation

Install the [NuGet package](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker):

```powershell
PM> Install-Package RJP.UmbracoMultiUrlPicker
```

or

Install the [Umbraco package](http://our.umbraco.org/projects/backoffice-extensions/multi-url-picker) from the Umbraco package repository.

## Usage

Add a new property to your document type and select the `Multi URL Picker` property editor in the `pickers` category.

If you're using the Models Builder, you can access the property on your model as `IEnumerable<Link>` (e.g. `Model.Links` if your property alias is `links`):

```csharp
@{ var links = Model.Links.ToList(); }
@if (links.Count > 0)
{
    <ul>
        @foreach (var link in links)
        {
            <li><a href="@link.Url" target="@link.Target">@link.Name</a></li>
        }
    </ul>
}
```

Or, if `Maximum number of items` is configured to 1, as `Link`:

```csharp
@{ var link = Model.Link; }
@if (link != null)
{
    <a href="@link.Url" target="@link.Target">@link.Name</a>
}
```

## Changelog

See the [changelog here](CHANGELOG.md).
