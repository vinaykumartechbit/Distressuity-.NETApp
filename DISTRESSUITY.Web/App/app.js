/// <reference path="Controllers/SuperAdmin/AllProjectsController.js" />
// Angular App

var app = angular.module('appDistressuity', ['ngRoute', 'validation', 'validation.rule'
, 'angularFileUpload', 'angular-loading-bar', 'ngSanitize', '720kb.datepicker']);

app.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    //cfpLoadingBarProvider.spinnerTemplate = '<div><span class="fa fa-spinner">Loading...</div>';
}]);

//var config = {};

//app.run(function ($http) {

//    $http.defaults.headers.common.Authorization = function () {
//        debugger
//        return 'Bearer ' + localStorage.getItem('accessToken');
//    }();
//});
var interceptor = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + localStorage.getItem('accessToken');
            return config;
        }
    }
};

app.config(function ($httpProvider) {

    $httpProvider.interceptors.push(interceptor);
});

app.config(function ($routeProvider, $locationProvider) {
    $routeProvider
    .when('/payments/:projectId', {
        templateUrl: 'App/Views/User/Payments.html',
        controller: 'paymentController',
        resolve:
        {
            projectRoute: function (projectService, $route) {
                var projectId = $route.current.params.projectId
                return projectService.getprojectbyid(projectId, '1'); // 1 pageType for payments
            }
        },
        authenticate: true
    })
    .when('/allProjects', {
        templateUrl: 'App/Views/SuperAdmin/AllProjects.html',
        controller: 'AllProjectsController'
    })
    .when('/reports', {
        templateUrl: 'App/Views/SuperAdmin/Reports.html',
        //controller: 'ReportsController'
    })
    .when('/projectsReport', {
        templateUrl: 'App/Views/SuperAdmin/projectsReport.html',
        controller: 'ReportsController'
    })
    .when('/totalEarningReport', {
        templateUrl: 'App/Views/SuperAdmin/totalEarningReport.html',
        controller: 'TotalEarningReportController',
        resolve:
        {
            FilteredFundedProjectList: function (superAdminService, $route) {
                var StartDate = null;
                var EndDate = null;
                return superAdminService.getFilteredFundedProjectList(StartDate, EndDate);
            }
        }
    })
    .when('//:searchKeyword?/:IndustryId?', {
        templateUrl: 'App/Views/Home.html',
        controller: 'homeController',
        resolve:
        {
            projectList: function (projectService, $route) {
                return projectService.getallprojects();
            }
        },
        authenticate: false
    })
    .when('/login', {
        templateUrl: 'App/Views/Account/Login.html',
        controller: 'loginController'
    })
    .when('/signup', {
        templateUrl: 'App/Views/Account/Register.html',
        controller: 'registerController'
    })
    .when('/profile', {
        templateUrl: 'App/Views/User/Profile.html',
        controller: 'profileController',
        resolve:
        {
            myProfile: function (userService) {
                return userService.getprofile();
            }
        },
        authenticate: true
    })
    .when('/setpassword/:userid', {
        templateUrl: 'App/Views/Account/SetPassword.html',
        controller: 'setPasswordController'
    })
    .when('/forgetpassword', {
        templateUrl: 'App/Views/Account/ForgetPassword.html',
        controller: 'forgetPasswordController'
    })
    .when('/resetpassword/:userid', {
        templateUrl: 'App/Views/Account/ResetPassword.html',
        controller: 'resetPasswordController'
    })
    .when('/project', {
        templateUrl: 'App/Views/User/Project.html',
        controller: 'projectController',
        resolve:
        {
            myProjectDetails: function () {
                return null;
            }
        },
        authenticate: true
    })
    .when('/project/:projectId', {
        templateUrl: 'App/Views/User/Project.html',
        controller: 'projectController',
        resolve:
        {
            myProjectDetails: function (projectService, $route) {
                var projectId = $route.current.params.projectId
                return projectService.getprojectbyid(projectId, '2'); // 2 pageType for Edit Project in MyProjects
            }
        },
        authenticate: true
    })
    .when('/projectdetail/:projectId', {
        templateUrl: 'App/Views/User/ProjectDetail.html',
        controller: 'projectDetailController',
        resolve:
        {
            projectDetails: function (projectService, $route) {
                var projectId = $route.current.params.projectId
                return projectService.getprojectbyid(projectId, '3'); // 3 pageType for View Project Details 
            }
        }
    })
    .when('/filteredprojects/:IndustryId', {
        templateUrl: 'App/Views/User/FilteredProjects.html',
        controller: 'homeController',
        resolve:
        {
            projectList: function (projectService, $route) {
                var IndustryId = $route.current.params.IndustryId
                return projectService.FilterProjectByIndustryId(IndustryId);
            }
        }
    })
    .when('/messages', {
        templateUrl: 'App/Views/User/Messages.html',
        controller: 'messagesController',
        authenticate: true
    })
    .when('/myprojects/:searchKeyword?/:IndustryId?', {
        templateUrl: 'App/Views/User/MyProject.html',
        controller: 'myProjectController',
        authenticate: true
    })
    .when('/mypayments', {
        templateUrl: 'App/Views/User/MyPayments.html',
        controller: 'myPaymentController',
        authenticate: true
    })
    .when('/discover', {
        templateUrl: 'App/Views/User/Discover.html'
        //controller: 'myProjectController'
    })
     .when('/projectfunding/:projectId', {
         templateUrl: 'App/Views/SuperAdmin/ProjectFunding.html',
         controller: 'ProjectFundingController',
         authenticate: true
     })
    //$routeProvider.otherwise({ redirectTo: "/" });

    //$locationProvider.html5Mode({
    //    enabled: true,
    //    requireBase: false
    //});
});

/// <summary>
/// It's use to shown Validation message(Format) 
/// </summary>
app.config(['$validationProvider', function ($validationProvider) {

    $validationProvider.showSuccessMessage = false;
    $validationProvider.showErrorMessage = true;
    $validationProvider.setValidMethod('submit');

    $validationProvider
        .setExpression({
            confirmpassword: function (value, scope, element, attrs, param) {
                if (attrs.pwd != "" && attrs.pwd == value)
                    return true;
                else if (attrs.pwd != "")
                    return false;
                return true;
            },//it Made for Password Length atleast 6 characters
            passwordlength: function (value, scope, element, attrs, param) {
                if (value.length > 5)
                    return true;
                else
                    return false;
            },
            //it Made for Name Length atleast 3 characters
            nameMinLength: function (value, scope, element, attrs, param) {
                if (value.length > 2)
                    return true;
                else
                    return false;
            },
            lessthanhundered: function (value, scope, element, attrs, param) {
                if (value <= 100)
                    return true;
                else
                    return false;
            },
            //it Made for Name Length should not exceed 20 characters
            nameMaxLength: function (value, scope, element, attrs, param) {
                if (value.length < 19)
                    return true;
                else
                    return false;
            },
            morethanzero: function (value, scope, element, attrs, param) {

                if (value > 0) {
                    return true;
                }
                else
                    return false;
            },
            specialsymbols: function (value, scope, element, attrs, param) {

                var regex = new RegExp("^[0-9.]+$");
                if (!regex.test(value)) {
                    //var dhg = value.contains('.');
                    return false;
                } else
                    return true;
            },
            numbersOnly: function (value, scope, element, attrs, param) {

                var regex = new RegExp("^[0-9]{1,6}$");
                if (!regex.test(value)) {
                    //var dhg = value.contains('.');s
                    return false;
                } else
                    return true;
            },
            objectrequired: function (value, scope, element, attrs, param) {      // This validation is for that controls, those are having object selected in ng-model. Such as for dropdown control.

                var obj = scope.$eval(attrs.ngModel);
                if (obj[attrs.validatecolumn] == undefined || obj[attrs.validatecolumn] == 0 || obj[attrs.validatecolumn] == "") {
                    return false;
                } else {
                    return true;
                }

            },
            contactNo: function (value, scope, element, attrs, param) {
                if (value.length >= 10) {
                    return true;
                }
                else {
                    return false;
                }
            },
            image: function (value, scope, element, attrs, param) {
                if (value != null || scope.Project.ImagePath != null) {
                    return true;
                }
                else {
                    return false;
                }
            },
            minPledge: function (value, scope, element, attrs, param) {
                if (scope.MinPledge <= value) {
                    return true;
                }
                else {
                    return false;
                }
            },
            minCardLength: function (value, scope, element, attrs, param) {
                if (value.length >= 15) {
                    return true;
                }
                else {
                    return false;
                }
            },
            exceedMax: function (value, scope, element, attrs, param) {
                if (value > 100000) {
                    return false;
                }
                else {
                    return true;
                }
            },
            luhnCheck: function (value, scope, element, attrs, param) {
                var len = value.length,
                mul = 0,
                prodArr = [
                    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
                    [0, 2, 4, 6, 8, 1, 3, 5, 7, 9]
                ],
                sum = 0;

                while (len--) {
                    sum += prodArr[mul][parseInt(value.charAt(len), 10)];
                    mul ^= 1;
                }

                return sum % 10 === 0 && sum > 0;
            },
            minCVVValue: function (value, scope, element, attrs, param) {
                if (value.length >= 3)
                    return true;
                else
                    return false;
            },
            minYearValue: function (value, scope, element, attrs, param) {
                var dt = new Date();
                if (value >= dt.getFullYear())
                    return true;
                else
                    return false;
            },
            minDateValue: function (value, scope, element, attrs, param) {
                var dt = new Date();
                var date = new Date(value, scope.Payment.ExpirationMonth);
                if (date >= dt)
                    return true;
                else
                    return false;
            },
            paypalVerification: function (value, scope, element, attrs, param) {
                if (scope.Project.PaypalAccountStatus == true) {
                    return true;
                }
                else {
                    return false;
                }
            },
        })
        .setDefaultMsg({
            minPledge: {
                error: 'Please add value more than minimum pledge'
            },
            numbersOnly: {
                error: 'Not a valid number'
            },
            image: {
                error: 'required'
            },
            confirmpassword: {
                error: 'Password doesnot match'
            },
            passwordlength: {
                error: 'Password must be of minimium six character long'
            },
            lessthanhundered: {
                error: 'Max Percentage should be 100'
            },
            nameMinLength: {
                error: 'Name must be of minimium three character long'
            },
            nameMaxLength: {
                error: 'Name can not be twenty character long'
            },
            morethanzero: {
                error: 'Not Valid',
            },
            specialsymbols: {
                error: 'only enter decimal value',
            },
            objectrequired: {
                error: 'This should be Required!!',
            },
            contactNo:
            {
                error: 'Contact no. must be of minimum ten digit long'
            },
            minCardLength:
            {
                error: 'Card Number not valid'
            },
            exceedMax:
            {
                error: 'Value cannot be more than 100,000'
            },
            luhnCheck:
            {
                error: 'Card Number not valid'
            },
            minCVVValue:
            {
                error: 'CVV cannot be less than 3 digit'
            },
            minYearValue:
            {
                error: 'Year cannot be less than current year'
            },
            minDateValue:
            {
                error: 'Expiry date cannot be less than current date'
            },
            paypalVerification:
            {
                error: 'Paypal account is not verified'
            }
        });
}]);

// Notification factory
app.factory('notification', function () {
    return {
        success: function (message) {
            new PNotify({
                title: "Success",
                text: message,
                //delay: 1000,
                type: 'success'
            });
        },
        error: function (message) {
            new PNotify({
                title: "Error",
                text: message == undefined ? "Something went wrong." : message,
                delay: 1000,
                type: 'error'
            });
        },
        notice: function (title, message) {
            new PNotify({
                title: title,
                text: message,
                delay: 1000,
            });
        },
        info: function (title, message) {
            new PNotify({
                title: title,
                text: message,
                delay: 1000,
                type: 'info'
            });
        }
    }
});

//Showing Alert message in Modal
app.factory('bootboxModal', function () {
    return {
        alert: function (message, callback) {
            bootbox.alert(message, callback == undefined ? null : callback);
        },
        prompt: function (message, callback) {
            bootbox.prompt(message, callback == undefined ? null : callback)
        },
        confirm: function (message, callback) {
            bootbox.confirm(message, callback == undefined ? null : callback)
        }
    };
});

//Loader factory
app.factory('loader', function () {
    return {
        show: function (currentElement) {
            var l = Ladda.create(currentElement);
            l.start();
        },
        hide: function () {
            Ladda.stopAll();
        },
        preLoader: function () {
            $("#preloader").show();
        },
        hidePreLoader: function (container) {

            $("#preloader").hide();
            if (container) {
                container.show();
            }
        }
    };
});

//Interceptors for Common Error handling
app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

// Common Modal popup 
app.factory('bootboxModal', function () {
    return {
        alert: function (message, callback) {
            bootbox.alert(message, callback == undefined ? null : callback);
        },
        serverError: function (message, callback) {
            message = nl2br(message);
            bootbox.dialog({
                message: message,
                title: "<b>Error From Server</b>",
                buttons: {
                    main: {
                        label: "Ok",
                        className: "btn-primary",
                        callback: function () {
                            if (callback)
                                callback()
                        }
                    }
                }
            });
            //  bootbox.alert(message, callback == undefined ? null : callback);
        },
        prompt: function (message, callback) {
            bootbox.prompt(message, callback == undefined ? null : callback)
        },
        confirm: function (message, callback) {
            bootbox.confirm(message, callback == undefined ? null : callback)
        }
    };
});

// Credit Card Type
app.directive('ngCreditCard', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attrs) {
            elem.on('keyup', function (e) {
                var creditCard = { Master: 'Master', Visa: 'Visa', AmericanExpress: 'AmericanExpress', Diners: 'Diners', Discover: 'Discover', Jcb: 'Jcb', Maestro: 'Maestro' };
                $(this).removeClass();
                $(this).addClass("form-control creditCardNumber");
                var value = $(this).val();
                scope.CardType =
                  (/^5[1-5]/.test(value)) ? creditCard.Master
                  : (/^4/.test(value)) ? creditCard.Visa
                  : (/^3[47]/.test(value)) ? creditCard.AmericanExpress
                  : (/^6011|65|64[4-9]|622(1(2[6-9]|[3-9]\d)|[2-8]\d{2}|9([01]\d|2[0-5]))/.test(value)) ? creditCard.Discover
                  : (/^(4026|417500|4508|4844|491(3|7))/.test(value)) ? 'Visa_Electron'
                  : (/^(5018|5020|5038|6304|6759|676[1-3])/.test(value)) ? creditCard.Maestro
                  : (/^35(2[89]|[3-8][0-9])/.test(value)) ? creditCard.Jcb
                  : undefined
                if (scope.CardType != undefined) {
                    $(this).addClass(scope.CardType);
                    $(this).addClass('valid')
                }
                else {
                    $(this).removeClass('valid');
                }
            });
        }
    };
});
//Unique Filter 
app.filter('unique', function () {
    return function (items, filterOn) {

        if (filterOn === false) {
            return items;
        }

        if ((filterOn || angular.isUndefined(filterOn)) && angular.isArray(items)) {
            var hashCheck = {}, newItems = [];

            var extractValueToCompare = function (item) {
                if (angular.isObject(item) && angular.isString(filterOn)) {
                    return item[filterOn];
                } else {
                    return item;
                }
            };

            angular.forEach(items, function (item) {
                var valueToCheck, isDuplicate = false;

                for (var i = 0; i < newItems.length; i++) {
                    if (angular.equals(extractValueToCompare(newItems[i]), extractValueToCompare(item))) {
                        isDuplicate = true;
                        break;
                    }
                }
                if (!isDuplicate) {
                    newItems.push(item);
                }

            });
            items = newItems;
        }
        return items;
    };
});


//paging filter
app.filter('range', function () {

    return function (input, total, currentPage) {
        total = parseInt(total);
        if (total > 0) {
            currentPage = parseInt(currentPage);
            var begin = currentPage - 1;
            var end = begin + 10;

            if (end >= total) {
                begin = total < 10 ? 0 : total - 10;
                end = total;
            }
            for (var i = 1; i <= total; i++) {
                input.push(i.toString());
            }
            if (end != total) {
                input[end - 1] = input[end - 1] + ' ...';
            }
            var aa = input.slice(begin, end);
            return input.slice(begin, end);
        }
        return input;

    };


});




//Services to be run on App Start
app.run(['accountService', '$rootScope', '$location', 'notification', function (accountService, $rootScope, $location, notification) {
    accountService.fillAuthData();
    $rootScope.$on('$routeChangeStart', function ($root, next, current) {
        if (next != undefined && next.authenticate && !accountService.isAuthenticated.isAuth) {
            $rootScope.returnToState = next.originalPath;
            $rootScope.returnToStateParams = next.params;
            $location.path('/login');
        }
        if (next == undefined) {
            $location.path('/');
        }

    });
    $rootScope.$on("$locationChangeSuccess", function () {
        //window.scroll(0, 0);
        window.setTimeout(function () {
            $('html,body').animate({
                scrollTop: 0
            }, 200);
        }, 800);
    });
}]);
