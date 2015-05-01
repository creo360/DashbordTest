/// <reference path="../../angular.js" />
/// <reference path="../../ui-grid-unstable.js" />


app.controller('NorthwindController', function ($scope, $http, $filter, $timeout,  uiGridConstants) {


    var url = 'http://localhost:61847/Northwind/Orders';
    $scope.promise = null;

    var paginationOptions = {
        pageNumber: 1,
        pageSize: 50,
        sort: null
    };


    function ordersReq(url, pageNumber, pageSize, filteredList) {

        var pageModel = {};
        pageModel.PageNumber = pageNumber;
        pageModel.PageSize = pageSize;
        pageModel.FilteredList = filteredList;
       
        var jsonData = JSON.stringify(pageModel);

        var req = {
            method: 'POST',
            url: url,
            headers: {
                'Content-Type': 'application/json'
            },
            data: jsonData,
        }

        return $http(req);
    }

    function loadGrid(promise, callback) {
        promise.success(function (result) {
            $scope.orderGrid.data = JSON.parse(result.data);
            //get columns in to array
            //$scope.orderGrid.columnDefs = null;

            if ($scope.orderGrid.columnDefs.length === 0) {
                var columnNames = [];
                angular.forEach($scope.orderGrid.data[0], function (value, key) {
                    columnNames.push({ field: key, width: 250, displayName: key });
                });
                $scope.orderGrid.columnDefs = columnNames;
            }
            

            $scope.orderGrid.totalItems = result.totCount;
            $scope.orderGrid.data = JSON.parse(result.data);

            callback();

        });
    }

    function setOverlay() {

        $('#shadowBox').show();
        //$('.ng-scope.ng-isolate-scope.ui-grid-native-scrollbar.horizontal').scrollLeft(4076);

        setTimeout(function () {
            //$('.ng-scope.ng-isolate-scope.ui-grid-native-scrollbar.horizontal').scrollLeft(0);
            $('#shadowBox').hide();
        }, 0);

    }

   

    $scope.orderGrid = {
        data: null,
        columnDefs:null,
        enableFiltering: true,
        paginationPageSizes: [2, 5, 10],
        paginationPageSize: 5,
        useExternalFiltering: true,
        useExternalPagination: true,
        useExternalSorting: false,
        enableColumnResizing: true,
        excessColumns:100,
        columnVirtualizationThreshold: 100,
        virtualizationThreshold:100,
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;

            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                paginationOptions.pageNumber = newPage;
                paginationOptions.pageSize = pageSize;
                var filteredListStr = collectFilterList(this.grid);
                $scope.promise = ordersReq(url, paginationOptions.pageNumber, paginationOptions.pageSize, filteredListStr);
                $scope.promise.success(function (result) {
                    $scope.orderGrid.data = JSON.parse(result.data);
                    $scope.orderGrid.totalItems = result.totCount;

                })

            });

            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                var filteredListStr = collectFilterList(grid);
                
                //$scope.promise = ordersReq(url, paginationOptions.pageNumber, paginationOptions.pageSize, filteredListStr);
                $scope.promise = ordersReq(url, null,null, filteredListStr);
                //loadGrid($scope.promise, setOverlay);
                
                $scope.promise.success(function (result) {
                    $scope.orderGrid.data = JSON.parse(result.data);
                    $scope.orderGrid.totalItems = result.totCount;
                    paginationOptions.pageNumber = 1;
                    //paginationOptions.pageSize = 5;
                    $scope.orderGrid.paginationCurrentPage = 1;
                })
            });
        }

    };

    $scope.filterData = function () {
        //$scope.promise = ordersReq(url, paginationOptions.pageNumber, paginationOptions.pageSize);
        //loadGrid($scope.promise, setOverlay);
    };

    $scope.promise = ordersReq(url);
    loadGrid($scope.promise, setOverlay);



    /*
        utility methods
    */
    function collectFilterList(grid) {

        var filteredList = [];

        for (var i = 0; i < grid.columns.length; i++) {
            value = grid.columns[i].filters[0].term;
            field = grid.columns[i].field;

            if (typeof value !== 'undefined' && value !== null && value.length !== 0) {
                var FilterObj = {};
                FilterObj.Value = value;
                FilterObj.Field = field;
                filteredList.push(FilterObj);

            }

            console.log('Field: ' + field + '\tSearch Value: ' + value);
        }

        //return JSON.stringify(filteredList);
        return filteredList;
    }


});