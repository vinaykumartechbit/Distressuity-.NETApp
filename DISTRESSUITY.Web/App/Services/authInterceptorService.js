app.factory('authInterceptorService', ['$q', '$injector', '$location', 'notification', 'loader', function ($q, $injecter, $location, notification, loader) {
    var authInterceptorServiceFactory = {};
    var _request = function (config) {
        config.headers = config.headers || {};
        var accessToken = localStorage.getItem("access_token");
        if(accessToken)
        {
            config.headers.Authorization = 'Bearer ' + accessToken;
        }

        return config;
    }

    var _responseError = function(rejection)
    {
        if(rejection.status === 401)
        {
          //  notification.error("Authorization has been denied for this request. Please Login first");
            var accountService = $injecter.get('accountService');
            accountService.logOut();
            $location.path("/");
        }

        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;

}]);