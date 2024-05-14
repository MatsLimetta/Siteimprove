
/**
 * Factory that is used by then menu actions in the edit scope
 */
angular.module('umbraco').factory('SiteImproveMenuActions', ['$http', '$rootScope', '$location', '$routeParams', '$log', 'appState', 'editorState', function ($http, $rootScope, $location, $routeParams, $log, appState, editorState) {
    var factory = new function () {
        var _si = window._si || [],
            siHelper = window.siteimprove.helper;

        if (window.siteimprove.initiated) {
            if(window.siteimprove.log)
                console.log('SiteImprove is already initiated');
        } else {
            $rootScope.$on('$routeChangeSuccess', siHelper.on$routeChangeSuccess.bind(siHelper));
        }

        /**
         * Action from SiteImproveStartMenuItem.cs
         */
        this.Start = function () {
            siHelper.handleFetchPushUrl('input', editorState.current.id);
        };

        /**
         * Action from SiteImproveRecheckMenuItem.cs
         */
        this.Recheck = function () {
            siHelper.handleFetchPushUrl('recheck', editorState.current.id);
        };

    };

    return factory;
}]);