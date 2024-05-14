function HookPublishButton($scope)
{
	// Hook on save and publish event
	$scope.$on('formSubmitted', function (form) {

		// Page has been submitted, but no id on the page. They just created a new page
		if (form.targetScope.content.id == 0) {
			window.siteimprove.helper.isCreatingPage = true;
			return;
		}

		if (window.siteimprove.recrawlIds.indexOf(form.targetScope.content.id.toString()) > -1) {
			window.siteimprove.helper.handleFetchPushUrl('recrawl', form.targetScope.content.id, true);
		} else {
			window.siteimprove.helper.handleFetchPushUrl('recheck', form.targetScope.content.id, true);
		}
	});
}

function InitializeSiteimprove($scope)
{
	
        var siHelper = window.siteimprove.helper;
//            $scope = angular.element('body').scope(); // Get $rootSope

        $.get(siHelper.backofficeApiUrl + '/GetCrawlingIds')
            .then(function (response) {
                window.siteimprove.recrawlIds = (response || '').split(',');
            });

        $.get(siHelper.backofficeApiUrl + '/getToken')
            .then(function (response) {
                window.siteimprove.token = response;
            });
			
        if ($scope) {
            $scope.$on('$routeChangeSuccess', siHelper.on$routeChangeSuccess.bind(siHelper));
        }
};

app.config(['$provide', function ($provide) {
    $provide.decorator('$controller', [
        '$delegate',
        function ($delegate) {
            return function(constructor, locals) {
                if (typeof constructor == "string") {
					if(constructor == "Umbraco.MainController")
					{
						InitializeSiteimprove(locals.$scope);
					}
					if(constructor == "Umbraco.Editors.Content.EditController") {
                        HookPublishButton(locals.$scope);
                    }
                }
                return $delegate.apply(this, [].slice.call(arguments));
            }
        }]);
}]);