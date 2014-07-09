﻿
describe('Unit test for MultiUrlPickerController', function () {
    var $scope, $location, $rootScope, createController;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, angularHelper, entityMocks, mocksUtils) {

        //mock the scope model
        $scope = $rootScope.$new();
        $scope.model = {
            alias: 'property',
            label: '<%= names.name %> property',
            description: 'desc',
            config: {}
        };

        //setup the controller for the test by setting its scope to
        //our mocked model
        createController = function () {
            return $controller('RJP.MultiUrlPickerController', {
                '$scope': $scope
            });
        };
    }));

});