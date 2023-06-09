'use strict';
app.controller('mainController', ['$scope', '$route', 'accountService', '$location', '$rootScope', 'projectService', 'commonService', 'notification', 'messageService',
function ($scope, $route, accountService, $location, $rootScope, projectService, commonService, notification, messageService) {


    $scope.User = {};
    $scope.SelectedIndustry = "Choose Industry";
    $scope.isAuthenticated = accountService.isAuthenticated;
    $scope.$root.userRole = localStorage.getItem('userRole');
    $scope.$root.chat = $.connection.chatHub;
    $scope.userName = localStorage.getItem('userName');
    $scope.$root.IsSearch = true;
    $scope.SearchProject = "";
 var id="";
   
    GetIndustries();
    $scope.$root.GetNewMessages = function () {
        messageService.getNewMessagesCount().then(function (response) {
            $scope.$root.NewMessagesCount = response.data.Data;
        },
          function (err) {
              notification.error(err.Message);
          });
    };
    $scope.$root.GetNewMessages();
    if ($scope.isAuthenticated.isAuth == true)
        GetUserProfile();

    function GetUserProfile() {
        accountService.getuser().then(function (response) {
            $scope.User = response;
        },
        function (err) {
            notification.error(err.Message);
        });

    }
    function GetIndustries() {
        projectService.getindustries().then(function (response) {
            $scope.LayoutIndustries = response;
        },
       function (err) {
           notification.error(err.Message);
       });
    }

    //GetNewMessages = function () {
    //    messageService.getNewMessagesCount().then(function (response) {
    //        $scope.$root.NewMessagesCount = response.data.Data;
    //    },
    //   function (err) {
    //       notification.error(err.Message);
    //   });
    //}

    $rootScope.$on("CallMainControllerGetUserProfileMethod", function () {
        GetUserProfile();
        $scope.$root.userRole = localStorage.getItem('userRole');
        $scope.$root.GetNewMessages();
    });

    $scope.logOut = function () {
        accountService.logOut();
        $location.path('/');
        $scope.$root.userRole = "";
        $route.reload();        
        $scope.$root.chat.server.disconnected();
    }

    
   
    $scope.filterProjectByIndustryId = function (data) {
        if (data == 0) {
            id = ""
            $scope.SelectedIndustry = "Choose Industry";
        }
        else {
            id = data.IndustryId;
            $scope.SelectedIndustry = data.Name;
        }
        $scope.searchProjects();
        //var path = $location.path();
        //var LastIndex = path.lastIndexOf('/');
        //if (LastIndex != -1 && LastIndex != 0) {
        //    path = path.slice(0, LastIndex);
        //}
        //if (path == '/') {
        //    projectService.FilterProjectByIndustryId(id).then(function (response) {
        //        if (response.data != null) {
        //            $scope.$emit('ChangeProjectList', response);
        //        }
        //    })
        //}
        //else
        //{
        //    $location.path("///" + id)
        //}     
    }


    $scope.searchProjects = function () {
        if (id == undefined)
        {
            id = "";
        }
        if ($scope.SearchProject != "") {
            var wordLength = $scope.SearchProject.replace(/ /g, '').length;           
        }
       
        if (wordLength!=1) {
            var path=$location.path();
            var LastIndex = path.indexOf("/", path.indexOf("/") + 1);//path.lastIndexOf('/');
            if (LastIndex != -1 && LastIndex != 0) {
                path = path.slice(0, LastIndex);
            }
            if (path == '/myprojects') {
                $location.path("/myprojects/"+ $scope.SearchProject+"/"+id)
            //    projectService.searchmyprojects($scope.SearchProject).then(function (response) {
            //        if (response.data != null)
            //            $scope.$emit('ChangeMyProjectList', response);
            //    },
               //function (err) {
               //    notification.error(err.Message);
               //});
            }
            else if (path == '/')
            {
                projectService.searchprojects($scope.SearchProject,id).then(function (response) {
                    if (response.data != null) {
                        $scope.$emit('ChangeProjectList', response);
                    }
                })
            }
            else {
                $location.path("//" + $scope.SearchProject + "/" + id);
            }
           
            //    projectService.searchprojects($scope.SearchProject).then(function (response) {
            //        var path = $location.path();
            //        if (path != "/") {
            //            $location.path("/"); if (response.data != null) {
            //                setTimeout(function () {
            //                    $scope.$emit('ChangeProjectList', response);
            //                    // commonService.scrollToPosition('.projects', 300);
            //                }, 3500)
            //            }

            //        }
            //        else {
            //            $scope.$emit('ChangeProjectList', response);
            //            //commonService.scrollToPosition('.projects', 300);
            //        }

            //    },
            //   function (err) {
            //       notification.error(err.Message);
            //   });
            //}
        }
    }


    $scope.clearSearch = function ($event) {
        var path = $location.path();
        var LastIndex = path.indexOf("/", path.indexOf("/") + 1);//path.lastIndexOf('/');
        if (LastIndex != -1 && LastIndex != 0)
        {
            path = path.slice(0, LastIndex);
        }
        if (path == '/') {
            $location.path('/');
            $scope.SearchProject = "";
            projectService.searchprojects($scope.SearchProject, id).then(function (response) {
                if (response.data != null) {
                    $scope.$emit('ChangeProjectList', response);
                }
            })
        }
        //else {
        //        var LastIndex = path.lastIndexOf('/');
        //    if (LastIndex != -1 && LastIndex != 0)
        //    {
        //        path = path.slice(0, LastIndex);
        //    }
        //        $location.path(path);
        //    }
        //}
    
    }
    //Applying SingalR for global use
    $.connection.hub.qs = { "userEmail": $scope.userName };
    $.connection.hub.start().done(function () {
        console.log('Now connected, connection ID=' + $.connection.hub.id);
        $scope.$root.chat.server.runMe($scope.userName);
    }).fail(function () {
        console.log('Could not Connect!');
    });
    $.connection.chatHub.client.connected = function (id, date) {
        console.log("connected: " + id + " : " + date + "</br>");
    };
    $.connection.chatHub.client.disconnected = function (id, date) {
        console.log("connected: " + id + " : " + date + "</br>");
    };

    $scope.$root.chat.client.broadcastMessage = function (connectionId, message) {
        $scope.$apply(function () {
            var path = $location.path();
            if (path == "/messages") {
                $scope.$root.UpdateMessage(connectionId, message);
            }
            else {
                $.each(connectionId, function () {
                    if ($.connection.hub.id == this) {
                        $scope.$root.NewMessagesCount += 1;
                    }
                })
            }
        });
    };
    if ($location.path() == "") {
        $location.path('/');
    }
}]);