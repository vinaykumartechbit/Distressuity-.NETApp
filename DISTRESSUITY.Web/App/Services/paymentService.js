app.factory('paymentService', ["$http", "$q", function ($http, $q) {
    var paymentServiceFactory = {};


    var _addpaymentinfo = function(data)
    {
        //var accesstoken = localStorage.getItem('accessToken');
        //var authHeaders = {};
        //if (accesstoken) {
        //    authHeaders.Authorization = 'Bearer ' + accesstoken;
        //}
        debugger
        var deferred = $q.defer();
        $http({
            url: "/api/User/AddPaymentInfo",
            method: "POST",
            data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            debugger
            deferred.resolve(result);
        }).error(function (result, status) {
            debugger
            deferred.reject(status);
        });
        return deferred.promise;
    }

    var _getmypayments = function (PageNumber, PageSize) {

        var deferred = $q.defer();
        $http({
            url: "/api/User/GetMyPayments?PageNumber=" + PageNumber + "&PageSize=" + PageSize,
            method: "GET"
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    paymentServiceFactory.addpaymentinfo = _addpaymentinfo;
    paymentServiceFactory.getmypayments = _getmypayments;
    return paymentServiceFactory;
}]);