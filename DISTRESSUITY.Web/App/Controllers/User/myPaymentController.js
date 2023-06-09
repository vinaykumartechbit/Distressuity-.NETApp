app.controller('myPaymentController', ['$scope', 'paymentService', 'commonService', 'notification', 'loader',
    function ($scope, paymentService,commonService, notification, loader) {
        $scope.MyPayments = [];
        $scope.PaymentsCount;
        //$scope.PageNumber = 1;
        $scope.CurrentPage = 1;
        $scope.PageSize=10;
        $scope.$root.IsSearch = true;
       // $scope.NoProjectFound = false;

        GetPayments();
        function GetPayments() {

            paymentService.getmypayments($scope.CurrentPage, $scope.PageSize).then(function (response) {
                $scope.TotalPages = parseInt(response.PaymentCount / $scope.PageSize);
                if (response.PaymentCount % $scope.PageSize != 0)
                {
                    $scope.TotalPages = $scope.TotalPages + 1;
                }
                $scope.MyPayments = response.result;
               // $scope.PageNumber = 2;
                $scope.PaymentsCount = response.PaymentCount;
                if ($scope.PaymentsCount == 0)
                    $scope.NoProjectFound = true;
                else
                    $scope.NoProjectFound = false;
               
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
                GetPayments();
            }
        }
        $scope.PrevPage = function () {
            if ($scope.CurrentPage > 1) {
                //$scope.PageNumber = $scope.PageNumber - 1;
                $scope.CurrentPage = $scope.CurrentPage - 1;
                GetPayments();
            }
        }
        $scope.SetPage = function (pageNumber) {
            if (parseInt(pageNumber) > $scope.TotalPages) {
                return;
            }
            if ($scope.CurrentPage != parseInt(pageNumber) && parseInt(pageNumber) > 0) {
                // $scope.PageNumber = parseInt(pageNumber);
                $scope.CurrentPage = parseInt(pageNumber);
                GetPayments();
            }
        }
        $scope.SetPageSize = function (PageSize) {
            $scope.PageSize = PageSize;
            GetPayments();
        }
    }]);