'use strict';

/* App Module */

var app = angular.module('mvcapp', ['mvcappFilters', 'ui.bootstrap', 'ui.utils']);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/index', { templateUrl: 'ngview/home/index.html', controller: IndexCtrl }).
        when('/about', { templateUrl: 'ngview/home/about.html', controller: AboutCtrl }).
        when('/students', { templateUrl: 'ngview/student/index.html', controller: StudentCtrl }).
        when('/students/create', { templateUrl: 'ngview/student/form.html', controller: StudentCreateCtrl }).
        when('/students/edit/:id', { templateUrl: 'ngview/student/form.html', controller: StudentEditCtrl }).
        when('/students/details/:id', { templateUrl: 'ngview/student/details.html', controller: StudentDetailsCtrl }).
        when('/courses', { templateUrl: 'ngview/course/index.html', controller: CourseCtrl }).
        when('/courses/create', { templateUrl: 'ngview/course/form.html', controller: CourseCreateCtrl }).
        when('/courses/edit/:id', { templateUrl: 'ngview/course/form.html', controller: CourseEditCtrl }).
        when('/courses/details/:id', { templateUrl: 'ngview/course/details.html', controller: CourseDetailsCtrl }).
        when('/instructors', { templateUrl: 'ngview/instructor/index.html', controller: InstructorCtrl }).
        when('/instructors/create', { templateUrl: 'ngview/instructor/form.html', controller: InstructorCreateCtrl }).
        when('/instructors/edit/:id', { templateUrl: 'ngview/instructor/form.html', controller: InstructorEditCtrl }).
        when('/instructors/details/:id', { templateUrl: 'ngview/instructor/details.html', controller: InstructorDetailsCtrl }).
        when('/departments', { templateUrl: 'ngview/department/index.html', controller: DepartmentCtrl }).
        when('/departments/create', { templateUrl: 'ngview/department/form.html', controller: DepartmentCreateCtrl }).
        when('/departments/edit/:id', { templateUrl: 'ngview/department/form.html', controller: DepartmentEditCtrl }).
        when('/departments/details/:id', { templateUrl: 'ngview/department/details.html', controller: DepartmentDetailsCtrl }).
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