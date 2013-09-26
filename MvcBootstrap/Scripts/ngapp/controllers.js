'use strict';

/* Controllers */

function HeadCtrl($scope, Page) {
    $scope.Page = Page;
}

function IndexCtrl($scope, Page) {
    $scope.menu = {
        home: true,
        about: false,
        students: false,
        courses: false,
        instructors: false,
        departments: false
    };

    Page.setTitle('Index');

    $scope.menuClick = function (a) {
        _.each($scope.menu, function (v, k, l) {
            $scope.menu[k] = false;
            if (a == k)
                $scope.menu[k] = true;
        });
    }
}

function AboutCtrl($scope, $http, Page) {
    Page.setTitle('Student Body Statistics');

    $http.get('/Ng/About').success(function (data) {
        $scope.model = data;
    });
}

function StudentCtrl($scope, Page) {
    Page.setTitle('Students');
}
