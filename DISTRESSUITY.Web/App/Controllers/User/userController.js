app.controller('profileController', ['$scope', 'userService', '$filter', 'loader', 'notification', 'myProfile', '$rootScope', 'accountService',
function ($scope, userService, $filter, loader, notification, myProfile, $rootScope, accountService) {
    $scope.profile = {};
    $scope.newUserPosition = {};
    $scope.copyOfSelectedPosition = {};
    $scope.OldPassword = "";
    $scope.NewPassword = "";
    $scope.ConfirmPassword = "";
    $scope.$root.IsSearch = true;
    //GetProfile();

    if (myProfile != null) {
        $scope.profile = myProfile.profile;
        $scope.months = myProfile.months;
        $scope.industries = myProfile.industries;
        if ($scope.newUserPosition.StartMonth != null && $scope.newUserPosition.StartMonth != "") {
            $scope.newUserPosition.StartMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.StartMonth }, true)[0].name;
        }
        if ($scope.newUserPosition.EndMonth != null && $scope.newUserPosition.EndMonth != "") {
            $scope.newUserPosition.EndMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.EndMonth }, true)[0].name;
        }
    }

    function GetProfile() {

        var profileData = userService.getprofile();
        profileData.then(function (resp) {
            $scope.profile = resp.profile;
            $scope.months = resp.months;
            $scope.industries = resp.industries;
            if ($scope.newUserPosition.StartMonth != null && $scope.newUserPosition.StartMonth != "") {
                $scope.newUserPosition.StartMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.StartMonth }, true)[0].name;
            }
            if ($scope.newUserPosition.EndMonth != null && $scope.newUserPosition.EndMonth != "") {
                $scope.newUserPosition.EndMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.EndMonth }, true)[0].name;
            }
        }, function (err, status) {
            //alert(err.Message);
        });
    }

    $scope.UpdateUserProfile = function ($event) {
        loader.show($event.currentTarget);
        var data = $scope.profile;
        var file = $scope.profile.ImageFile;
        userService.updateprofile(data, file).then(function (response) {
            $scope.profile = response.Data;
            $rootScope.$emit("CallMainControllerGetUserProfileMethod", {});
            notification.success(response.Message);
            loader.hide();
        }, function (err) { loader.hide(); });
    }

    $scope.AddUserPosition = function () {
        $scope.newUserPosition.IsDeleted = false;
        if ($scope.newUserPosition.StartMonth != null && $scope.newUserPosition.StartMonth != "") {
            $scope.newUserPosition.StartMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.StartMonth }, true)[0].name;
        }
        if ($scope.newUserPosition.EndMonth != null && $scope.newUserPosition.EndMonth != "") {
            $scope.newUserPosition.EndMonthName = $filter('filter')($scope.months, { id: $scope.newUserPosition.EndMonth }, true)[0].name;
        }

        var currentDate = new Date();
        var currentMonth = currentDate.getMonth();
        var currentYear = currentDate.getFullYear();
        var startYear = parseInt($scope.newUserPosition.StartYear);
        var endYear = parseInt($scope.newUserPosition.EndYear);
        var startMonth = parseInt($scope.newUserPosition.StartMonth) - 1;
        var endMonth = parseInt($scope.newUserPosition.EndMonth) - 1;

        if (startYear < 1900 || startYear > currentYear || endYear < 1900 || endYear > currentYear) {
            alert("Please enter a year between 1900 and current year");
            return;
        }

        var startDate = new Date(startYear, startMonth);
        var endDate = new Date(endYear, endMonth);
        if (startDate > endDate) {
            alert("Start date should be less than End date.");
            return;
        }

        if (startDate > currentDate || endDate > currentDate) {
            alert("Date should be less than current date.");
            return;
        }

        $scope.profile.UserPositions.push($scope.newUserPosition);
        $scope.AddPosition = false;
        $scope.newUserPosition = {};
    }

    $scope.UpdateSelectedPosition = function (data) {
        if (data.StartMonth != null && data.StartMonth != "") {
            data.StartMonthName = $filter('filter')($scope.months, { id: data.StartMonth }, true)[0].name;
        }
        if (data.EndMonth != null && data.EndMonth != "") {
            data.EndMonthName = $filter('filter')($scope.months, { id: data.EndMonth }, true)[0].name;
        }
    }

    $scope.EditPosition = function (data) {
        $scope.copyOfSelectedPosition = angular.copy(data);
    }

    $scope.CancelPositionEdit = function (data) {
        angular.copy($scope.copyOfSelectedPosition, data);
    }

    $scope.CheckIsCurrent = function (EndYear) {
        if (EndYear) {
            $scope.newUserPosition.EndMonth = "";
            $scope.newUserPosition.EndYear = "";
        }
    }

    $scope.EditCheckIsCurrent = function (data) {
        if (data.IsCurrent) {
            data.IsCurrent = false;
        }
        else {
            data.EndYear = "";
            data.EndMonth = "";
            data.IsCurrent = true;
        }

    }

    $scope.DeleteUserPosition = function (data) {
        bootbox.confirm("Are you sure that you want to remove this Position ?", function (result) {
            if (result) {
                data.IsDeleted = true;
            }
        });

    }

    $scope.uploadProfileImage = function ($files) {

        userService.uploadprofileimage($files).then(function (response) {
            $scope.profile.PictureUrl = response.PictureUrl;
        },
        function (Error) {
            //alert(Error)
        });
    }

    $scope.NewPassword = function ($event) {
        loader.show($event.currentTarget);
        var original = $scope.changePassword;
        var oldPassword = $scope.changePassword.OldPassword;
        var newPassword = $scope.changePassword.NewPassword;
        userService.changePassword(oldPassword, newPassword).then(function (response) {
            loader.hide();
            if (response.Success)
                notification.success(response.Message);
            else
                notification.error(response.Message);
            $scope.changePassword = angular.copy(original);
            $scope.ChangePassword.$setPristine();

            $('#changePasswordModal').modal('hide');

        }, function (err) {
            loader.hide();
            notification.error(err);
        });
    }
    $scope.CloseChangePassword = function () {
        $('#changePasswordModal').modal('hide');
    }
}]);