app.controller('paymentController', ['paymentService', 'projectRoute', '$location', '$scope', 'notification', 'loader', 'commonService', 'messageService',
function (paymentService, projectRoute, $location, $scope, notification, loader, commonService, messageService) {

        if (projectRoute == null) {
            $location.path("/");
        }
        $scope.Payment = projectRoute.data.project;
        $scope.Months = projectRoute.data.months;
        $scope.Payment.FundingAmount = $scope.Payment.MinPledge;
        $scope.MinPledge = projectRoute.data.project.MinPledge;
        $scope.InitialPayment = $scope.Payment.FundingAmount;
        $scope.$root.IsSearch = true;
        calculate();

        $scope.calculateFundingAmount = function () {
            calculate();
        }

        function calculate() {            
            $scope.FinalPayment = commonService.roundToTwoDecimal((parseFloat($scope.InitialPayment) + 0.30) / (1 - 0.029));//commonService.roundToTwoDecimal(parseFloat($scope.InitialPayment) + $scope.ProcessingFees);
            $scope.ProcessingFees = commonService.roundToTwoDecimal($scope.FinalPayment - $scope.InitialPayment);//commonService.roundToTwoDecimal(((2.9 * parseFloat($scope.InitialPayment)) / 100) + 0.30);
        }

        $scope.addPaymentInfo = function ($event, paymentData) {

            $scope.PrivateMessageData = {};
            $scope.PrivateMessageData.ProjectID = paymentData.ProjectId;
            $scope.PrivateMessageData.Message = "I have successfully invested on your idea";


            loader.show($event.currentTarget);
            paymentData.EquityDemanded = paymentData.EquityOffered;
            paymentData.FundingAmount = $scope.InitialPayment;
            paymentData.ProcessingFees = $scope.ProcessingFees;
            paymentService.addpaymentinfo(paymentData).then(function (response) {
                if (response.Success) {
                    notification.success('Your payment is scheduled successfully');
                    messageService.addConversation($scope.PrivateMessageData).then(function (response) {
                        if (response.Success)
                            notification.success(response.Message);
                        else
                            notification.error(response.Message);
                        $scope.$root.chat.server.send("abc", $scope.Payment.User.Email, response.Data);
                        $location.path("/projectdetail/" + paymentData.ProjectId);
                    })                    
                }
                else {
                    notification.error(response.Message)
                }
            },
           function (err) {
               notification.error(err.Message);
           });
        }

    }]);