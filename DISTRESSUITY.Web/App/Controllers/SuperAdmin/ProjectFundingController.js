app.controller('ProjectFundingController', ['$scope', '$routeParams','superAdminService', 'commonService', 'notification', 'loader',
    function ($scope, $routeParams,superAdminService, commonService, notification, loader) {
        $scope.ProjectFunding = [];
        $scope.NoRecordFound = false;
        $scope.ProjectFundingCount;
        //$scope.PageNumber = 1;
        $scope.CurrentPage = 1;
        $scope.PageSize = 10;
        $scope.TotalFundingAmount = 0;
        $scope.TransactionCharges = 0;
        var ProjectId = $routeParams.projectId;
        $scope.$root.IsSearch = true;

        GetProjectFundings();
        function GetProjectFundings() {

            superAdminService.getProjectFundings(ProjectId, $scope.CurrentPage, $scope.PageSize).then(function (response) {
                $scope.TotalPages = parseInt(response.ProjectFundingCount / $scope.PageSize);
                if (response.ProjectFundingCount % $scope.PageSize != 0) {
                    $scope.TotalPages = $scope.TotalPages + 1;
                }
                $scope.ProjectFunding = response.result;

                angular.forEach($scope.ProjectFunding, function (value, index) {
                    if(value.StatusId==7)
                    {
                        $scope.TotalFundingAmount += value.FundingAmount;
                        $scope.TransactionCharges += value.ProcessingFees;
                    }

                })
                $scope.PaymentReceived = $scope.TotalFundingAmount + $scope.TransactionCharges;
                //$scope.SiteCommission = commonService.roundToTwoDecimal($scope.TotalFundingAmount * 0.02);
                //$scope.TransactionCharges = $scope.TotalFundingAmount !=0?commonService.roundToTwoDecimal(((2.9 * parseFloat($scope.TotalFundingAmount - $scope.SiteCommission)) / 100) + 0.30):0;
                //$scope.PaymentReceived = commonService.roundToTwoDecimal(parseFloat($scope.TotalFundingAmount) - $scope.SiteCommission - $scope.TransactionCharges);

                // $scope.PageNumber = 2;
                $scope.ProjectFundingCount = response.ProjectFundingCount;
                if ($scope.ProjectFunding.length == 0)
                    $scope.NoRecordFound = true;
                else
                    $scope.NoRecordFound = false;
            }, function (err, status) {
                notification.error(err.Message);
            });
        }

        $scope.fundingDate = function (date) {
            return commonService.formatDate(date);
        }

        $scope.NextPage = function () {
            if ($scope.CurrentPage < $scope.TotalPages) {
                //$scope.PageNumber = $scope.PageNumber ;
                $scope.CurrentPage = $scope.CurrentPage + 1;
                GetProjectFundings();
            }
        }
        $scope.PrevPage = function () {
            if ($scope.CurrentPage > 1) {
                //$scope.PageNumber = $scope.PageNumber - 1;
                $scope.CurrentPage = $scope.CurrentPage - 1;
                GetProjectFundings();
            }
        }
        $scope.SetPage = function (pageNumber) {
            if (parseInt(pageNumber) > $scope.TotalPages) {
                return;
            }
            if ($scope.CurrentPage != parseInt(pageNumber) && parseInt(pageNumber) > 0) {
                // $scope.PageNumber = parseInt(pageNumber);
                $scope.CurrentPage = parseInt(pageNumber);
                GetProjectFundings();
            }
        }
        $scope.SetPageSize = function (PageSize) {
            $scope.PageSize = PageSize;
            GetProjectFundings();
        }
    }]);