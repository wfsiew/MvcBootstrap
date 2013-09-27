'use strict';

/* App Module */

var app = angular.module('mvcapp', ['mvcappFilters', 'ui.bootstrap', 'ui.utils']);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/index', { templateUrl: 'ngview/home/index.html', controller: IndexCtrl }).
        when('/about', { templateUrl: 'ngview/home/about.html', controller: AboutCtrl }).
        when('/students', { templateUrl: 'ngview/student/index.html', controller: StudentCtrl }).
        when('/students/create', { templateUrl: 'ngview/student/form.html', controller: StudentCreateCtrl }).
        otherwise({ redirectTo: '/index' });
}]);

app.factory('Page', function () {
    var title = 'Index';
    var message = {
        text: null,
        show: false
    };
    return {
        title: function () {
            return title;
        },
        setTitle: function (a) {
            title = a;
        },
        message: function () {
            return message;
        },
        setMessage: function (a) {
            message.text = a;
            message.show = true;
        },
        resetMessage: function () {
            message.text = null;
            message.show = false;
        }
    };
});

app.factory('Menu', function () {
    var menu = {
        home: true,
        about: false,
        students: false,
        courses: false,
        instructors: false,
        departments: false
    };

    return {
        menu: function () {
            return menu;
        },
        setMenu: function (a) {
            _.each(menu, function (v, k, l) {
                menu[k] = false;
                if (a == k)
                    menu[k] = true;
            });
        }
    };
});