﻿'use strict';

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

    $scope.selected = {
        all: false,
        count: 0,
        message: function () {
            return this.count + " item" + (this.count > 1 ? 's' : '') + " selected";
        }
    };

    if (Page.message().show) {
        $scope.message = _.clone(Page.message());
        Page.resetMessage();
    }

    $http.get('/Ngstudent/Index').success(function (data) {
        $scope.pager = data.pager;
        $scope.model = data.model;
        $scope.currentSort = data.sortOrder;
        $scope.sortByName = true;
    });

    $scope.find = function () {
        var params = {
            SearchString: $scope.SearchString,
            sortOrder: $scope.currentSort
        };
        $http.get('/Ngstudent/Index', { params: params }).success(function (data) {
            $scope.pager = data.pager;
            $scope.model = data.model;
        });
    }

    $scope.gotoPage = function (page) {
        var params = {
            SearchString: $scope.SearchString,
            sortOrder: $scope.currentSort,
            page: page
        };
        $http.get('/Ngstudent/Index', { params: params }).success(function (data) {
            $scope.pager = data.pager;
            $scope.model = data.model;
        });
    }

    $scope.sort = function (a) {
        if (a == 'Name') {
            if ($scope.currentSort == null || $scope.currentSort == '')
                $scope.currentSort = 'Name_desc';

            else
                $scope.currentSort = '';
        }

        else if (a == 'FirstName') {
            if ($scope.currentSort == 'FirstName')
                $scope.currentSort = 'FirstName_desc';

            else
                $scope.currentSort = 'FirstName';
        }

        else if (a == 'Date') {
            if ($scope.currentSort == 'Date')
                $scope.currentSort = 'Date_desc';

            else
                $scope.currentSort = 'Date';
        }

        $scope.gotoPage($scope.pager.PageNum);
    }

    $scope.getSortCss = function (a) {
        var up = 'icon-chevron-up icon-white';
        var down = 'icon-chevron-down icon-white';

        if (($scope.currentSort == null || $scope.currentSort == '') && a == 'Name')
            return up;

        if ($scope.currentSort.indexOf(a) == 0) {
            if ($scope.currentSort.indexOf('desc') > 0)
                return down;

            else
                return up;
        }

        return null;
    }

    $scope.selectRow = function ($event, o) {
        $event.stopPropagation();

        if (o.selected)
            ++$scope.selected.count;

        else
            --$scope.selected.count;
    }

    $scope.selectAll = function ($event) {
        $event.stopPropagation();

        var list = null;
        var n = 0;

        if ($scope.model != null)
            list = $scope.model;

        if (list != null)
            n = list.length;

        for (var i = 0; i < n; i++) {
            var o = list[i];
            o.selected = $scope.selected.all;
        }

        if ($scope.selected.all)
            $scope.selected.count = n;

        else
            $scope.selected.count = 0;
    }

    $scope.removeItems = function () {
        if ($scope.selected.count < 1)
            return;

        alert('del');
    }
}

function StudentCreateCtrl($scope, $http, $timeout, Page, Menu) {
    Page.setTitle('Create');
    Menu.setMenu('students');

    $scope.action = 'Create';

    $scope.save = function () {
        var o = {
            LastName: $scope.model.LastName,
            FirstMidName: $scope.model.FirstMidName,
            EnrollmentDate: utils.getDateStr($scope.model.EnrollmentDate)
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

function StudentEditCtrl($scope, $http, $routeParams, $timeout, Page, Menu) {
    Page.setTitle('Edit');
    Menu.setMenu('students');

    $scope.action = 'Save';

    $http.get('/Ngstudent/Edit/' + $routeParams.id).success(function (data) {
        $scope.model = data;
        $scope.model.EnrollmentDate = utils.getDate(data.EnrollmentDate);
    });

    $scope.save = function () {

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

function StudentDetailsCtrl($scope, $http, $routeParams, Page, Menu) {
    Page.setTitle('Details');
    Menu.setMenu('students');

    $http.get('/Ngstudent/Details/' + $routeParams.id).success(function (data) {
        $scope.model = data.model;
        $scope.enrollments = data.enrollments;
    });
}
