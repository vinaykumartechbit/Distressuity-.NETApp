app.factory('superAdminService', ["$http", '$q', function ($http, $q) {
    var superAdminService = {};
   

    //var accesstoken = localStorage.getItem('accessToken');
    //var authHeaders = {};
    //if (accesstoken) {
    //    authHeaders.Authorization = 'Bearer ' + accesstoken;
    //}

    _getAllProjetcs = function (PageNumber, PageSize) {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetAllProject?PageNumber=" + PageNumber + "&PageSize=" + PageSize,
            method: "Get",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    _getFundedProjectList = function (PageNumber,PageSize) {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetFundedProject?PageNumber=" + PageNumber + "&PageSize=" + PageSize,
            method: "Get",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    _getAllProjetcsByStatus = function (statusId, searchKeyword ) {
        var deferred = $q.defer();
        var response = $http({
            //url: "/api/User/GetAllProjetcsByStatus/" + statusId + "/" + searchKeyword,
            url: "/api/User/GetAllProjetcsByStatus?statusId=" + statusId + "&searchKeyword=" + searchKeyword,
            method: "Get",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    _UpdateProjectStatus = function (Status, projectId) {
        var deferred = $q.defer();

        $http({
            url: '/api/User/UpdateProjectStatus/' + projectId + '/' + Status,
            method: "POST",
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    _TransferAmount = function (PaypalAccount, FinalAmount, projectId) {
        var deferred = $q.defer();
        var response = $http({
            //url: "/api/User/GetAllProjetcsByStatus/" + statusId + "/" + searchKeyword,
            url: "/api/User/TransferAmount?PaypalAccount=" + PaypalAccount + "&Amount=" + FinalAmount + "&projectId=" + projectId,
            method: "POST",
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

    _getProjectFundings = function (ProjectId,PageNumber, PageSize) {
        var deferred = $q.defer();
        var response = $http({
            url: "/api/User/GetProjectFundings/?ProjectId=" + ProjectId + "&PageNumber=" + PageNumber + "&PageSize=" + PageSize,
            //url:"/api/User/GetProjectFundings",
            method: "GET",
            //data: { ProjectId:ProjectId,PageNumber: PageNumber, PageSize: PageSize }
            //headers: authHeaders
        }).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }
    _searchadminprojects = function (name, statusId) {
        var deferred = $q.defer();
        $http.get('/api/User/SearchAdminProjects?name=' + name + '&statusId=' + statusId).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    _getFilteredFundedProjectList=function(StartDate,Enddate)
    {
        var deferred = $q.defer();
        $http.get('/api/User/GetFilteredFundedProjectList?StartDate=' + StartDate + '&Enddate=' + Enddate).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    superAdminService.getAllProjetcs = _getAllProjetcs;
    superAdminService.getAllProjetcsByStatus = _getAllProjetcsByStatus;
    superAdminService.UpdateProjectStatus = _UpdateProjectStatus;
    superAdminService.getFundedProjectList = _getFundedProjectList;
    superAdminService.TransferAmount = _TransferAmount;
    superAdminService.getProjectFundings = _getProjectFundings;
    superAdminService.searchadminprojects = _searchadminprojects;
     superAdminService.getFilteredFundedProjectList = _getFilteredFundedProjectList;
    
    return superAdminService;
}])