angular.module("umbraco").controller("SiteImprove.DashboardController", [
    '$scope', '$http', 'assetsService',
    function ($scope, $http, assetsService) {

        assetsService.load(["https://cdn.siteimprove.net/cms/overlay.js?v=2"], $scope).then(function () {
            console.log('Overlay loaded');
        });

        $scope.token = "";
        $scope.crawlingIds = "";
        $scope.pathMap = { currentUrlPart: '', newUrlPart: ''};
        $scope.output = "";
        $scope.requestNewToken = function () {
            $scope.token = "Loading...";
            $http.get('/umbraco/backoffice/api/SiteImproveApi/requestNewToken')
                .then(function (resp) {
                    $scope.token = resp.data;
                });
        };

        $scope.savePathMap = function () {
            $http.post('/umbraco/backoffice/api/SiteImproveApi/SaveUrlMap',
                {
                    PageId: siteimprove.currentPageId,
                    CurrentUrlPart: $scope.pathMap.currentUrlPart,
                    NewUrlPart: $scope.pathMap.newUrlPart
                })
                .then(function(resp) {
                    $scope.output = resp.data.message;
                    if (resp.data.success === true) {
                        setTimeout(siteimprove.reloadPage, 1000);
                    }
                });


            /*$http.post('/umbraco/backoffice/api/SiteImproveApi/setCrawlingIds?ids=' + $scope.crawlingIds)
                .then(function () {
                    $scope.output = "Saved!";

                    if (window.siteimprove) {
                        window.siteimprove.recrawlIds = ($scope.crawlingIds || '').split(',');
                    }

                    setTimeout(function () {
                        $scope.output = "";
                    }, 2000);
                });*/
        };

        $scope.saveIds = function () {
            $http.post('/umbraco/backoffice/api/SiteImproveApi/setCrawlingIds?ids=' + $scope.crawlingIds)
                .then(function () {
                    $scope.output = "Saved!";
                    
                    if (window.siteimprove) {
                        window.siteimprove.recrawlIds = ($scope.crawlingIds || '').split(',');
                    }

                    setTimeout(function () {
                        $scope.output = "";
                    }, 2000);
                });
        };

        $scope.loadData = function () {
            $http.get('/umbraco/backoffice/api/SiteImproveApi/getSettings', { params: { pageId : siteimprove.currentPageId } })
                .then(function (resp) {
                    $scope.token = resp.data.token;
                    $scope.crawlingIds = resp.data.crawlingIds;
                    $scope.pathMap.currentUrlPart = resp.data.currentUrlPart;
                    $scope.pathMap.newUrlPart = resp.data.newUrlPart;
                });
        };
        siteimprove.reloadData = $scope.loadData;
        $scope.loadData();
    }
]);