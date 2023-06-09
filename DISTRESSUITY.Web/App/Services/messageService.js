app.factory('messageService', ['$http', '$q', function ($http, $q) {
    var messageService = {}
    //var accesstoken = localStorage.getItem('accessToken');
    //var authHeaders = {};
    //if (accesstoken) {
    //    authHeaders.Authorization = 'Bearer ' + accesstoken;
    //}


    //Private Conversation
    var _getNewMessagesCount = function () {
        var deferred = $q.defer();

        $http.get('api/Conversation/GetNewMessagesCount').then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _getConversation = function () {
        var deferred = $q.defer();

        $http.get('api/Conversation/GetConversations').then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _changeConversation = function (conversationId) {
        var deferred = $q.defer();

        $http.get('api/Conversation/ChangeConversation/' + conversationId).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    //In this case we take Logged in user as 'Creating' user and project owner as 'To' User in convesation
    var _addConversation = function (data) {
        var deferred = $q.defer();

        $http({
            url: "/api/Conversation/AddConversation",
            method: "POST",
            data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });

        return deferred.promise;
    }

    //In this case we take Logged in user is Project owner itself which is used  as 'Creating' user and we need to send 'To' User(other user) for convesation
    var _addConversationWithSpecificUser = function (projectId, userId, reply) {
        var deferred = $q.defer();

        $http({
            url: "/api/Conversation/AddConversationWithSpecificUser/" + projectId + "/" + userId + "/" + reply,
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

    _addConversationReply = function (data)
    {
        var deferred = $q.defer();

        $http({
            url: "/api/Conversation/AddConversationReply",
            method: "POST",
            data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });

        return deferred.promise;
    }


    var _deleteConversation = function (message) {
        var deferred = $q.defer();

        $http({
            url: '/api/Conversation/DeleteConversation',
            method: 'POST',
            data: message,
            //header: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function () {
            deferred.reject(result);
        })

        return deferred.promise;
    }

    var _getConversationsByProjectId = function (projectId)
    {
        var deferred = $q.defer();

        $http.get('api/Conversation/GetConversationsByProjectId/' + projectId).then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }

    //Private Conversation Ends
    //Public Message

    var _getPublicMessage = function () {
        var deferred = $q.defer();

        $http.get('api/Conversation/GetPublicMessage').then(function (result) {
            deferred.resolve(result);
        }, function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    }
    var _addPublicMessage = function (data) {
        var deferred = $q.defer();

        $http({
            url: "/api/Conversation/AddPublicMessage",
            method: "POST",
            data: data,
            //headers: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (response, status) {
            deferred.reject(status);
        });

        return deferred.promise;
    }
    var _deletePublicMessage = function (message) {
        var deferred = $q.defer();

        $http({
            url: '/api/Conversation/DeletePublicMessage',
            method: 'POST',
            data: message,
            //header: { 'Authorization': 'Bearer ' + accesstoken }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function () {
            deferred.reject(result);
        })

        return deferred.promise;
    }

    //Public Message Ends

    messageService.getNewMessagesCount = _getNewMessagesCount;
    messageService.getConversation = _getConversation;
    messageService.changeConversation = _changeConversation;
    messageService.addConversation = _addConversation;
    messageService.addConversationWithSpecificUser = _addConversationWithSpecificUser;
    messageService.addConversationReply = _addConversationReply;
    messageService.deleteConversation = _deleteConversation;
    messageService.getConversationsByProjectId = _getConversationsByProjectId;

    messageService.getPublicMessage = _getPublicMessage;
    messageService.addPublicMessage = _addPublicMessage;
    messageService.deletePublicMessage = _deletePublicMessage;

    return messageService;
}])