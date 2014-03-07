# Multi Url Picker for Umbraco 7

Allows editors to pick and sort multiple urls, it uses Umbraco's link picker which supports internal and external links and media. 

## Installation
Install the [package](http://our.umbraco.org/projects/backoffice-extensions/multi-url-picker) from the Umbraco package repository.

or

Install the NuGet [package](https://www.nuget.org/packages/RJP.UmbracoMultiUrlPicker).

## Usage

Create a new Data Type and select the "Multi Url Picker" property editor.

Example view:

```
@using Newtonsoft.Json
@inherits UmbracoTemplatePage
@{
  dynamic links = JsonConvert.DeserializeObject(Model.Content.GetPropertyValue<string>("links"));
}

<ul>
  @foreach (var link in links) {
    var url = link.url;
    // resolve the url if the link is internal
    if (link.id != null) {
      url = Umbraco.NiceUrl((int)link.id);
    }
    <li>
      <a href="@url" title="@link.name" target="@link.target">@link.name</a>
    </li>
  }
</ul>
```