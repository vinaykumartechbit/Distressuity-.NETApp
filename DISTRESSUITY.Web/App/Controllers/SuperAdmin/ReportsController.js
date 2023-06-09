app.controller('ReportsController', ['$scope', '$location', 'superAdminService', 'commonService', 'notification', 'messageService', 'loader',
function ($scope, $location,superAdminService,commonService, notification, messageService, loader) {
    $scope.FundedProjectsList = {};
    $scope.NoProjectFound = false;
    $scope.ProjectCount;
    //$scope.PageNumber = 1;
    $scope.CurrentPage = 1;
    $scope.PageSize = 10;
    $scope.$root.IsSearch = true;

    if (localStorage.getItem('userRole').toLowerCase() != "admin") {
        $location.path('/');
    }
    else {
        getFundedProjectList = function () {
            superAdminService.getFundedProjectList($scope.CurrentPage, $scope.PageSize).then(function (response) {
                $scope.TotalPages = parseInt(response.ProjectCount / $scope.PageSize);
                if (response.ProjectCount % $scope.PageSize != 0) {
                    $scope.TotalPages = $scope.TotalPages + 1;
                }

                $scope.FundedProjectsList = response.result;
                $scope.ProjectCount = response.ProjectCount;

                if ($scope.FundedProjectsList.length == 0)
                    $scope.NoProjectFound = true;
                else
                    $scope.NoProjectFound = false;
            })
        }

        getFundedProjectList();


        $scope.NextPage = function () {
            if ($scope.CurrentPage < $scope.TotalPages) {
                //$scope.PageNumber = $scope.PageNumber ;
                $scope.CurrentPage = $scope.CurrentPage + 1;
                getFundedProjectList();
            }
        }
        $scope.PrevPage = function () {
            if ($scope.CurrentPage > 1) {
                //$scope.PageNumber = $scope.PageNumber - 1;
                $scope.CurrentPage = $scope.CurrentPage - 1;
                getFundedProjectList();
            }
        }
        $scope.SetPage = function (pageNumber) {
            if (parseInt(pageNumber) > $scope.TotalPages) {
                return;
            }
            if ($scope.CurrentPage != parseInt(pageNumber) && parseInt(pageNumber) > 0) {
                // $scope.PageNumber = parseInt(pageNumber);
                $scope.CurrentPage = parseInt(pageNumber);
                getFundedProjectList();
            }
        }
        $scope.SetPageSize = function (PageSize) {
            $scope.PageSize = PageSize;
            getFundedProjectList();
        }
       
  
        $scope.openTransferPaymentPopup = function (project) {
            $scope.Project = project;
            $scope.Project.ProcessingFees = project.TotalFundingAmount != 0 ? commonService.roundToTwoDecimal(((2.9 * parseFloat(project.TotalFundingAmount - project.SiteCommission)) / 100) + 0.30) : 0;//project.TotalFundingAmount != 0 ? commonService.roundToTwoDecimal( ((0.3 + parseFloat(project.TotalFundingAmount - project.SiteCommission)) / (1 - 0.029)) - parseFloat(project.TotalFundingAmount - project.SiteCommission)) : 0;
            $scope.Project.FinalAmount = commonService.roundToTwoDecimal(parseFloat(project.TotalFundingAmount - project.SiteCommission - project.ProcessingFees));
            
            $('#transferPaymentModal').modal('show');
        }
        $scope.closeTransferPaymentPopup = function () {
            $('#transferPaymentModal').modal('hide');
        }

        $scope.TransferAmount = function (PaypalAccount,projectId, $event) {
            loader.show($event.currentTarget);
            var FinalAmount = commonService.roundToTwoDecimal(parseFloat($scope.Project.TotalFundingAmount - $scope.Project.SiteCommission));
            
            superAdminService.TransferAmount(PaypalAccount, FinalAmount, projectId).then(function (response) {
                
                if(response.payKey!=null && response.paymentExecStatus == "COMPLETED")
                {
                    notification.success("Amount transfered successfully");
                    $scope.Project.IsTransfered = true;
                }
                else
                {
                    notification.error("Amount transfered failed");
                }
                loader.hide();              
                $('#transferPaymentModal').modal('hide');
            })

        }       
    }
}])


app.controller('TotalEarningReportController', ['$scope', '$location', 'FilteredFundedProjectList', 'superAdminService', 'commonService', 'loader',
function ($scope, $location, FilteredFundedProjectList, superAdminService, commonService, loader) {
    
    $scope.NoProjectFound = false;
    var date = new Date();
    $scope.StartDate = commonService.formatOnlyDate(new Date(date.getFullYear(), date.getMonth(), 1));
    $scope.EndDate = commonService.formatOnlyDate(new Date(date.getFullYear(), date.getMonth() + 1, 0));
  
    $scope.$root.IsSearch = true;

    if (localStorage.getItem('userRole').toLowerCase() != "admin") {
        $location.path('/');
    }
    else {  
       
        if (FilteredFundedProjectList != undefined && FilteredFundedProjectList != null) {
            $scope.TotalEarning = 0;
            $scope.FilteredFundedProjectList = FilteredFundedProjectList.data.result;
            angular.forEach($scope.FilteredFundedProjectList, function (value, index) {
                $scope.TotalEarning = commonService.roundToTwoDecimal($scope.TotalEarning + value.SiteCommission);
            });
            if ($scope.FilteredFundedProjectList.length == 0)
                $scope.NoProjectFound = true;
            else
                $scope.NoProjectFound = false;
        }

        $scope.GetFilteredFundedProjectList = function () {
            $scope.TotalEarning = 0;
            superAdminService.getFilteredFundedProjectList($scope.StartDate, $scope.EndDate).then(function (response) {

                $scope.FilteredFundedProjectList = response.data.result;
                angular.forEach($scope.FilteredFundedProjectList, function (value, index) {
                    $scope.TotalEarning = commonService.roundToTwoDecimal($scope.TotalEarning + value.SiteCommission);
                });
                if ($scope.FilteredFundedProjectList.length == 0)
                    $scope.NoProjectFound = true;
                else
                    $scope.NoProjectFound = false;
            })
        }
    }

    $scope.fundingDate = function (date) {
        return commonService.formatDate(date);
    }
}])