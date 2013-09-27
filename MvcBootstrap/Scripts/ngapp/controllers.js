'use strict';

/* Controllers */

function PageCtrl($scope, Page, Menu) {
    $scope.Page = Page;
    $scope.Menu = Menu;
}

function IndexCtrl($scope, Page, Menu) {
    Page.setTitle('Index');
    Menu.setMenu('home');
}

function AboutCtrl($scope, $http, Page, Menu) {
    Page.setTitle('Student Body Statistics');
    Menu.setMenu('about');

    $http.get('/Ng/About').success(function (data) {
        $scope.model = data;
    });
}

function StudentCtrl($scope, $http, Page, Menu) {
    Page.setTitle('Students');
    Menu.setMenu('students');

    if (Page.message().show) {
        $scope.message = { text: Page.message().text, show: Page.message().show };
        Page.resetMessage();
    }

    $http.get('/Ngstudent/Index').success(function (data) {
        $scope.pager = data.pager;
        $scope.model = data.model;
    });

    $scope.find = function () {
        var keyword = $scope.SearchString;
        $http.get('/Ngstudent/Index', { params: { SearchString: keyword } }).success(function (data) {
            $scope.pager = data.pager;
            $scope.model = data.model;
        });
    }
}

function StudentCreateCtrl($scope, $http, $timeout, Page, Menu) {
    Page.setTitle('Create');
    Menu.setMenu('students');

    $scope.save = function () {
        var o = {
            LastName: $scope.LastName,
            FirstMidName: $scope.FirstMidName,
            EnrollmentDate: utils.getDateStr($scope.EnrollmentDate)
        };

        $http.post('/Ngstudent/Create', o).success(function (data) {
            if (data.success == 1) {
                Page.setMessage(data.message);
                window.location.href = '#/students';
            }

            else if (data.error == 1) {
                $scope.error = true;
                $scope.errorText = data.message;
            }
        });
    }

    $scope.open = function () {
        $timeout(function () {
            $scope.opened = true;
        });
    }

    $scope.dismissAlert = function () {
        $scope.error = false;
    }
}
