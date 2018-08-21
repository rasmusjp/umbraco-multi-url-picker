# Change log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

# 2.2.0
### Features

  * #83 - Umbraco version 7.12.0 breaks UI
  * #82 - Moved `ng-repeat` to the `umb-node-preview` directive
  * Localized validation messages
  * Added helper messages for min/max number of items

# 2.1.0
### Features

  * #45 - Feature Request : Querystring Parameters. Thanks to @mattbrailsford
  * No more "Could not find persisted pre-value for field (minNumberOfItems|maxNumberOfItems)" warnings in the Umbraco log

### Bugfixes

  * #60 - Only returns first item on startup

# 2.0.1
### Bugfixes

  * #54 - Multi Url Picker cant add nodes

# 2.0.0
### Features

  * Updated styling to match Umbraco v7.6
  * Added Udi support
  * New data types with `max number of items` set to 1 will return a single `Link` or `null`.  
    Existing data types will continue to return `IEnumerable<Link>`

### Breaking

 * Upgraded Umbraco to v7.6

# 1.3.2
### Bugfixes

  * #48 - Editing a media item breaks the link
  * #51 - Picked nodes are not resolved when deleted

# 1.3.1
### Bugfixes

  * Fixed bug introduced when cleaning code

# 1.3.0
### Features

  * #22 - Usage in macro parameter

### Bugfixes

  * #28 - Not working in combination with Doc Type Grid Editor Package

# 1.2.0
### Features

  * #14 - Use url as name when Page Title is left empty

# 1.1.0
### Features

  * #8 - Hide handle when only one item.

### Bugfixes

  * #11 - Validation problem with min/max and external links.
  * #13 - Gracefully handle when the recycling bin is chosen

# 1.0.0
### Features

  * Added min/max items field
  * Added document icons to the editor

### Breaking

  * Upgraded to Umbraco v7.1.2
  * Removed Pick multiple items field

# 0.2.0
### Features

  * Added property value converter

# 0.1.1
### Bugfixes

  * Data is now stored in the ntext column

# 0.1.0

  * Initial release
