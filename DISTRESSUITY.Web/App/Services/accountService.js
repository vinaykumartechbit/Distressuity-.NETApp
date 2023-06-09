'use strict';

app.factory('accountService', ['$http', '$q', function ($http, $q) {
    var accountServiceFactory = {};

    var _isAuthenticated = {
        isAuth: false,
        userName: ""
    };

    var _fillAuthData = function () {

        var accessToken = localStorage.getItem('accessToken');
        if (accessToken) {
            _isAuthenticated.isAuth = true;
            _isAuthenticated.userName = localStorage.getItem('userName');
        }
    }

    var _createaccount = function (modeldata) {
        var deferred = $q.defer();
        $http.post('api/Account/Register', modeldata).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _loginuser = function (userlogin) {
        var data = "grant_type=password&username=" + userlogin.username + "&password=" + userlogin.password;

        var deferred = $q.defer();
        $http.post("/TOKEN", data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
            _isAuthenticated.isAuth = true;
            _isAuthenticated.userName = data.userName;
            //Store the token information in the localstorage
            //So that it can be accessed for other views
            localStorage.setItem('userName', response.userName);
            localStorage.setItem('accessToken', response.access_token);
            localStorage.setItem('refreshToken', response.refresh_token);
            localStorage.setItem('userRole', response.userRole);

 
            //accesstoken = response.access_token;

            //if (accesstoken) {
            //    authHeaders.Authorization = 'Bearer ' + accesstoken;
            //}
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _logOut = function () {
        //var accesstoken = localStorage.getItem('accessToken');
        //var authHeaders = {};
        //if (accesstoken) {
        //    authHeaders.Authorization = 'Bearer ' + accesstoken;
        //}
        var deferred = $q.defer();

        var response = $http({
            url: "/api/Account/Logout",
            metod: "GET",
            //headers: authHeaders
        }).success(function (response) {
            localStorage.clear();
            _isAuthenticated.isAuth = false;
            _isAuthenticated.userName = "";
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _createsession = function (name) {

        _isAuthenticated.isAuth = true;
        _isAuthenticated.userName = name;
        return 'success';
    }

    var _confirmemail = function (userId, code) {
        var data = { userId: userId, code: code };
        var deferred = $q.defer();
        $http({
            url: "api/Account/ConfirmEmail",
            method: "POST",
            params: data
        }).then(function (response) {
            deferred.resolve(response.data);
            // this callback will be called asynchronously
            // when the response is available
        }, function (err, status) {
            deferred.reject(err);
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        //$http.post('api/Account/ConfirmEmail', data).success(function (response) {
        //    deferred.resolve(response);
        //}).error(function (err, status) {
        //    deferred.reject(err);
        //});
        return deferred.promise;
    }

    var _setpassword = function (userId, password, confirmPassword) {

        var data = JSON.stringify({ UserId: userId, Password: password, ConfirmPassword: confirmPassword });
        var deferred = $q.defer();

        //    $http({
        //        url: "api/Account/SetPassword",
        //        method: "POST",
        //        params: data
        //    }).then(function (response) {
        //        deferred.resolve(response.data);
        //    // this callback will be called asynchronously
        //    // when the response is available
        //}, function (response) {
        //    // called asynchronously if an error occurs
        //    // or server returns response with an error status.
        //});

        $http.post('api/Account/SetPassword', data).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _forgetpassword = function (data) {
        var deferred = $q.defer();
        $http.post('api/Account/ForgetPassword', data).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _resetpassword = function (userId, password, confirmPassword) {
        var data = JSON.stringify({ UserId: userId, Password: password, ConfirmPassword: confirmPassword });
        var deferred = $q.defer();

        $http.post('api/Account/ResetPassword', data).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _getuser = function () {
        //var accesstoken = localStorage.getItem('accessToken');
        //var authHeaders = {};
        //if (accesstoken) {
        //    authHeaders.Authorization = 'Bearer ' + accesstoken;
        //}
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetUser",
            method: "GET",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }
   

    accountServiceFactory.getuser = _getuser;
    accountServiceFactory.createaccount = _createaccount;
    accountServiceFactory.loginuser = _loginuser;
    accountServiceFactory.createsession = _createsession;
    accountServiceFactory.isAuthenticated = _isAuthenticated;
    accountServiceFactory.fillAuthData = _fillAuthData;
    accountServiceFactory.logOut = _logOut;
    accountServiceFactory.confirmemail = _confirmemail;
    accountServiceFactory.setpassword = _setpassword;
    accountServiceFactory.forgetpassword = _forgetpassword;
    accountServiceFactory.resetpassword = _resetpassword;
    return accountServiceFactory;
}]);