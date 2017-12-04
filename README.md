# Multi Url Picker for Umbraco 7

[![NuGet release](https://img.shields.io/nuget/v/RJP.UmbracoMultiUrlPicker.svg)](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker) 
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/multi-url-picker)

Allows editors to pick and sort multiple urls, it uses Umbraco's link picker which supports internal and external links and media. 

## Installation

Install the NuGet [package](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker).

or

Install the [package](http://our.umbraco.org/projects/backoffice-extensions/multi-url-picker) from the Umbraco package repository.

## Usage

Add a new property to your document type and select the `Multi Url Picker` property editor in the `pickers` category.

If you're using the models builder, you can access the property on your model e.g. `Model.Links` if your property alias is `links`.

```csharp
@{ var links = Model.Links.ToList(); }

@if (links.Count > 0)
{
  <ul>
    @foreach (var item in links)
    {
      <li><a href="@item.Url" target="@item.Target">@item.Name</a></li>
    }
    </ul>
}
```

If `Max number of items` is configured to 1

```csharp
@if(Model.Link != null)
{
  <a href="@Model.Link.Url" target="@Model.Link.Target">@Model.Link.Name</a>
}

```

## Changelog
See the [changelog here](CHANGELOG.md)
