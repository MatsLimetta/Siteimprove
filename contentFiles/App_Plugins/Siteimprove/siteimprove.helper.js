var siteimprove = {
    log: false,
    recrawlIds: [],
    token: '',
    currentPageId: 0,
    reloadData: function () { },
    reloadPage:  function () {
        location.reload();
    }

};

siteimprove.helper = {

    backofficeApiUrl: '/umbraco/backoffice/api/SiteImproveApi',
    isCreatingPage: false,

    /**
     * Will fetch _si token and push of method to _si
     */
    pushSi: function (method, url) {
        var _si = window._si || [];

        // Get token from backoffice if not set
        if (!siteimprove.token) {
            $.get(this.backofficeApiUrl + '/getToken')
                .then(function (response) {
                    siteimprove.token = response;

                    if (siteimprove.log)
                        console.log('SiteImprove pass: ' + method + ' - ' + url);

                    // Build full URL
                    _si.push([method, url, response]);
                })
                .fail(function (error) {
                    if (siteimprove.log)
                        console.log("pushSi error:", error);
                });
        } else {
            if (siteimprove.log)
                console.log('SiteImprove pass: ' + method + ' - ' + url);

            // Build full URL
            _si.push([method, url, siteimprove.token]);
        }
    },

    /**
     * Workaround for closing the _si window
     */
    closeSi: function () {
        (window._si || []).push(['input', '', '']);
    },

    /**
     * Wrap simple ajax call getPageUrl
     * @return {Promise}
     */
    getPageUrl: function (pageId) {
        return $.get(siteimprove.helper.backofficeApiUrl + '/getPageUrl?pageid=' + pageId)
            .then()
            .fail(function (error) {
                if (siteimprove.log)
                    console.log("getPageUrl error:", error);
            });
    },

    /**
     * Requests pageUrl from backoffice and send to SI
     */
    handleFetchPushUrl: function (method, pageId, isFormPublish) {
        this.getPageUrl(pageId)
            .then(function (response) {

                if (response.success) {

                    // When recieved the url => send off to _si
                    siteimprove.helper.pushSi(method, response.url);

                }
                else {
                    if (isFormPublish) {
                        siteimprove.helper.pushSi('input', '');
                        return;
                    }

                    // If can't find page pass empty url
                    siteimprove.helper.pushSi(method, '');
                }

            })
            .fail(function (error) {
                if (siteimprove.log)
                    console.log("handleFetchPushUrl error:", error);
                siteimprove.helper.closeSi();
            });
    },

    /**
     * Handles the RouteChangeSuccess event. Will take care of pushing to _si and listen on publish event for the new edit scope.
     */
    on$routeChangeSuccess: function (e, next, current) {
        if (next.params.id) {
            siteimprove.currentPageId = next.params.id;
        }
        // Only listen when user works on the content tree
        if (next.params.tree === 'content') {

            if (siteimprove.recrawlIds.length < 1) {
                $.get(siteimprove.helper.backofficeApiUrl + '/GetCrawlingIds')
                    .then(function (response) {
                        siteimprove.recrawlIds = (response + '' || '').split(',');
                    })
                    .fail(function (error) {
                        if (siteimprove.log)
                            console.log("pushSi error:", error);
                    });
            }

            if (!next.params.hasOwnProperty('create') && ( current === undefined || !current.params.hasOwnProperty('create') )&& next.params.id) {
                siteimprove.helper.handleFetchPushUrl('input', next.params.id);
            }
            else if (siteimprove.helper.isCreatingPage) {
                siteimprove.helper.isCreatingPage = false;
                siteimprove.helper.handleFetchPushUrl('recheck', next.params.id);
            }

        } else if (next.params.tree !== 'content') {
            siteimprove.helper.pushSi('domain', '');
        }
    }
}