app.factory('projectService', ["$http", "$q", '$upload', function ($http, $q, $upload) {
    var projectServiceFactory = {};
 
    var files = [];

    function appendFile(data) {
        files.push(data);
    }

    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        angular.forEach(data, function (value, key) {
            dataAsFormData.append(key, value);
        });
        return dataAsFormData;
    };

    var _saveproject = function (Project) {

        var deferred = $q.defer();
        $http({
            url: "/api/User/AddProject",
            method: "POST",
            data: Project,
           // headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _updateproject = function (projectModel) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/UpdateProject",
            method: "POST",
            data: projectModel,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _verifyPaypalAccount = function (projectModel) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/VerifyPaypalAccount",
            method: "POST",
            data: projectModel,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _currentUserPaypalAccounts = function () {
        var deferred = $q.defer();
        $http({
            url: "/api/User/CurrentUserPaypalAccounts",
            method: "Get"
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getindustries = function () {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetIndustries",
            method: "Get",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _getprojecttabenums = function () {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetProjectTabEnums",
            method: "Get",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    var _onfileselect = function ($files, data) {
        var deferred = $q.defer();
        //$files: an array of files selected, each file has name, size, and type.
        $upload.upload({
            url: "/api/User/UploadDocument", // webapi url
            method: "POST",
            data: { ProjectId: data },
            file: $files,
            //headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _addtempimage = function ($files) {
        var deferred = $q.defer();
        //$files: an array of files selected, each file has name, size, and type.
        $upload.upload({
            url: "/api/User/AddTempImage", // webapi url
            method: "POST",
            //data: { fileUploadObj: data },
            file: $files,
           // headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            //$scope.ImageUploadPercentage = parseInt(100.0 * evt.loaded / evt.total) + "%";
            //if ($scope.ImageUploadPercentage == "100%") {
            //    $scope.showImageEncoderDiv = true;
            //    $scope.ImageUploadPercentage = "";
            //}
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _deleteprojectdocument = function (data) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/DeleteProjectDocument",
            method: "POST",
            data: data,
           // headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getmyprojects = function () {
       
        var deferred = $q.defer();
        $http({
            url: "/api/User/GetMyProjects",
            method: "GET",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getallprojects = function () {
        var deferred = $q.defer();
        $http({
            url: "/api/User/GetProjects",
            method: "GET",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getprojectbyid = function (projectId, pageType) {
        var deferred = $q.defer();
        //var accesstoken1 = localStorage.getItem('accessToken');
        //var authHeaders1 = {};
        //if (accesstoken1) {
        //    authHeaders1.Authorization = 'Bearer ' + accesstoken1;
        //}

        var url

        $http.get('/api/User/GetProjectById?id=' + projectId + '&pageType=' + pageType).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _deleteproject = function (projectId) {
        var deferred = $q.defer();
        $http.get('/api/User/DeleteMyProject?id=' + projectId).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;

    }

    var _submitprojectreview = function (projectId) {
        var deferred = $q.defer();
        $http.get('/api/User/SubmitProjectReview?id=' + projectId).then(function (result) {
            deferred.resolve(result.data);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _onvideoselect = function ($file, projectId) {

        $upload.upload({
            url: "./api/User/UploadProjectVideo", // webapi url
            method: "POST",
            data: { fileUploadObj: projectId },
            file: $file,
            //headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            // get upload percentage
            console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        }).success(function (data, status, headers, config) {
            // file is uploaded successfully
            console.log(data);
        }).error(function (data, status, headers, config) {
            // file failed to upload
            console.log(data);
        });
    }

    var _FilterProjectByIndustryId = function (id) {
        var deferred = $q.defer();
        $http.get('/api/User/GetProjectsByIndustry?id=' + id).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {

            deferred.reject(status);
        });
        return deferred.promise;
    }
 
    var _searchprojects = function (name,id) {
        var deferred = $q.defer();
        $http.get('/api/User/SearchProjects?name=' + name + "&Id=" + id).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _searchmyprojects = function (name,id) {
        //var accesstoken1 = localStorage.getItem('accessToken');
        //var authHeaders1 = {};
        //if (accesstoken1) {
        //    authHeaders1.Authorization = 'Bearer ' + accesstoken1;
        //}
        var deferred = $q.defer();
        $http.get('/api/User/SearchMyProjects?name=' + name+"&Id="+id).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _searchadminprojects = function (name) {
        var deferred = $q.defer();
        $http.get('/api/User/SearchAdminProjects?name=' + name).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _loadmoreprojects = function (pageNumber, name) {
        if (name == undefined) {
            name = '';
        }
        var deferred = $q.defer();
        $http.get('/api/User/LoadMoreProjects?pageNumber=' + pageNumber + '&name=' + name).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _loadmorefeaturedprojects = function (pageNumber, name) {
        if (name == undefined) {
            name = '';
        }
        var deferred = $q.defer();
        $http.get('/api/User/LoadMoreFeaturedProjects?pageNumber=' + pageNumber + '&name=' + name).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _loadmoremyprojects = function (pageNumber, name) {
        if (name == undefined) {
            name = '';
        }
        //var accesstoken1 = localStorage.getItem('accessToken');
        //var authHeaders1 = {};
        //if (accesstoken1) {
        //    authHeaders1.Authorization = 'Bearer ' + accesstoken1;
        //}
        var deferred = $q.defer();
        $http.get('/api/User/LoadMoreMyProjects?pageNumber=' + pageNumber + '&name=' + name).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _updateFundingStatus = function (isApproved, projectFundingId) {
        var deferred = $q.defer();

        $http({
            url: '/api/User/UpdateFundingStatus/' + projectFundingId + '/' + isApproved,
            method: "POST"
           // headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getIfLogginInUserCanPay = function (projectId) {
        //var accesstoken1 = localStorage.getItem('accessToken');
        //var authHeaders1 = {};
        //if (accesstoken1) {
        //    authHeaders1.Authorization = 'Bearer ' + accesstoken1;
        //}
        var deferred = $q.defer();
        $http.get('/api/User/GetIfLogginInUserCanPay/' + projectId).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    _addPaymentInfo = function (data) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/AddFeaturedClicksPayment",
            method: "POST",
            data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    };

    _getPayment = function (projectId) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/GetPayment/" + projectId,
            method: "POST",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    _deductFeaturedClick = function (projectId) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/DeductFeaturedClick/" + projectId,
            method: "POST",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    _getClicksLog = function (projectId) {
        var deferred = $q.defer();
        $http({
            url: "/api/User/GetClicksLog/" + projectId,
            method: "GET",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    projectServiceFactory.addtempimage = _addtempimage;
    projectServiceFactory.loadmoreprojects = _loadmoreprojects;
    projectServiceFactory.loadmorefeaturedprojects = _loadmorefeaturedprojects;
    projectServiceFactory.loadmoremyprojects = _loadmoremyprojects;
    projectServiceFactory.FilterProjectByIndustryId = _FilterProjectByIndustryId;
    projectServiceFactory.deleteprojectdocument = _deleteprojectdocument;
    projectServiceFactory.getallprojects = _getallprojects;
    projectServiceFactory.onvideoselect = _onvideoselect;
    projectServiceFactory.deleteproject = _deleteproject;
    projectServiceFactory.getmyprojects = _getmyprojects;
    projectServiceFactory.getprojectbyid = _getprojectbyid;
    projectServiceFactory.onfileselect = _onfileselect;
    projectServiceFactory.saveproject = _saveproject;
    projectServiceFactory.getindustries = _getindustries;
    projectServiceFactory.getprojecttabenums = _getprojecttabenums;
    projectServiceFactory.updateproject = _updateproject;
    projectServiceFactory.submitprojectreview = _submitprojectreview;
    projectServiceFactory.searchprojects = _searchprojects;
    projectServiceFactory.searchmyprojects = _searchmyprojects;
    projectServiceFactory.searchadminprojects = _searchadminprojects;
    projectServiceFactory.updateFundingStatus = _updateFundingStatus;
    projectServiceFactory.getIfLogginInUserCanPay = _getIfLogginInUserCanPay;
    projectServiceFactory.addPaymentInfo = _addPaymentInfo;
    projectServiceFactory.getPayment = _getPayment;
    projectServiceFactory.deductFeaturedClick = _deductFeaturedClick;
    projectServiceFactory.getClicksLog = _getClicksLog;
    projectServiceFactory.verifyPaypalAccount = _verifyPaypalAccount;
    projectServiceFactory.currentUserPaypalAccounts = _currentUserPaypalAccounts;
        

    return projectServiceFactory;
}]);


app.factory('projectDetailService', ["$http", "$q", function ($http, $q) {
    var projectDetailService = {};
    //var accesstoken = localStorage.getItem('accessToken');
    //var authHeaders = {};
    //if (accesstoken) {
    //    authHeaders.Authorization = 'Bearer ' + accesstoken;
    //}

    var _backItFunding = function (id) {
        var deferred = $q.defer();

        $http({
            url: '/api/User/BackItFunding/' + id,
            method: "POST",
           // headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    projectDetailService.backItFunding = _backItFunding;

    return projectDetailService;
}]);