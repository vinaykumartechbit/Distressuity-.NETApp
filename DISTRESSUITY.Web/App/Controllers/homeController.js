app.controller('homeController', ['$scope', 'commonService', 'projectList', '$rootScope', '$routeParams', 'notification', 'projectService', 'loader',
function ($scope, commonService, projectList, $rootScope, $routeParams, notification, projectService, loader) {

    $scope.ProjectList = projectList.result != undefined ? projectList.result : projectList.data.result;
    $scope.FeaturedProjectList = projectList.featuredProjects != undefined ? projectList.featuredProjects : projectList.data.featuredProjects;
    $scope.IndustriesList = projectList.Industries;
    $scope.PageNumber = 2;
    $scope.FeaturedProjectPageNumber = 2;
    $scope.ProjectCount = projectList.projectsCount != undefined ? projectList.projectsCount : projectList.data.projectsCount;
    $scope.FeaturedProjectCount = projectList.featuredProjectsCount != undefined ? projectList.featuredProjectsCount : projectList.data.featuredProjectsCount;
    //$scope.Message = false;
    $scope.FeaturedMessage = false;
    $scope.searchKeyword = $routeParams.searchKeyword;
    $scope.IndustryId = $routeParams.IndustryId;
    $scope.$root.IsSearch = true;


    if ($scope.ProjectList.length < $scope.ProjectCount) {
        $scope.ShowLoadMore = true;
    }
    else {
        $scope.ShowLoadMore = false;
    }

    if ($scope.FeaturedProjectList.length < $scope.FeaturedProjectCount) { //$scope.ProjectCount
        $scope.ShowFeaturedLoadMore = true;
    }
    else {
        $scope.ShowFeaturedLoadMore = false;
    }

    if ($scope.ProjectCount == 0 || $scope.ProjectList == "") {
        $scope.Message = true;
        $scope.NoProject = true;
    }
    else
    {
        $scope.NoProject = false;
    }

    if ($scope.FeaturedProjectCount == 0 || $scope.FeaturedProjectList == "") {
        $scope.FeaturedMessage = true;
        $scope.NoFeaturedProject = true;
    }
    else
    {
        $scope.NoFeaturedProject = false;
    }
    if (($scope.searchKeyword != "" && $scope.searchKeyword != undefined) || ($scope.IndustryId != "" && $scope.IndustryId != undefined)) {
        if ($scope.IndustryId == undefined)
        {
            $scope.IndustryId = "";
        }
        projectService.searchprojects($scope.SearchProject, $scope.IndustryId).then(function (response) {
            $scope.$emit('ChangeProjectList', response);

        },
        function (err) {
            notification.error(err.Message);
        });
    }

    //if ($scope.IndustryId != "" && $scope.IndustryId != undefined) {
    //    projectService.FilterProjectByIndustryId($scope.IndustryId).then(function (response) {
    //        $scope.$emit('ChangeProjectList', response);

    //    },
    //    function (err) {
    //        notification.error(err.Message);
    //    });
    //}



    $rootScope.$on("ChangeProjectList", function (event, args) {
        $scope.PageNumber = 2;
        $scope.FeaturedProjectPageNumber = 2;
        $scope.ProjectList = args.data.result;
        $scope.ProjectCount = args.data.projectsCount;
        $scope.FeaturedProjectList = args.data.featuredProjects;
        $scope.FeaturedProjectCount = args.data.featuredProjectsCount;

        if (args.data.result.length == 0) {
            $scope.Message = true;
            $scope.NoProject = true;
        }
        else {
            $scope.Message = false;
            $scope.NoProject = false;
        }

        if (args.data.featuredProjects.length == 0) {
            $scope.FeaturedMessage = true;
            $scope.NoFeaturedProject = true;
        }
        else {
            $scope.FeaturedMessage = false;
            $scope.NoFeaturedProject = false;
        }

        if ($scope.ProjectList.length < $scope.ProjectCount) {
            $scope.ShowLoadMore = true;
        }
        else {
            $scope.ShowLoadMore = false;
        }

        if ($scope.FeaturedProjectList.length < $scope.FeaturedProjectCount) {
            $scope.ShowFeaturedLoadMore = true;
        }
        else {
            $scope.ShowFeaturedLoadMore = false;
        }
        commonService.scrollToPosition('.projects', 300);
    });
    $scope.filteredProjectList = function (filteredProjectList) {
        $scope.PageNumber = 2;
        $scope.FeaturedProjectPageNumber = 2;
        $scope.ProjectList = filteredProjectList.data.result;
        $scope.ProjectCount = filteredProjectList.data.projectsCount;
        $scope.FeaturedProjectList = filteredProjectList.data.featuredProjects;
        $scope.FeaturedProjectCount = filteredProjectList.data.featuredProjectsCount;

        if (filteredProjectList.data.result.length == 0) {
            $scope.Message = true;
        }
        else {
            $scope.Message = false;
        }

        if (filteredProjectList.data.featuredProjects.length == 0) {
            $scope.FeaturedMessage = true;
        }
        else {
            $scope.FeaturedMessage = false;
        }

        if ($scope.ProjectList.length < $scope.ProjectCount) {
            $scope.ShowLoadMore = true;
        }
        else {
            $scope.ShowLoadMore = false;
        }

        if ($scope.FeaturedProjectList.length < $scope.FeaturedProjectCount) {
            $scope.ShowFeaturedLoadMore = true;
        }
        else {
            $scope.ShowFeaturedLoadMore = false;
        }
    }
    $scope.loadMoreProjects = function ($event) {
        loader.show($event.currentTarget);
        $scope.PageNumber += 1;
        projectService.loadmoreprojects($scope.PageNumber, $scope.SearchProject).then(function (response) {
            loader.hide();
            if (response.data.length > 0) {
                for (var i = 0 ; i < response.data.length; i++) {
                    $scope.ProjectList.push(response.data[i]);
                }

                if ($scope.ProjectList.length < $scope.ProjectCount) {
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

    $scope.loadMoreFeaturedProjects = function ($event) {
        loader.show($event.currentTarget);
        $scope.FeaturedProjectPageNumber += 1;
        projectService.loadmorefeaturedprojects($scope.FeaturedProjectPageNumber, $scope.SearchProject).then(function (response) {
            loader.hide();
            if (response.data.length > 0) {
                for (var i = 0 ; i < response.data.length; i++) {
                    $scope.FeaturedProjectList.push(response.data[i]);
                }

                if ($scope.FeaturedProjectList.length < $scope.FeaturedProjectCount) {
                    $scope.ShowFeaturedLoadMore = true;
                }
                else {
                    $scope.ShowFeaturedLoadMore = false;
                }
            }
        },
       function (err) {
           loader.hide();
           $scope.FeaturedProjectPageNumber -= 1;
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

    $scope.deductFeaturedClick = function (projectId) {
        projectService.deductFeaturedClick(projectId).then(function (response) {
            if (response.data.length > 0) {
            }
        },
       function (err) {
           notification.error(err.Message);
       });
    }
}]);