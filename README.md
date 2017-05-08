# Multi Url Picker for Umbraco 7

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

## Changelog
See the [changelog here](CHANGELOG.md)
