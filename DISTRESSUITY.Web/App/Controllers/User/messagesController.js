app.controller('messagesController', ['$scope', 'messageService', 'commonService', 'notification', 'loader', '$rootScope', '$filter',
    function ($scope, messageService, commonService, notification, loader, $rootScope, $filter) {
        $scope.Conversations = {};
        $scope.OpenedConversation = {};
        $scope.conversationReply = {};
        $scope.DistinctConversation = {};
        $scope.selectedProject = '';
        $scope.visibleMessages = [];
        $scope.visibleMessagesCount = 5;
        $scope.visibleMessagesIndex = 0;
        $scope.showReadMore = false;
        $scope.selectedConversation = 0;
        //$scope.chat = $.connection.chatHub;
        $scope.loggedInUserName = localStorage.getItem('userName');
        getConversations();
        $scope.$root.IsSearch = true;

        //messageService.initialize();
        function getConversations() {
            loader.preLoader();
            messageService.getConversation().then(function (response) {
                $scope.Conversations = response.data.MultipleData[0];
                $scope.DistinctConversation = response.data.MultipleData[1];
                $scope.OpenedConversation = $scope.Conversations[0];
                loader.hidePreLoader();
                $scope.$root.NewMessagesCount = 0;
                //visibleMessages($scope.OpenedConversation);
            })
        };

        //$.connection.hub.start().done(function () {
        //    console.log('Now connected, connection ID=' + $.connection.hub.id);
        //    $scope.$root.chat.server.runMe();
        //}).fail(function () {
        //    console.log('Could not Connect!');
        //});

        //function visibleMessages(Conversation) {
        //    var messages = [];
        //    var diff = $scope.visibleMessagesCount * $scope.visibleMessagesIndex;
        //    var startingPoint = ((Conversation.ConversationReplyModel.length - 1)) - diff;   //From Where to Start
        //    var endPoint = ((Conversation.ConversationReplyModel.length - 1) - $scope.visibleMessagesCount) - diff;    // When loop terminates
        //    for (var i = startingPoint; i > endPoint; i--) { // Reverse Loop on Reply's
        //        if (i >= 0)
        //            messages.push(Conversation.ConversationReplyModel[i]);
        //    }
        //    messages.reverse(); //Finally Reverse direction of messages 


        //    $.each(messages, function (index, value) {
        //        if ($scope.visibleMessages[index] != undefined)
        //            //$scope.visibleMessages.insert(index, value);
        //            $scope.visibleMessages.splice(index, 0, value)
        //        else {
        //            $scope.visibleMessages.push(value);

        //        }
        //        //alert(index + ": " + value);
        //    });
        //    $scope.$apply();
        //    $scope.showReadMore = endPoint >= 0 ? true : false;

        //}

        $scope.formatDateTime = function (Date) {
            return commonService.formatDateTime(Date);
        };

        $scope.formatDate = function (Date) {
            return commonService.formatDate(Date);
        };

        $scope.formatDateConversationsList = function (conversation) {
            if (conversation.UpdatedDate != null)
                return commonService.formatDateTime(conversation.UpdatedDate);
            else
                return commonService.formatDateTime(conversation.CreatedDate);
        };

        $scope.changeConversation = function (conversationId, $index) {
            loader.preLoader();
            $scope.visibleMessages = [];
            messageService.changeConversation(conversationId).then(function (response) {
                $scope.OpenedConversation = response.data.Data;
                var conversation = $scope.Conversations[$index];
                conversation.UnreadMessages = $scope.OpenedConversation.UnreadMessages;
                conversation.IsActive = true;
                $scope.selectedConversation = $index;
                loader.hidePreLoader();
                //$("p.conversation-index[value='"+$index+"']");
                //visibleMessages($scope.OpenedConversation);
            })
        };

        $scope.addConversationReply = function (openedConversationId, $event) {
            if ($scope.conversationReply.Reply != undefined) {
                loader.show($event.currentTarget);
            var loggedInUserName = localStorage.getItem('userName');
            $scope.conversationReply.ConversationId = openedConversationId;
           
                messageService.addConversationReply($scope.conversationReply).then(function (response) {
                    //$scope.OpenedConversation.ConversationReplyModel.push(response.Data);
                    var conversation = $filter('getByConversationId')($scope.Conversations, response.Data.ConversationId);
                    if (conversation != null) {
                        var toUser = "";
                        if (conversation.ToUserModel != undefined && conversation.ToUserModel.Email.toLowerCase() == $scope.loggedInUserName) {
                            toUser = conversation.CreatedByUserModel.Email;
                        }
                        else if (conversation.CreatedByUserModel != undefined && conversation.CreatedByUserModel.Email.toLowerCase() == $scope.loggedInUserName) {
                            toUser = conversation.ToUserModel.Email;
                        }
                        $scope.$root.chat.server.send("abc", toUser, response.Data);
                        $scope.conversationReply = {};
                        loader.hide();
                    }
                }, function (err) { loader.hide(); });
            }
        };

        $scope.$root.UpdateMessage = function (name, message) {
            if ($scope.OpenedConversation.ConversationId == message.ConversationId) {
                $scope.OpenedConversation.ConversationReplyModel.push(message)
            }
            else {
                var conversation = $filter('getByConversationId')($scope.Conversations, message.ConversationId);
                if (conversation != null) {
                    conversation.ConversationReplyModel.push(message);
                    conversation.UnreadMessages += 1;
                }
            }
        }
        //$scope.$root.chat.client.broadcastMessage = function (name, message) {
        //    $scope.$apply(function () {
        //        if ($scope.OpenedConversation.ConversationId == message.ConversationId) {
        //            $scope.OpenedConversation.ConversationReplyModel.push(message)
        //        }
        //        else {
        //            var conversation = $filter('getByConversationId')($scope.Conversations, message.ConversationId);
        //            if (conversation != null) {
        //                conversation.ConversationReplyModel.push(message);
        //                conversation.UnreadMessages += 1;
        //            }
        //        }
        //    });
        //};

        $scope.getConversationsByProjectId = function () {
            if ($scope.selectedProject == null) {
                getConversations();
            }
            else {
                loader.preLoader();
                messageService.getConversationsByProjectId($scope.selectedProject.ProjectModel.ProjectId).then(function (response) {
                    $scope.Conversations = response.data.Data;
                    $scope.OpenedConversation = $scope.Conversations[0];
                    loader.hidePreLoader();
                })
            }
        }

        $scope.getAllConversations = function () {
            getConversations();
        }
        //$scope.loadPreviousMessage = function () {
        //    $scope.visibleMessagesIndex += 1;
        //    //visibleMessages($scope.OpenedConversation);
        //}
        $scope.calculateUnreadMessages = function (converstion) {
            var unread = 0;
            $.each(converstion.ConversationReplyModel, function (index, value) {
                if (value.UserModel.Email != $scope.loggedInUserName && value.Read == false)
                    unread += 1;
            })
            $scope.showUndread = unread == 0 ? false : true;
            return unread == 0 ? "" : unread;
        }
        $scope.FindToUser = function (conversation, $index) {
            if (conversation.ToUserModel.Email.toLowerCase() == $scope.loggedInUserName) {
                $scope.ToUserModel = conversation.CreatedByUserModel;
            }
            else {
                $scope.ToUserModel = conversation.ToUserModel;
            }
            if ($index != undefined)
                $scope.ConversationIndex = $index;
        }
        $scope.alignReplies = function (user) {
            if (user.Email.toLowerCase() == $scope.loggedInUserName)
                return true;
            else
                return false;
        }

        $scope.findOpenedConversationUser = function (conversation) {

            if (conversation.ToUserModel != undefined && conversation.ToUserModel.Email.toLowerCase() == $scope.loggedInUserName) {
                return conversation.CreatedByUserModel.FirstName + " " + conversation.CreatedByUserModel.LastName;
            }
            else if (conversation.CreatedByUserModel != undefined && conversation.CreatedByUserModel.Email.toLowerCase() == $scope.loggedInUserName) {
                return conversation.ToUserModel.FirstName + " " + conversation.ToUserModel.LastName;
            }
        }

        $scope.toolTip = function (firstName, lastName, projectName) {
            return '<div class="li-tooltip-container"><p><strong>' + firstName + ' ' + lastName + '</strong></p> <p>' + projectName + '</p> </div>';
        }
    }])

app.filter('getByConversationId', function () {
    return function (input, id) {
        var len = input.length;
        for (i = 0; i < input.length; i++) {
            if (input[i].ConversationId == id) {
                return input[i];
            }
        }
        return null;
    }
});