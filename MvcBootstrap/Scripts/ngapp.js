'use strict';

/* App Module */

var app = angular.module('mvcapp', ['mvcappFilters', 'ui.bootstrap']);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/', { templateUrl: 'ngview/home/index.html', controller: IndexCtrl }).
        when('/about', { templateUrl: 'ngview/home/about.html', controller: AboutCtrl }).
        when('/students', { templateUrl: 'ngview/student/index.html', controller: StudentCtrl }).
        otherwise({ redirectTo: '/' });
}]);

app.factory('Page', function () {
    var _title = 'Index';
    return {
        title: _title,
        setTitle: function (a) {
            _title = a;
        }
    };
});