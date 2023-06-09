app.controller('projectController', ['$scope', '$route', 'projectService', 'commonService', "$routeParams", 'loader', 'notification', '$upload',
'myProjectDetails', 'messageService', function ($scope, $route, projectService, commonService, $routeParams, loader, notification, $upload,
myProjectDetails, messageService) {

    var accesstoken = localStorage.getItem('accessToken');
    var authHeaders = {};
    $scope.Months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    $scope.FeaturedPayment = {};
    $scope.FeaturedClickLeft = 0;
    $scope.OpenClicks = false;
    $scope.ShowStatusMessage = false;
    $scope.$root.IsSearch = true;
    $scope.NoPaypalAccount = false;
    $scope.IsUploading = false;


    //$scope.ShowApproveDisapprove = true;
    //bkLib.onDomLoaded(function () { nicEditors.allTextAreas() });

    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    if (myProjectDetails != null) {
        $scope.Project = {};
        $scope.Project = myProjectDetails.data.project;

        if ($scope.Project.AnnualSales == 0) {
            $scope.Project.AnnualSales = '';
        }
        if ($scope.Project.EquityOffered == 0) {
            $scope.Project.EquityOffered = '';
        }
        if ($scope.Project.InvestmentAmount == 0) {
            $scope.Project.InvestmentAmount = '';
        }
        if ($scope.Project.MinPledge == 0) {
            $scope.Project.MinPledge = '';
        }

        if ($scope.Project.FinancialBreakdowns.length == 0) {
            $scope.NoFinancialBreakdowns = true;
        }
        else {
            $scope.NoFinancialBreakdowns = false;
        }


        $scope.Industries = myProjectDetails.data.industries;
        $scope.DefaultExpirationDate = new Date(myProjectDetails.data.project.FundingDuration);
        $.each($scope.Project.FeaturedIdeas, function () {
            $scope.FeaturedClickLeft += this.ClciksLeft;
        })
    }
    else {
        GetIndustries();
        $scope.Project = {
            FinancialBreakdowns: []
        };
        var now = new Date();
        $scope.DefaultExpirationDate = new Date(now.setDate(now.getDate() + 30));
    }

    $(document).ready()
    {
        nicEditors.editors.push(
        new nicEditor().panelInstance(
            document.getElementById('nicEditDescription')
            )
        );
        //nicEditors.allTextAreas();
        $(".nicEdit-main").html($scope.Project.Description);
        //commonService.changeProjectSearchTextbox(true, false); // allProjects, myProjects  resp
    }
    $scope.ProjectDocument = {};

    function GetIndustries() {
        projectService.getindustries().then(function (response) {
            $scope.Industries = response;
        }, function (err, status) {
            //alert(err.Message);
        });
    }

    function GetProjectTabEnums() {
        projectService.getprojecttabenums().then(function (response) {
            $scope.ProjectTabs = response;
        }, function (err, status) {
            //alert(err.Message);
        });
    }

    $scope.addFinancialBreakDown = function () {
        $scope.NoFinancialBreakdowns = false;
        $scope.Project.FinancialBreakdowns.push({ ProjectId: $scope.Project.ProjectId });
    }

    $scope.deleteFinancialBreakDown = function (data) {
        var index = $scope.Project.FinancialBreakdowns.indexOf(data);
        $scope.Project.FinancialBreakdowns.splice(index, 1);
        if (index == 0 && $scope.Project.FinancialBreakdowns.length == 0) {
            $scope.NoFinancialBreakdowns = true;
        }
        else {
            $scope.NoFinancialBreakdowns = false;
        }
    }


    $scope.saveProject = function (data, type, $event, active) {
        if ($scope.IsUploading == false) {
            $scope.Project.Description = $(".nicEdit-main").html();
            loader.show($event.currentTarget);
            data.TabType = type;
            projectService.saveproject(data).then(function (response) {
                //response.ImageFile = $scope.Project.ImageFile;
                //response.VideoFile = $scope.Project.VideoFile;
                $scope.Project.ProjectId = response.ProjectId;
                $scope.Project.ImagePath = response.ImagePath;
                notification.success("Project created Successfully");
                loader.hide();
                if (active != null && active != "") {
                    commonHeaderChanges(active);
                    $(".active").removeClass("active");
                    $("." + active).addClass("active");
                    var body = $("body:first");
                    $(window).scrollTop(body);
                    $("body").focus();
                }
            }, function (err, status) {
                loader.hide();
                //alert(err);
            });
        }
    }

    //Create new project and add projectId into the scope
    //$scope.saveProject = function (data, type, $event, active) {

    //    loader.show($event.currentTarget);
    //    data.TabType = type;
    //    if ($scope.ImageFile)
    //        data.ImageFile = $scope.ImageFile;
    //    if ($scope.VideoFile)
    //        data.VideoFile = $scope.VideoFile;
    //    projectService.saveproject(data).then(function (response) {
    //        response.ImageFile = $scope.Project.ImageFile;
    //        $scope.Project.ProjectId = response.ProjectId;
    //        notification.success("Added Records Successfully");
    //        loader.hide();
    //        if (active != null && active != "") {
    //            $(".active").removeClass("active");
    //            $("." + active).addClass("active");
    //            var body = $("body:first");
    //            $(window).scrollTop(body);
    //            $("body").focus();
    //        }
    //    }, function (err, status) {
    //        loader.hide();
    //        alert(err.Message);
    //    });
    //}

    $scope.updateProject = function (data, type, $event, active) {
        if ($scope.IsUploading == false) {
            loader.show($event.currentTarget);
            data.TabType = type;
            projectService.updateproject(data).then(function (response) {
                if (response.result == "failed") {
                    notification.error(response.Message);
                    $scope.Project.PaypalAccountStatus = false;
                    loader.hide();
                }
                else {
                    $scope.Project = response;
                    notification.success("Project updated Successfully");
                    loader.hide();
                    if (active != null && active != "") {
                        commonHeaderChanges(active);
                        $(".active").removeClass("active");
                        $("." + active).addClass("active");
                        var body = $("body:first");
                        $(window).scrollTop(body);
                        $("body").focus();
                    }
                }
            }, function (err, status) {
                notification.error("Something Went Wrong");
            });
        }
    }
    $scope.verifyPaypalAccount = function (Project, $event) {
        if ($("#PaypalAccount").val() != "" && $scope.IsUploading == false) {
            loader.show($event.currentTarget);
            //if ($scope.Project.PaypalAccountStatus != "VERIFIED") {
            projectService.verifyPaypalAccount(Project).then(function (response) {
                if (response != null) {
                    if (response.result == "failed")
                    {
                        notification.error(response.Message);
                    }
                    else {
  $scope.Project = response;
                    $scope.ShowStatusMessage = true;
                    }
                  
                    loader.hide();
                }
            });
        }
       // }
    }


    $scope.currentUserPaypalAccounts = function () {
        $scope.PaypalAccountList = [];
        projectService.currentUserPaypalAccounts().then(function (response) {
            $('#delete-referral').modal();
            if (response != null) {
                angular.forEach(response, function (value, index) {
                    if (value != null) {
                        $scope.PaypalAccountList.push(value);
                    }
                })
            }
            else
            {
                $scope.NoPaypalAccount = true;
            }
            if($scope.PaypalAccountList.length==0)
            {
                $scope.NoPaypalAccount = true;
            }
        })
    }


    $scope.selectPaypalaccount = function (PaypalAccount) {

        $scope.Project.PaypalAccount = PaypalAccount;
        $scope.Project.PaypalAccountStatus = true;
        $scope.ShowStatusMessage = true;
        $('#delete-referral').modal('hide');
    }


    $scope.deleteProjectDocument = function (data) {

        bootbox.confirm("Are you sure that you want to remove this Document ?", function (result) {
            if (result) {
                projectService.deleteprojectdocument(data).then(function (response) {
                    var index = $scope.Project.ProjectDocuments.indexOf(data);
                    $scope.Project.ProjectDocuments.splice(index, 1);
                }, function (err) {
                    //alert(err);
                });
            }
        });
    }

    $scope.sendApprovalRequest = function (projectId,$event) {
        if (projectId != null && $scope.IsUploading == false) {
            loader.show($event.currentTarget);
            projectService.submitprojectreview(projectId).then(function (response) {
                $scope.Project.StatusId = response.Data.StatusId;
                notification.success(response.Message);
                loader.hide();
            }, function (err, status) {
                notification.error(err.Message);
            });
        }
        else {
            alert("No Project is selected for approval");
        }
    }

    $scope.onFileSelect = function ($files) {
        $scope.IsUploading = true;
        $upload.upload({
            url: "/api/User/UploadDocument", // webapi url
            method: "POST",
            data: { ProjectId: $scope.Project.ProjectId },
            file: $files,
            headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            // get upload percentage
            $scope.FileUploadPercentage = parseInt(100.0 * evt.loaded / evt.total) + "%";
            if ($scope.FileUploadPercentage == "100%") {
                $scope.FileUploadPercentage = "";
            }
        }).success(function (response) {
            $scope.FileUploadPercentage = false;
            $scope.Project.ProjectDocuments.push(response);
            notification.success("Uploaded SuccessFully");
            $scope.IsUploading = false;
        }).error(function (result, status) {
            console.log(result);
        });
        //projectService.onfileselect($files, $scope.Project.ProjectId).then(function (response) {
        //    $scope.Project.ProjectDocuments.push(response);
        //}, function (err) {
        //    //alert(err);
        //});
    }

    $scope.onVideoSelect = function ($file) {
        $scope.IsUploading = true;
        $upload.upload({
            url: "./api/User/UploadProjectVideo", // webapi url
            method: "POST",
            data: { ProjectId: $scope.Project.ProjectId },
            file: $file,
            headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            // get upload percentage
            $scope.VideoUploadPercentage = parseInt(100.0 * evt.loaded / evt.total) + "%";
            if ($scope.VideoUploadPercentage == "100%") {
                $scope.showEncoderDiv = true;
            }
        }).success(function (data, status, headers, config) {
            // file is uploaded successfully
            $scope.showEncoderDiv = false;
            $scope.VideoUploadPercentage = false;
            $scope.Project.VideoPath = data.VideoPath;
            $scope.Project.VideoName = data.VideoName;
            notification.success("Uploaded SuccessFully");
            $scope.IsUploading = false;
        }).error(function (data, status, headers, config) {
            // file failed to upload
            console.log(data);
        });
    }

    $scope.getTotalPledged = function () {
        var total = 0;
        $scope.TotalInvetsors = 0;
        $scope.TotalApprovedInvetsors = 0;
        $scope.Project.TotalFundingAmount = 0;
        for (var i = 0; i < $scope.Project.ProjectFundings.length; i++) {
            //var funding = $scope.Project.ProjectFundings[i];
            if ($scope.Project.ProjectFundings[i].StatusId == "6" || $scope.Project.ProjectFundings[i].StatusId == "7") {
                total += $scope.Project.ProjectFundings[i].FundingAmount;
                $scope.TotalInvetsors += 1;
            }
            if ($scope.Project.ProjectFundings[i].StatusId == "6") {
                $scope.TotalApprovedInvetsors += 1;
            }
            if ($scope.Project.ProjectFundings[i].StatusId == "6" || $scope.Project.ProjectFundings[i].StatusId == "7") {
                $scope.Project.TotalFundingAmount = $scope.Project.TotalFundingAmount + $scope.Project.ProjectFundings[i].FundingAmount;
            $scope.Project.SiteCommission = commonService.roundToTwoDecimal($scope.Project.TotalFundingAmount * 0.02);

            $scope.Project.TransactionCharges = commonService.roundToTwoDecimal(((2.9 * parseFloat($scope.Project.TotalFundingAmount - $scope.Project.SiteCommission)) / 100) + 0.30);
            $scope.Project.PaymentReceived = commonService.roundToTwoDecimal(parseFloat($scope.Project.TotalFundingAmount) - $scope.Project.SiteCommission - $scope.Project.TransactionCharges);
            }
        }
        $scope.TotalInvetsors = $scope.TotalInvetsors.toLocaleString();
        $scope.TotalPledged = total.toLocaleString();
    }

    $scope.openNextTab = function (active) {
        commonHeaderChanges(active);
        $(".active").removeClass("active");
        $("." + active).addClass("active");
        $("body").focus();
    }
    $scope.changeHeaderText = function (tabName) {
        commonHeaderChanges(tabName);
    }
    function commonHeaderChanges(tabName) {
        $('.tab-description').removeClass("active").addClass("hidden");
        $('.tab-description-' + tabName).addClass("active").removeClass("hidden");
    }

    if ($scope.Project.ProjectFundings != null) {
        $scope.getTotalPledged();
    }

    $scope.onImageSelect = function ($files) {
        $scope.IsUploading = true;
        $upload.upload({
            url: "/api/User/AddTempImage", // webapi url
            method: "POST",
            file: $files,
            headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            $scope.ImageUploadPercentage = parseInt(100.0 * evt.loaded / evt.total) + "%";
            if ($scope.ImageUploadPercentage == "100%") {
                $scope.ImageUploadPercentage = "";
            }
        }).success(function (response) {
            $scope.ImageUploadPercentage = false;
            $scope.Project.ImagePath = response;
            notification.success("Uploaded SuccessFully");
            $scope.IsUploading = false;
        }).error(function (result, status) {
            console.log(result);
        });

        //projectService.addtempimage($files).then(function (response) {
        //    $scope.Project.ImagePath = response;
        //}, function (err) {
        //    //alert(err);
        //});
    }

    $scope.onLogoSelect = function ($files) {
        $scope.IsUploading = true;
        $upload.upload({
            url: "/api/User/AddTempImage", // webapi url
            method: "POST",
            file: $files,
            headers: { 'Authorization': 'Bearer ' + accesstoken, 'Content-Type': undefined }
        }).progress(function (evt) {
            $scope.LogoUploadPercentage = parseInt(100.0 * evt.loaded / evt.total) + "%";
            if ($scope.LogoUploadPercentage == "100%") {
                $scope.LogoUploadPercentage = "";
            }
        }).success(function (response) {
            $scope.LogoUploadPercentage = false;
            $scope.Project.LogoPath = response;
            notification.success("Uploaded SuccessFully");
            $scope.IsUploading = false;
        }).error(function (result, status) {
            console.log(result);
        });
    }


    $scope.FundingStatus = function (isApproved, projectFundingId, $index, $event, fundingUserId) {
        loader.show($event.currentTarget);
        projectService.updateFundingStatus(isApproved, projectFundingId).then(function (response) {
            if (response.Success) {
                var message = isApproved == true ? "Your funding is approved" : "Your funding is disapproved";
                messageService.addConversationWithSpecificUser($scope.Project.ProjectId, fundingUserId, message).then(function (messageResponse) {
                    if (messageResponse.Success)
                        notification.success(messageResponse.Message);
                    else
                        notification.error(messageResponse.Message);
                    var projectFundings = $scope.Project.ProjectFundings[$index];
                    $scope.$root.chat.server.send("abc", projectFundings.UserModel.Email, messageResponse.Data);
                    notification.success(response.Message);
                    var result = response.Data;
                    projectFundings.StatusId = result.StatusId;
                    projectFundings.Status.Name = result.Status.Name;
                    if (result.StatusId == '6') {
                        $scope.TotalInvetsors = (parseInt($scope.TotalInvetsors.replace(/,/g, "")) + 1).toLocaleString();
                        $scope.TotalPledged = (parseInt($scope.TotalPledged.replace(/,/g, "")) + result.FundingAmount).toLocaleString();
                        $scope.TotalApprovedInvetsors = $scope.TotalApprovedInvetsors + 1;
                        debugger;
                        $scope.Project.TotalFundingAmount = $scope.Project.TotalFundingAmount + result.FundingAmount;
                        $scope.Project.SiteCommission = commonService.roundToTwoDecimal($scope.Project.TotalFundingAmount * 0.02);

                        $scope.Project.TransactionCharges = commonService.roundToTwoDecimal(((2.9 * parseFloat($scope.Project.TotalFundingAmount - $scope.Project.SiteCommission)) / 100) + 0.30);//commonService.roundToTwoDecimal($scope.Project.PaymentReceived - $scope.Project.TotalFundingAmount);
                        $scope.Project.PaymentReceived = commonService.roundToTwoDecimal(parseFloat($scope.Project.TotalFundingAmount) - $scope.Project.SiteCommission - $scope.Project.TransactionCharges);//commonService.roundToTwoDecimal((0.30 + parseFloat($scope.Project.TotalFundingAmount - $scope.Project.SiteCommission)) / (1 - 0.029));
                       
                    }
                    loader.hide();
                });
            }
            else {
                notification.error(response.Message);
                loader.hide();
            }
        })
    }
    $scope.fundingDate = function (date) {
        return commonService.formatDate(date);
    }
    $scope.formatDate = function (date) {
        return commonService.formatDate(date);
    }

    //$scope.ShowHideApproveDisapproveInvestment = function (statusId) {
    //    if (statusId != null) {
    //        $scope.EditApproveDisapprove = false;
    //        $scope.ApproveDisapprove = false;
    //        if (statusId == 4) {
    //            $scope.ApproveDisapprove = true;
    //            return $scope.ApproveDisapprove;
    //        }
    //        else if (statusId == 6 || statusId == 2) {
    //            $scope.EditApproveDisapprove = true;
    //            return $scope.EditApproveDisapprove;
    //        }
    //    }
    //}

    //$scope.EditApproveDisapproveInvestment = function (statusId) {
    //    if (statusId == 6 || statusId == 2)
    //        $scope.EditApproveDisapprove = true;
    //    else
    //        $scope.EditApproveDisapprove = false;
    //    return $scope.EditApproveDisapprove;
    //}

    //$scope.ShowApproveDisapproveInvestment = function (statusId) {
    //    $scope.EditApproveDisapprove = !$scope.EditApproveDisapprove;
    //    $scope.ApproveDisapprove = !$scope.ApproveDisapprove;
    //}
    //$scope.ShowHideApproveDisapproveInvestment();

    $scope.calculateAmountFeaturedClicks = function (ClicksCount, clickPrice) {
        $scope.FeaturedClicksInitialPayment = commonService.roundToTwoDecimal(ClicksCount * clickPrice);
        //if ($scope.FeaturedClicksInitialPayment < 3000) {
        
        //}
        //else if ($scope.FeaturedClicksInitialPayment > 3000 && $scope.FeaturedClicksInitialPayment < 10000)
        //{
        //    $scope.FeaturedClicksTax = commonService.roundToTwoDecimal(((3.9 * $scope.FeaturedClicksInitialPayment) / 100) + 0.30);
        //}
        //else if ($scope.FeaturedClicksInitialPayment > 10000 && $scope.FeaturedClicksInitialPayment < 1000000)
        //{
        //    $scope.FeaturedClicksTax = commonService.roundToTwoDecimal(((3.7 * $scope.FeaturedClicksInitialPayment) / 100) + 0.30);
        //}
        $scope.FeaturedClicksFinalPayment = commonService.roundToTwoDecimal((0.30 + $scope.FeaturedClicksInitialPayment) / (1 - 0.029));//commonService.roundToTwoDecimal($scope.FeaturedClicksInitialPayment + $scope.FeaturedClicksTax);
        $scope.FeaturedClicksTax = commonService.roundToTwoDecimal($scope.FeaturedClicksFinalPayment - $scope.FeaturedClicksInitialPayment);//commonService.roundToTwoDecimal(((2.9 * $scope.FeaturedClicksInitialPayment) / 100) + 0.30);
    }

    $scope.addFeaturedClicks = function ($event, paymentData) {
        paymentData.ProjectId = $scope.Project.ProjectId;
        paymentData.Amount = $('.totalPayment').text().toLocaleString();
        paymentData.CardType = commonService.detectCardType(paymentData.CardNumber)
        loader.show($event.currentTarget);
        projectService.addPaymentInfo(paymentData).then(function (response) {
            if (response.Success) {
                notification.success('Payment added successfully');
                $scope.FeaturedClickLeft += response.Data.ClciksLeft;
                $scope.OpenClicks = false;
                $scope.FeaturedPayment = {};
                $scope.FeaturedClicksInitialPayment = $scope.FeaturedClicksTax = $scope.FeaturedClicksFinalPayment = '';
            }
            else {
                notification.error(response.Message)
            }
            loader.hide();
        },
      function (err) {
          notification.error(err.Message);
          loader.hide();
      });
    }

    $scope.openClicksPayment = function (value) {
        $scope.OpenClicks = value;
    }
    $scope.getPayment = function ($event, projectId) {
        loader.show($event.currentTarget);
        projectService.getPayment(projectId).then(function (response) {
            if (response.Result.Success) {
                angular.forEach(response.Result.Data, function (value, index) {
                    $scope.Project.ProjectFundings[index].Status = value.Status;
                    $scope.Project.ProjectFundings[index].StatusId = value.StatusId;
                })
                $scope.getTotalPledged();
                

                //notification.success(response.Message);
            }
            else {
                notification.error("Something went wrong")
            }
            loader.hide();
        },
      function (err) {
          notification.error(err.Message);
          loader.hide();
      });
    }
    $scope.getClicksLog = function ($event, projectId) {
        loader.show($event.currentTarget);
        $scope.ClicksLog = {};
        projectService.getClicksLog(projectId).then(function (response) {
            loader.hide();
            $scope.ClicksLog = response;
            $('#clicksLogModal').modal();
        },
      function (err) {
          notification.error(err.Message);
          loader.hide();
      });
    }
    $scope.formatLogDate = function (date) {
        return commonService.formatDateTime(date);
    }
}]);

app.controller('projectDetailController', ['$scope', '$route', 'projectDetails', 'commonService', 'messageService', 'notification', 'projectDetailService', 'loader',
'accountService', '$location', function ($scope, $route, projectDetails, commonService, messageService, notification, projectDetailService, loader,
 accountService, $location) {

    $scope.ProjectDetail = projectDetails.data.project;
    $scope.$root.IsSearch = true;

    $(document).ready()
    {
        nicEditors.editors.push(
            new nicEditor({ buttonList: ['bold', 'italic', 'underline', 'fontSize', 'ol', 'ul'] })
                .panelInstance(document.getElementById('newPublicMessage'))
        );
        $('.nicEdit-panelContain').parent().width('100%');
        $('.nicEdit-panelContain').parent().next().width('98%').css("min-hiehgt", "100px");
        $('.nicEdit-main').width('99%');
    }
    $(window).load(function () {
        $('.nicEdit-main').css({ "min-height": "100px !important" });
    });

    $scope.getFundingPercentage = function (data) {
        var totalFundingAmount = 0, investmentAmount = data.InvestmentAmount, totalAmountInPercent = 0;
        for (var i = 0; i < data.ProjectFundings.length; i++) {
            totalFundingAmount += data.ProjectFundings[i].FundingAmount == undefined ? 0 : data.ProjectFundings[i].FundingAmount;
        }
        totalAmountInPercent = totalFundingAmount != 0 ? commonService.roundToTwoDecimal((totalFundingAmount * 100) / investmentAmount) : 0;

        $scope.TotalFundingInPercent = totalAmountInPercent;
        return totalAmountInPercent;
    }
    $scope.VideoPlayed = false;
    $scope.isUserAuthenticated = accountService.isAuthenticated.isAuth;
    $scope.newPublicMessage = {};
    //$scope.newPrivateMessage = {};
    $scope.userInvestment = projectDetails.data.loggedInUserFunding;
    $scope.FundindDate = '';
    if ($scope.userInvestment != null) {
        $scope.AlreadyInvested = $scope.userInvestment.Result.StatusId == 2 || $scope.userInvestment.Result.StatusId == 5 ? false : true;
        $scope.UserFundindDate = commonService.formatDate($scope.userInvestment.Result.FundingDate);
        $scope.FundingDateOver = new Date($scope.ProjectDetail.FundingDuration) > new Date() ? false : true;
    }
    else {
        $scope.AlreadyInvested = false;
    }
    $scope.IfLoggedInUserIsProjectOwner = projectDetails.data.ifLoggedInUserIsProjectOwner.Result;
    $scope.ValidTill = commonService.formatDate($scope.ProjectDetail.FundingDuration); //month + " " + date + ", " + year;


    $scope.getTotalPledged = function () {

        var total = 0;
        for (var i = 0; i < $scope.ProjectDetail.ProjectFundings.length; i++) {
            var funding = $scope.ProjectDetail.ProjectFundings[i];
            total += funding.FundingAmount;
        }
        $scope.TotalPledged = total;
    }

    if ($scope.ProjectDetail.ProjectFundings != null) {
        $scope.getTotalPledged();
    }

    $scope.backItFunding = function (projectId) {
        projectDetailService.backItFunding(projectId).then(function (response) {

            if (response.backitFunding) {
                $scope.AlreadyInvested = false;
                notification.success("Retracted Successfully");
                $route.reload();
            }
            else {
                $scope.AlreadyInvested = true;
                notification.error("Some Problem Occured");
            }
        })
    }

    //Public Message
    $scope.addPublicMessage = function ($event) {
       
        if (accountService.isAuthenticated.isAuth) {
            $scope.newPublicMessage.MessageBody = $(".nicEdit-main").html().replace(/&nbsp;/g, '').replace(/<br>/g,'').trim();
            if ($scope.newPublicMessage.MessageBody != '' && $scope.newPublicMessage.MessageBody != undefined) {
                loader.show($event.currentTarget);
                $scope.newPublicMessage.ProjectId = $scope.ProjectDetail.ProjectId;
                messageService.addPublicMessage($scope.newPublicMessage).then(function (response) {
                    $scope.ProjectDetail.PublicMessages.push(response.Data);
                    $scope.newPublicMessage = {};
                    $(".nicEdit-main").html('');
                    loader.hide();
                })

            }
        }
        else {
            $location.path('/login');
        }

    }
    $scope.formatCommentDate = function (commentDate) {
        return commonService.formatDateTime(commentDate);
    }
    $scope.deletePublicMessage = function (message, $index) {
        if (message != null) {
            bootbox.confirm("Are you sure you want to delete this message?", function (result) {
                if (result) {
                    messageService.deletePublicMessage(message).then(function (response) {
                        $scope.ProjectDetail.PublicMessages.splice($index, 1);
                        notification.success(response.Message);
                    });
                }
            });
        }
    }
    //Public Message Ends
    //Private Conversation
    //$scope.addConversation = function (data, ){ 
    //    data.ProjectId = $scope.ProjectDetail.ProjectId;
    //    data.IsPrivate = false;
    //    messageService.addConversation(data).then(function (response) {
    //        $scope.ProjectDetail.Messages.push(response.Data);
    //        $scope.newPrivateMessage = {};
    //    })
    //}
    //$scope.openNewConversationPopUp = function () {
    //    bootbox.dialog({
    //        message: "Message : <br/><br/>" +
    //                 " <div class='form-group'><textarea  class='form-control newPrivateMessage' type='text' placeholder='Your Message' wrap='hard' required /></div>",
    //        title: "Add Private Message",
    //        buttons: {
    //            success: {
    //                label: "Submit",
    //                className: "btn-success",
    //                callback: function () {
    //                    if ($('.newPrivateMessage').val() != '') {
    //                        //$scope.newPrivateMessage.MessageBody = $('.newPrivateMessage').val();
    //                        //$scope.newPrivateMessage.ProjectId = $scope.ProjectDetail.ProjectId;
    //                        //$scope.newPrivateMessage.IsPrivate = true;
    //                        messageService.addConversation($scope.ProjectDetail.ProjectId, $('.newPrivateMessage').val()).then(function (response) {
    //                            //if()
    //                            notification.success(response.Message);
    //                            $scope.$root.chat.server.send("abc", $scope.ProjectDetail.User.Email, response.Data);
    //                            //$scope.newPrivateMessage = {};
    //                            $('.newPrivateMessage').val('');
    //                        })
    //                    }
    //                    else {
    //                        $('.newPrivateMessage').css("border-color", "red");
    //                    }
    //                }
    //            },
    //            danger: {
    //                label: "Cancel",
    //                className: "btn-danger",
    //                callback: function () {
    //                    //Example.show("uh oh, look out!");
    //                }
    //            }
    //        }
    //    });
    //}
    //Private Conversation Ends
    $scope.playVideo = function () {
        if ($scope.ProjectDetail.VideoName != null)
        {
            $scope.VideoPlayed = true;
            $('.project-video').removeClass("hidden");
            $('video')[0].play();
        }
    }

    $scope.CloseNewPrivateMessage = function () {
        $('#newPrivateMessageModal').modal('hide');
    }
    $scope.AddPrivateMessage = function ($event) {

        $scope.PrivateMessageData = {};
        $scope.PrivateMessageData.ProjectID = $scope.ProjectDetail.ProjectId;
        $scope.PrivateMessageData.Message = $scope.newPrivateMessage;

        loader.show($event.currentTarget);
        messageService.addConversation($scope.PrivateMessageData).then(function (response) {
            notification.success(response.Message);
            $scope.$root.chat.server.send("abc", $scope.ProjectDetail.User.Email, response.Data);
            $('#newPrivateMessageModal').modal('hide');
            loader.hide();
            $scope.newPrivateMessage = "";
        })
    }
  
}]);