(function () {
  'use strict'

  var MultiUrlPickerController = function ($scope, angularHelper, entityResource, iconHelper) {
    this.renderModel = []

    if ($scope.preview) {
      return
    }

    if (!Array.isArray($scope.model.value)) {
      $scope.model.value = []
    }

    $scope.model.value.forEach(function (link) {
      link.icon = iconHelper.convertFromLegacyIcon(link.icon)
      this.renderModel.push(link)
    }.bind(this))

    $scope.$on('formSubmitting', function () {
      $scope.model.value = this.renderModel
    }.bind(this))

    $scope.$watch(function () {
      return this.renderModel.length
    }.bind(this), function () {
      if ($scope.model.config && $scope.model.config.minNumberOfItems) {
        $scope.multiUrlPickerForm.minCount.$setValidity('minCount', +$scope.model.config.minNumberOfItems <= this.renderModel.length)
      }
      if ($scope.model.config && $scope.model.config.maxNumberOfItems) {
        $scope.multiUrlPickerForm.maxCount.$setValidity('maxCount', +$scope.model.config.maxNumberOfItems >= this.renderModel.length)
      }
      this.sortableOptions.disabled = this.renderModel.length === 1
    }.bind(this))

    this.sortableOptions = {
      distance: 10,
      tolerance: 'pointer',
      scroll: true,
      zIndex: 6000,
      update: function () {
        angularHelper.getCurrentForm($scope).$setDirty()
      }
    }

    this.remove = function ($index) {
      this.renderModel.splice($index, 1)

      angularHelper.getCurrentForm($scope).$setDirty()
    }

    this.openLinkPicker = function (link, $index) {
      var target = link ? {
        name: link.name,
        // the linkPicker breaks if it get an id or udi for media
        id: link.isMedia ? null : link.id,
        udi: link.isMedia ? null : link.udi,
        url: link.url,
        querystring: link.querystring,
        target: link.target
      } : null


      this.linkPickerOverlay = {
        view: 'linkpicker',
        currentTarget: target,
        show: true,
        querystring: true,
        submit: function (model) {
          if (model.target.url) {
            if (link) {
              if (link.isMedia && link.url === model.target.url) {
                // we can assume the existing media item is changed and no new file has been selected
                // so we don't need to update the id, udi and isMedia fields
              } else {
                link.id = model.target.id
                link.udi = model.target.udi
                link.isMedia = model.target.isMedia
              }

              link.name = model.target.name || model.target.url
              link.target = model.target.target
              link.url = model.target.url
              link.querystring = model.target.querystring
            } else {
              link = {
                id: model.target.id,
                isMedia: model.target.isMedia,
                name: model.target.name || model.target.url,
                target: model.target.target,
                udi: model.target.udi,
                url: model.target.url,
                querystring: model.target.querystring
              }
              this.renderModel.push(link)
            }

            if (link.udi) {
              var entityType = link.isMedia ? 'media' : 'document'

              entityResource.getById(link.udi, entityType)
              .then(function (data) {
                link.icon = iconHelper.convertFromLegacyIcon(data.icon)
                link.published = !(data.metaData && data.metaData.IsPublished === false && entityType === 'document')
              })
            } else {
              link.published = true
              link.icon = 'icon-link'
            }

            angularHelper.getCurrentForm($scope).$setDirty()
          }

          this.linkPickerOverlay.show = false
          this.linkPickerOverlay = null
        }.bind(this)
      }
    }
  }

  var mupHttpProvider = function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
      return {
        response: function (response) {
          if (response.config.url.indexOf('views/common/overlays/linkpicker/linkpicker.html') !== -1) {
            // Inject the querystring field
            var $markup = $(response.data)
            var $urlField = $markup.find('[label="@defaultdialogs_urlLinkPicker"]')
            $urlField.after('<umb-control-group label="Query String" ng-if="model.querystring"><input type="text" placeholder="Query String" class="umb-editor umb-textstring" ng-model="model.target.querystring"/></umb-control-group>')
            response.data = $markup[0]
          }
          return response
        }
      }
    })
  }

  angular.module('umbraco').controller('RJP.MultiUrlPickerController', MultiUrlPickerController)
  angular.module("umbraco.services").config(['$httpProvider', mupHttpProvider]);
})()
