app.factory('userService', ["$http", "$q", "$upload", function ($http, $q, $upload) {
    var userServiceFactory = {};
    //var accesstoken = localStorage.getItem('accessToken');
    //var authHeaders = {};
    //if (accesstoken) {
    //    authHeaders.Authorization = 'Bearer ' + accesstoken;
    //}

    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        angular.forEach(data, function (value, key) {
            dataAsFormData.append(key, value);
        });
        return dataAsFormData;
    };

    var _getprofile = function () {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetUserProfile",
            method: "GET",
           // headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _updateProfile = function (model, $file) {
        if (!$file) {
            var deferred = $q.defer();
            var response = $http({
                url: "/api/User/UpdateUserProfile",
                method: "Post",
                //headers: authHeaders,
                data: model
            }).success(function (response) {
                deferred.resolve(response);
            }).error(function (err, status) {
                deferred.reject(err);
            });
            return deferred.promise;
        }
        else {
            var deferred = $q.defer();
            //$files: an array of files selected, each file has name, size, and type.
            $upload.upload({
                url: "/api/User/UpdateUserProfileAndImage", // webapi url
                method: "POST",
                data: { fileUploadObj: model },
                file: $file,
                //headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
            }).success(function (result) {
                deferred.resolve(result);
            }).error(function (result, status) {
                deferred.reject(status);
            });
            return deferred.promise;
        }

    }

    var _uploadprofileimage = function ($files) {

        var deferred = $q.defer();
        //$files: an array of files selected, each file has name, size, and type.
        $upload.upload({
            url: "/api/User/UploadProfileImage", // webapi url
            method: "POST",
            //data: { fileUploadObj: data },
            file: $files,
            //headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _changePassword = function (oldPassword, newPassword) {

        //var accesstoken = localStorage.getItem('accessToken');
        //var authHeaders = {};
        //if (accesstoken) {
        //    authHeaders.Authorization = 'Bearer ' + accesstoken;
        //}

        var deferred = $q.defer();

        $http({
            url: "/api/User/ChangePassword/" + oldPassword + "/" + newPassword,
            method: "POST",
            //data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    userServiceFactory.uploadprofileimage = _uploadprofileimage;
    userServiceFactory.getprofile = _getprofile;
    userServiceFactory.updateprofile = _updateProfile;
    userServiceFactory.changePassword = _changePassword;
    return userServiceFactory;
}]);