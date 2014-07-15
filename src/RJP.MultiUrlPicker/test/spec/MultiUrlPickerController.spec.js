
describe('RJP.MultiUrlPickerController', function () {
    var $scope, $location, $rootScope, controller, createController;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, angularHelper, entityMocks, mocksUtils) {

        controller = $controller;

        //mock the scope model
        $scope = $rootScope.$new();
        $scope.model = {
            alias: 'property',
            label: 'RJP.MultiUrlPicker property',
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

    it("doesn't try to get entity when id is less than 1", inject(function ($q, entityResource) {
        var e = {
            id: -20,
            name: 'Recycle Bin',
            url: '/',
            isMedia: false,
            icon: ''
        };

        controller('RJP.MultiUrlPickerController', {
            $scope: $scope,
            entityResource: entityResource
        });

        spyOn(entityResource, 'getById').andCallFake(function () {
            var deferred = $q.defer();
            deferred.resolve(e);
            return deferred.promise;
        });

        $scope.onContentSelected(e);

        expect(entityResource.getById).not.toHaveBeenCalled();
    }));

    it('uses the url if the Page Title field is left empty', function () {
        var e = {
            id: null,
            name: '',
            url: 'http://www.google.com/',
            isMedia: false,
            icon: ''
        };

        createController();

        expect($scope.renderModel.length).toBe(0);

        $scope.onContentSelected(e);

        expect($scope.renderModel.length).toBe(1);
        expect($scope.renderModel[0].name).toBe(e.url);
    });

    it('shows the url is name is empty when loaded from the database', function () {

        var link = {
            id: 1,
            name: '',
            url: 'http://www.google.com/',
            isMedia: false,
            icon: 'icon-link'
        };

        $scope.model.value = [link];

        createController();

        expect($scope.renderModel.length).toBe(1);
        expect($scope.renderModel[0].name).toBe(link.url);
    })
});