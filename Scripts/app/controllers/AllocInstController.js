
app.controller('AllocInstController', ['$scope', '$http', '$interval', 'uiGridConstants', function ($scope, $http, $interval, uiGridConstants) {

    var url = 'http://localhost:61847/TradeAllocation/AllocationInstruction';
    $scope.promise = null;

    var paginationOptions = {
        pageNumber: 1,
        pageSize: 25,
        sort: null
    };

    $scope.gridOptions = {
        enableFiltering: true,
        useExternalFiltering: true,
        columnDefs: null,
        enableColumnResizing: true,
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;

                /*
                    $http.get('http://localhost:61847/TradeAllocation/AllocationInstruction')
                    .success(function (data) {
                        $scope.gridOptions.data = data;
                    });
                */
                var filteredListStr = collectFilterList(grid);
                $scope.promise = allocReq(url, paginationOptions.pageNumber, paginationOptions.pageSize, filteredListStr);
                loadGrid($scope.promise, setScroll);

            });
        }
    };

    /*
    $http.get('http://localhost:61847/TradeAllocation/AllocationInstruction')
    .success(function (data) {
        $scope.gridOptions.data = data;
    });
    */

    $scope.promise = allocReq(url, paginationOptions.pageNumber, paginationOptions.pageSize);
    loadGrid($scope.promise, setScroll);

    function allocReq(url, pageNumber, pageSize, filteredList) {

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


            $scope.gridOptions.data = JSON.parse(result.allocData);
            var columnNames = [];
            angular.forEach($scope.gridOptions.data[0], function (value, key) {
                columnNames.push({ field: key, width: 250, name: key });
            });
            $scope.gridOptions.columnDefs = columnNames;

            $scope.gridOptions.totalItems = JSON.parse(result.totCount);
            callback();

        });
    }

    function setScroll() {

        $('#shadowBox').show();
        $('.ng-scope.ng-isolate-scope.ui-grid-native-scrollbar.horizontal').scrollLeft(4076);

        setTimeout(function () {
            $('.ng-scope.ng-isolate-scope.ui-grid-native-scrollbar.horizontal').scrollLeft(0);
            $('#shadowBox').hide();
        }, 10);

    }

    function collectFilterList(grid) {

        var filteredList = [];

        for (var i = 0; i < grid.columns.length; i++) {
            value = grid.columns[i].filters[0].term;
            field = grid.columns[i].field;

            if (typeof value !== 'undefined') {
                var FilterObj = {};
                FilterObj.Value = value;
                FilterObj.Field = field;
                filteredList.push(FilterObj);

            }

            console.log('Field: ' + field + '\tSearch Value: ' + value);
        }

        return filteredList;
    }

}]);
