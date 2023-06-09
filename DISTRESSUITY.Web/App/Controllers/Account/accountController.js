'use strict';

app.controller('registerController', ['$scope', '$location', 'accountService', 'notification', 'loader', '$rootScope', function ($scope, $location, accountService, notification, loader, $rootScope) {
    $scope.User = {};
    $scope.$root.IsSearch = true;
    //$scope.User.UserId = userId;

    $scope.createUser = function ($event) {
        loader.show($event.currentTarget);
        accountService.createaccount($scope.User).then(function (response) {
            if (response != "" && response != null && response.Success == true) {
                $scope.User = {};
                notification.success(response.Message);
                $location.path("/login");
            }
            else {
                notification.error(response.Message);
                loader.hide();
                $scope.User.Email = "";
            }

        });
    }
}]);

app.controller('loginController', ['$scope', 'accountService', 'projectService', '$location', 'notification', 'loader', '$rootScope', function ($scope, accountService, projectService, $location, notification, loader, $rootScope) {
    $scope.Login = {};
    $scope.$root.IsSearch = true;
    //$scope.User.UserId = userId;

    $scope.loginUser = function ($event) {
        loader.show($event.currentTarget);
        var userLogin = {
            grant_type: 'password',
            username: $scope.Login.UserName,
            password: $scope.Login.Password
        };

        accountService.loginuser(userLogin).then(function (resp) {
            $scope.$root.GetNewMessages();
            accountService.fillAuthData();
            $rootScope.$emit("CallMainControllerGetUserProfileMethod", {});
            $scope.userName = resp.userName;
            //$scope.userRole = resp.userRole.toLowerCase();

            if (resp.userRole.toLowerCase() == 'admin') {
                $scope.isAdmin = true;
                $location.path('/allProjects');
            }
            else if ($rootScope.returnToState) {
                $scope.isAdmin = false;
                //redirect all others after login to /rooms
                if ($rootScope.returnToStateParams) {
                    var returnUrl = $rootScope.returnToState.split('/')[1];
                    var returnParameters = $rootScope.returnToStateParams[$rootScope.returnToState.split('/')[2].slice(1)];

                    if (returnUrl == "payments") {

                        projectService.getIfLogginInUserCanPay(returnParameters).then(function (response) {

                            if (response.data.ifUserCanPay.Result) {
                                $location.path('/' + returnUrl + '/' + returnParameters);

                            }
                            else {
                                $location.path('/projectdetail/' + returnParameters);
                            }
                        })
                    }
                    else {
                        $location.path('/' + returnUrl + '/' + returnParameters);
                    }
                }
                else {
                    $location.path($rootScope.returnToState);
                }
                resetRootScope();
            }
            else {
                $scope.isAdmin = false;
                $location.path('/');
            }
            $scope.$root.chat.server.connected();

        }, function (err) {
            notification.error(err.error_description);
            loader.hide();
        });
    };

    var resetRootScope = function () {
        for (var prop in $rootScope) {
            if (prop.substring(0, 1) !== '$') {
                delete $rootScope[prop];
            }
        }
    }

    $scope.externalLoginUser = function (provider) {
        var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
        var externalProviderUrl = location.protocol + '//' + location.host + "/api/Account/ExternalLogin?provider=" + provider + "&redirect_uri=" + redirectUri;
        window.$windowScope = $scope;
        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
    };

    $scope.authCompletedCB = function (fragment) {

        $scope.$apply(function () {
            var success = fragment.haslocalaccount;
            if (success != "False" && success != "False") {
                accountService.createsession(fragment.external_user_name)
                localStorage.setItem('userName', fragment.external_user_name);
                localStorage.setItem('accessToken', fragment.access_token);
                $rootScope.$emit("CallMainControllerGetUserProfileMethod", {});
                if ($rootScope.returnToState) {
                    //redirect all others after login to /rooms
                    $location.path($rootScope.returnToState);
                }
                else {
                    $location.path('/');
                }

            }
            else {
                notification.error(fragment.error_message);
            }
        });
    };

    $scope.loginController = function (emailId) {

    }

}]);

app.controller('setPasswordController', ['$scope', 'accountService', "$routeParams", "$location", 'notification', 'loader', '$rootScope', function ($scope, accountService, $routeParams, $location, notification, loader, $rootScope) {
    $scope.User = {};
    $scope.settingPassword = true;
    var userId = $routeParams.userid;
    var code = $routeParams.Key;
    $scope.$root.IsSearch = true;

    var data = accountService.confirmemail(userId, code);
    data.then(function (response) {
        if (response.Success) {
            $scope.settingPassword = false;
        }
        else {
            notification.error(response.Message);
        }
    });

    $scope.setPassword = function ($event) {
        loader.show($event.currentTarget);
        var password = $scope.User.password;
        var confirmPassword = $scope.User.ConfirmPassword;
        var userLogin = {
            grant_type: 'password',
            username: "",
            password: ""
        };
        var data = accountService.setpassword(userId, password, confirmPassword);
        data.then(function (response) {
            if (response.Success) {
                userLogin.username = response.Data;
                userLogin.password = password;
                accountService.loginuser(userLogin).then(function (response1) {
                    accountService.fillAuthData();
                    $rootScope.$emit("CallMainControllerGetUserProfileMethod", {});
                    $location.path('/');
                }, function (err) {
                    notification.error(err.error_description);
                    loader.hide();
                });
            }
        });
    }

}]);

app.controller('forgetPasswordController', ['$scope', 'accountService', 'notification', 'loader', function ($scope, accountService, notification, loader) {
    $scope.User = {};
    $scope.$root.IsSearch = true;
    $scope.forgetPassword = function ($event) {
        loader.show($event.currentTarget);
        accountService.forgetpassword($scope.User).then(function (response) {
            if (response.Success) {
                $scope.User = {};
                notification.success(response.Message);
                loader.hide();
            }
            else {
                $scope.User = {};
                notification.error(response.Message);
                loader.hide();
            }
        });
    }
}]);

app.controller('resetPasswordController', ['$scope', 'accountService', "$routeParams", "$location", 'notification', 'loader', function ($scope, accountService, $routeParams, $location, notification, loader) {
    $scope.User = {};
    $scope.$root.IsSearch = true;
    $scope.settingPassword = true;
    var userId = $routeParams.userid;
    var code = $routeParams.Key;

    var data = accountService.confirmemail(userId, code);
    data.then(function (response) {
        if (response.Success) {
            $scope.settingPassword = false;
        }
        else {
            notification.error(response.Message);
        }
    });

    $scope.resetPassword = function ($event) {
        loader.show($event.currentTarget);
        var password = $scope.User.password;
        var confirmPassword = $scope.User.ConfirmPassword;
        var userLogin = {
            grant_type: 'password',
            username: "",
            password: ""
        };
        var data = accountService.resetpassword(userId, password, confirmPassword);
        data.then(function (response) {
            if (response.Success) {
                userLogin.username = response.Data;
                userLogin.password = password;
                accountService.loginuser(userLogin).then(function (response1) {
                    $location.path('/home');
                }, function (err) {
                    notification.error(err.error_description);
                    loader.hide();
                });
            }
        });
    }

}]);