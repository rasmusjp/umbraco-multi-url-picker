# Multi Url Picker for Umbraco 7

Allows editors to pick and sort multiple urls, it uses Umbraco's link picker which supports internal and external links and media. 

## Installation
Install the [package](http://our.umbraco.org/projects/backoffice-extensions/multi-url-picker) from the Umbraco package repository.

or

Install the NuGet [package](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker).

## Usage

Create a new Data Type and select the "Multi Url Picker" property editor.

### Typed

Add a reference to RJP.MultiUrlPicker.dll in your project

```
@{
  var multiUrlPicker = Model.Content.GetPropertyValue<MultiUrls>("multiUrlPicker");
  if (multiUrlPicker.Any())
  {
    <ul>
      @foreach (var item in multiUrlPicker)
      {
        <li><a href="@item.Url" target="@item.Target">@item.Name</a></li>
      }
    </ul>
  }
}
```

### Dynamic
```
@{
  var multiUrlPickerDyn = CurrentPage.multiUrlPicker;
  if (multiUrlPickerDyn.Any())
  {
    <ul>
      @foreach (var item in multiUrlPickerDyn)
      {
        <li><a href="@item.Url" target="@item.Target">@item.Name</a></li>
      }
    </ul>
  }
}
```

## Changelog
See the [changelog here](CHANGELOG)