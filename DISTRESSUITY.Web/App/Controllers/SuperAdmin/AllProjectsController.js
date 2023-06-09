app.controller('AllProjectsController', ['$scope', '$location', 'commonService', 'superAdminService', 'notification', 'messageService', '$rootScope', 'loader',
function ($scope, $location, commonService, superAdminService, notification, messageService, $rootScope, loader) {
    $scope.allProjects = {};
    $scope.statusList = {}
    $scope.selectedStatus = '';
    $scope.NoProjectFound = false;
    $scope.SearchProject = "";
    $scope.$root.IsSearch = false;
    $scope.allProjectCount;
    //$scope.PageNumber = 1;
    $scope.CurrentPage = 1;
    $scope.PageSize = 10;

    if (localStorage.getItem('userRole').toLowerCase() != "admin") {
        $location.path('/');
    }
    else {
        getAllProjects = function () {
            superAdminService.getAllProjetcs($scope.CurrentPage, $scope.PageSize).then(function (response) {
                $scope.TotalPages = parseInt(response.AllProjectCount / $scope.PageSize);
                if (response.PaymentCount % $scope.PageSize != 0) {
                    $scope.TotalPages = $scope.TotalPages + 1;
                }
                $scope.allProjects = response.result;
                $scope.allProjectCount = response.AllProjectCount;
                $scope.statusList = response.statusList;
                if ($scope.allProjects.length == 0)
                    $scope.NoProjectFound = true;
                else
                    $scope.NoProjectFound = false;
            })
        }

        getAllProjects();


        $scope.NextPage = function () {
            if ($scope.CurrentPage < $scope.TotalPages) {
                //$scope.PageNumber = $scope.PageNumber ;
                $scope.CurrentPage = $scope.CurrentPage + 1;
                getAllProjects();
            }
        }
        $scope.PrevPage = function () {
            if ($scope.CurrentPage > 1) {
                //$scope.PageNumber = $scope.PageNumber - 1;
                $scope.CurrentPage = $scope.CurrentPage-1;
                getAllProjects();
            }
        }
        $scope.SetPage = function (pageNumber) {
            if (parseInt(pageNumber) > $scope.TotalPages) {
                return;
            }
            if ($scope.CurrentPage != parseInt(pageNumber) && parseInt(pageNumber)>0) {
               // $scope.PageNumber = parseInt(pageNumber);
                $scope.CurrentPage = parseInt(pageNumber);
                getAllProjects();
            }
        }
        $scope.SetPageSize = function (PageSize) {
            $scope.PageSize = PageSize;
            getAllProjects();
        }




        $rootScope.$on("ChangeAllProjectList", function (event, args) {
            $scope.allProjects = args.data.result;
            $scope.selectedStatus = '';
            if ($scope.allProjects.length == 0)
                $scope.NoProjectFound = true;
            else
                $scope.NoProjectFound = false;
        });

        $scope.UpdateProjectStatus = function (value, projectId, $index, userEmail) {

            $scope.PrivateMessageData = {};
            $scope.PrivateMessageData.ProjectID = projectId;
            $scope.PrivateMessageData.Message = "Congratulations Your project is accepted by Admin";


            messageService.addConversation($scope.PrivateMessageData).then(function (response) {
                if (response.Success)
                    notification.success(response.Message);
                else
                    notification.error(response.Message);
                $scope.$root.chat.server.send("abc", userEmail, response.Data);
                updateProjectStatus(value, projectId, $index);
            })
            //updateProjectStatus(value, projectId, $index);
        }

        function updateProjectStatus(value, projectId, $index) {
            superAdminService.UpdateProjectStatus(value, projectId).then(function (response) {
                if (response.Success) {
                    notification.success(response.Message);
                    var project = $scope.allProjects[$index];
                    project.Status.Name = response.Data.Name;
                    project.StatusId = response.Data.StatusId;
                }
                else {
                    notification.error(response.Message);
                }
            })
        }

        $scope.openMessageWindow = function (value, projectId, $index, userEmail) {
            bootbox.dialog({
                message: "Reason For Disapprove : <br/><br/>" +
                         " <div class='form-group'><textarea  class='form-control newPrivateMessage' type='text' placeholder='Your Message' wrap='hard' required /></div>",
                title: "Add Message",
                buttons: {
                    success: {
                        label: "Submit",
                        className: "btn-success",
                        callback: function () {
                            if ($('.newPrivateMessage').val() != '') {
                                $scope.PrivateMessageData = {};
                                $scope.PrivateMessageData.ProjectID = projectId;
                                $scope.PrivateMessageData.Message = $('.newPrivateMessage').val();

                                messageService.addConversation($scope.PrivateMessageData).then(function (response) {
                                    if (response.Success)
                                        notification.success(response.Message);
                                    else
                                        notification.error(response.Message);
                                    $scope.$root.chat.server.send("abc", userEmail, response.Data);
                                    $('.newPrivateMessage').val('');
                                    updateProjectStatus(value, projectId, $index);
                                })
                            }
                            else {
                                $('.newPrivateMessage').css("border-color", "red");
                            }
                        }
                    },
                    danger: {
                        label: "Cancel",
                        className: "btn-danger",
                        callback: function () {
                        }
                    }
                }
            });
        }


        $scope.CloseDecliningMessagePopup = function () {
            $('#declineMessageModal').modal('hide');
        }
        $scope.openMessageWindow = function (value, projectId, $index, userEmail) {
            $('.value').text(value);
            $('.projectId').text(projectId);
            $('.index').text($index);
            $('.userEmail').text(userEmail);
            $('#declineMessageModal').modal();
        }
        $scope.AddDecliningMessageForm = function ($event) {
            loader.show($event.currentTarget);

            $scope.PrivateMessageData = {};
            $scope.PrivateMessageData.ProjectID = $('.projectId').text();
            $scope.PrivateMessageData.Message = $scope.declineMessage;


            messageService.addConversation($scope.PrivateMessageData).then(function (response) {
                if (response.Success)
                    notification.success(response.Message);
                else
                    notification.error(response.Message);
                $scope.$root.chat.server.send("abc", $('.userEmail').text(), response.Data);
                updateProjectStatus($('.value').text(), $('.projectId').text(), $('.index').text());
                loader.hide();
                $('#declineMessageModal').modal('hide');
                //clearing data from modal popup
                $scope.declineMessage = "";
                $('.value').text('');
                $('.projectId').text('');
                $('.index').text('');
                $('.userEmail').text('');
            })
        }


        $scope.searchadminprojects = function ($event) {
            if ($scope.selectedStatus == "" || $scope.selectedStatus == null) {
                $scope.selectedStatus = {
                    StatusId: 0
                };
            }
            //if ($scope.SearchProject == undefined)
            //{
            //    $scope.SearchProject = "";
            //}
            //var wordLength = $scope.SearchProject.replace(/ /g, '').length;
            //if (wordLength > 1 || wordLength < 1) {

            superAdminService.searchadminprojects($scope.SearchProject, $scope.selectedStatus.StatusId).then(function (response) {
                if (response.data != null) {
                    $scope.allProjects = response.data.result;

                    if ($scope.allProjects.length == 0)
                        $scope.NoProjectFound = true;
                    else
                        $scope.NoProjectFound = false;
                }
                // $scope.$emit('ChangeAllProjectList', response);
                commonService.scrollTop();
            },
               function (err) {
                   notification.error(err.Message);
               });

            // }
        }

        //$scope.searchByStatus = function (statusId) {
        //    if ($scope.selectedStatus == null) {
        //        if ($scope.SearchProject != undefined && $scope.SearchProject != "") {
        //            //var statusId = "0";
        //            superAdminService.getAllProjetcsByStatus("", $scope.SearchProject).then(function (response) {
        //                $scope.allProjects = response;
        //                if ($scope.allProjects.length == 0)
        //                    $scope.NoProjectFound = true;
        //                else
        //                    $scope.NoProjectFound = false;
        //            })
        //        }
        //        else {
        //            getAllProjects();
        //        }
        //    }
        //    else {
        //        if ($scope.SearchProject != undefined && $scope.SearchProject != "") {
        //            superAdminService.getAllProjetcsByStatus($scope.selectedStatus.StatusId, $scope.SearchProject).then(function (response) {
        //                $scope.allProjects = response;
        //                if ($scope.allProjects.length == 0)
        //                    $scope.NoProjectFound = true;
        //                else
        //                    $scope.NoProjectFound = false;
        //            })
        //        }
        //        else {
        //            //var searchProject = "0"; //send search Project 
        //            superAdminService.getAllProjetcsByStatus($scope.selectedStatus.StatusId, "").then(function (response) {
        //                $scope.allProjects = response;
        //                if ($scope.allProjects.length == 0)
        //                    $scope.NoProjectFound = true;
        //                else
        //                    $scope.NoProjectFound = false;
        //            })
        //        }
        //    }
        //}
    }
}])
