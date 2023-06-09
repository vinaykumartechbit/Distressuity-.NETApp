app.controller('myProjectController', ['$scope', '$routeParams', 'commonService', 'projectService', 'notification', 'loader', '$rootScope',
    function ($scope,$routeParams, commonService, projectService, notification, loader, $rootScope) {
        $scope.MyProjects = {};
        $scope.searchKeyword = $routeParams.searchKeyword;
        $scope.IndustryId = $routeParams.IndustryId;
        $scope.$root.IsSearch = true;

        if ($scope.searchKeyword != "" && $scope.searchKeyword != undefined || $scope.IndustryId != "" && $scope.IndustryId != undefined) {
            if ($scope.IndustryId == undefined) {
                $scope.IndustryId = "";
            }
            projectService.searchmyprojects($scope.SearchProject, $scope.IndustryId).then(function (response) {
                $scope.$emit('ChangeMyProjectList', response);

            },

                   function (err) {
                       notification.error(err.Message);
                   });
        }
        else {
            GetProjects();
        }
        function GetProjects() {

            projectService.getmyprojects().then(function (response) {

                if (response.projectsCount == 0) {
                    $scope.NoProjectAdded = true;
                }
                else {
                    $scope.NoProjectAdded = false;
                }

                $scope.MyProjects = response.result;
                $scope.PageNumber = 2;
                $scope.ProjectCount = response.projectsCount;
                if ($scope.MyProjects.length < $scope.ProjectCount) {
                    $scope.ShowLoadMore = true;
                }
                else {
                    $scope.ShowLoadMore = false;
                }
            }, function (err, status) {
                notification.error(err.Message);
            });
        }

        $scope.deleteProject = function (data) {
            if (data != null) {
                bootbox.confirm("Are you sure that you want to remove this Project ?", function (result) {
                    if (result) {
                        projectService.deleteproject(data.ProjectId).then(function (response) {

                            notification.success(response.data.Message);
                            var index = $scope.MyProjects.indexOf(data);
                            $scope.MyProjects.splice(index, 1);
                        }, function (err) {
                            //alert(err);
                            notification.error(response.Message);
                        });
                    }
                });
            }
        }

        $rootScope.$on("ChangeMyProjectList", function (event, args) {
            debugger;
            $scope.PageNumber = 2;
            $scope.MyProjects = args.data.result;
            $scope.ProjectCount = args.data.projectsCount;
            if (args.data.result.length == 0) {
                $scope.Message = "No Records Found";
            }
            else {
                $scope.Message = "";
            }
            if ($scope.MyProjects.length < $scope.ProjectCount) {
                $scope.ShowLoadMore = true;
            }
            else {
                $scope.ShowLoadMore = false;
            }
        });

        $scope.loadMoreProjects = function ($event) {
            loader.show($event.currentTarget);
            $scope.PageNumber += 1;
            projectService.loadmoremyprojects($scope.PageNumber, $scope.SearchProject).then(function (response) {
                loader.hide();
                if (response.data.length > 0) {
                    for (var i = 0 ; i < response.data.length; i++) {
                        $scope.MyProjects.push(response.data[i]);
                    }

                    if ($scope.MyProjects.length < $scope.ProjectCount) {
                        $scope.ShowLoadMore = true;
                    }
                    else {
                        $scope.ShowLoadMore = false;
                    }
                }
            },
           function (err) {
               loader.hide();
               $scope.PageNumber -= 1;
               notification.error(err.Message);
           });
        }

        $scope.getFundingPercentage = function (data) {
            var totalFundingAmount = 0, investmentAmount = data.InvestmentAmount, totalAmountInPercent = 0;
            for (var i = 0; i < data.ProjectFundings.length; i++) {
                totalFundingAmount += data.ProjectFundings[i].FundingAmount == undefined ? 0 : data.ProjectFundings[i].FundingAmount;
            }
            totalAmountInPercent = totalFundingAmount != 0 ? commonService.roundToTwoDecimal((totalFundingAmount * 100) / investmentAmount) + '%' : '0%';

            $scope.TotalFundingInPercent = totalAmountInPercent;
            return totalAmountInPercent;
        }

    }]);